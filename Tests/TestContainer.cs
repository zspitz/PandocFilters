using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using static Tests.Functions;
using ZSpitz.Util;
using static System.IO.Path;
using static System.IO.Directory;
using static ZSpitz.Util.Functions;
using OneOf;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Tests {
    public class TestContainer {
        private static readonly string filesRoot = GetFullFilename(@"..\..\..\files");
        private static readonly string filtersRoot = GetFullFilename(@"..\..\..\..\TestFilters");
        private static readonly Dictionary<string, OneOf<bool, string>> formatMap = new Dictionary<string, OneOf<bool, string>>() {
            [""] = false,
            [".asciidoc"] = false,
            [".asciidoctor"] = false,
            [".context"] = false,
            ["csl"] = false, // the CSL files seem to be in XML, but I'm not sure if they all should be parsed this way.
            ["csv"] = "csv",
            [".custom"] = true,
            [".docbook"] = "docbook",
            [".docbook4"] = "docbook",
            [".docbook5"] = "docbook",
            [".docx"] = true,
            [".dokuwiki"] = true,
            [".epub"] = true,
            [".fb2"] = true,
            [".gif"] = false,
            [".haddock"] = "haddock",
            [".hs"] = false,
            [".html"] = true,
            [".html+lhs"] = "html+lhs",
            [".html4"] = "html",
            [".html5"] = "html",
            [".icml"] = false,
            [".ipynb"] = true,
            [".jats_archiving"] = "jats",
            [".jats_articleauthoring"] = "jats",
            [".jats_publishing"] = "jats",
            [".jira"] = "jira",
            [".jpg"] = false,
            [".latex"] = true,
            [".latex+lhs"] = "latex+lhs",
            [".lua"] = false,
            [".man"] = "man",
            [".markdown"] = true,
            [".markdown+lhs"] = "markdown+lhs",
            [".md"] = "markdown",
            [".mediawiki"] = "mediawiki",
            [".native"] = "native",
            [".ms"] = false,
            [".muse"] = true,
            [".odt"] = true,
            [".opendocument"] = true,
            [".opml"] = true,
            [".org"] = true,
            [".pptx"] = false,
            [".plain"] = true,
            [".png"] = false,
            [".rst"] = true,
            [".rst+lhs"] = "rst+lhs",
            [".rtf"] = false,
            [".sh"] = false,
            [".t2t"] = true,
            [".tei"] = false,
            [".texinfo"] = false,
            [".textile"] = true,
            [".tikiwiki"] = "tikiwiki ",
            [".twiki"] = "twiki",
            [".txt"] = true,
            [".wiki"] = true,
            [".xwiki"] = false,
            [".zimwiki"] = false,

            ["jats-reader.xml"] = "jats"
        };

        private static readonly Dictionary<string, OneOf<bool, string>> fileFormatMapping =
            EnumerateFiles(filesRoot, "*.*", SearchOption.AllDirectories)
                .Select(x => {
                    var (path, ext) = (x.Replace(filesRoot, ""), GetExtension(x));
                    if (!formatMap.TryGetValue(ext, out var mapping)) {
                        mapping = formatMap.Keys.FirstOrDefault(y => x.EndsWith(y));
                    }
                    return (path, mapping);
                })
                .ToDictionary();

        public static TheoryData<OneOf<bool, string>> FileFormatMappingData =
            fileFormatMapping
                .Values
                .ToTheoryData();

        //[Theory]
        //[MemberData(nameof(FileFormatMappingData))]
        //public void VerifyFileMapping(OneOf<bool, string> mapping) =>
        //    Assert.True(mapping.IsT0 || mapping.AsT1 is not null);

        private static readonly Dictionary<string, (Lazy<ProcessResult> ast, Lazy<ProcessResult> json)> generators =
            fileFormatMapping
                .WhereKVP((path, v) => v.Match(
                    b => b,
                    s => !s.IsNullOrWhitespace()
                ))
                .SelectKVP((path, v) => {
                    var fullPath = $"{filesRoot}\\{path}";
                    var format = v.Match(
                        b => "",
                        s => s
                    );
                    return (
                        path,
                        (
                            Lazy(() => GetAst(fullPath, "", format)),
                            Lazy(() => GetJson(fullPath, format))
                        )
                    );
                })
                .ToDictionary();

        public static TheoryData<string, string> TestData = IIFE(() => {
            var filters = EnumerateDirectories(filtersRoot).Select(x => GetFileName(x)!);
            return generators.Keys.SelectMany(doc => filters.Select(filter => (doc, filter)))
            .Take(20);
        }).ToTheoryData();

        //[MemberData(nameof(TestData))]
        //[SkippableTheory]
        //public void AstTest(string docPath, string filterName) {
        //    var astResult = generators[docPath].ast.Value;
        //    Skip.If(
        //        astResult.ExitCode != 0 || astResult.StdOut.IsNullOrEmpty() || !astResult.StdErr.IsNullOrEmpty(),
        //        $"{(!astResult.StdErr.IsNullOrEmpty() ? astResult.StdErr : "")} - {(astResult.ExitCode != 0 ? astResult.ExitCode.ToString() : "")}"
        //    );

        //    var format =
        //        fileFormatMapping.TryGetValue(docPath, out var v) ?
        //            v.Match(
        //                b => "",
        //                s => s
        //            ) :
        //            "";

        //    docPath = $"{filesRoot}\\{docPath}";
        //    var filterPath = $@"{filtersRoot}\{filterName}\bin\Debug\net5.0\{filterName}.exe";
        //    var result = GetAst(docPath, filterPath, format);
        //    Assert.Equal("", result.StdErr);
        //    Assert.Equal(0, result.ExitCode);
        //    Assert.Equal(astResult.StdOut, result.StdOut);
        //}

        [SkippableTheory]
        [MemberData(nameof(TestData))]
        public void JsonTest(string docPath, string filterName) {
            var jsonResult = generators[docPath].json.Value;
            Skip.If(
                jsonResult.ExitCode != 0 || jsonResult.StdOut.IsNullOrEmpty() || !jsonResult.StdErr.IsNullOrEmpty(),
                $"{(!jsonResult.StdErr.IsNullOrEmpty() ? jsonResult.StdErr : "")} - {(jsonResult.ExitCode != 0 ? jsonResult.ExitCode.ToString() : "")}"
            );

            var process = new Process {
                StartInfo = {
                    FileName = $@"{filtersRoot}\{filterName}\bin\Debug\net5.0\{filterName}.exe",
                    UseShellExecute = false,
                    CreateNoWindow=true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };
            var result = RunProcess(process, jsonResult.StdOut);
            Assert.Equal("", result.StdErr);
            Assert.Equal(0, result.ExitCode);
            Assert.True(
                JToken.DeepEquals(
                    JToken.Parse(jsonResult.StdOut),
                    JToken.Parse(result.StdOut)
                )
            );
        }
    }
}
