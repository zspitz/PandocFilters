using PandocFilters.RawTypes;
using PandocFilters.Types;

namespace PandocFilters {
    public abstract class FilterBase<TPandoc> {
        internal protected abstract TPandoc Parse(TPandoc pandoc);
    }

    public abstract class FilterBase : FilterBase<Pandoc> { }

    public abstract class RawFilterBase : FilterBase<RawPandoc> { }
}
