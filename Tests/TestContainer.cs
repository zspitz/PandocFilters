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
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Tests {
    public class TestContainer {
        private static readonly string filesRoot = GetFullFilename(@"..\..\..\files");
        private static readonly string filtersRoot = GetFullFilename(@"..\..\..\..\TestFilters");
        private static readonly string[] extensionExclusions = new[] {
            ".gif",
            ".jpg",
            ".png"
        };

        private static readonly Dictionary<string, (Lazy<ProcessResult> ast, Lazy<ProcessResult> json)> files =
            EnumerateFiles(filesRoot, "*.*", SearchOption.AllDirectories)
                .Select(x => x.Replace(filesRoot, ""))
                .Where(x => !x.EndsWithAny(extensionExclusions))
                .ToDictionary(x => x, x => (
                    Lazy(() => GetAst($"{filesRoot}\\{x}")),
                    Lazy(() => GetJson($"{filesRoot}\\{x}"))
                ));

        public static TheoryData<string, string> TestData = IIFE(() => {
            var filters = EnumerateDirectories(filtersRoot).Select(GetFileName);
            return files.Keys.SelectMany(doc => filters.Select(filter => (doc, filter)));
        }).ToTheoryData();

        //[MemberData(nameof(TestData))]
        //[SkippableTheory]
        //public void AstTest(string docPath, string filterName) {
        //    var astResult = files[docPath].json.Value;
        //    Skip.If(
        //        astResult.ExitCode != 0 || astResult.StdOut.IsNullOrEmpty() || !astResult.StdErr.IsNullOrEmpty(),
        //        $"{(!astResult.StdErr.IsNullOrEmpty() ? astResult.StdErr : "")} - {(astResult.ExitCode != 0 ? astResult.ExitCode.ToString() : "")}"
        //    );

        //    docPath = $"{filesRoot}\\{docPath}";
        //    var filterPath = $@"{filtersRoot}\{filterName}\bin\Debug\net5.0\{filterName}.exe";
        //    var result = GetAst(docPath, filterPath);
        //    Assert.Equal("", result.StdErr);
        //    Assert.Equal(0, result.ExitCode);
        //    Assert.Equal(astResult.StdOut, result.StdOut);
        //}

        [SkippableTheory]
        [MemberData(nameof(TestData))]
        public void JsonTest(string docPath, string filterName) {
            var jsonResult = files[docPath].json.Value;
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
