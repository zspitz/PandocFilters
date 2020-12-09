using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.Ast;

Debugger.Launch();

var visitor = new RemoveImageStyling();
Filter.Run(visitor);

class RemoveImageStyling : VisitorBase {
    public override Image VisitImage(Image image) =>
        image with {
            Attr = image.Attr with {
                KeyValuePairs = ImmutableList.Create<(string, string)>()
            }
        };
}
