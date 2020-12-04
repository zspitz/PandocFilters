using Newtonsoft.Json;
using PandocFilters.RawTypes;
using PandocFilters.Types;

namespace PandocFilters {
    public abstract class FilterBase<TPandoc> {
        protected internal abstract TPandoc Parse(TPandoc pandoc);
        internal readonly JsonConverter[] converters;
        protected FilterBase(params JsonConverter[] converters) => this.converters = converters;
    }

    public abstract class FilterBase : FilterBase<Pandoc> {
        protected FilterBase() : base(
            new OneOfJsonConverter(),
            new PandocTypesConverter(),
            new TupleConverter()
        ) { }
    }

    public abstract class RawFilterBase : FilterBase<RawPandoc> {
        protected RawFilterBase() : base(
            new OneOfJsonConverter()
        ) { }
    }
}
