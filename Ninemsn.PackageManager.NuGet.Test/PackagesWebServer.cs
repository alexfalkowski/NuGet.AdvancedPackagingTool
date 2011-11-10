namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    public class PackagesWebServer
    {
        private Process process;

        public void StartUp()
        {
            if (Debugger.IsAttached)
            {
                return;
            }

            foreach (var runningProcess in Process.GetProcessesByName("iisexpress"))
            {
                try
                {
                    runningProcess.Kill();
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                    // Ignore the exception.
                }
            }

            var currentExecutingDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            if (currentExecutingDirectoryName == null)
            {
                return;
            }

            const string FileName = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";
            var currentPath = new Uri(currentExecutingDirectoryName).LocalPath;
            var path = Path.GetFullPath(Path.Combine(currentPath, @"..\..\..\Ninemsn.PackageManager.NuGet.Service"));
            var arguments = string.Format(CultureInfo.CurrentCulture, @"/path:""{0}"" /port:1544", path);

            this.process = Process.Start(FileName, arguments);
            Thread.Sleep(3000);
        }

        public void Stop()
        {
            if (Debugger.IsAttached)
            {
                return;
            }

            if (this.process != null)
            {
                this.process.Kill();
            }
        } 
    }
}
