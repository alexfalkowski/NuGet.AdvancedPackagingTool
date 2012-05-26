namespace NuGet.AdvancedPackagingTool.Core
{
    using System;

    public class SourcePackageRepositoryFactory : ISourcePackageRepositoryFactory
    {
        private readonly PackageSource packageSource;

        public SourcePackageRepositoryFactory(PackageSource packageSource)
        {
            this.packageSource = packageSource;
        }

        public IPackageRepository CreatePackageRepository()
        {
            var source = this.packageSource.Source;
            var isUri = source.IsHttpUri();
            return isUri
                       ? (IPackageRepository)new DataServicePackageRepository(new Uri(source))
                       : new LocalPackageRepository(source);
        }
    }
}