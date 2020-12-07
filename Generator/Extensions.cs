using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator {
    public static class Extensions {
        public static string? ToCamelCase(this string s) =>
            s is not null && s.Length >= 1 ?
                char.ToLowerInvariant(s[0]) + s[1..] :
                s;
    }
}
