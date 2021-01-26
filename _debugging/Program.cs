using PandocFilters;
using PandocFilters.Ast;
using PandocFilters.Raw;
using System.Diagnostics;

Debugger.Launch();

Filter.Run(new TestVisitor());

class TestVisitor : VisitorBase { }

class TestRawVisitor : RawVisitorBase { }
