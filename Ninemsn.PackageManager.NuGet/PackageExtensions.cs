namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

    using global::NuGet;

    public static class PackageExtensions
    {
        public static IEnumerable<IPackageFile> GetToolsFiles(this IPackage package)
        {
            return package.GetFiles().Where(packageFile => packageFile.Path.StartsWith("tools"));
        }

        public static IPackageFile GetInitPackageFile(this IPackage package)
        {
            return new InitPackageFile(GetToolFile(package.GetToolsFiles(), "Init.ps1"));
        }

        public static IPackageFile GetInstallPackageFile(this IPackage package)
        {
            return new InstallPackageFile(GetToolFile(package.GetToolsFiles(), "Install.ps1"));
        }

        public static IPackageFile GetUninstallPackageFile(this IPackage package)
        {
            return new UninstallPackageFile(GetToolFile(package.GetToolsFiles(), "Uninstall.ps1"));
        }

        private static IPackageFile GetToolFile(IEnumerable<IPackageFile> toolsFiles, string fileName)
        {
            var powershellFile = toolsFiles.Where(packgeFile => packgeFile.Path.EndsWith(fileName)).FirstOrDefault();

            if (powershellFile == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.FileNameDoesNotExistInToolsFolder, fileName);
            }

            return powershellFile;
        }
    }
}