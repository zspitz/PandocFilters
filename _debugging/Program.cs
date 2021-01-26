using PandocFilters;
using PandocFilters.Ast;
using PandocFilters.Raw;
using System.Diagnostics;

Debugger.Launch();

Filter.Run(new TestVisitor());

class TestVisitor : VisitorBase {
    public override Pandoc VisitPandoc(Pandoc pandoc) =>
        base.VisitPandoc(
            pandoc with
            {
                Meta = pandoc.Meta.Add("sort2", "0")
            }
        );
}

class TestRawVisitor : RawVisitorBase { }
