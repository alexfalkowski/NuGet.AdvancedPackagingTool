namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;
    using System.Linq;

    using global::NuGet;

    public class PackageManager : IPackageManager
    {
        private readonly IPackageRepository sourceRepository;

        private readonly IPackageRepository localRepository;

        private readonly PackageLogger logger;

        public PackageManager(
            IPackageRepository sourceRepository, 
            IPackageRepository localRepository, 
            IProjectSystem project, 
            PackageLogger logger)
        {
            if (sourceRepository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("sourceRepository");
            }

            if (localRepository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("localRepository");
            }

            if (project == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("project");
            }

            if (logger == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("logger");
            }

            this.sourceRepository = sourceRepository;
            this.localRepository = localRepository;
            this.logger = logger;
            var pathResolver = new DefaultPackagePathResolver(localRepository.Source);
            this.ProjectManager = new ProjectManager(sourceRepository, pathResolver, project, localRepository)
                {
                    Logger = logger 
                };
        }

        public IEnumerable<string> Logs
        {
            get
            {
                return this.logger.Logs;
            }
        }

        internal IProjectManager ProjectManager { get; private set; }

        public IPackage GetUpdate(IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return this.sourceRepository.GetUpdates(new[] { package }).SingleOrDefault();
        }

        public void InstallPackage(IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            this.ProjectManager.AddPackageReference(package.Id, package.Version, false);
        }

        public bool IsPackageInstalled(IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return this.localRepository.Exists(package);
        }

        public void UninstallPackage(IPackage package, bool removeDependencies)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            this.ProjectManager.RemovePackageReference(package.Id, false, removeDependencies);
        }

        public void ExecutePowerShell(IPackageFile file)
        {
            if (file == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("file");
            }

            file.ExecutePowerShell(this.logger);
        }
    }
}