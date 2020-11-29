using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PandocFilters.RawTypes;
using PandocFilters.Types;

namespace PandocFilters {
    public abstract class FilterBase<TPandoc> {
        protected abstract TPandoc Parse(TPandoc pandoc);
        private readonly JsonConverter[] converters;
        public FilterBase(params JsonConverter[] converters) => this.converters = converters;

        public virtual void Loop() {
            string? s;
            while (true) {
                s = Console.ReadLine();
                if (s is null) { break; }

                var settings = new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new KebabCaseNamingStrategy() },
                    Converters = converters,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var pandoc = JsonConvert.DeserializeObject<TPandoc>(s, settings)!;
                var output = Parse(pandoc);
                var serialized = JsonConvert.SerializeObject(output, settings);
                Console.WriteLine(serialized);
            }
        }
    }

    //public abstract class FilterBase : FilterBase<Pandoc> {
    //    protected FilterBase() : base(new OneOfJsonConverter()) { }
    //}

    public abstract class RawFilterBase : FilterBase<RawPandoc> {
        protected RawFilterBase() : base(new OneOfJsonConverter()) { }
    }
}
