using System;
using static PandocFilters.Functions;

namespace PandocFilters.Ast {
    public sealed class DelegateVisitor : VisitorBase {
        private Func<Pandoc, Pandoc>? pandocDelegate;
        private Func<Block, Block>? blockDelegate;
        private Func<Plain, Plain>? plainDelegate;
        private Func<Para, Para>? paraDelegate;
        private Func<LineBlock, LineBlock>? lineBlockDelegate;
        private Func<CodeBlock, CodeBlock>? codeBlockDelegate;
        private Func<RawBlock, RawBlock>? rawBlockDelegate;
        private Func<BlockQuote, BlockQuote>? blockQuoteDelegate;
        private Func<OrderedList, OrderedList>? orderedListDelegate;
        private Func<BulletList, BulletList>? bulletListDelegate;
        private Func<DefinitionList, DefinitionList>? definitionListDelegate;
        private Func<Header, Header>? headerDelegate;
        private Func<HorizontalRule, HorizontalRule>? horizontalRuleDelegate;
        private Func<Table, Table>? tableDelegate;
        private Func<Div, Div>? divDelegate;
        private Func<Null, Null>? @nullDelegate;
        private Func<Inline, Inline>? inlineDelegate;
        private Func<Str, Str>? strDelegate;
        private Func<Emph, Emph>? emphDelegate;
        private Func<Underline, Underline>? underlineDelegate;
        private Func<Strong, Strong>? strongDelegate;
        private Func<Strikeout, Strikeout>? strikeoutDelegate;
        private Func<Superscript, Superscript>? superscriptDelegate;
        private Func<Subscript, Subscript>? subscriptDelegate;
        private Func<SmallCaps, SmallCaps>? smallCapsDelegate;
        private Func<Quoted, Quoted>? quotedDelegate;
        private Func<Cite, Cite>? citeDelegate;
        private Func<Code, Code>? codeDelegate;
        private Func<Space, Space>? spaceDelegate;
        private Func<SoftBreak, SoftBreak>? softBreakDelegate;
        private Func<LineBreak, LineBreak>? lineBreakDelegate;
        private Func<Math, Math>? mathDelegate;
        private Func<RowInline, RowInline>? rowInlineDelegate;
        private Func<Link, Link>? linkDelegate;
        private Func<Image, Image>? imageDelegate;
        private Func<Note, Note>? noteDelegate;
        private Func<Span, Span>? spanDelegate;
        private Func<ListAttributes, ListAttributes>? listAttributesDelegate;
        private Func<Attr, Attr>? attrDelegate;
        private Func<Caption, Caption>? captionDelegate;
        private Func<Row, Row>? rowDelegate;
        private Func<TableHead, TableHead>? tableHeadDelegate;
        private Func<TableBody, TableBody>? tableBodyDelegate;
        private Func<TableFoot, TableFoot>? tableFootDelegate;
        private Func<Cell, Cell>? cellDelegate;
        private Func<Citation, Citation>? citationDelegate;

        public void Add(Func<Pandoc, Pandoc> del) => AddDelegate(ref pandocDelegate, del);
        public void Add(Func<Block, Block> del) => AddDelegate(ref blockDelegate, del);
        public void Add(Func<Plain, Plain> del) => AddDelegate(ref plainDelegate, del);
        public void Add(Func<Para, Para> del) => AddDelegate(ref paraDelegate, del);
        public void Add(Func<LineBlock, LineBlock> del) => AddDelegate(ref lineBlockDelegate, del);
        public void Add(Func<CodeBlock, CodeBlock> del) => AddDelegate(ref codeBlockDelegate, del);
        public void Add(Func<RawBlock, RawBlock> del) => AddDelegate(ref rawBlockDelegate, del);
        public void Add(Func<BlockQuote, BlockQuote> del) => AddDelegate(ref blockQuoteDelegate, del);
        public void Add(Func<OrderedList, OrderedList> del) => AddDelegate(ref orderedListDelegate, del);
        public void Add(Func<BulletList, BulletList> del) => AddDelegate(ref bulletListDelegate, del);
        public void Add(Func<DefinitionList, DefinitionList> del) => AddDelegate(ref definitionListDelegate, del);
        public void Add(Func<Header, Header> del) => AddDelegate(ref headerDelegate, del);
        public void Add(Func<HorizontalRule, HorizontalRule> del) => AddDelegate(ref horizontalRuleDelegate, del);
        public void Add(Func<Table, Table> del) => AddDelegate(ref tableDelegate, del);
        public void Add(Func<Div, Div> del) => AddDelegate(ref divDelegate, del);
        public void Add(Func<Null, Null> del) => AddDelegate(ref @nullDelegate, del);
        public void Add(Func<Inline, Inline> del) => AddDelegate(ref inlineDelegate, del);
        public void Add(Func<Str, Str> del) => AddDelegate(ref strDelegate, del);
        public void Add(Func<Emph, Emph> del) => AddDelegate(ref emphDelegate, del);
        public void Add(Func<Underline, Underline> del) => AddDelegate(ref underlineDelegate, del);
        public void Add(Func<Strong, Strong> del) => AddDelegate(ref strongDelegate, del);
        public void Add(Func<Strikeout, Strikeout> del) => AddDelegate(ref strikeoutDelegate, del);
        public void Add(Func<Superscript, Superscript> del) => AddDelegate(ref superscriptDelegate, del);
        public void Add(Func<Subscript, Subscript> del) => AddDelegate(ref subscriptDelegate, del);
        public void Add(Func<SmallCaps, SmallCaps> del) => AddDelegate(ref smallCapsDelegate, del);
        public void Add(Func<Quoted, Quoted> del) => AddDelegate(ref quotedDelegate, del);
        public void Add(Func<Cite, Cite> del) => AddDelegate(ref citeDelegate, del);
        public void Add(Func<Code, Code> del) => AddDelegate(ref codeDelegate, del);
        public void Add(Func<Space, Space> del) => AddDelegate(ref spaceDelegate, del);
        public void Add(Func<SoftBreak, SoftBreak> del) => AddDelegate(ref softBreakDelegate, del);
        public void Add(Func<LineBreak, LineBreak> del) => AddDelegate(ref lineBreakDelegate, del);
        public void Add(Func<Math, Math> del) => AddDelegate(ref mathDelegate, del);
        public void Add(Func<RowInline, RowInline> del) => AddDelegate(ref rowInlineDelegate, del);
        public void Add(Func<Link, Link> del) => AddDelegate(ref linkDelegate, del);
        public void Add(Func<Image, Image> del) => AddDelegate(ref imageDelegate, del);
        public void Add(Func<Note, Note> del) => AddDelegate(ref noteDelegate, del);
        public void Add(Func<Span, Span> del) => AddDelegate(ref spanDelegate, del);
        public void Add(Func<ListAttributes, ListAttributes> del) => AddDelegate(ref listAttributesDelegate, del);
        public void Add(Func<Attr, Attr> del) => AddDelegate(ref attrDelegate, del);
        public void Add(Func<Caption, Caption> del) => AddDelegate(ref captionDelegate, del);
        public void Add(Func<Row, Row> del) => AddDelegate(ref rowDelegate, del);
        public void Add(Func<TableHead, TableHead> del) => AddDelegate(ref tableHeadDelegate, del);
        public void Add(Func<TableBody, TableBody> del) => AddDelegate(ref tableBodyDelegate, del);
        public void Add(Func<TableFoot, TableFoot> del) => AddDelegate(ref tableFootDelegate, del);
        public void Add(Func<Cell, Cell> del) => AddDelegate(ref cellDelegate, del);
        public void Add(Func<Citation, Citation> del) => AddDelegate(ref citationDelegate, del);


        public override Pandoc VisitPandoc(Pandoc pandoc) {
            pandoc = pandocDelegate?.Invoke(pandoc) ?? pandoc;
            return base.VisitPandoc(pandoc);
        }
        public override Block VisitBlock(Block block) {
            block = blockDelegate?.Invoke(block) ?? block;
            return base.VisitBlock(block);
        }
        public override Plain VisitPlain(Plain plain) {
            plain = plainDelegate?.Invoke(plain) ?? plain;
            return base.VisitPlain(plain);
        }
        public override Para VisitPara(Para para) {
            para = paraDelegate?.Invoke(para) ?? para;
            return base.VisitPara(para);
        }
        public override LineBlock VisitLineBlock(LineBlock lineBlock) {
            lineBlock = lineBlockDelegate?.Invoke(lineBlock) ?? lineBlock;
            return base.VisitLineBlock(lineBlock);
        }
        public override CodeBlock VisitCodeBlock(CodeBlock codeBlock) {
            codeBlock = codeBlockDelegate?.Invoke(codeBlock) ?? codeBlock;
            return base.VisitCodeBlock(codeBlock);
        }
        public override RawBlock VisitRawBlock(RawBlock rawBlock) {
            rawBlock = rawBlockDelegate?.Invoke(rawBlock) ?? rawBlock;
            return base.VisitRawBlock(rawBlock);
        }
        public override BlockQuote VisitBlockQuote(BlockQuote blockQuote) {
            blockQuote = blockQuoteDelegate?.Invoke(blockQuote) ?? blockQuote;
            return base.VisitBlockQuote(blockQuote);
        }
        public override OrderedList VisitOrderedList(OrderedList orderedList) {
            orderedList = orderedListDelegate?.Invoke(orderedList) ?? orderedList;
            return base.VisitOrderedList(orderedList);
        }
        public override BulletList VisitBulletList(BulletList bulletList) {
            bulletList = bulletListDelegate?.Invoke(bulletList) ?? bulletList;
            return base.VisitBulletList(bulletList);
        }
        public override DefinitionList VisitDefinitionList(DefinitionList definitionList) {
            definitionList = definitionListDelegate?.Invoke(definitionList) ?? definitionList;
            return base.VisitDefinitionList(definitionList);
        }
        public override Header VisitHeader(Header header) {
            header = headerDelegate?.Invoke(header) ?? header;
            return base.VisitHeader(header);
        }
        public override HorizontalRule VisitHorizontalRule(HorizontalRule horizontalRule) {
            horizontalRule = horizontalRuleDelegate?.Invoke(horizontalRule) ?? horizontalRule;
            return base.VisitHorizontalRule(horizontalRule);
        }
        public override Table VisitTable(Table table) {
            table = tableDelegate?.Invoke(table) ?? table;
            return base.VisitTable(table);
        }
        public override Div VisitDiv(Div div) {
            div = divDelegate?.Invoke(div) ?? div;
            return base.VisitDiv(div);
        }
        public override Null VisitNull(Null @null) {
            @null = @nullDelegate?.Invoke(@null) ?? @null;
            return base.VisitNull(@null);
        }
        public override Inline VisitInline(Inline inline) {
            inline = inlineDelegate?.Invoke(inline) ?? inline;
            return base.VisitInline(inline);
        }
        public override Str VisitStr(Str str) {
            str = strDelegate?.Invoke(str) ?? str;
            return base.VisitStr(str);
        }
        public override Emph VisitEmph(Emph emph) {
            emph = emphDelegate?.Invoke(emph) ?? emph;
            return base.VisitEmph(emph);
        }
        public override Underline VisitUnderline(Underline underline) {
            underline = underlineDelegate?.Invoke(underline) ?? underline;
            return base.VisitUnderline(underline);
        }
        public override Strong VisitStrong(Strong strong) {
            strong = strongDelegate?.Invoke(strong) ?? strong;
            return base.VisitStrong(strong);
        }
        public override Strikeout VisitStrikeout(Strikeout strikeout) {
            strikeout = strikeoutDelegate?.Invoke(strikeout) ?? strikeout;
            return base.VisitStrikeout(strikeout);
        }
        public override Superscript VisitSuperscript(Superscript superscript) {
            superscript = superscriptDelegate?.Invoke(superscript) ?? superscript;
            return base.VisitSuperscript(superscript);
        }
        public override Subscript VisitSubscript(Subscript subscript) {
            subscript = subscriptDelegate?.Invoke(subscript) ?? subscript;
            return base.VisitSubscript(subscript);
        }
        public override SmallCaps VisitSmallCaps(SmallCaps smallCaps) {
            smallCaps = smallCapsDelegate?.Invoke(smallCaps) ?? smallCaps;
            return base.VisitSmallCaps(smallCaps);
        }
        public override Quoted VisitQuoted(Quoted quoted) {
            quoted = quotedDelegate?.Invoke(quoted) ?? quoted;
            return base.VisitQuoted(quoted);
        }
        public override Cite VisitCite(Cite cite) {
            cite = citeDelegate?.Invoke(cite) ?? cite;
            return base.VisitCite(cite);
        }
        public override Code VisitCode(Code code) {
            code = codeDelegate?.Invoke(code) ?? code;
            return base.VisitCode(code);
        }
        public override Space VisitSpace(Space space) {
            space = spaceDelegate?.Invoke(space) ?? space;
            return base.VisitSpace(space);
        }
        public override SoftBreak VisitSoftBreak(SoftBreak softBreak) {
            softBreak = softBreakDelegate?.Invoke(softBreak) ?? softBreak;
            return base.VisitSoftBreak(softBreak);
        }
        public override LineBreak VisitLineBreak(LineBreak lineBreak) {
            lineBreak = lineBreakDelegate?.Invoke(lineBreak) ?? lineBreak;
            return base.VisitLineBreak(lineBreak);
        }
        public override Math VisitMath(Math math) {
            math = mathDelegate?.Invoke(math) ?? math;
            return base.VisitMath(math);
        }
        public override RowInline VisitRowInline(RowInline rowInline) {
            rowInline = rowInlineDelegate?.Invoke(rowInline) ?? rowInline;
            return base.VisitRowInline(rowInline);
        }
        public override Link VisitLink(Link link) {
            link = linkDelegate?.Invoke(link) ?? link;
            return base.VisitLink(link);
        }
        public override Image VisitImage(Image image) {
            image = imageDelegate?.Invoke(image) ?? image;
            return base.VisitImage(image);
        }
        public override Note VisitNote(Note note) {
            note = noteDelegate?.Invoke(note) ?? note;
            return base.VisitNote(note);
        }
        public override Span VisitSpan(Span span) {
            span = spanDelegate?.Invoke(span) ?? span;
            return base.VisitSpan(span);
        }
        public override ListAttributes VisitListAttributes(ListAttributes listAttributes) {
            listAttributes = listAttributesDelegate?.Invoke(listAttributes) ?? listAttributes;
            return base.VisitListAttributes(listAttributes);
        }
        public override Attr VisitAttr(Attr attr) {
            attr = attrDelegate?.Invoke(attr) ?? attr;
            return base.VisitAttr(attr);
        }
        public override Caption VisitCaption(Caption caption) {
            caption = captionDelegate?.Invoke(caption) ?? caption;
            return base.VisitCaption(caption);
        }
        public override Row VisitRow(Row row) {
            row = rowDelegate?.Invoke(row) ?? row;
            return base.VisitRow(row);
        }
        public override TableHead VisitTableHead(TableHead tableHead) {
            tableHead = tableHeadDelegate?.Invoke(tableHead) ?? tableHead;
            return base.VisitTableHead(tableHead);
        }
        public override TableBody VisitTableBody(TableBody tableBody) {
            tableBody = tableBodyDelegate?.Invoke(tableBody) ?? tableBody;
            return base.VisitTableBody(tableBody);
        }
        public override TableFoot VisitTableFoot(TableFoot tableFoot) {
            tableFoot = tableFootDelegate?.Invoke(tableFoot) ?? tableFoot;
            return base.VisitTableFoot(tableFoot);
        }
        public override Cell VisitCell(Cell cell) {
            cell = cellDelegate?.Invoke(cell) ?? cell;
            return base.VisitCell(cell);
        }
        public override Citation VisitCitation(Citation citation) {
            citation = citationDelegate?.Invoke(citation) ?? citation;
            return base.VisitCitation(citation);
        }
    }
}
