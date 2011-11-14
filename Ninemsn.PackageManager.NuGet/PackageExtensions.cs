namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;

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

        public static void ExecutePowerShell(this IPackageFile file, ILogger logger)
        {
            if (file == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("file");
            }

            if (logger == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("logger");
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

                logger.Log(MessageLevel.Info, executePowerShell);
            }
        }

        private static IPackageFile GetToolFile(IEnumerable<IPackageFile> toolsFiles, string fileName)
        {
            var powershellFilesQuery = toolsFiles.Where(packgeFile => packgeFile.Path.Contains(fileName));
            var powershellFile = powershellFilesQuery.FirstOrDefault();

            return powershellFile ?? new NullPackageFile();
        }
    }
}