namespace NuGet.AdvancedPackagingTool.Core
{
    public class PackageInstallerFactory : IPackageInstallerFactory
    {
        private readonly IConfigurationManager configurationManager;

        private readonly ISourcePackageRepositoryFactory source;

        public PackageInstallerFactory(
            ISourcePackageRepositoryFactory source, IConfigurationManager configurationManager)
        {
            this.source = source;
            this.configurationManager = configurationManager;
        }

        public IPackageInstaller CreatePackageInstaller(
            string packageSourceId, string packageId, bool areArgumentsValid)
        {
            if (areArgumentsValid)
            {
                var packagePath = this.configurationManager.PackagePath;
                var sourceRepository = this.source.CreatePackageRepository();

                return new ValidPackageInstaller(sourceRepository, packagePath, packageId);
            }

            return new InvalidPackageInstaller();
        }
    }
}