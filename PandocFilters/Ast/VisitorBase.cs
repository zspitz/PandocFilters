using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ZSpitz.Util;
using static ZSpitz.Util.Functions;

namespace PandocFilters.Ast {
    public abstract class VisitorBase : IVisitor<Pandoc> {
        public virtual Pandoc VisitPandoc(Pandoc pandoc) =>
            pandoc with
            {
                Blocks = pandoc.Blocks.Select(VisitBlock).ToImmutableList()
            };

        public virtual MetaValue VisitMetaValue(MetaValue metaValue) =>
                metaValue.Match(
                    dict => dict.SelectKVP((key, value) => KVP(key, VisitMetaValue(value))).ToImmutableDictionary(),
                    lst => lst.Select(VisitMetaValue).ToImmutableList(),
                    b => VisitMetaValue(b),
                    s => VisitMetaValue(s),
                    inlines => VisitMetaValue(inlines),
                    blocks => VisitMetaValue(blocks)
                );

        public virtual Block VisitBlock(Block block) =>
            block.Match<Block>(
                plain => VisitPlain(plain),
                para => VisitPara(para),
                lineBlock => VisitLineBlock(lineBlock),
                codeBlock => VisitCodeBlock(codeBlock),
                rawBlock => VisitRawBlock(rawBlock),
                blockQuote => VisitBlockQuote(blockQuote),
                orderedList => VisitOrderedList(orderedList),
                bulletList => VisitBulletList(bulletList),
                definitionList => VisitDefinitionList(definitionList),
                header => VisitHeader(header),
                horizontalRule => VisitHorizontalRule(horizontalRule),
                table => VisitTable(table),
                div => VisitDiv(div),
                @null => VisitNull(@null)
            );

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
            inline.Match<Inline>(
                str => VisitStr(str),
                emph => VisitEmph(emph),
                underline => VisitUnderline(underline),
                strong => VisitStrong(strong),
                strikeout => VisitStrikeout(strikeout),
                superscript => VisitSuperscript(superscript),
                subscript => VisitSubscript(subscript),
                smallCaps => VisitSmallCaps(smallCaps),
                quoted => VisitQuoted(quoted),
                cite => VisitCite(cite),
                code => VisitCode(code),
                space => VisitSpace(space),
                softBreak => VisitSoftBreak(softBreak),
                lineBreak => VisitLineBreak(lineBreak),
                math => VisitMath(math),
                rawInline => VisitRawInline(rawInline),
                link => VisitLink(link),
                image => VisitImage(image),
                note => VisitNote(note),
                span => VisitSpan(span)
            );

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

        public virtual RawInline VisitRawInline(RawInline rawInline) => rawInline;

        public virtual Link VisitLink(Link link) =>
            link with
            {
                Attr = VisitAttr(link.Attr),
                AltText = link.AltText.Select(VisitInline).ToImmutableList()
            };

        public virtual Image VisitImage(Image image) =>
            image with
            {
                Attr = VisitAttr(image.Attr),
                AltText = image.AltText.Select(VisitInline).ToImmutableList()
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

        public virtual ColWidthBase VisitColWidthBase(ColWidthBase colWidthBase) =>
            colWidthBase.Match<ColWidthBase>(
                colWidth => VisitColWidth(colWidth),
                colWidthDefault => VisitColWidthDefault(colWidthDefault)
            );

        public virtual ColWidth VisitColWidth(ColWidth colWidth) => colWidth;

        public virtual ColWidthDefault VisitColWidthDefault(ColWidthDefault colWidthDefault) => colWidthDefault;

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
