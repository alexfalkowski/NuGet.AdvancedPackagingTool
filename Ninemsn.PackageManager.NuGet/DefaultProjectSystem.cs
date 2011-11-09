namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;

    public class DefaultProjectSystem : ProjectSystemBase
    {
        public DefaultProjectSystem(string root, string installationPath)
            : base(root, installationPath)
        {
        }

        public override void RemoveReference(string name)
        {
            this.DeleteFile(this.GetReferencePath(name));
        }

        protected override string GetReferencePath(string name)
        {
            return Path.Combine(this.InstallationPath, name);
        }
    }
}