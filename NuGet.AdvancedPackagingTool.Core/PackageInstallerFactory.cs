namespace NuGet.AdvancedPackagingTool.Core
{
    using System;

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

                var source = packageSource.Source;
                var sourceRepository = source.IsUri()
                                           ? new LocalPackageRepository(source)
                                           : (IPackageRepository)new DataServicePackageRepository(new Uri(source));

                return new ValidPackageInstaller(sourceRepository, packagePath, packageId);
            }

            return new InvalidPackageInstaller();
        }
    }
}