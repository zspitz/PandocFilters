using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandocFilters
{
    public static class Extensions
    {
        // based on https://stackoverflow.com/a/8728686/111794
        public static Func<T1, TResult> WrapWith<T1, T2, TResult>(this Func<T1, T2> innerFunc, Func<T2, TResult> outerFunc) => 
            arg => outerFunc(innerFunc(arg));

        public static Func<T, T> Chain<T>(this IEnumerable<Func<T, T>> src) =>
            src.Aggregate((running, next) => running.WrapWith(next));
    }
}
