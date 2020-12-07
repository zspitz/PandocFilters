using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSpitz.Util;

namespace Generator {
    public static class Functions {
        public static void Prompt(string message, out int ret) =>
            Prompt(message, out ret, s => (
                !int.TryParse(s, out var result) ? "Not a number." : "", 
                result
            ));

        // TODO we could change the returned error from string to OneOf<string, bool> and use a generic error message when returning false
        public  static void Prompt<T>(string message, out T ret, Func<string?, (string error, T result)> parserValidator) {
            string error;
            while (true) {
                Console.WriteLine(message);
                Console.WriteLine("? ");
                (error, ret) = parserValidator(Console.ReadLine());
                if (error.IsNullOrWhitespace()) { break; }
                Console.WriteLine(error);
                Console.WriteLine();
            }
        }

        public static (bool isGeneric, Type? firstArg, Type? def) Generics(Type t) =>
            t.IsGenericType ?
                (true, t.GetGenericArguments()[0], t.GetGenericTypeDefinition()) :
                (false, null, null);
    }
}
