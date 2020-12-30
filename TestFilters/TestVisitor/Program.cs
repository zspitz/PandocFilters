using PandocFilters;
using PandocFilters.Ast;

Filter.Run(new TestVisitor());

class TestVisitor : VisitorBase { }
