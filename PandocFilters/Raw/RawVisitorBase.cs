using OneOf;
using System.Collections.Immutable;
using System.Linq;
using DataValue = OneOf.OneOf<PandocFilters.Raw.TagContent?, string, long, double, PandocFilters.Raw.TagContent1, PandocFilters.Raw.Citation>;
using ZSpitz.Util;
using static ZSpitz.Util.Functions;

namespace PandocFilters.Raw {
    public abstract class RawVisitorBase : IVisitor<RawPandoc> {
        public virtual RawPandoc VisitPandoc(RawPandoc rawPandoc) =>
            rawPandoc with
            {
                Blocks = rawPandoc.Blocks.Select(VisitTagContent).ToImmutableList()!
            };

        public virtual TagContent? VisitTagContent(TagContent? tagContent) => 
            tagContent is null
                ? null
                : (tagContent with
                {
                    C = tagContent.C?.Match<OneOf<ImmutableDictionary<string, DataValue>, DataValue[], DataValue>>(
                        dict => dict.SelectKVP((key, val) => KVP(key, VisitDataValue(val))).ToImmutableDictionary(),
                        arr => arr.Select(VisitDataValue).ToArray(),
                        dv => VisitDataValue(dv)
                    )
                });

        public virtual DataValue VisitDataValue(DataValue dv) => dv.Match<DataValue>(
                tagContent => VisitTagContent(tagContent),
                s => s,
                l => l,
                d => d,
                tagContent1 => {
                    var ret = new TagContent1();
                    tagContent1?.Select(VisitDataValue).AddRangeTo(ret);
                    return ret;
                },
                citation => VisitCitation(citation)
            );

        public virtual Citation VisitCitation(Citation citation) => citation;
    }
}
