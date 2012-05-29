namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IShellPackageFile
    {
        void Execute(IPackageFile file, IPackage package, ILogger logger);
    }
}