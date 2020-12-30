using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZSpitz.Util;
using PandocFilters.Ast;
using PandocFilters.Raw;
using Newtonsoft.Json.Linq;

namespace PandocFilters {
    public static class Filter {
        private static readonly JsonConverter[] rawConverters = new[] {
            new OneOfJsonConverter() {
                tokenHandler = (token, objectType, serializer) => {
                    (object?, Type?) ret = (null,null);
                    if (token is JObject obj && obj.ContainsKey("citationId")) {
                        ret = (
                            token.ToObject<Raw.Citation>(serializer),
                            typeof(Raw.Citation)
                        );
                    }
                    return ret;
                }
            }
        };

        private static readonly JsonConverter[] higherConverters = new JsonConverter[] {
            new PandocTypesConverter(),
            new OneOfJsonConverter(),
            new TupleConverter()
        };

        private static void run<TPandoc>(JsonConverter[] converters, params IVisitor<TPandoc>[] visitors) {
            string? s;
            while (true) {
                s = Console.ReadLine();
                if (s is null) { break; }

                if (visitors.None()) {
                    Console.WriteLine(s);
                    continue;
                }

                var settings = new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                    Converters = converters,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateParseHandling = DateParseHandling.None,
                    MissingMemberHandling = MissingMemberHandling.Error
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
