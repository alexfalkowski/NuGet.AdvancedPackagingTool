namespace Ninemsn.PackageManager.NuGet
{
    using System;
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

        public static Tuple<InitPackageFile, InstallPackageFile, UninstallPackageFile> GetPowerShellFiles(this IPackage package)
        {
            var initPowershellFile = new InitPackageFile(GetToolFile(package.GetToolsFiles(), "Init.ps1"));
            var installPowershellFile = new InstallPackageFile(GetToolFile(package.GetToolsFiles(), "Install.ps1"));
            var unistallPowershellFile = new UninstallPackageFile(GetToolFile(package.GetToolsFiles(), "Uninstall.ps1"));

            return new Tuple<InitPackageFile, InstallPackageFile, UninstallPackageFile>(
                initPowershellFile, installPowershellFile, unistallPowershellFile);
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