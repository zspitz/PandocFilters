using PandocFilters;
using PandocFilters.Ast;
using PandocFilters.Raw;
using System.Diagnostics;

Debugger.Launch();

Filter.Run(new TestRawVisitor());

class TestVisitor : VisitorBase { }

class TestRawVisitor : RawVisitorBase { }
