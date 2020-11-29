using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneOf;
using System;
using System.Linq;
using ZSpitz.Util;

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
            if (!Functions.IsTypeMatch(token, objectType, out var matchedType)) {
                throw new InvalidOperationException($"Cannot map token to any subtype of '{objectType}'.");
            }

            // TODO handle types inheriting from OneOfBase
            // There are two issues:
            // 1. The implicit conversions defined in OneOfBase converts to OneOfBase, not the derived type
            // In order to get an implicit conversion to the derived type, the conversion cannot be inherited from OneOfBase
            // 2. Presumably, the best place to define said conversion would be on the derived type.
            var conversion = objectType.OneOfType()!.GetMethod("op_Implicit", new[] { matchedType });
            if (conversion is null) {
                throw new InvalidOperationException($"Cannot find implicit conversion for token of type '{token.Type}' from '{matchedType}' to '{objectType}");
            }

            return conversion.Invoke(null, new[] { token.ToObject(matchedType!, serializer) });
        }

        public override bool CanConvert(Type objectType) => objectType.OneOfSubtypes().Any();
    }

    public class TupleConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => 
            serializer.Serialize(writer, Functions.TupleValues(value));

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            var token = JToken.ReadFrom(reader);
            if (!Functions.IsTypeMatch(token, objectType, out _)) {
                if (token.Type == JTokenType.Array) {
                    // return default conversion here
                    serializer.Populate(reader, existingValue!);
                    return existingValue;
                }
                throw new InvalidOperationException($"Cannot map token to tuple type '{objectType}'.");
            }

            var types = objectType.GetGenericArguments();
            var tupleFactoryType =
                objectType.Name.StartsWith("ValueTuple") ? 
                    typeof(ValueTuple) :
                    typeof(Tuple);

            var objects = types.Zip(token)
                .SelectT((type, token) => token.ToObject(type, serializer))
                .ToArray();

            return tupleFactoryType
                .GetMethods().First(x => x.Name == "Create" && x.GetGenericArguments().Length == types.Length)
                .MakeGenericMethod(objectType.GetGenericArguments())
                .Invoke(null, objects);
        }

        public override bool CanConvert(Type objectType) => objectType.IsTupleType();
    }
}
