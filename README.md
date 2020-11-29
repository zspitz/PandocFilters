# PandocFilters

Write Pandoc filters in .NET, using  strongly-typed data structures for the Pandoc AST.

> This is very much still a work in progress; in particular, FilterBase cannot be used yet -- it's missing a converter.

## Pandoc filters

[Pandoc](https://pandoc.org/) is a command-line program and Haskell library for converting documents from and to many different formats. Documents are translated from one format to an AST (defined in the Haskell [Text.Pandoc.Definition](https://hackage.haskell.org/package/pandoc-types-1.22/docs/Text-Pandoc-Definition.html) module), which is then used to create the output format.

Pandoc allows writing [**filters**](https://pandoc.org/filters.html) -- programs that intercept the AST as JSON from standard input, modify the AST, and write it back out to standard output. Filters can be run using the pipe operator (`|` on Linux, `>` on Windows):

```none
pandoc -s input.md -t json | my-filter | pandoc -s -f json -o output.html
```

or using the Pandoc `--filter` command-line option:

```none
pandoc -s input.md --filter my-filter -o output.html
```

## Pandoc AST

Much of the AST comes in the form of objects with a `t` and `c` property:

```json
{
    "t": "Para",
    "c": [

    ]
}
```

This corresponds to a `PandocFilters.Types.Para` object with it's properties filled with the values at the `c` property.

The library defines types for both levels:

* the "raw" types -- objects with `t` and `c` properties -- are in the `PandocFilters.Raw` namespace; and can be accessed by inherting from `PandocFilters.RawFilterBase`.
* the higher-level AST types are in the `PandocFilters.Types` namespace; and can be accessed by inheriting from `PandocFilters.

## Usage

1. Create a console application.
2. Install the `PandocFilters` NuGet package (not yet available, https://github.com/zspitz/PandocFilters/issues/2)
3. Write a class inheriting from `PandocFilters.FilterBase` or `PandocFilters.RawFilterBase`.
4. In the `Main` method of your application:
   1. create a new instance of the class.
   2. Call the `Loop` method on the class.
5. Either pass your program to Pandoc using `--filter`; or pipe the JSON output from Pandoc into your program, and pipe the outout back into Pandoc.

## Sample

```csharp
using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.Types;

var filter = new RemoveImageStyling();
filter.Loop();

class RemoveImageStyling : FilterBase {
    protected override Pandoc Parse(Pandoc pandoc) {
        foreach (var img in pandoc.Blocks.OfType<Image>()) {
            img.Attr.KeyValuePairs.Clear();
        }
        return pandoc;
    }
}
```

## Debugging

There is an issue with debugging; see https://github.com/zspitz/PandocFilters/issues/4.
