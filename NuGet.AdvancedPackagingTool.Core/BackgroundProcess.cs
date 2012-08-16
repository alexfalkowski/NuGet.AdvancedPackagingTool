namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class BackgroundProcess : IProcess
    {
        public Process CreateStartedProcess(string command, string arguments)
        {
            var processInfo = CreateProcessStartInfo(command, arguments);

            return Process.Start(processInfo);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "No idea why this is complainging.")]
        public ProcessExitInfo ExecuteProcess(string command, string arguments)
        {
            var processStartInfo = CreateProcessStartInfo(command, arguments);

            using (var process = new Process { StartInfo = processStartInfo })
            {
                var output = new StringBuilder();
                var error = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                return new ProcessExitInfo
                    {
                        ExitCode = process.ExitCode, 
                        OutputMessage = output.ToString(), 
                        ErrorMessage = error.ToString()
                    };
            }
        }

        private static ProcessStartInfo CreateProcessStartInfo(string command, string arguments)
        {
            var processInfo = new ProcessStartInfo(command, arguments)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
            return processInfo;
        }
    }
}