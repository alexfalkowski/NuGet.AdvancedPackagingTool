namespace Ninemsn.PackageManager.NuGet
{
    public static class PackageInstallerFactory
    {
        public static IPackageInstaller CreatePackageInstaller(Arguments args)
        {
            if (args == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("args");
            }

            if (args.IsValid)
            {
                var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
                var packageManager = new PackageManagerModule(packageSourceFile);
                var packageSource = string.IsNullOrWhiteSpace(args.Source)
                                        ? packageManager.ActiveSource
                                        : packageManager.GetSource(args.Source);
                var packagePath = ConfigurationManager.PackagePath;

                return new PackageInstaller(packageSource, packagePath, args.Package);
            }

            return new NullPackageInstaller();
        }
    }
}