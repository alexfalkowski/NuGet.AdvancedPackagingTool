namespace Ninemsn.PackageManager.NuGet.App
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Ninemsn.PackageManager.NuGet.Properties;

    public class Arguments
    {
        private readonly IList<string> errors = new Collection<string>();

        public bool Install { get; set; }

        public bool Uninstall { get; set; }

        public string Package { get; set; }

        public string Destination { get; set; }

        public bool IsValid()
        {
            if (this.Install && this.Uninstall)
            {
                this.errors.Add(Resources.InvalidInstallUninstallFlag);
            }

            return this.errors.Count == 0;
        }
    }
}