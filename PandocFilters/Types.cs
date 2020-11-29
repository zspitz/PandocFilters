using System.Collections.Generic;
using OneOf;

namespace PandocFilters.Types {
    public record Pandoc(int[] ApiVersion, IDictionary<string, MetaValue> Meta, ICollection<Block> Blocks);

    // replace Meta with Dictionary<string, MetaValue>

    public class MetaValue : OneOfBase<
        Dictionary<string, MetaValue>,
        ICollection<MetaValue>,
        bool,
        string,
        ICollection<Inline>,
        ICollection<Block>
    > { }

    public abstract record Block;

    public record Plain(ICollection<Inline> Inlines) : Block;
    public record Para(ICollection<Inline> Inlines) : Block;
    public record LineBlock(ICollection<ICollection<Inline>> NestedInlines) : Block;
    public record CodeBlock(Attr Attr, string Code) : Block;
    public record RawBlock(string Format, string Text) : Block;
    public record BlockQuote(ICollection<Block> Blocks) : Block;
    public record OrderedList(ListAttributes ListAttributes, ICollection<ICollection<Block>> NestedBlocks);
    public record BulletList(ICollection<ICollection<Block>> NestedBlocks);
    public record DefinitionList(ICollection<(ICollection<Inline> term, ICollection<ICollection<Block>> definitions)> Items);
    public record Header(int Level, Attr Attr, ICollection<Inline> Text);
    public record HorizontalRule : Block;
    public record Table(
        Attr Attr,
        Caption Caption,
        ICollection<(Alignment Alignment, double? ColWidth)> ColSpecs,
        TableHead TableHead,
        ICollection<TableBody> TableBodies,
        TableFoot TableFoot
    ) : Block;
    public record Div(Attr Attr, ICollection<Block> Blocks);
    public record Null : Block;

    public abstract record Inline;

    public record Str(string Text) : Inline;
    public record Emph(ICollection<Inline> Inlines) : Inline;
    public record Underline(ICollection<Inline> Inlines) : Inline;
    public record Strong(ICollection<Inline> Inlines) : Inline;
    public record Strikeout(ICollection<Inline> Inlines) : Inline;
    public record Superscript(ICollection<Inline> Inlines) : Inline;
    public record Subscript(ICollection<Inline> Inlines) : Inline;
    public record SmallCaps(ICollection<Inline> Inlines) : Inline;
    public record Quoted(QuoteType QuoteType, ICollection<Inline> Inlines) : Inline;
    public record Cite(ICollection<Citation> Citations, ICollection<Inline> Inlines) : Inline;
    public record Code(Attr Attr, string Text) : Inline;
    public record Space : Inline;
    public record SoftBreak : Inline;
    public record LineBreak : Inline;
    public record Math(MathType MathType, string Text) : Inline;
    public record RowInline(string Format, string Text) : Inline;
    public record Link(Attr Attr, ICollection<Inline> Inlines, (string Url, string Title) Target) : Inline;
    public record Image(Attr Attr, ICollection<Inline> Inlines, (string Url, string Title) Target) : Inline;
    public record Note(ICollection<Block> Blocks) : Inline;
    public record Span(Attr Attr, ICollection<Inline> Inlines) : Inline;

    public record ListAttributes(int StartNumber, ListNumberStyle ListNumberStyle, ListNumberDelim ListNumberDelim);

    public enum ListNumberStyle {
        DefaultStyle,
        Example,
        Decimal,
        LowerRoman,
        UpperRoman,
        LowerAlpha,
        UpperAlpha
    }

    public enum ListNumberDelim {
        DefaultDelim,
        Period,
        OneParen,
        TwoParen
    }

    // replace Format with string

    // <summary>Attributes: identifier, classes, key-value pairs</summary>
    public record Attr(string Identifier, ICollection<string> Classes, IDictionary<string, string> KeyValuePairs);

    /// <summary>The caption of a table, with an optional short caption.</summary>
    public record Caption(ICollection<Inline>? ShortCaption, ICollection<Block> Blocks);

    // replace ShortCaption with ICollection<Inline>

    // replace RowHeadColumns with int

    public enum Alignment {
        AlignLeft,
        AlignRight,
        AlignCenter,
        AlignDefault
    }

    // replace ColWidth with Nullable<double>

    // replace ColSpec with (Alignment Alignment, double? ColWidth)

    public record Row(Attr Attr, ICollection<Cell> Cells);

    public record TableHead(Attr Attr, ICollection<Row> Rows);

    public record TableBody(Attr Attr, int RowReadColumns, ICollection<Row> Rows1, ICollection<Row> Rows2);

    public record TableFoot(Attr Attr, ICollection<Row> Rows);

    /// <summary>A table cell</summary>
    public record Cell(Attr Attr, Alignment Alignment, int RowSpan, int ColSpan, ICollection<Block> Blocks);

    // replace ColSpan and RowSpan with named int

    public enum QuoteType {
        SingleQuote,
        DoubleQuote
    }

    // replace Target with (string Url, string title)

    public enum MathType {
        DisplayMath,
        InlineMath
    }

    public record Citation(string CitationId, ICollection<Inline> CitationPrefix, ICollection<Inline> CitationSuffix, CitationMode CitationMode, int CitationNoteNum, int CitationHash);

    public enum CitationMode {
        AuthorInText,
        SuppressAuthor,
        NormalCitation
    }
}
