using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace PandocFilters.Types {

    public record Pandoc(
        [property:JsonProperty("pandoc-api-version")] int[] ApiVersion, 
        IDictionary<string, MetaValue> Meta, 
        ICollection<Block> Blocks
    );

    // replace Meta with Dictionary<string, MetaValue>

    public class MetaValue : OneOfBase<
        Dictionary<string, MetaValue>,
        List<MetaValue>,
        bool,
        string,
        List<Inline>,
        List<Block>
    > {
        public MetaValue(OneOf<
            Dictionary<string, MetaValue>,
            List<MetaValue>,
            bool,
            string,
            List<Inline>,
            List<Block>
        > value) : base(value) { }
    }

    public abstract record Block;

    public record Plain(List<Inline> Inlines) : Block;
    public record Para(List<Inline> Inlines) : Block;
    public record LineBlock(List<List<Inline>> NestedInlines) : Block;
    public record CodeBlock(Attr Attr, string Code) : Block;
    public record RawBlock(string Format, string Text) : Block;
    public record BlockQuote(List<Block> Blocks) : Block;
    public record OrderedList(ListAttributes ListAttributes, List<List<Block>> NestedBlocks) : Block;
    public record BulletList(List<List<Block>> NestedBlocks) : Block;
    public record DefinitionList(List<(List<Inline> term, List<List<Block>> definitions)> Items) : Block;
    public record Header(int Level, Attr Attr, List<Inline> Text) : Block;
    public record HorizontalRule : Block;
    public record Table(
        Attr Attr,
        Caption Caption,
        List<(Alignment Alignment, OneOf<double, ColWidth> ColWidth)> ColSpecs,
        TableHead TableHead,
        List<TableBody> TableBodies,
        TableFoot TableFoot
    ) : Block;
    public record Div(Attr Attr, List<Block> Blocks);
    public record Null : Block;

    public abstract record Inline;

    public record Str(string Text) : Inline;
    public record Emph(List<Inline> Inlines) : Inline;
    public record Underline(List<Inline> Inlines) : Inline;
    public record Strong(List<Inline> Inlines) : Inline;
    public record Strikeout(List<Inline> Inlines) : Inline;
    public record Superscript(List<Inline> Inlines) : Inline;
    public record Subscript(List<Inline> Inlines) : Inline;
    public record SmallCaps(List<Inline> Inlines) : Inline;
    public record Quoted(QuoteType QuoteType, List<Inline> Inlines) : Inline;
    public record Cite(List<Citation> Citations, List<Inline> Inlines) : Inline;
    public record Code(Attr Attr, string Text) : Inline;
    public record Space : Inline;
    public record SoftBreak : Inline;
    public record LineBreak : Inline;
    public record Math(MathType MathType, string Text) : Inline;
    public record RowInline(string Format, string Text) : Inline;
    public record Link(Attr Attr, List<Inline> Inlines, (string Url, string Title) Target) : Inline;
    public record Image(Attr Attr, List<Inline> Inlines, (string Url, string Title) Target) : Inline;
    public record Note(List<Block> Blocks) : Inline;
    public record Span(Attr Attr, List<Inline> Inlines) : Inline;

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
    public record Attr(string Identifier, List<string> Classes, List<(string, string)> KeyValuePairs);

    /// <summary>The caption of a table, with an optional short caption.</summary>
    public record Caption(List<Inline>? ShortCaption, List<Block> Blocks);

    // replace ShortCaption with List<Inline>

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

    public record Row(Attr Attr, List<Cell> Cells);

    public record TableHead(Attr Attr, List<Row> Rows);

    public record TableBody(Attr Attr, int RowReadColumns, List<Row> Rows1, List<Row> Rows2);

    public record TableFoot(Attr Attr, List<Row> Rows);

    /// <summary>A table cell</summary>
    public record Cell(Attr Attr, Alignment Alignment, int RowSpan, int ColSpan, List<Block> Blocks);

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

    public record Citation(string CitationId, List<Inline> CitationPrefix, List<Inline> CitationSuffix, CitationMode CitationMode, int CitationNoteNum, int CitationHash);

    public enum CitationMode {
        AuthorInText,
        SuppressAuthor,
        NormalCitation
    }
}
