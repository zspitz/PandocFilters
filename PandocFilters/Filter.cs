using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;
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

        private static void run<TPandoc>(JsonConverter[] converters, OneOf<FilterBase<TPandoc>, IVisitor<TPandoc>>[] transforms) {
            string? s;
            while (true) {
                s = Console.ReadLine();
                if (s is null) { break; }

                if (transforms.None()) {
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
                foreach (var transform in transforms) {
                    pandoc = transform.Match(
                        filter => filter.Parse(pandoc),
                        visitor => visitor.VisitPandoc(pandoc)
                    );
                }
                var serialized = JsonConvert.SerializeObject(pandoc, settings);
                Console.WriteLine(serialized);
            }
        }

        public static void Run(params OneOf<FilterBase, VisitorBase>[] transforms) =>
            run(
                higherConverters, 
                transforms.Cast<OneOf<FilterBase<Types.Pandoc>, IVisitor<Types.Pandoc>>>().ToArray()
            );
        public static void Run(params OneOf<RawFilterBase, RawVisitorBase>[] transforms) =>
            run(
                rawConverters, 
                transforms.Cast<OneOf<FilterBase<RawTypes.RawPandoc>, IVisitor<RawTypes.RawPandoc>>>().ToArray()
            );
    }
}
