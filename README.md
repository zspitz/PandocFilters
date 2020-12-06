# PandocFilters

[![AppVeyor build status](https://img.shields.io/appveyor/ci/zspitz/pandocfilters?style=flat&max-age=86400)](https://ci.appveyor.com/project/zspitz/pandocfilters) [![NuGet Status](https://img.shields.io/nuget/v/pandocfilters.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/pandocfilters/)

Write Pandoc filters in .NET, using  strongly-typed data structures for the Pandoc AST.

## Pandoc filters

[Pandoc](https://pandoc.org/) is a command-line program and Haskell library for converting documents from and to many different formats. Documents are translated from the input format to an AST (defined in the [Text.Pandoc.Definition](https://hackage.haskell.org/package/pandoc-types-1.22/docs/Text-Pandoc-Definition.html) module), which is then used to create the output format.

Pandoc allows writing [**filters**](https://pandoc.org/filters.html) -- programs that intercept the AST as JSON from standard input, modify the AST, and write it back out to standard output. Filters can be run using the pipe operator (`|` on Linux, `>` on Windows):

```none
pandoc -s input.md -t json | my-filter | pandoc -s -f json -o output.html
```

or using the Pandoc `--filter` command-line option:

```none
pandoc -s input.md --filter my-filter -o output.html
```

## Pandoc AST

Much of the JSON-serialized AST comes in the form of objects with a `t` and `c` property:

```json
{
    "t": "Para",
    "c": [

    ]
}
```

This corresponds to a `PandocFilters.Types.Para` object with properties filled with the values at the `c` property.

The library defines types for both levels:

* the "raw" types -- objects with `t` and `c` properties -- are in the `PandocFilters.Raw` namespace; and can be accessed by inherting from `PandocFilters.RawFilterBase`.
* the higher-level AST types are in the `PandocFilters.Types` namespace; and can be accessed by inheriting from `PandocFilters.FilterBase`.

## Usage

1. Create a console application.
2. Install the `PandocFilters` NuGet package.
3. Write a class inheriting from `PandocFilters.FilterBase` or `PandocFilters.RawFilterBase`.
4. In the `Main` method of your application:
   1. create a new instance of the filter class.
   2. Pass this instance into `Filter.Loop`.
5. Either pass your program to Pandoc using `--filter`; or pipe the JSON output from Pandoc into your program, and pipe the outout back into Pandoc.

## Sample

```csharp
using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.Types;

var filter = new RemoveImageStyling();
Filter.Run(filter);

class RemoveImageStyling : FilterBase {
    protected override Pandoc Parse(Pandoc pandoc) {
        foreach (var img in pandoc.Blocks.OfType<Image>()) {
            img.Attr.KeyValuePairs.Clear();
        }
        return pandoc;
    }
}
// Note that this will only remove styling from top-level images; it doesn't recurse.
// Pending https://github.com/zspitz/PandocFilters/issues/8
```

## Credits

* John McFarlane and contributors for Pandoc
* [dbramucci](https://www.reddit.com/user/dbramucci) and [Lalaithion42](https://www.reddit.com/user/Lalaithion42) for their [help in understanding Haskell data types](https://www.reddit.com/r/haskell/comments/jx9lf7/basic_guide_to_reading_haskell_type_definition/)

## Notes

* PandocFilters is written against the types in [**pandoc-types 1.22**](https://hackage.haskell.org/package/pandoc-types-1.22). When pandoc-types is updated, code written against the raw types will successfully receive the JSON-source data structures; while code written against the higher-level types will conceivably fail in the JSON parsing stage.
