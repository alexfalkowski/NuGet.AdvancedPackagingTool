namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System.Diagnostics;
    using System.Threading;

    public class PackagesWebServer
    {
        private Process process;

        public string Source
        {
            get
            {
                return "http://localhost:4907/DataServices/Packages.svc";
            }
        }

        public void StartUp()
        {
            const string FileName = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";
            const string Arguments =
                @"/path:""C:\Users\Alex\Documents\visual studio 2010\Projects\Ninemsn.PackageManager\Ninemsn.PackageManager.NuGet.Server"" /port:4907";

            this.process = Process.Start(FileName, Arguments);

            Thread.Sleep(3000);
        }

        public void Stop()
        {
            this.process.Kill();
        }
    }
}
