using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.RawTypes;
using PandocFilters.Types;

Debugger.Launch();

//var filter = new TestFilter();
var visitor = new RemoveImageStyling();
Filter.Run(visitor);

class TestFilter : RawFilterBase {
    protected override RawPandoc Parse(RawPandoc pandoc) => pandoc;
}

class RemoveImageStyling : VisitorBase {
    public override Image VisitImage(Image image) =>
        image with {
            Attr = image.Attr with {
                KeyValuePairs = ImmutableList.Create<(string, string)>()
            }
        };
}
