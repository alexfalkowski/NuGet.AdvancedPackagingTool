namespace Ninemsn.PackageManager.NuGet.App
{
    public static class PackageInstallerFactory
    {
        public static IPackageInstaller CreatePackageInstaller(Arguments args)
        {
            if (args.IsValid())
            {
            }

            return new NullPackageInstaller();
        }
    }
}