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
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition().In(oneOfDefinitions));

        internal static Type[] OneOfSubtypes(this Type t) => 
            t.OneOfType()?.GetGenericArguments() ?? Array.Empty<Type>();
    }
}
