namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NuGet;

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
            return package.GetPackageFile("Setup");
        }

        public static IPackageFile GetInstallPackageFile(this IPackage package)
        {
            return package.GetPackageFile("Install");
        }

        public static IPackageFile GetUninstallPackageFile(this IPackage package)
        {
            return package.GetPackageFile("Uninstall");
        }

        public static IPackageFile GetTeardownPackageFile(this IPackage package)
        {
            return package.GetPackageFile("Teardown");
        }

        public static IPackageFile GetConfigurationPackageFile(this IPackage package)
        {
            return package.GetPackageFile("Configuration");
        }

        public static bool IsValid(this IPackageMetadata package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            return package.ProjectUrl != null;
        }

        private static IPackageFile GetPackageFile(this IPackage package, string fileName)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException(PackageParameterName);
            }

            var toolFiles = package.GetFiles(Constants.ToolsDirectory);

            return GetToolFile(toolFiles, fileName);
        }

        private static IPackageFile GetToolFile(IEnumerable<IPackageFile> toolsFiles, string fileName)
        {
            var powershellFilesQuery =
                toolsFiles.Where(
                    packgeFile => packgeFile.Path.Contains(fileName, StringComparison.CurrentCultureIgnoreCase));
            var powershellFile = powershellFilesQuery.FirstOrDefault();

            return powershellFile ?? new NullPackageFile();
        }
    }
}