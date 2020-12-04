using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.RawTypes;
using PandocFilters.Types;

Debugger.Launch();

//var filter = new TestFilter();
var filter = new RemoveImageStyling();
filter.Loop();

class TestFilter : RawFilterBase {
    protected override RawPandoc Parse(RawPandoc pandoc) => pandoc;
}

class RemoveImageStyling : FilterBase {
    protected override Pandoc Parse(Pandoc pandoc) {
        foreach (var img in pandoc.Blocks.OfType<Image>()) {
            img.Attr.KeyValuePairs.Clear();
        }
        return pandoc;
    }
}
