namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using global::NuGet;

    public class PowerShellConsole
    {
        private readonly IPackage package;

        private readonly string script;

        public PowerShellConsole(IPackage package, string script)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            if (string.IsNullOrWhiteSpace("script"))
            {
                throw ExceptionFactory.CreateArgumentNullException("script");
            }

            this.package = package;
            this.script = script;
        }

        private string ModulesScript
        {
            get
            {
                var modules = this.package.GetModuleFiles();
                var moduleStringBuilder = new StringBuilder();

                foreach (var packageFile in modules)
                {
                    moduleStringBuilder.AppendLine(packageFile.GetStream().ReadToEnd());
                }

                return moduleStringBuilder.ToString();
            }
        }

        public ProcessExitInfo Start()
        {
            var firstLineFromScript = this.script.Substring(0, this.script.IndexOf(Environment.NewLine, StringComparison.CurrentCulture));
            var restOfScript = this.script.Replace(firstLineFromScript, string.Empty);
            var completeScript = string.Concat(
                firstLineFromScript, Environment.NewLine, this.ModulesScript, Environment.NewLine, restOfScript);
            var tempFile = Path.GetTempPath() + @"\" + Guid.NewGuid() + ".ps1";

            File.WriteAllText(tempFile, completeScript);

            const string Format = "-inputformat none -NoProfile -ExecutionPolicy unrestricted -Command \" & '{0}' '{1}' '{2}' \" ";
            var parameters = string.Format(CultureInfo.CurrentCulture, Format, tempFile, this.package.ProjectUrl.AbsolutePath, "$null");

            var info = ProcessHelper.ExecuteBackgroundProcess("powershell.exe", parameters);
            PathHelper.SafeDelete(tempFile);

            return info;
        }
    }
}