using System.Collections.Immutable;
using System.Linq;

namespace PandocFilters.Ast {
    public abstract class VisitorBase : IVisitor<Pandoc> {
        public virtual Pandoc VisitPandoc(Pandoc pandoc) =>
            pandoc with
            {
                Blocks = pandoc.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual Block VisitBlock(Block block) =>
            block switch {
                Plain plain => VisitPlain(plain),
                Para para => VisitPara(para),
                LineBlock lineBlock => VisitLineBlock(lineBlock),
                CodeBlock codeBlock => VisitCodeBlock(codeBlock),
                RawBlock rawBlock => VisitRawBlock(rawBlock),
                BlockQuote blockQuote => VisitBlockQuote(blockQuote),
                OrderedList orderedList => VisitOrderedList(orderedList),
                BulletList bulletList => VisitBulletList(bulletList),
                DefinitionList definitionList => VisitDefinitionList(definitionList),
                Header header => VisitHeader(header),
                HorizontalRule horizontalRule => VisitHorizontalRule(horizontalRule),
                Table table => VisitTable(table),
                Div div => VisitDiv(div),
                Null @null => VisitNull(@null),
                _ => throw new System.InvalidOperationException()
            };

        public virtual Plain VisitPlain(Plain plain) =>
            plain with
            {
                Inlines = plain.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Para VisitPara(Para para) =>
            para with
            {
                Inlines = para.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual LineBlock VisitLineBlock(LineBlock lineBlock) => lineBlock;

        public virtual CodeBlock VisitCodeBlock(CodeBlock codeBlock) =>
            codeBlock with
            {
                Attr = VisitAttr(codeBlock.Attr)
            };

        public virtual RawBlock VisitRawBlock(RawBlock rawBlock) => rawBlock;

        public virtual BlockQuote VisitBlockQuote(BlockQuote blockQuote) =>
            blockQuote with
            {
                Blocks = blockQuote.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual OrderedList VisitOrderedList(OrderedList orderedList) =>
            orderedList with
            {
                ListAttributes = VisitListAttributes(orderedList.ListAttributes)
            };

        public virtual BulletList VisitBulletList(BulletList bulletList) => bulletList;

        public virtual DefinitionList VisitDefinitionList(DefinitionList definitionList) => definitionList;

        public virtual Header VisitHeader(Header header) =>
            header with
            {
                Attr = VisitAttr(header.Attr),
                Text = header.Text.Select(VisitInline).ToImmutableList()
            };

        public virtual HorizontalRule VisitHorizontalRule(HorizontalRule horizontalRule) => horizontalRule;

        public virtual Table VisitTable(Table table) =>
            table with
            {
                Attr = VisitAttr(table.Attr),
                Caption = VisitCaption(table.Caption),
                TableHead = VisitTableHead(table.TableHead),
                TableBodies = table.TableBodies.Select(VisitTableBody).ToImmutableList(),
                TableFoot = VisitTableFoot(table.TableFoot)
            };

        public virtual Div VisitDiv(Div div) =>
            div with
            {
                Attr = VisitAttr(div.Attr),
                Blocks = div.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual Null VisitNull(Null @null) => @null;

        public virtual Inline VisitInline(Inline inline) =>
            inline switch {
                Str str => VisitStr(str),
                Emph emph => VisitEmph(emph),
                Underline underline => VisitUnderline(underline),
                Strong strong => VisitStrong(strong),
                Strikeout strikeout => VisitStrikeout(strikeout),
                Superscript superscript => VisitSuperscript(superscript),
                Subscript subscript => VisitSubscript(subscript),
                SmallCaps smallCaps => VisitSmallCaps(smallCaps),
                Quoted quoted => VisitQuoted(quoted),
                Cite cite => VisitCite(cite),
                Code code => VisitCode(code),
                Space space => VisitSpace(space),
                SoftBreak softBreak => VisitSoftBreak(softBreak),
                LineBreak lineBreak => VisitLineBreak(lineBreak),
                Math math => VisitMath(math),
                RawInline rawInline => VisitRawInline(rawInline),
                Link link => VisitLink(link),
                Image image => VisitImage(image),
                Note note => VisitNote(note),
                Span span => VisitSpan(span),
                _ => throw new System.InvalidOperationException()
            };

        public virtual Str VisitStr(Str str) => str;

        public virtual Emph VisitEmph(Emph emph) =>
            emph with
            {
                Inlines = emph.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Underline VisitUnderline(Underline underline) =>
            underline with
            {
                Inlines = underline.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Strong VisitStrong(Strong strong) =>
            strong with
            {
                Inlines = strong.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Strikeout VisitStrikeout(Strikeout strikeout) =>
            strikeout with
            {
                Inlines = strikeout.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Superscript VisitSuperscript(Superscript superscript) =>
            superscript with
            {
                Inlines = superscript.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Subscript VisitSubscript(Subscript subscript) =>
            subscript with
            {
                Inlines = subscript.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual SmallCaps VisitSmallCaps(SmallCaps smallCaps) =>
            smallCaps with
            {
                Inlines = smallCaps.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Quoted VisitQuoted(Quoted quoted) =>
            quoted with
            {
                Inlines = quoted.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Cite VisitCite(Cite cite) =>
            cite with
            {
                Citations = cite.Citations.Select(VisitCitation).ToImmutableList(),
                Inlines = cite.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Code VisitCode(Code code) =>
            code with
            {
                Attr = VisitAttr(code.Attr)
            };

        public virtual Space VisitSpace(Space space) => space;

        public virtual SoftBreak VisitSoftBreak(SoftBreak softBreak) => softBreak;

        public virtual LineBreak VisitLineBreak(LineBreak lineBreak) => lineBreak;

        public virtual Math VisitMath(Math math) => math;

        public virtual RawInline VisitRawInline(RawInline rowInline) => rowInline;

        public virtual Link VisitLink(Link link) =>
            link with
            {
                Attr = VisitAttr(link.Attr),
                Inlines = link.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Image VisitImage(Image image) =>
            image with
            {
                Attr = VisitAttr(image.Attr),
                Inlines = image.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual Note VisitNote(Note note) =>
            note with
            {
                Blocks = note.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual Span VisitSpan(Span span) =>
            span with
            {
                Attr = VisitAttr(span.Attr),
                Inlines = span.Inlines.Select(VisitInline).ToImmutableList()
            };

        public virtual ListAttributes VisitListAttributes(ListAttributes listAttributes) => listAttributes;

        public virtual Attr VisitAttr(Attr attr) => attr;

        public virtual Caption VisitCaption(Caption caption) =>
            caption with
            {
                ShortCaption = caption.ShortCaption?.Select(VisitInline).ToImmutableList(),
                Blocks = caption.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual Row VisitRow(Row row) =>
            row with
            {
                Attr = VisitAttr(row.Attr),
                Cells = row.Cells.Select(VisitCell).ToImmutableList()
            };

        public virtual TableHead VisitTableHead(TableHead tableHead) =>
            tableHead with
            {
                Attr = VisitAttr(tableHead.Attr),
                Rows = tableHead.Rows.Select(VisitRow).ToImmutableList()
            };

        public virtual TableBody VisitTableBody(TableBody tableBody) =>
            tableBody with
            {
                Attr = VisitAttr(tableBody.Attr),
                Rows1 = tableBody.Rows1.Select(VisitRow).ToImmutableList(),
                Rows2 = tableBody.Rows2.Select(VisitRow).ToImmutableList()
            };

        public virtual TableFoot VisitTableFoot(TableFoot tableFoot) =>
            tableFoot with
            {
                Attr = VisitAttr(tableFoot.Attr),
                Rows = tableFoot.Rows.Select(VisitRow).ToImmutableList()
            };

        public virtual Cell VisitCell(Cell cell) =>
            cell with
            {
                Attr = VisitAttr(cell.Attr),
                Blocks = cell.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual Citation VisitCitation(Citation citation) =>
            citation with
            {
                CitationPrefix = citation.CitationPrefix.Select(VisitInline).ToImmutableList(),
                CitationSuffix = citation.CitationSuffix.Select(VisitInline).ToImmutableList()
            };
    }
}
