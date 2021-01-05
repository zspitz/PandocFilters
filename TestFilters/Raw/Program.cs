using PandocFilters;
using PandocFilters.Raw;

Filter.Run(
    new TestRawVisitor(),
    new RawDelegateVisitor()
);

class TestRawVisitor : RawVisitorBase { }

