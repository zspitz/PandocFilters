using PandocFilters;
using PandocFilters.Ast;

Filter.Run(
    new TestVisitor(),
    new DelegateVisitor()
);

class TestVisitor : VisitorBase { }
