namespace Ninemsn.PackageManager.NuGet
{
    public class ProcessExitInfo
    {
        public int ExitCode { get; set; }

        public string ErrorMessage { get; set; }

        public string OutputMessage { get; set; }
    }
}