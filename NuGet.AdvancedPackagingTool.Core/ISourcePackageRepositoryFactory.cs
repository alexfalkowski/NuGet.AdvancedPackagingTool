namespace NuGet.AdvancedPackagingTool.Core
{
    public interface ISourcePackageRepositoryFactory
    {
        IPackageRepository CreatePackageRepository();
    }
}