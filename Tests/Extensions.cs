using System.Collections.Generic;
using Xunit;

namespace Tests {
    internal static class Extensions {
        internal static TheoryData<T1> ToTheoryData<T1>(this IEnumerable<T1> src) {
            var ret = new TheoryData<T1>();
            foreach (var a in src) {
                ret.Add(a);
            }
            return ret;
        }
        internal static TheoryData<T1, T2> ToTheoryData<T1, T2>(this IEnumerable<(T1, T2)> src) {
            var ret = new TheoryData<T1, T2>();
            foreach (var (a, b) in src) {
                ret.Add(a, b);
            }
            return ret;
        }

    }
}
