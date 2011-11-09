namespace Ninemsn.PackageManager.NuGet
{
    public class PackageSource : global::NuGet.PackageSource
    {
        public PackageSource(string source, string name) : base(source, name)
        {
        }

        public bool FilterPreferredPackages
        {
            get; set;
        }

        public override bool Equals(object obj)
        {
            var source = (PackageSource)obj;
            return this.Equals(source) && (this.FilterPreferredPackages == source.FilterPreferredPackages);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (this.FilterPreferredPackages ? 1 : 0);
        }
    }
}