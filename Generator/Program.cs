using PandocFilters.Ast;
using System;
using System.Linq;
using System.Reflection;
using static Generator.Functions;
using ZSpitz.Util;
using System.Collections.Immutable;
using Octokit;
using PandocFilters;

var delegates =
    new (Action action, string title)[] {
        (GeneratePandocVisitor, "Generate Pandoc visitor base class"),
        (GenerateDelegateVisitor, "Generate delegate visitor"),
        (DownloadPandocTestDocuments,"Download Pandoc test files to test project folder")
    };

var msg = $@"Select the number of a script to run:
{delegates.Joined("\n", (x, index) => $"{index + 1}. {x.title}")}";

Prompt(msg, out var result, s => (
    !int.TryParse(s, out var result) ? "Not a number." :
        (result < 1 || result > delegates.Length) ? "Invalid selection." :
        "",
    result
));

delegates[result - 1].action();


// generators:
// 1. generate visitor class
// 2. download test documents
// 3. generate delegates overload for Filter.Run

static void GeneratePandocVisitor() {
    // The tree intenionally doesn't recurse over MetaValue -- https://github.com/zspitz/PandocFilters/issues/10
    var types = Assembly.GetAssembly(typeof(Pandoc))!.GetTypes()
        .Where(x => x.Namespace == "PandocFilters.Ast" && !x.IsEnum && x != typeof(MetaValue));

    var names = types.ToDictionary(
        x => x,
        x => {
            var (name, camelCase) = (x.Name, x.Name.ToCamelCase());
            if (camelCase == "null") { camelCase = "@null"; }
            return (name, camelCase);
        }
    );

    Console.WriteLine($@"
namespace PandocFilters {{
    public abstract class PandocVisitorBase : IVisitor<TPandoc> {{
        {types.Joined(@"

        ",
        t => {
            var (name, camelCase) = names[t];
            var signature = $@"public virtual {name} Visit{name}({name} {camelCase}) =>";

            if (t.IsAbstract) {
                return $@"{signature}
            {camelCase} switch {{
                {types.Where(x => x.BaseType == t).Joined(@",
                ", subtype => $"{names[subtype].name} {names[subtype].camelCase} => Visit{names[subtype].name}({names[subtype].camelCase})")},
                _ => throw new System.InvalidOperationException()
            }};";
            }

            var properties =
                t.GetProperties()
                    .Where(x =>
                        x.PropertyType.In(types) ||
                        (x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments().First().In(types))
                    )
                    .ToArray();
            if (properties.None()) {
                return $"{signature} {camelCase};";
            }

            return $@"{signature}
            {camelCase} with {{
                {properties.Joined(@",
                ",
                prp => {
                    var (isGeneric, firstArg, def) = Generics(prp.PropertyType);
                    if (!isGeneric) {
                        return $"{prp.Name} = Visit{prp.PropertyType.Name}({camelCase}.{prp.Name})";
                    }
                    if (def == typeof(ImmutableList<>)) {
                        var (isGeneric1, firstArg1, def1) = Generics(firstArg!);
                        if (!isGeneric1) {
                            // Rows = tableHead.Rows.Select(VisitRow)
                            return $"{prp.Name} = {camelCase}.{prp.Name}.Select(Visit{names[firstArg!].name}).ToImmutableList()";
                        }
                        if (def1 == typeof(ImmutableList<>)) {
                            // NestedInlines = lineBlock.NestedInlines.Select(x => 
                            //          x.Select(VisitInline).ToImmutableList()
                            //  ).ToImmutableList()
                            return $"{camelCase}.{prp.Name}.Select(x => x.Select(Visit{names[firstArg1!].name}).ToImmutableList()).ToImmutableList()";
                        }
                    }

                    throw new NotImplementedException($"Not handling property {prp} on type {t}.");
                })}
            }};";
        })}
    }}
}}
");
}

static void GenerateDelegateVisitor() {
    var types = Assembly.GetAssembly(typeof(Pandoc))!.GetTypes()
        .Where(x =>
            x.Namespace == "PandocFilters.Ast" && !(
                x.IsEnum ||
                x == typeof(MetaValue) ||
                typeof(IVisitor<Pandoc>).IsAssignableFrom(x)
            )
        ).Select(x => (
            name: x.Name,
            camelCase: 
                x.Name == "Null" ?
                    "@null" :            
                    x.Name.ToCamelCase()
        ))
        .ToArray();

    Console.WriteLine(@$"
namespace PandocFilters.Ast {{
    internal sealed class DelegateVisitor : VisitorBase {{
        {types.JoinedT(@"
        ", (name, camelCase) => $"private Func<{name}, {name}>? {camelCase}Delegate;")}

        {types.JoinedT(@"
        ", (name, camelCase) => $"public void Add(Func<{name}, {name}> del) => AddDelegate(ref {camelCase}Delegate, del);")}


        {types.JoinedT(@"
        ", (name, camelCase) => $@"public override {name} Visit{name}({name} {camelCase}) {{
            {camelCase} = {camelCase}Delegate?.Invoke({camelCase}) ?? {camelCase};
            return base.Visit{name}({camelCase});
        }}")}
    }}
}}
");

        // delegate variables
        // OneOf type for constructor
        // switch on delegate
        // overrides

}

static void DownloadPandocTestDocuments() {
    throw new NotImplementedException();
    //var client = new GitHubClient(new ProductHeaderValue("PandocFilters-TestFileDownloader"));
    //var lst = client.Repository.Content.GetAllContents()
}


