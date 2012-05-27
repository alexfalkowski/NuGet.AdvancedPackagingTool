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

        public IPackageInstaller CreatePackageInstaller(string packageId, bool areArgumentsValid)
        {
            if (areArgumentsValid)
            {
                var packagePath = this.configurationManager.PackagePath;
                var sourceRepository = this.source.CreatePackageRepository();
                var logger = new PackageLogger();
                var packagePathResolver = new DefaultPackagePathResolver(packagePath);
                var fileSystem = new PhysicalFileSystem(packagePath) { Logger = logger };
                var destinationRepository = new LocalPackageRepository(packagePath);
                var manager = new PackageManager(
                    sourceRepository, packagePathResolver, fileSystem, destinationRepository) { Logger = logger };

                return new ValidPackageInstaller(manager, logger, packageId);
            }

            return new InvalidPackageInstaller();
        }
    }
}