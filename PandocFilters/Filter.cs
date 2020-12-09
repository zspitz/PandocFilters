using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZSpitz.Util;

namespace PandocFilters {
    public static class Filter {
        private static readonly JsonConverter[] rawConverters = new[] {
            new OneOfJsonConverter()
        };

        private static readonly JsonConverter[] higherConverters = new JsonConverter[] {
            new OneOfJsonConverter(),
            new PandocTypesConverter(),
            new TupleConverter()
        };

        private static void run<TPandoc>(JsonConverter[] converters, IVisitor<TPandoc>[] visitors) {
            string? s;
            while (true) {
                s = Console.ReadLine();
                if (s is null) { break; }

                if (visitors.None()) {
                    Console.WriteLine(s);
                    continue;
                }
                
                var settings = new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new KebabCaseNamingStrategy() },
                    Converters = converters,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var start = JsonConvert.DeserializeObject<TPandoc>(s, settings)!;
                var pandoc = start;
                foreach (var visitor in visitors) {
                    pandoc = visitor.VisitPandoc(pandoc);
                }
                var serialized = JsonConvert.SerializeObject(pandoc, settings);
                Console.WriteLine(serialized);
            }
        }

        public static void Run(params VisitorBase[] visitors) =>
            run(higherConverters, visitors);
        public static void Run(params RawVisitorBase[] visitors) =>
            run(rawConverters, visitors);
    }
}
