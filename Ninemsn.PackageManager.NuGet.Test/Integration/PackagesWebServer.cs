namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System.Diagnostics;
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

            const string FileName = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";

            const string Arguments =
                @"/path:""C:\Users\Alex\Documents\visual studio 2010\Projects\Ninemsn.PackageManager\Ninemsn.PackageManager.NuGet.Server"" /port:4907";

            this.process = Process.Start(FileName, Arguments);

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
