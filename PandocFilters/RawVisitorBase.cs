using OneOf;
using PandocFilters.RawTypes;
using System.Collections.Immutable;
using System.Linq;
using DataValue = OneOf.OneOf<PandocFilters.RawTypes.TagContent, string, long, PandocFilters.RawTypes.TagContent1>;
using ZSpitz.Util;

namespace PandocFilters {
    public abstract class RawVisitorBase : IVisitor<RawPandoc> {
        public RawPandoc VisitPandoc(RawPandoc rawPandoc) =>
            rawPandoc with
            {
                Blocks = rawPandoc.Blocks.Select(VisitTagContent).ToImmutableList()
            };

        public virtual TagContent VisitTagContent(TagContent tagContent) =>
            tagContent with
            {
                C = tagContent.C?.Match<OneOf<DataValue[], DataValue>>(
                    arr => arr.Select(VisitDataValue).ToArray(),
                    dv => VisitDataValue(dv)
                )
            };

        public virtual DataValue VisitDataValue(DataValue dv) => dv.Match<DataValue>(
                tagContent => VisitTagContent(tagContent),
                s => s,
                l => l,
                tagContent1 => {
                    var ret = new TagContent1();
                    tagContent1.Select(VisitDataValue).AddRangeTo(ret);
                    return ret;
                }
            );
    }
}
