namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;

    public class DefaultProjectSystem : ProjectSystemBase
    {
        public DefaultProjectSystem(string root, string installationPath)
            : base(root, installationPath)
        {
        }

        protected override string GetReferencePath(string name)
        {
            return Path.Combine(this.InstallationPath, name);
        }
    }
}