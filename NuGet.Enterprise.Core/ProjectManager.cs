namespace NuGet.Enterprise.Core
{
    using System;

    public class ProjectManager : NuGet.ProjectManager
    {
        public ProjectManager(
            IPackageRepository sourceRepository,
            IPackagePathResolver pathResolver,
            IProjectSystem project,
            IPackageRepository localRepository)
            : base(sourceRepository, pathResolver, project, localRepository)
        {
        }

        public override void AddPackageReference(string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            var package = PackageHelper.ResolvePackage(SourceRepository, LocalRepository, NullConstraintProvider.Instance, packageId, version, allowPrereleaseVersions);

            using (package as IDisposable)
            {
                AddPackageReference(package, ignoreDependencies, allowPrereleaseVersions);
            }
        }
    }
}