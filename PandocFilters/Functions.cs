using Newtonsoft.Json.Linq;
using PandocFilters.Ast;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using ZSpitz.Util;
using static ZSpitz.Util.Functions;

namespace PandocFilters {
    internal static class Functions {
        internal static bool IsTypeMatch(JToken token, Type type, [NotNullWhen(true)] out Type? matchedType) {
            var oneofTypes = type.OneOfSubtypes();
            if (oneofTypes.Length > 0) {
                matchedType = oneofTypes.FirstOrDefault(x => IsTypeMatch(token, x, out _));
                return matchedType is not null;
            }

            matchedType = type;
            var underlying = type.UnderlyingIfNullable();
            return token.Type switch {
                JTokenType.String => type == typeof(string),
                JTokenType.Boolean => underlying == typeof(bool),
                JTokenType.Date => underlying == typeof(DateTime),
                JTokenType.TimeSpan => underlying == typeof(TimeSpan),
                JTokenType.Integer => underlying.IsIntegral(),
                JTokenType.Float => underlying.IsNonIntegral(),
                JTokenType.Null => type.IsNullable(true),
                // TODO handle other types -- Uri, Guid?

                // if tuple type, and tuple arity matches, return true
                JTokenType.Array when underlying.IsTupleType() => token.Count() == underlying.GetGenericArguments().Length,

                // if array type, and array element type matches all subtokens
                JTokenType.Array when IIFE(() => {
                    var elementType = type.GetElementType();
                    return type.IsArray && token.All(x => IsTypeMatch(x, elementType!, out _));
                }) => true,

                // .NET collection
                JTokenType.Array when typeof(IList).IsAssignableFrom(type) => true,

                JTokenType.Object => !(
                    underlying.IsPrimitive ||
                    type == typeof(string) ||
                    underlying.IsEnum
                ),

                _ => false
            };
        }

        internal static void AddDelegate<T>([NotNull] ref Func<T, T>? field, Func<T, T> del) =>
            field = field is null ? del : field.WrapWith(del);

        internal static object? ConvertTo(object? source, Type target, ConversionStrategy? strategy, MethodInfo? method) {
            if (source is null) {
                return target.IsNullable(true) ?
                    null :
                    throw new InvalidCastException($"Unable to convert 'null' to '{target}'.");
            }

            if (strategy is null) {
                (strategy, method) = source.GetType().GetImplicitConversionTo(target);
            }

            return strategy switch {
                ConversionStrategy.Method => method!.Invoke(null, new[] { source }),
                ConversionStrategy.Assignable => source,
                ConversionStrategy.BuiltIn => Convert.ChangeType(source, target.UnderlyingIfNullable()),
                _ => throw new InvalidOperationException($"Unable to find valid conversion from '{source.GetType()}' to '{target}'.")
            };
        }

        internal static object? ConvertTo(object? source, Type target) => ConvertTo(source, target, null, null);

        internal static string WriteTarget((string Url, string Title) target) {
            var (url, title) = target;
            return title.IsNullOrWhitespace() ?
                $" {url} " :
                $"[{title}]({url})";
        }
    }
}

