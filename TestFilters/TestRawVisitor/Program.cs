using PandocFilters;
using PandocFilters.Raw;

Filter.Run(new TestRawVisitor());

class TestRawVisitor : RawVisitorBase { }
