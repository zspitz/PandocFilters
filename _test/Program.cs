using System;
using System.Collections.Immutable;
using System.Diagnostics;
using PandocFilters;
using PandocFilters.Ast;
using PandocFilters.Raw;

Debugger.Launch();

//var visitor = new TestFilter();
//var visitor = new RemoveImageStyling();
//Filter.Run(visitor);

//var visitor = new RawDelegateVisitor();
//visitor.Add((TagContent? tc) => tc);
//visitor.Add((RawPandoc pandoc) => pandoc);
//Filter.Run(visitor);

var visitor = new DelegateVisitor();
visitor.Add((Image image) => image with {
    Attr = image.Attr with {
        KeyValuePairs = ImmutableList.Create<(string, string)>()
    }
});
Filter.Run(visitor);

class TestFilter : RawVisitorBase {
    public override RawPandoc VisitPandoc(RawPandoc rawPandoc) => rawPandoc;
}

class RemoveImageStyling : VisitorBase {
    public override Image VisitImage(Image image) =>
        image with
        {
            Attr = image.Attr with
            {
                KeyValuePairs = ImmutableList.Create<(string, string)>()
            }
        };
}
