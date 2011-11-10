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

        public override void RemoveReference(string name)
        {
            base.RemoveReference(name);

            if (!this.GetFiles("bin").Any())
            {
                this.DeleteDirectory("bin");
            }
        }

        protected override string GetReferencePath(string name)
        {
            return Path.Combine(this.InstallationPath, "bin", name);
        }
    }
}