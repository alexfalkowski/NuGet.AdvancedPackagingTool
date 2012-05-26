namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IPackageSourceFileFactory
    {
        IPackagesSourceFile CreatePackageSourceFile();
    }
}