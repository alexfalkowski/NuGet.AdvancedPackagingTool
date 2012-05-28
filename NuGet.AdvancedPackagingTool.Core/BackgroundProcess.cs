namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Diagnostics;

    public class BackgroundProcess : IProcess
    {
        public Process CreateProcess(string command, string arguments)
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

        public ProcessExitInfo ExecuteProcess(string command, string arguments)
        {
            using (var process = this.CreateProcess(command, arguments))
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