using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ZSpitz.Util;

namespace Tests {
    public static class Functions {
        const string pandocPath = @"c:\Program Files\Pandoc\pandoc.exe";

        private static Process getProcess(string docPath, string filter = "", string outputFormat = "native", string inputFormat = "") {
            if (docPath.IsNullOrWhitespace()) { throw new InvalidOperationException("Missing document path."); }
            string args = docPath;
            if (!inputFormat.IsNullOrWhitespace()) { args += $" --from {inputFormat}"; }
            if (!outputFormat.IsNullOrWhitespace()) { args += $" --to {outputFormat}"; }
            if (!filter.IsNullOrWhitespace()) { args += $" --filter {filter}"; }

            return new Process {
                StartInfo = {
                        FileName = $@"""{pandocPath}""",
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

        public static ProcessResult RunProcess(Process process, string input = "") {
            var output = "";
            var error = "";
            process.OutputDataReceived += (s, ea) => output += ea.Data;
            process.ErrorDataReceived += (s, ea) => error += ea.Data;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!input.IsNullOrEmpty()) {
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
            }
            process.WaitForExit();
            return new ProcessResult(process.ExitCode, output, error);
        }


        public static async Task<ProcessResult> GetAstAsync(string docPath, string filter = "") {
            using var process = getProcess(docPath, filter, "native");
            return await RunProcessAsync(process);
        }

        public static async Task<ProcessResult> GetJsonAsync(string docPath) {
            using var process = getProcess(docPath, "", "json");
            return await RunProcessAsync(process);
        }

        public static Task<ProcessResult> RunProcessAsync(Process process, string input = "") {
            var tcs = new TaskCompletionSource<ProcessResult>();
            var (output, error) = ("", "");
            process.Exited += (s, e) => tcs.SetResult(new ProcessResult(process.ExitCode, output, error));
            process.OutputDataReceived += (s, ea) => output += ea.Data;
            process.ErrorDataReceived += (s, ea) => error += ea.Data;

            if (!process.Start()) {
                // what happens to the Exited event if process doesn't start successfully?
                throw new InvalidOperationException();
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!input.IsNullOrEmpty()) {
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
            }

            return tcs.Task;
        }

        public static string GetFullFilename(string relativePath) {
            var codeBase = Assembly.GetExecutingAssembly().Location;
            if (codeBase == null) { throw new InvalidOperationException(); }
            var executable = new Uri(codeBase).LocalPath;
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(executable)!, relativePath));
        }

        public static Lazy<T> Lazy<T>(Func<T> valueFactory) => new Lazy<T>(valueFactory);
    }
}
