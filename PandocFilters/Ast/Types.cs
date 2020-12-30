using System.Collections.Immutable;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace PandocFilters.Ast {

    public record Pandoc(
        [property: JsonProperty("pandoc-api-version")] int[] ApiVersion,
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

    public class Block : OneOfBase<
        Plain,
        Para,
        LineBlock,
        CodeBlock,
        RawBlock,
        BlockQuote,
        OrderedList,
        BulletList,
        DefinitionList,
        Header,
        HorizontalRule,
        Table,
        Div,
        Null
    > {
        private Block(OneOf<
            Plain,
            Para,
            LineBlock,
            CodeBlock,
            RawBlock,
            BlockQuote,
            OrderedList,
            BulletList,
            DefinitionList,
            Header,
            HorizontalRule,
            Table,
            Div,
            Null
        > value) : base(value) { }

        public static implicit operator Block(Plain plain) => new Block(plain);
        public static implicit operator Block(Para para) => new Block(para);
        public static implicit operator Block(LineBlock lineblock) => new Block(lineblock);
        public static implicit operator Block(CodeBlock codeblock) => new Block(codeblock);
        public static implicit operator Block(RawBlock rawblock) => new Block(rawblock);
        public static implicit operator Block(BlockQuote blockquote) => new Block(blockquote);
        public static implicit operator Block(OrderedList orderedlist) => new Block(orderedlist);
        public static implicit operator Block(BulletList bulletlist) => new Block(bulletlist);
        public static implicit operator Block(DefinitionList definitionlist) => new Block(definitionlist);
        public static implicit operator Block(Header header) => new Block(header);
        public static implicit operator Block(HorizontalRule horizontalrule) => new Block(horizontalrule);
        public static implicit operator Block(Table table) => new Block(table);
        public static implicit operator Block(Div div) => new Block(div);
        public static implicit operator Block(Null @null) => new Block(@null);
    }

    public record Plain(ImmutableList<Inline> Inlines);
    public record Para(ImmutableList<Inline> Inlines);
    public record LineBlock(ImmutableList<ImmutableList<Inline>> NestedInlines);
    public record CodeBlock(Attr Attr, string Code);
    public record RawBlock(string Format, string Text);
    public record BlockQuote(ImmutableList<Block> Blocks);
    public record OrderedList(ListAttributes ListAttributes, ImmutableList<ImmutableList<Block>> NestedBlocks);
    public record BulletList(ImmutableList<ImmutableList<Block>> NestedBlocks);
    public record DefinitionList(ImmutableList<(ImmutableList<Inline> term, ImmutableList<ImmutableList<Block>> definitions)> Items);
    public record Header(int Level, Attr Attr, ImmutableList<Inline> Text);
    public record HorizontalRule;

    // represent ColWidth as a double?
    public record Table(
        Attr Attr,
        Caption Caption,
        ImmutableList<(Alignment Alignment, ColWidthBase ColWidthBase)> ColSpecs,
        TableHead TableHead,
        ImmutableList<TableBody> TableBodies,
        TableFoot TableFoot
    );
    public record Div(Attr Attr, ImmutableList<Block> Blocks);
    public record Null;

    //public abstract record Inline;
    public class Inline : OneOfBase<
        Str,
        Emph,
        Underline,
        Strong,
        Strikeout,
        Superscript,
        Subscript,
        SmallCaps,
        Quoted,
        Cite,
        Code,
        Space,
        SoftBreak,
        LineBreak,
        Math,
        RawInline,
        Link,
        Image,
        Note,
        Span
    > {
        private Inline(OneOf<
            Str, 
            Emph, 
            Underline, 
            Strong, 
            Strikeout, 
            Superscript, 
            Subscript, 
            SmallCaps, 
            Quoted, 
            Cite, 
            Code, 
            Space, 
            SoftBreak, 
            LineBreak, 
            Math, 
            RawInline, 
            Link, 
            Image, 
            Note, 
            Span
        > value) : base(value) { }

        public static implicit operator Inline(Str str) => new Inline(str);
        public static implicit operator Inline(Emph emph) => new Inline(emph);
        public static implicit operator Inline(Underline underline) => new Inline(underline);
        public static implicit operator Inline(Strong strong) => new Inline(strong);
        public static implicit operator Inline(Strikeout strikeout) => new Inline(strikeout);
        public static implicit operator Inline(Superscript superscript) => new Inline(superscript);
        public static implicit operator Inline(Subscript subscript) => new Inline(subscript);
        public static implicit operator Inline(SmallCaps smallCaps) => new Inline(smallCaps);
        public static implicit operator Inline(Quoted quoted) => new Inline(quoted);
        public static implicit operator Inline(Cite cite) => new Inline(cite);
        public static implicit operator Inline(Code code) => new Inline(code);
        public static implicit operator Inline(Space space) => new Inline(space);
        public static implicit operator Inline(SoftBreak softBreak) => new Inline(softBreak);
        public static implicit operator Inline(LineBreak lineBreak) => new Inline(lineBreak);
        public static implicit operator Inline(Math math) => new Inline(math);
        public static implicit operator Inline(RawInline rawInline) => new Inline(rawInline);
        public static implicit operator Inline(Link link) => new Inline(link);
        public static implicit operator Inline(Image image) => new Inline(image);
        public static implicit operator Inline(Note note) => new Inline(note);
        public static implicit operator Inline(Span span) => new Inline(span);
    }

    public record Str(string Text);
    public record Emph(ImmutableList<Inline> Inlines);
    public record Underline(ImmutableList<Inline> Inlines);
    public record Strong(ImmutableList<Inline> Inlines);
    public record Strikeout(ImmutableList<Inline> Inlines);
    public record Superscript(ImmutableList<Inline> Inlines);
    public record Subscript(ImmutableList<Inline> Inlines);
    public record SmallCaps(ImmutableList<Inline> Inlines);
    public record Quoted(QuoteType QuoteType, ImmutableList<Inline> Inlines);
    public record Cite(ImmutableList<Citation> Citations, ImmutableList<Inline> Inlines);
    public record Code(Attr Attr, string Text);
    public record Space;
    public record SoftBreak;
    public record LineBreak;
    public record Math(MathType MathType, string Text);
    public record RawInline(string Format, string Text);
    public record Link(Attr Attr, ImmutableList<Inline> Inlines, (string Url, string Title) Target);
    public record Image(Attr Attr, ImmutableList<Inline> Inlines, (string Url, string Title) Target);
    public record Note(ImmutableList<Block> Blocks);
    public record Span(Attr Attr, ImmutableList<Inline> Inlines);

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
        TwoParens
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

    public class ColWidthBase : OneOfBase<ColWidth, ColWidthDefault> {
        private ColWidthBase(OneOf<ColWidth, ColWidthDefault> value) : base(value) { }
        public static implicit operator ColWidthBase(ColWidth value) => new ColWidthBase(value);
        public static implicit operator ColWidthBase(ColWidthDefault colWidthDefault) => new ColWidthBase(colWidthDefault);
    }

    public record ColWidth(double Double);
    public record ColWidthDefault();

    // replace ColSpec with (Alignment Alignment, ColWidthBase ColWidthBase)

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
