using OneOf;
using System;
using ZSpitz.Util;

namespace PandocFilters {
    internal static class Extensions {
        private static readonly Type[] oneOfDefinitions = new[] {
            typeof(OneOf<>),
            typeof(OneOf<,>),
            typeof(OneOf<,,>),
            typeof(OneOf<,,,>),
            typeof(OneOf<,,,,>),
            typeof(OneOf<,,,,,>),
            typeof(OneOf<,,,,,,>),
            typeof(OneOf<,,,,,,,>),
            typeof(OneOf<,,,,,,,,>)
            // Currently, types inheriting from OneOfBase are unsupported
            // until we find a way to leverage the implicit conversions on OneOfBase
            //typeof(OneOfBase<>),
            //typeof(OneOfBase<,>),
            //typeof(OneOfBase<,,>),
            //typeof(OneOfBase<,,,>),
            //typeof(OneOfBase<,,,,>),
            //typeof(OneOfBase<,,,,,>),
            //typeof(OneOfBase<,,,,,,>),
            //typeof(OneOfBase<,,,,,,,>),
            //typeof(OneOfBase<,,,,,,,,>)
        };

        internal static Type? OneOfType(this Type t) {
            t = t.UnderlyingIfNullable();
            t = t.UnderlyingIfNullable();
            return
                t.IsGenericType && t.GetGenericTypeDefinition().In(oneOfDefinitions) ?
                    t :
                    null;

            /*var current = t;
            while (current is { }) {
                if (current.IsGenericType) {
                    var def = current.GetGenericTypeDefinition();
                    if (def.In(OneOfDefinitions)) {
                        return current;
                    }
                }
                current = current.BaseType;
            }
            return null;*/
        }

        internal static Type[] OneOfSubtypes(this Type t) {
            var type = t.OneOfType();
            return type is null ?
                Array.Empty<Type>() :
                type.GetGenericArguments();
        }

    }
}
