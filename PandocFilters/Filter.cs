using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZSpitz.Util;

namespace PandocFilters {
    public static class Filter {
        private static void run<TPandoc>(params FilterBase<TPandoc>[] filters) {
            string? s;
            while (true) {
                s = Console.ReadLine();
                if (s is null) { break; }

                if (filters.None()) {
                    Console.WriteLine(s);
                    continue;
                }

                var settings = new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new KebabCaseNamingStrategy() },
                    Converters = filters[0].converters,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var start = JsonConvert.DeserializeObject<TPandoc>(s, settings)!;
                var pandoc = start;
                foreach (var filter in filters) {
                    pandoc = filter.Parse(pandoc);
                }
                var serialized = JsonConvert.SerializeObject(pandoc, settings);
                Console.WriteLine(serialized);
            }
        }

        public static void Run(params FilterBase[] filters) => run(filters);
        public static void Run(params RawFilterBase[] filters) => run(filters);
    }
}
