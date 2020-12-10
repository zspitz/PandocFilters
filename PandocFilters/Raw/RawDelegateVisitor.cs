using System;
using DataValue = OneOf.OneOf<PandocFilters.Raw.TagContent?, string, long, PandocFilters.Raw.TagContent1>;
using static PandocFilters.Functions;

namespace PandocFilters.Raw {
    public sealed class RawDelegateVisitor : RawVisitorBase {
        private Func<RawPandoc, RawPandoc>? rawPandocDelegate;
        private Func<TagContent?, TagContent?>? tagContentDelegate;
        private Func<DataValue, DataValue>? dataValueDelegate;

        public void Add(Func<RawPandoc, RawPandoc> del) => AddDelegate(ref rawPandocDelegate, del);
        public void Add(Func<TagContent?, TagContent?> del) => AddDelegate(ref tagContentDelegate, del);
        public void Add(Func<DataValue, DataValue> del) => AddDelegate(ref dataValueDelegate, del);

        public override RawPandoc VisitPandoc(RawPandoc rawPandoc) {
            rawPandoc = rawPandocDelegate?.Invoke(rawPandoc) ?? rawPandoc;
            return base.VisitPandoc(rawPandoc);
        }
        public override DataValue VisitDataValue(DataValue dv) {
            dv = dataValueDelegate?.Invoke(dv) ?? dv;
            return base.VisitDataValue(dv);
        }
        public override TagContent? VisitTagContent(TagContent? tagContent) {
            tagContent = tagContentDelegate?.Invoke(tagContent) ?? tagContent;
            return base.VisitTagContent(tagContent);
        }
    }
}
