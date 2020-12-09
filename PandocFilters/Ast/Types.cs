using System.Collections.Immutable;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace PandocFilters.Ast {

    public record Pandoc(
        [property:JsonProperty("pandoc-api-version")] int[] ApiVersion, 
        ImmutableDictionary<string, MetaValue> Meta, 
        ImmutableList<Block> Blocks
    );

    // replace Meta with ImmutableDictionary<string, MetaValue>

    public class MetaValue : OneOfBase<
        ImmutableDictionary<string, MetaValue>,
        ImmutableList<MetaValue>,
        bool,
        string,
        ImmutableList<Inline>,
        ImmutableList<Block>
    > {
        public MetaValue(OneOf<
            ImmutableDictionary<string, MetaValue>,
            ImmutableList<MetaValue>,
            bool,
            string,
            ImmutableList<Inline>,
            ImmutableList<Block>
        > value) : base(value) { }
    }

    public abstract record Block;

    public record Plain(ImmutableList<Inline> Inlines) : Block;
    public record Para(ImmutableList<Inline> Inlines) : Block;
    public record LineBlock(ImmutableList<ImmutableList<Inline>> NestedInlines) : Block;
    public record CodeBlock(Attr Attr, string Code) : Block;
    public record RawBlock(string Format, string Text) : Block;
    public record BlockQuote(ImmutableList<Block> Blocks) : Block;
    public record OrderedList(ListAttributes ListAttributes, ImmutableList<ImmutableList<Block>> NestedBlocks) : Block;
    public record BulletList(ImmutableList<ImmutableList<Block>> NestedBlocks) : Block;
    public record DefinitionList(ImmutableList<(ImmutableList<Inline> term, ImmutableList<ImmutableList<Block>> definitions)> Items) : Block;
    public record Header(int Level, Attr Attr, ImmutableList<Inline> Text) : Block;
    public record HorizontalRule : Block;
    public record Table(
        Attr Attr,
        Caption Caption,
        ImmutableList<(Alignment Alignment, OneOf<double, ColWidth> ColWidth)> ColSpecs,
        TableHead TableHead,
        ImmutableList<TableBody> TableBodies,
        TableFoot TableFoot
    ) : Block;
    public record Div(Attr Attr, ImmutableList<Block> Blocks) : Block;
    public record Null : Block;

    public abstract record Inline;

    public record Str(string Text) : Inline;
    public record Emph(ImmutableList<Inline> Inlines) : Inline;
    public record Underline(ImmutableList<Inline> Inlines) : Inline;
    public record Strong(ImmutableList<Inline> Inlines) : Inline;
    public record Strikeout(ImmutableList<Inline> Inlines) : Inline;
    public record Superscript(ImmutableList<Inline> Inlines) : Inline;
    public record Subscript(ImmutableList<Inline> Inlines) : Inline;
    public record SmallCaps(ImmutableList<Inline> Inlines) : Inline;
    public record Quoted(QuoteType QuoteType, ImmutableList<Inline> Inlines) : Inline;
    public record Cite(ImmutableList<Citation> Citations, ImmutableList<Inline> Inlines) : Inline;
    public record Code(Attr Attr, string Text) : Inline;
    public record Space : Inline;
    public record SoftBreak : Inline;
    public record LineBreak : Inline;
    public record Math(MathType MathType, string Text) : Inline;
    public record RowInline(string Format, string Text) : Inline;
    public record Link(Attr Attr, ImmutableList<Inline> Inlines, (string Url, string Title) Target) : Inline;
    public record Image(Attr Attr, ImmutableList<Inline> Inlines, (string Url, string Title) Target) : Inline;
    public record Note(ImmutableList<Block> Blocks) : Inline;
    public record Span(Attr Attr, ImmutableList<Inline> Inlines) : Inline;

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
    public record Attr(string Identifier, ImmutableList<string> Classes, ImmutableList<(string, string)> KeyValuePairs);

    /// <summary>The caption of a table, with an optional short caption.</summary>
    public record Caption(ImmutableList<Inline>? ShortCaption, ImmutableList<Block> Blocks);

    // replace ShortCaption with ImmutableList<Inline>

    // replace RowHeadColumns with int

    public enum Alignment {
        AlignLeft,
        AlignRight,
        AlignCenter,
        AlignDefault
    }

    public enum ColWidth {
        ColWidthDefault
    }

    // replace ColSpec with (Alignment Alignment, OneOf<double, ColWidth> ColWidth)

    public record Row(Attr Attr, ImmutableList<Cell> Cells);

    public record TableHead(Attr Attr, ImmutableList<Row> Rows);

    public record TableBody(Attr Attr, int RowReadColumns, ImmutableList<Row> Rows1, ImmutableList<Row> Rows2);

    public record TableFoot(Attr Attr, ImmutableList<Row> Rows);

    /// <summary>A table cell</summary>
    public record Cell(Attr Attr, Alignment Alignment, int RowSpan, int ColSpan, ImmutableList<Block> Blocks);

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

    public record Citation(string CitationId, ImmutableList<Inline> CitationPrefix, ImmutableList<Inline> CitationSuffix, CitationMode CitationMode, int CitationNoteNum, int CitationHash);

    public enum CitationMode {
        AuthorInText,
        SuppressAuthor,
        NormalCitation
    }
}
