namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;
    using System.Linq;

    public class WebProjectSystem : DefaultProjectSystem
    {
        private const string BinFolderName = "bin";

        public WebProjectSystem(string installationPath)
            : base(installationPath)
        {
        }

        protected override string ReferencePath
        {
            get
            {
                return Path.Combine(base.ReferencePath, BinFolderName);
            }
        }

        public override void RemoveReference(string name)
        {
            base.RemoveReference(name);

            if (!this.GetFiles(BinFolderName).Any())
            {
                this.DeleteDirectory(BinFolderName);
            }
        }
    }
}