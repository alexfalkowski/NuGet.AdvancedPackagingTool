namespace NuGet.AdvancedPackagingTool.Core
{
    public class PackageSourceFileFactory : IPackageSourceFileFactory
    {
        public IPackagesSourceFile CreatePackageSourceFile()
        {
            return new PackageSourceFile("PackageSources.config");
        }
    }
}