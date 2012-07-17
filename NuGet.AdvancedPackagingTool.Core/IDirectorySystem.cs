namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IDirectorySystem
    {
        string CurrentDirectory { get; }

        string TemporaryPath { get; }
    }
}