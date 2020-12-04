using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using ZSpitz.Util;

namespace PandocFilters {
    internal static class Extensions {
        private static readonly HashSet<Type> oneOfDefinitions = new HashSet<Type> {
            typeof(OneOf<>),
            typeof(OneOf<,>),
            typeof(OneOf<,,>),
            typeof(OneOf<,,,>),
            typeof(OneOf<,,,,>),
            typeof(OneOf<,,,,,>),
            typeof(OneOf<,,,,,,>),
            typeof(OneOf<,,,,,,,>),
            typeof(OneOf<,,,,,,,,>),
            typeof(OneOfBase<>),
            typeof(OneOfBase<,>),
            typeof(OneOfBase<,,>),
            typeof(OneOfBase<,,,>),
            typeof(OneOfBase<,,,,>),
            typeof(OneOfBase<,,,,,>),
            typeof(OneOfBase<,,,,,,>),
            typeof(OneOfBase<,,,,,,,>),
            typeof(OneOfBase<,,,,,,,,>)
        };

        internal static Type? OneOfType(this Type t) =>
            t.BaseTypes(false, true)
                .FirstOrDefault(x => {
                    if (!x.IsGenericType) { return false; }
                    return x.GetGenericTypeDefinition().In(oneOfDefinitions);
                });

        internal static Type[] OneOfSubtypes(this Type t) {
            var type = t.OneOfType();
            return type is null ?
                Array.Empty<Type>() :
                type.GetGenericArguments();
        }
    }
}
