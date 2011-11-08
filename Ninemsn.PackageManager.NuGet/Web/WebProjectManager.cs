namespace Ninemsn.PackageManager.NuGet.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::NuGet;

    public class WebProjectManager
    {
        private readonly IProjectManager projectManager;

        public WebProjectManager(string remoteSource, string localSource)
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(remoteSource);
            var pathResolver = new DefaultPackagePathResolver(localSource);
            var localRepository = PackageRepositoryFactory.Default.CreateRepository(localSource);
            var project = new WebProjectSystem(localSource);
            this.projectManager = new ProjectManager(sourceRepository, pathResolver, project, localRepository);
        }

        public IPackageRepository LocalRepository
        {
            get
            {
                return this.projectManager.LocalRepository;
            }
        }

        public IPackageRepository SourceRepository
        {
            get
            {
                return this.projectManager.SourceRepository;
            }
        }

        public static IQueryable<IPackage> GetPackages(IQueryable<IPackage> packages, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                packages = packages.Find(searchTerm.Split(new char[0])[0]);
            }

            return packages;
        }

        public static IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            return
                GetPackageDependencies(package, localRepository, sourceRepository).Where(
                    p => p.RequireLicenseAcceptance);
        }

        public IQueryable<IPackage> GetPackages(IPackageRepository repository, string searchTerm)
        {
            return GetPackages(repository.GetPackages(), searchTerm);
        }

        public IQueryable<IPackage> GetInstalledPackages(string searchTerms)
        {
            return GetPackages(this.LocalRepository, searchTerms);
        }

        public IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package)
        {
            var localRepository = this.LocalRepository;
            var sourceRepository = this.SourceRepository;
            return GetPackagesRequiringLicenseAcceptance(package, localRepository, sourceRepository);
        }

        public IQueryable<IPackage> GetPackagesWithUpdates(string searchTerms)
        {
            return GetPackages(this.SourceRepository.GetUpdates(this.LocalRepository.GetPackages()).AsQueryable(), searchTerms);
        }

        public IQueryable<IPackage> GetRemotePackages(string searchTerms)
        {
            return GetPackages(this.SourceRepository, searchTerms);
        }

        public IPackage GetUpdate(IPackage package)
        {
            return this.SourceRepository.GetUpdates(new[] { package }).SingleOrDefault();
        }

        public IEnumerable<string> InstallPackage(IPackage package)
        {
            return this.PerformLoggedAction(
                () => this.projectManager.AddPackageReference(package.Id, package.Version, false));
        }

        public bool IsPackageInstalled(IPackage package)
        {
            return this.LocalRepository.Exists(package);
        }

        public IEnumerable<string> UninstallPackage(IPackage package, bool removeDependencies)
        {
            return this.PerformLoggedAction(
                () => this.projectManager.RemovePackageReference(package.Id, false, removeDependencies));
        }

        public IEnumerable<string> UpdatePackage(IPackage package)
        {
            return
                this.PerformLoggedAction(
                    () => this.projectManager.UpdatePackageReference(package.Id, package.Version, true));
        }

        private static IEnumerable<IPackage> GetPackageDependencies(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            var repository = localRepository;
            var repository2 = sourceRepository;
            var instance = NullLogger.Instance;
            var walker = new InstallWalker(repository, repository2, instance, false);
            return
                walker.ResolveOperations(package).Where(operation => operation.Action == PackageAction.Install).Select(
                    operation => operation.Package);
        }

        private IEnumerable<string> PerformLoggedAction(Action action)
        {
            var logger = new ErrorLogger();
            this.projectManager.Logger = logger;
            try
            {
                action();
            }
            finally
            {
                this.projectManager.Logger = null;
            }

            return logger.Errors;
        }
    }
}