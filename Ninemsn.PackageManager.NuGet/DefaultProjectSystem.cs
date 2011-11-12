namespace Ninemsn.PackageManager.NuGet
{
    public class DefaultProjectSystem : ProjectSystemBase
    {
        public DefaultProjectSystem(string installationPath)
            : base(installationPath)
        {
        }

        protected override string ReferencePath
        {
            get
            {
                return this.Root;
            }
        }
    }
}