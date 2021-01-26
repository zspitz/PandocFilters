using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneOf;
using PandocFilters.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ZSpitz.Util;
using static ZSpitz.Util.Functions;
using static PandocFilters.Functions;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace PandocFilters {
    public class OneOfJsonConverter : JsonConverter {
        public Func<JToken, Type?, JsonSerializer, (object? value, Type? matchedType)>? tokenHandler;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            if (value is IOneOf of) {
                value = of.Value;
            }
            serializer.Serialize(writer, value);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            var token = JToken.ReadFrom(reader);

            object? value = null;
            Type? matchedType = null;
            if (tokenHandler is { }) {
                (value, matchedType) = tokenHandler(token, objectType, serializer);
            }

            var underlying = objectType.UnderlyingIfNullable();

            if (matchedType is null) {
                if (!IsTypeMatch(token, underlying, out matchedType)) {
                    throw new InvalidOperationException($"Cannot map token to any subtype of '{objectType}'.");
                }

                // For OneOf, the implicit conversions are defined on OneOf
                // For OneOfBase, try to find implicit conversions on objectType
                // For Nullable<OneOf>, it's enough to return the value converted to OneOf; JSON.NET handles the conversion to Nullable
                value = token.ToObject(matchedType!, serializer);
            }

            var conversion = underlying.GetMethod("op_Implicit", new[] { matchedType });
            if (conversion is { }) {
                return conversion.Invoke(null, new[] { value });
            }

            (ConversionStrategy strategy, MethodInfo? method) conversion1 = default;
            Type? target = default;
            // try to find a constructor with a single parameter, which has an implicit conversion from the matched type to the type of the parameter
            var ctor = underlying.GetConstructors().FirstOrDefault(ctor => {
                var prms = ctor.GetParameters();
                if (prms.Length != 1) { return false; }
                // TODO what if the parameter type is Nullable of the matched type?
                // or of the conversion target?
                conversion1 = matchedType.GetImplicitConversionTo(prms[0].ParameterType.UnderlyingIfNullable());
                if (conversion1.strategy != ConversionStrategy.None) {
                    target = prms[0].ParameterType.UnderlyingIfNullable();
                    return true;
                }
                return false;
            });
            if (ctor is { }) {
                var converted = ConvertTo(value, target!);
                var ret = ctor.Invoke(new[] { converted });
                return ret;
            }

            throw new InvalidOperationException($"Cannot find implicit conversion or appropriate constructor for token of type '{token.Type}' from '{matchedType}' to '{objectType.UnderlyingIfNullable()}");
        }

        public override bool CanConvert(Type objectType) => objectType.UnderlyingIfNullable().OneOfSubtypes().Any();
    }

    public class TupleConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
            serializer.Serialize(writer, TupleValues(value!));

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            var token = JToken.ReadFrom(reader);
            if (token.Type == JTokenType.Null) { return null; }

            var underlying = objectType.UnderlyingIfNullable();
            if (!IsTypeMatch(token, objectType, out _)) {
                if (token.Type == JTokenType.Array) {
                    // return default conversion here
                    serializer.Populate(reader, existingValue!);
                    return existingValue;
                }
                throw new InvalidOperationException($"Cannot map token to tuple type '{objectType}'.");
            }

            var isValueTuple = underlying.Name.StartsWith("ValueTuple");
            var types = underlying.GetGenericArguments();
            var objects = types.Zip(token)
                .SelectT((type, token) => token.ToObject(type, serializer))
                .ToArray();
            return MakeTuple(isValueTuple, objects, types);
        }

        public override bool CanConvert(Type objectType) => objectType.UnderlyingIfNullable().IsTupleType();
    }

    public class PandocTypesConverter : JsonConverter {
        private static readonly string[] metaNames = new [] {
            "MetaMap",
            "MetaList",
            "MetaBool",
            "MetaString",
            "MetaInlines",
            "MetaBlocks"
        };

        // nameof only returns the last segment of the fully-qualified name - https://stackoverflow.com/a/38584443/111794
        private static readonly Dictionary<string, ConstructorInfo?> handledTypes = 
            typeof(Pandoc).Assembly.GetTypes()
                .Where(x => x.Namespace == $"{nameof(PandocFilters)}.{nameof(Ast)}" && !(
                    x == typeof(Pandoc) ||
                    x == typeof(MetaValue) ||
                    x.HasAttribute<CompilerGeneratedAttribute>()
                ))
                .Select(x => (
                    x.Name,
                    x.IsEnum ? null : x.GetConstructors().SingleOrDefault()
                ))
                .ToDictionary()!;

        private static readonly HashSet<string> tupleRecordNames = new() {
            nameof(Attr),
            nameof(Caption),
            nameof(TableHead),
            nameof(Row),
            nameof(Cell),
            nameof(TableBody),
            nameof(TableFoot),
            nameof(ListAttributes)
        };

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            if (value is null) { return; }
            var (isMetaValue, ctor) =
                value is MetaValue mv ?
                    (true, metaNames[mv.Index]) :
                    (false, "");
            if (value is IOneOf oneOf) { value = oneOf.Value; }

            if (value is Citation citation) {
                // Don't know why Citation needs to be serialized as a plain object, vs. all other objects which are serialized with t and c properties

                // Also, serializing the citation itself causes some kind of recursion, because PandocTypes.WriteJson will be called again from within the serialization
                // We create a dynamic copy of the data in the citation, and serialize that.

                dynamic toSerialize = new ExpandoObject();
                toSerialize.citationId = citation.CitationId;
                toSerialize.citationPrefix = citation.CitationPrefix;
                toSerialize.citationSuffix = citation.CitationSuffix;
                toSerialize.citationMode = citation.CitationMode;
                toSerialize.citationNoteNum = citation.CitationNoteNum;
                toSerialize.citationHash = citation.CitationHash;

                serializer.Serialize(writer, toSerialize);
                return;
            }

            var type = value.GetType();
            string? t = null;
            object? c = null;
            if (type.IsEnum) {
                t = value.ToString();
            } else {
                if (isMetaValue) {
                    t = ctor;
                } else if (type.Name.NotIn(tupleRecordNames)) { 
                    t = type.Name; 
                }
                var deconstruct = type.GetMethod("Deconstruct");
                object[] deconstructed;
                if (isMetaValue) {
                    deconstructed = new object[] { value };
                } else if (deconstruct is null) {
                    deconstructed = Array.Empty<object>();
                } else {
                    deconstructed = new object[deconstruct.GetParameters().Length];
                    deconstruct.Invoke(value, deconstructed);
                }
                if (deconstructed.Length == 1) {
                    c = deconstructed[0];
                } else if (deconstructed.Length > 1) {
                    c = deconstructed;
                }
            }
            if (type.Name.In(tupleRecordNames)) {
                serializer.Serialize(writer, c);
            } else {
                dynamic tc = new ExpandoObject();
                if (t is { }) { tc.t = t; }
                if (c is { }) { tc.c = c; }
                serializer.Serialize(writer, tc);
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            var token = JToken.ReadFrom(reader);
            ConstructorInfo? ctor;
            ParameterInfo[]? parameters;
            object[] args;

            switch (token.Type) {
                case JTokenType.Object:
                    var t = (string?)token["t"];
                    var c = token["c"];

                    if (objectType.IsEnum && c is null) {
                        if (t is null) { throw new InvalidOperationException(); }
                        // Enum values come in with only the `t` property
                        return Enum.Parse(objectType, t);
                    }

                    // handle Meta* types here
                    var metaIndex = Array.IndexOf(metaNames, t);
                    if (metaIndex != -1) {
                        ctor = typeof(MetaValue).GetConstructors().SingleOrDefault();
                        var parameterType = ctor.GetParameters()[0].ParameterType;
                        var metaTargetType = objectType.OneOfSubtypes()[metaIndex];
                        args = new[] { 
                            ConvertTo(
                                c!.ToObject(metaTargetType, serializer),
                                parameterType
                            )!
                        };
                    } else {
                        // Most of the time, the concrete type of the serialized token will be at the 't' property
                        // Sometimes (e.g. Citation), the token is just a plain object, without a 't' property.
                        // In that case, the constructor comes from the `objectType`.

                        ctor = handledTypes[t ?? objectType.Name];
                        if (ctor is null) { throw new InvalidOperationException(); }
                        parameters = ctor.GetParameters();
                        if (c is not null && parameters.Length == 0) {
                            throw new InvalidOperationException();
                        }

                        if (parameters.Length == 0) {
                            args = Array.Empty<object>();
                        } else if (c is null) {
                            // plain object, e.g. Citation -- read constructor parameters from object properties
                            args = parameters.Select(prm => token[prm.Name.ToCamelCase()!]!.ToObject(prm.ParameterType, serializer)!).ToArray();
                        } else {
                            args = (parameters.Length switch {
                                // single JSON object
                                1 => new[] { c!.ToObject(parameters[0].ParameterType, serializer)! },

                                // JSON array
                                _ => parameters.Zip(c!).SelectT((prm, token) => token.ToObject(prm.ParameterType, serializer)!).ToArray()
                            });
                        }
                    }

                    var ret = ctor.Invoke(args);
                    ret = ConvertTo(ret, objectType);
                    return ret;
                case JTokenType.Array:
                    // some Haskell types are represented as arrays instead of objects with the t and c properties
                    ctor = objectType.GetConstructors().Single();
                    if (ctor is null) { throw new InvalidOperationException(); }
                    parameters = ctor.GetParameters();
                    if (parameters.Length != token.Count()) {
                        throw new InvalidOperationException();
                    }
                    args = parameters.Zip(token).SelectT((prm, token) => token.ToObject(prm.ParameterType, serializer)!).ToArray();
                    return ctor.Invoke(args);
                default:
                    throw new InvalidOperationException($"Unhandled token type '{token.Type}'.");
            }

            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) => handledTypes.ContainsKey(objectType.Name) || objectType.Name == nameof(MetaValue);
    }
}
