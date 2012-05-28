namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using NuGet;
    using NuGet.AdvancedPackagingTool.Core.Properties;

    public static class PackageExtensions
    {
        private const string PackageParameterName = "package";

        public static IEnumerable<IPackageFile> GetModuleFiles(this IPackage package)
        {
            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

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

            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

            return GetToolFile(toolFiles, "Setup");
        }

        public static IPackageFile GetInstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

            return GetToolFile(toolFiles, "Install");
        }

        public static IPackageFile GetUninstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

            return GetToolFile(toolFiles, "Uninstall");
        }

        public static IPackageFile GetTeardownPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

            return GetToolFile(toolFiles, "Teardown");
        }

        public static IPackageFile GetConfigurationPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

            return GetToolFile(toolFiles, "Configuration");
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

            var console = new PowerShellConsole(package, new BackgroundProcess(), file.GetStream().ReadToEnd());
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

            foreach (var message in processExitInfo.OutputMessage.Split('\n').Where(message => !string.IsNullOrWhiteSpace(message)))
            {
                logger.Log(MessageLevel.Info, message.Trim());
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