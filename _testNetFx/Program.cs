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
        public override Image VisitImage(Image image) =>
            new(image.Attr, image.AltText, image.Target);
    }
}
