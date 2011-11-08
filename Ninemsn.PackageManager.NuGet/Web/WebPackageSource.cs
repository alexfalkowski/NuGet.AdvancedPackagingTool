namespace Ninemsn.PackageManager.NuGet.Web
{
    using global::NuGet;

    public class WebPackageSource : PackageSource
    {
        public WebPackageSource(string source, string name) : base(source, name)
        {
        }

        public bool FilterPreferredPackages
        {
            get; set;
        }

        public override bool Equals(object obj)
        {
            var source = (WebPackageSource)obj;
            return this.Equals(source) && (this.FilterPreferredPackages == source.FilterPreferredPackages);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (this.FilterPreferredPackages ? 1 : 0);
        }
    }
}