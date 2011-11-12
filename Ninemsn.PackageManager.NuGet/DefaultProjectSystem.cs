namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;

    public class DefaultProjectSystem : ProjectSystemBase
    {
        public DefaultProjectSystem(string root, string installationPath)
            : base(root, installationPath)
        {
        }

        protected override string ReferencePath
        {
            get
            {
                return this.InstallationPath;
            }
        }
    }
}