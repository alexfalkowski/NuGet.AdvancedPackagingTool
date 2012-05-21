namespace NuGet.AdvancedPackagingTool.Core
{
    public static class PackageInstallerFactory
    {
        public static IPackageInstaller CreatePackageInstaller(
            string packageSourceId, string packageId, bool isPackageValid)
        {
            if (isPackageValid)
            {
                var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
                var packageManager = new PackageManagerModule(packageSourceFile);
                var packageSource = string.IsNullOrWhiteSpace(packageSourceId)
                                        ? packageManager.ActiveSource
                                        : packageManager.GetSource(packageSourceId);
                var packagePath = ConfigurationManager.PackagePath;

                return new ZipPackageInstaller(packageSource, packagePath, packageId);
            }

            return new NullPackageInstaller();
        }
    }
}