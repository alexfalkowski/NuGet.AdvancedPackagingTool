namespace NuGet.AdvancedPackagingTool.Core
{
    public class ProcessExitInfo
    {
        public int ExitCode { get; set; }

        public string ErrorMessage { get; set; }

        public string OutputMessage { get; set; }
    }
}