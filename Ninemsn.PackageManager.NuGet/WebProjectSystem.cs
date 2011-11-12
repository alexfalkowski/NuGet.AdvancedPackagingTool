namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;
    using System.Linq;

    public class WebProjectSystem : ProjectSystemBase
    {
        public WebProjectSystem(string root, string installationPath)
            : base(root, installationPath)
        {
        }

        protected override string ReferencePath
        {
            get
            {
                return Path.Combine(this.InstallationPath, "bin");
            }
        }

        public override void RemoveReference(string name)
        {
            base.RemoveReference(name);

            if (!Directory.GetFiles(this.ReferencePath).Any())
            {
                Directory.Delete(this.ReferencePath);
            }
        }
    }
}