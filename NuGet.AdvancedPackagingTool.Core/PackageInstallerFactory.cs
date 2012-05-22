namespace NuGet.AdvancedPackagingTool.Core
{
    public static class PackageInstallerFactory
    {
        public static IPackageInstaller CreatePackageInstaller(
            string packageSourceId, string packageId, bool areArgumentsValid)
        {
            if (areArgumentsValid)
            {
                var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
                var packageManager = new PackageManagerModule(packageSourceFile);
                var packageSource = string.IsNullOrWhiteSpace(packageSourceId)
                                        ? packageManager.ActiveSource
                                        : packageManager.GetSource(packageSourceId);
                var packagePath = ConfigurationManager.PackagePath;

                return new ValidPackageInstaller(packageSource, packagePath, packageId);
            }

            return new InvalidPackageInstaller();
        }
    }
}