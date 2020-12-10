using OneOf;
using System;
using DataValue = OneOf.OneOf<PandocFilters.Raw.TagContent?, string, long, PandocFilters.Raw.TagContent1>;
using static PandocFilters.Functions;

namespace PandocFilters.Raw {
    internal sealed class RawDelegateVisitor : RawVisitorBase {
        private Func<RawPandoc, RawPandoc>? rawPandocDelegate;
        private Func<TagContent?, TagContent?>? tagContentDelegate;
        private Func<DataValue, DataValue>? dataValueDelegate;

        internal RawDelegateVisitor(OneOf<
                Func<RawPandoc, RawPandoc>,
                Func<TagContent?, TagContent?>,
                Func<DataValue, DataValue>
            >[] delegates) {

            foreach (var del in delegates) {
                del.Switch(
                    d => AddDelegate(ref rawPandocDelegate, d),
                    d => AddDelegate(ref tagContentDelegate, d),
                    d => AddDelegate(ref dataValueDelegate, d)
                );
            }
        }

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
