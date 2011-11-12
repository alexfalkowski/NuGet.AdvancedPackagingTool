namespace Ninemsn.PackageManager.NuGet.App
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Ninemsn.PackageManager.NuGet.Properties;

    public class Arguments
    {
        private readonly IList<string> errors = new Collection<string>();

        private bool hasValidated;

        public bool Install { get; set; }

        public bool Uninstall { get; set; }

        public string Package { get; set; }

        public string Destination { get; set; }

        public string Source { get; set; }

        public IEnumerable<string> Errors
        {
            get
            {
                return this.errors;
            }
        }

        public bool IsValid()
        {
            if (!this.hasValidated)
            {
                this.hasValidated = true;

                if ((this.Install && this.Uninstall) || (!this.Install && !this.Uninstall))
                {
                    return this.LogError(Resources.InvalidInstallUninstallFlag);
                }

                var isPackageSpecified = string.IsNullOrWhiteSpace(this.Package);

                if (this.Install && isPackageSpecified)
                {
                    return this.LogError(Resources.PackageNotSpecified);
                }

                if (this.Uninstall && isPackageSpecified)
                {
                    return this.LogError(Resources.PackageNotSpecified);
                }

                if (string.IsNullOrWhiteSpace(this.Source))
                {
                    return this.LogError(Resources.SourceNotSpecified);
                }

                return true;
            }

            return this.errors.Count == 0;
        }

        private bool LogError(string errorMessage)
        {
            this.errors.Add(errorMessage);

            return false;
        }
    }
}