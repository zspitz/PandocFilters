namespace PandocFilters {
    public interface IVisitor<TPandoc> {
        public TPandoc VisitPandoc(TPandoc pandoc);
    }
}
