using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ZSpitz.Util;

namespace PandocFilters {
    internal static class Functions {
        // TODO handle more than 8 values
        internal static object?[] TupleValues(object? tuple) {
            if (tuple is null) { throw new InvalidOperationException("No tuple values in null."); }
            var type = tuple.GetType();
            if (!type.IsTupleType()) { throw new InvalidOperationException($"Type '{type}' is not a tuple type."); }
            var fields = type.GetFields();
            if (fields.Any()) { return type.GetFields().Select(x => x.GetValue(tuple)).ToArray(); }
            return type.GetProperties().Select(x => x.GetValue(tuple)).ToArray();
        }

        internal static bool IsTypeMatch(JToken token, Type type, [NotNullWhen(true)] out Type? matchedType) {
            var oneofTypes = type.OneOfSubtypes();
            if (oneofTypes.Length > 0) {
                matchedType = oneofTypes.FirstOrDefault(x => IsTypeMatch(token, x, out _));
                return matchedType is not null;
            }

            matchedType = type;
            var underlying = type.UnderlyingIfNullable();
            bool? ret = token.Type switch {
                JTokenType.String => type == typeof(string),
                JTokenType.Boolean => underlying == typeof(bool),
                JTokenType.Date => underlying == typeof(DateTime),
                JTokenType.TimeSpan => underlying == typeof(TimeSpan),
                JTokenType.Integer => underlying.IsIntegral(),
                JTokenType.Float => underlying.IsNumeric(),
                JTokenType.Null => type.IsNullable(true),
                // TODO handle other types -- Uri, Guid?
                _ => null
            };

            if (ret is { }) { return ret.Value; }

            if (token.Type == JTokenType.Array) {
                if (underlying.IsTupleType()) {
                    // if tuple arity and subtypes match, return true
                    var tupleSubtypes = underlying.GetGenericArguments();
                    return token.Count() == tupleSubtypes.Length && token.Zip(tupleSubtypes).AllT((subtoken, subtype) => IsTypeMatch(subtoken, subtype, out _));
                } else if (type.IsArray) {
                    var elementType = type.GetElementType();
                    return type.IsArray && token.All(x => IsTypeMatch(x, elementType!, out _));
                } else if (typeof(IList).IsAssignableFrom(type)) {
                    // TODO extract the generic parameter from the appropriate interface: IEnumerable<T>, IList<T> etc.
                    // check for type match against the generic parameter
                    return true;
                }
                // TODO handle other collection types here
                return false;
            }

            return token.Type == JTokenType.Object && !underlying.IsPrimitive && !(type == typeof(string));
        }
    }
}
