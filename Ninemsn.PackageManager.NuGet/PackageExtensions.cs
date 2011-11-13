namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::NuGet;

    public static class PackageExtensions
    {
        public static IEnumerable<IPackageFile> GetToolsFiles(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return
                package.GetFiles().Where(
                    packageFile => packageFile.Path.StartsWith("tools", StringComparison.CurrentCultureIgnoreCase));
        }

        public static IPackageFile GetInitPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Init"));
        }

        public static IPackageFile GetInstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Install"));
        }

        public static IPackageFile GetUninstallPackageFile(this IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return new PowerShellPackageFile(GetToolFile(package.GetToolsFiles(), "Uninstall"));
        }

        private static IPackageFile GetToolFile(IEnumerable<IPackageFile> toolsFiles, string fileName)
        {
            var powershellFilesQuery = toolsFiles.Where(packgeFile => packgeFile.Path.Contains(fileName));
            var powershellFile = powershellFilesQuery.FirstOrDefault();

            return powershellFile ?? new NullPackageFile();
        }
    }
}