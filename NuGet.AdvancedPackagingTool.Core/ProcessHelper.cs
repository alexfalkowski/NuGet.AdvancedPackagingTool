namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Diagnostics;

    public static class ProcessHelper
    {
        public static Process CreateBackgroundProcess(string command, string arguments)
        {
            var processInfo = new ProcessStartInfo(command, arguments)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

            return Process.Start(processInfo);
        }

        public static ProcessExitInfo ExecuteBackgroundProcess(string command, string arguments)
        {
            using (var process = CreateBackgroundProcess(command, arguments))
            {
                process.WaitForExit();

                return new ProcessExitInfo
                    {
                        ExitCode = process.ExitCode,
                        OutputMessage = process.StandardOutput.ReadToEnd(),
                        ErrorMessage = process.StandardError.ReadToEnd()
                    };
            }
        }
    }
}