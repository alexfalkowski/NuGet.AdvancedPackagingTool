﻿namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NuGet;

    public static class PackageExtensions
    {
        private const string PackageParameterName = "package";

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