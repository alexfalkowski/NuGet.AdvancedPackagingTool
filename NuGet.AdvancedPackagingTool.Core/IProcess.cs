namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Diagnostics;

    public interface IProcess
    {
        Process CreateProcess(string command, string arguments);

        ProcessExitInfo ExecuteProcess(string command, string arguments);
    }
}