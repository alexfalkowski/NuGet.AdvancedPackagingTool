namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;

    using global::NuGet;

    public class PackageManager : IPackageManager
    {
        private readonly IPackageRepository sourceRepository;

        private readonly IPackageRepository localRepository;

        private readonly PackageLogger logger;

        private readonly IProjectManager projectManager;

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
            this.projectManager = new ProjectManager(sourceRepository, pathResolver, project, localRepository);
        }

        public IEnumerable<string> Logs
        {
            get
            {
                return this.logger.Logs;
            }
        }

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

            this.ExecuteUsingLogger(() => this.projectManager.AddPackageReference(package.Id, package.Version, false));
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

            this.ExecuteUsingLogger(
                () => this.projectManager.RemovePackageReference(package.Id, false, removeDependencies));
        }

        public void ExecutePowerShell(IPackageFile file)
        {
            if (file == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("file");
            }

            using (var powerShell = PowerShell.Create())
            {
                var scriptContents = file.GetStream().ReadToEnd();
                powerShell.AddScript(scriptContents);

                var stringBuilder = new StringBuilder();

                foreach (var result in powerShell.Invoke())
                {
                    stringBuilder.AppendLine(result.ToString());
                }

                var executePowerShell = stringBuilder.ToString().Trim();

                this.logger.Log(MessageLevel.Info, executePowerShell);
            }
        }

        private void ExecuteUsingLogger(Action actionToExecute)
        {
            this.projectManager.Logger = this.logger;

            try
            {
                actionToExecute();
            }
            finally
            {
                this.projectManager.Logger = null;
            }
        }
    }
}