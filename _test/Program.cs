using System;
using System.Diagnostics;
using PandocFilters;
using PandocFilters.RawTypes;
using PandocFilters.Types;

Debugger.Launch();
var filter = new TestFilter();
filter.Loop();

class TestFilter : RawFilterBase {
    protected override RawPandoc Parse(RawPandoc pandoc) => pandoc;
}