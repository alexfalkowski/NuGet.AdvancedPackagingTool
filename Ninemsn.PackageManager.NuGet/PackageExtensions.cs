namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

    using global::NuGet;

    public static class PackageExtensions
    {
        private const string PackageParameterName = "package";

        public static IEnumerable<IPackageFile> GetToolsFiles(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            return
                package.GetFiles().Where(
                    packageFile => packageFile.Path.StartsWith("tools", StringComparison.CurrentCultureIgnoreCase));
        }

        public static IEnumerable<IPackageFile> GetModuleFiles(this IPackage package)
        {
            var toolFiles = package.GetToolsFiles();

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

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Setup"));
        }

        public static IPackageFile GetInstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Install"));
        }

        public static IPackageFile GetUninstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Uninstall"));
        }

        public static IPackageFile GetTeardownPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Teardown"));
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
    }
}