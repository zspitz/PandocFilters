using PandocFilters.Ast;
using System;
using System.Collections.Generic;

namespace PandocFilters {
    public static class Extensions {
        // based on https://stackoverflow.com/a/8728686/111794
        internal static Func<T1, TResult> WrapWith<T1, T2, TResult>(this Func<T1, T2> innerFunc, Func<T2, TResult> outerFunc) =>
            arg => outerFunc(innerFunc(arg));

        public static IEnumerable<Inline> ToInlines(this string s) {
            var firstWord = true;
            foreach (var word in s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) {
                if (firstWord) {
                    firstWord = false;
                } else {
                    yield return new Space();
                }
                yield return new Str(word);
            }
        }
    }
}
