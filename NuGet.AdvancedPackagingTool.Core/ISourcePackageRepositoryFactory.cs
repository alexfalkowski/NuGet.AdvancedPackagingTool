namespace NuGet.AdvancedPackagingTool.Core
{
    using System;

    public interface ISourcePackageRepositoryFactory
    {
        IPackageRepository CreatePackageRepository();
    }

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
            return source.IsUri()
                       ? new LocalPackageRepository(source)
                       : (IPackageRepository)new DataServicePackageRepository(new Uri(source));
        }
    }
}