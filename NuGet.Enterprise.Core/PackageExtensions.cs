namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using NuGet;
    using NuGet.Enterprise.Core.Properties;

    public static class PackageExtensions
    {
        private const string PackageParameterName = "package";

        private static readonly IDictionary<string, IList<IPackageFile>> Cache = new Dictionary<string, IList<IPackageFile>>();

        public static IEnumerable<IPackageFile> GetModuleFiles(this IPackage package)
        {
            var toolFiles = GetCachedToolFiles(package);

            return toolFiles.Where(packageFile =>
                {
                    var extension = Path.GetExtension(packageFile.Path);

                    return extension != null && extension.Equals(".psm1", StringComparison.CurrentCultureIgnoreCase);
                });
        }

        public static IPackageFile GetSetupPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = GetCachedToolFiles(package);

            return new PowerShellPackageFile(GetToolFile(toolFiles, "Setup"));
        }

        public static IPackageFile GetInstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = GetCachedToolFiles(package);

            return new PowerShellPackageFile(GetToolFile(toolFiles, "Install"));
        }

        public static IPackageFile GetUninstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = GetCachedToolFiles(package);

            return new PowerShellPackageFile(GetToolFile(toolFiles, "Uninstall"));
        }

        public static IPackageFile GetTeardownPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = GetCachedToolFiles(package);

            return new PowerShellPackageFile(GetToolFile(toolFiles, "Teardown"));
        }

        public static IPackageFile GetConfigurationPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = GetCachedToolFiles(package);

            return new PowerShellPackageFile(GetToolFile(toolFiles, "Configuration"));
        }

        public static bool IsValid(this IPackageMetadata package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            return package.ProjectUrl != null;
        }

        public static void ExecutePowerShell(this IPackageFile file, IPackage package, ILogger logger)
        {
            if (file == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("file");
            }

            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            if (logger == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("logger");
            }

            var console = new PowerShellConsole(package, file.GetStream().ReadToEnd());
            var processExitInfo = console.Start();

            if (processExitInfo.ExitCode > 0)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PowershellErrorMessage,
                        file.Path,
                        processExitInfo.ErrorMessage));
            }

            foreach (var message in processExitInfo.OutputMessage.Split('\n'))
            {
                logger.Log(MessageLevel.Info, message);
            }
        }

        private static IPackageFile GetToolFile(IEnumerable<IPackageFile> toolsFiles, string fileName)
        {
            var powershellFilesQuery = toolsFiles.Where(packgeFile => packgeFile.Path.Contains(fileName, StringComparison.CurrentCultureIgnoreCase));
            var powershellFile = powershellFilesQuery.FirstOrDefault();

            return powershellFile ?? new NullPackageFile();
        }

        private static IEnumerable<IPackageFile> GetCachedToolFiles(IPackage package)
        {
            IList<IPackageFile> toolFiles;
            var fullName = package.GetFullName();

            if (!Cache.TryGetValue(fullName, out toolFiles))
            {
                toolFiles = package.GetFiles(Constants.ToolsDirectory).ToList();
                Cache.Add(fullName, toolFiles);
            }

            return toolFiles;
        }
    }
}