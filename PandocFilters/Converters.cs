using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneOf;
using PandocFilters.Types;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ZSpitz.Util;
using static ZSpitz.Util.Functions;

namespace PandocFilters {
    public class OneOfJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            if (value is IOneOf of) {
                value = of.Value;
            }
            serializer.Serialize(writer, value);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            var token = JToken.ReadFrom(reader);

            var underlying = objectType.UnderlyingIfNullable();
            if (!Functions.IsTypeMatch(token, underlying, out var matchedType)) {
                throw new InvalidOperationException($"Cannot map token to any subtype of '{objectType}'.");
            }

            // For OneOf, the implicit conversions are defined on OneOf
            // For OneOfBase, try to find implicit conversions on objectType
            // For Nullable<OneOf>, it's enough to return the value converted to OneOf; JSON.NET handles the conversion to Nullable
            var value = token.ToObject(matchedType!, serializer);

            var conversion = underlying.GetMethod("op_Implicit", new[] { matchedType });
            if (conversion is { }) {
                return conversion.Invoke(null, new[] { value });
            }

            (ConversionStrategy strategy, MethodInfo? method) conversion1 = default;
            // try to find a constructor with a single parameter, which has an implicit conversion from the matched type to the type of the parameter
            var ctor = underlying.GetConstructors().FirstOrDefault(ctor => {
                var prms = ctor.GetParameters();
                if (prms.Length != 1) { return false; }
                // TODO what if the parameter type is Nullable of the matched type?
                // or of the conversion target?
                conversion1= matchedType.GetImplicitConversionTo(prms[0].ParameterType.UnderlyingIfNullable());
                return conversion1.strategy != ConversionStrategy.None;
            });
            if (ctor is { }) {
                var converted =
                    conversion1.strategy switch {
                        ConversionStrategy.Method => conversion1.method!.Invoke(null, new[] { value }),
                        ConversionStrategy.Assignable => value,
                        ConversionStrategy.BuiltIn => Convert.ChangeType(value, ctor.GetParameters()[0].ParameterType.UnderlyingIfNullable()),
                        _ => throw new InvalidOperationException()
                    };
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
            if (!Functions.IsTypeMatch(token, objectType, out _)) {
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
        // nameof only returns the last segment of the fully-qualified name - https://stackoverflow.com/a/38584443/111794
        private static readonly Dictionary<string, ConstructorInfo?> handledTypes =
            typeof(Pandoc).Assembly.GetTypes()
                .Where(x => x.Namespace == $"{nameof(PandocFilters)}.{nameof(Types)}" &&!(
                    x == typeof(Pandoc) ||
                    x == typeof(MetaValue)
                ))
                .Select(x => (
                    x.Name, 
                    x.IsEnum ? null : x.GetConstructors().SingleOrDefault()
                ))
                .ToDictionary()!; // SingleOrDefault should be typed as ConstructorInfo? but returns ConstructorInfo

        private static readonly HashSet<string> tupleRecordNames = new() { "Attr", "Caption", "TableHead", "Row","Cell", "TableBody", "TableFoot"};

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            if (value is null) { return; }
            var type = value.GetType();
            string? t = null;
            object? c = null;
            if (type.IsEnum) {
                t = value.ToString();
            } else {
                if (type.Name.NotIn(tupleRecordNames)) { t = type.Name; }
                var deconstruct = type.GetMethod("Deconstruct");
                object[] deconstructed;
                if (deconstruct is null) {
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
                    var t = ((string)token["t"])!;
                    if (objectType.IsEnum && token["c"] is null) {
                        // Enum values come in with only the `t` property
                        return Enum.Parse(objectType, t);
                    }
                    ctor = handledTypes[t];
                    var c = token["c"];
                    if (ctor is null) { throw new InvalidOperationException(); }
                    parameters = ctor.GetParameters();
                    if (parameters.Length == 0 ^ c is null) {
                        throw new InvalidOperationException();
                    }
                    args = parameters.Length switch {
                        0 => Array.Empty<object>(),
                        1 => new[] { c!.ToObject(parameters[0].ParameterType, serializer)! },
                        _ => ctor.GetParameters().Zip(c!).SelectT((prm, token) => token.ToObject(prm.ParameterType, serializer)!).ToArray()
                    };
                    return ctor.Invoke(args);
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

        public override bool CanConvert(Type objectType) => handledTypes.ContainsKey(objectType.Name);
    }
}
