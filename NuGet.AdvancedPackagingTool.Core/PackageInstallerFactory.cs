namespace NuGet.AdvancedPackagingTool.Core
{
    public class PackageInstallerFactory : IPackageInstallerFactory
    {
        private readonly IConfigurationManager configurationManager;

        private readonly IDirectorySystem directorySystem;

        private readonly ISourcePackageRepositoryFactory source;

        public PackageInstallerFactory(
            ISourcePackageRepositoryFactory source,
            IConfigurationManager configurationManager,
            IDirectorySystem directorySystem)
        {
            this.source = source;
            this.configurationManager = configurationManager;
            this.directorySystem = directorySystem;
        }

        public IPackageInstaller CreatePackageInstaller(
            bool areArgumentsValid, string installationPath, string configurationPath)
        {
            if (areArgumentsValid)
            {
                var packagePath = this.configurationManager.PackagePath;
                var sourceRepository = this.source.CreatePackageRepository();
                var logger = new PackageLogger();
                var packagePathResolver = new DefaultPackagePathResolver(packagePath);
                var fileSystem = new PhysicalFileSystem(installationPath ?? this.directorySystem.CurrentDirectory)
                    { Logger = logger };
                var destinationRepository = new LocalPackageRepository(packagePath);
                var manager = new PackageManager(
                    sourceRepository, packagePathResolver, fileSystem, destinationRepository) { Logger = logger };
                var powerShellPackageFile = new PowerShellPackageFile(
                    new BackgroundProcess(),
                    manager,
                    new PhysicalFileSystem(this.directorySystem.TemporaryPath),
                    configurationPath);

                return new ValidPackageInstaller(manager, powerShellPackageFile, logger);
            }

            return new InvalidPackageInstaller();
        }
    }
}