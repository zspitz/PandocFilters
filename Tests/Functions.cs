using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ZSpitz.Util;
using static ZSpitz.Util.Functions;

namespace Tests {
    public static class Functions {
        const string pandocPath = @"C:\Program Files\Pandoc\pandoc.exe";

        private static Process getProcess(string docPath, string filter = "", string outputFormat = "native", string inputFormat = "") {
            if (docPath.IsNullOrWhitespace()) { throw new InvalidOperationException("Missing document path."); }
            var args = docPath;
            if (!inputFormat.IsNullOrWhitespace()) { args += $" --from {inputFormat}"; }
            if (!outputFormat.IsNullOrWhitespace()) { args += $" --to {outputFormat}"; }
            if (!filter.IsNullOrWhitespace()) { args += $" --filter {filter}"; }

            return new() {
                StartInfo = {
                    FileName = pandocPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };
        }

        public static ProcessResult GetAst(string docPath, string filter = "", string inputFormat = "") {
            using var process = getProcess(docPath, filter, "native", inputFormat);
            return RunProcess(process);
        }

        public static ProcessResult GetJson(string docPath, string inputFormat = "") {
            using var process = getProcess(docPath, "", "json", inputFormat);
            return RunProcess(process);
        }

        public static async Task<ProcessResult> GetAstAsync(string docPath, string filter = "") {
            using var process = getProcess(docPath, filter, "native");
            return await RunProcessAsync(process);
        }

        public static async Task<ProcessResult> GetJsonAsync(string docPath) {
            using var process = getProcess(docPath, "", "json");
            return await RunProcessAsync(process);
        }

        public static string GetFullFilename(string relativePath) {
            var codeBase = Assembly.GetExecutingAssembly().Location;
            string folder;
            if (codeBase.Contains("AppData\\Local\\Temp", StringComparison.InvariantCultureIgnoreCase)) {
                folder = Directory.GetCurrentDirectory();
            } else {
                if (codeBase == null) { throw new InvalidOperationException(); }
                var executable = new Uri(codeBase).LocalPath;
                folder = Path.GetDirectoryName(executable)!;
            }
            return Path.GetFullPath(Path.Combine(folder, relativePath));
        }

        public static Lazy<T> Lazy<T>(Func<T> valueFactory) => new(valueFactory);
    }
}
