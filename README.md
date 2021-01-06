# PandocFilters

[![AppVeyor build status](https://img.shields.io/appveyor/ci/zspitz/pandocfilters?style=flat&max-age=86400)](https://ci.appveyor.com/project/zspitz/pandocfilters) [![NuGet Status](https://img.shields.io/nuget/v/pandocfilters.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/pandocfilters/)

Write Pandoc filters in .NET, using  strongly-typed data structures for the Pandoc AST.

## Pandoc filters

[Pandoc](https://pandoc.org/) is a command-line program and Haskell library for converting documents from and to many different formats. Documents are translated from the input format to an AST (defined in the [Text.Pandoc.Definition](https://hackage.haskell.org/package/pandoc-types-1.22/docs/Text-Pandoc-Definition.html) module), which is then used to create the output format.

Pandoc allows writing [**filters**](https://pandoc.org/filters.html) &mdash; programs that intercept the AST as JSON from standard input, modify the AST, and write it back out to standard output. Filters can be run using the pipe operator (`|` on Linux, `>` on Windows):

```none
pandoc -s input.md -t json | my-filter | pandoc -s -f json -o output.html
```

or using the Pandoc `--filter` command-line option:

```none
pandoc -s input.md --filter my-filter -o output.html
```

## Pandoc AST

Much of the JSON-serialized AST comes in the form of objects with a `t` and `c` property<sup>1</sup>:

```json
{
    "t": "Para",
    "c": [

    ]
}
```

This corresponds to a `Para` object with properties filled with the values at the `c` property.

The library defines types and base classes for both levels:

| Type level | Description | Namespace | Visitor base class |
| -- | -- | -- | -- |
| Raw | Objects with a `t` and `c` property|  `PandocFilters.Raw` | `RawVisitorBase` |
| Higher-level AST | e.g. `Para` type |`PandocFilters.Ast` | `VisitorBase` |

The library also includes two predefined visitors &mdash; `DelegateVisitor` and `RawDelegateVisitor` &mdash; which can be extended by adding delegates via the `Add` method, instead of defining a new class (see below for sample).

<sup>1. All the types in [pandoc-types](https://hackage.haskell.org/package/pandoc-types-1.22/docs/Text-Pandoc-Definition.html) except for the root [Pandoc](https://hackage.haskell.org/package/pandoc-types-1.22/docs/Text-Pandoc-Definition.html#t:Pandoc) type and the [Citation](https://hackage.haskell.org/package/pandoc-types-1.22/docs/Text-Pandoc-Definition.html#t:Citation) type.</sup>

## Usage

1. Create a console application.
2. Install the `PandocFilters` NuGet package.
3. Define your visitor &mdash; either
   * write a class that inherits from one of the visitor base classes, and create an instance of the class, or  
   * create an instance of the appropriate delegate visitor class, and append delegates using the `Add` methods.
4. Pass the instance into `Filter.Run`.
5. Either pass your program to Pandoc using `--filter`; or pipe the JSON output from Pandoc into your program, and pipe the outout back into Pandoc.

Note that `Filter.Run` takes an arbitrary number of visitors &mdash; you can create multiple visitors and pass them into `Filter.Run`.

## Sample

```csharp
using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.Types;

var visitor = new RemoveImageStyling();
Filter.Run(visitor);

class RemoveImageStyling : VisitorBase {
    public override Image VisitImage(Image image) =>
        image with {
            Attr = image.Attr with {
                KeyValuePairs = ImmutableList.Create<(string, string)>()
            }
        };
}
```

Using the delegate visitor:

```csharp
using System.Diagnostics;
using System.Linq;
using PandocFilters;
using PandocFilters.Types;

var visitor = new DelegateVisitor();
visitor.Add((Image image) => image with {
    Attr = image.Attr with {
        KeyValuePairs = ImmutableList.Create<(string, string)>()
    }
});
Filter.Run(visitor);
```

## Credits

* John McFarlane and contributors for Pandoc
* @mcintyre321, for [OneOf](https://github.com/mcintyre321/OneOf)
* [dbramucci](https://www.reddit.com/user/dbramucci) and [Lalaithion42](https://www.reddit.com/user/Lalaithion42) for their [help in understanding Haskell data types](https://www.reddit.com/r/haskell/comments/jx9lf7/basic_guide_to_reading_haskell_type_definition/)

## Notes

* PandocFilters is written against the types in [**pandoc-types 1.22**](https://hackage.haskell.org/package/pandoc-types-1.22). When pandoc-types is updated, code written against the raw types will successfully receive the JSON-source data structures; while code written against the higher-level types will conceivably fail in the JSON parsing stage.
* The library uses C# 9 record types (and [System.Collections.Immutable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable?view=net-5.0#remarks)) to enforce immutability; otherwise we'd have to check for circular references before serializing. If you're using C# 9 or later, you can use the `with` keyword to clone/initialize the returned instance; otherwise you'll have to pass in all arguments to the constructor.
