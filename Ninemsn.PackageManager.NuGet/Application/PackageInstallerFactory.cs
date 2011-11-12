namespace Ninemsn.PackageManager.NuGet.Application
{
    using System.Configuration;
    using System.IO;
    using System.Linq;

    public static class PackageInstallerFactory
    {
        public static IPackageInstaller CreatePackageInstaller(Arguments args)
        {
            if (args.IsValid)
            {
                var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
                var packageManager = new PackageManagerModule(packageSourceFile);
                var packageSource = string.IsNullOrWhiteSpace(args.Source)
                                        ? packageManager.PackageSources.First()
                                        : packageManager.GetSource(args.Source);
                var packagePath = ConfigurationManager.AppSettings["PackagePath"];
                var installationPath = Path.Combine(Directory.GetCurrentDirectory(), args.Destination ?? string.Empty);

                return new PackageInstaller(packageSource, packagePath, args.Package, installationPath);
            }

            return new NullPackageInstaller();
        }
    }
}