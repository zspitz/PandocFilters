using System;
using PandocFilters;
using PandocFilters.Ast;

namespace _testNetFx {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
        }
    }

    class TestVisitor : VisitorBase {
        public override Image VisitImage(Image image) {
            return new Image(image.Attr, image.Inlines, image.Target);
        }
    }
}
