namespace NuGet.Enterprise.Core
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Versioning;

    using Ionic.Zip;

    using NuGet.Enterprise.Core.Properties;

    public class ZipPackageAssemblyReference : ZipPackageFile, IPackageAssemblyReference
    {
        public ZipPackageAssemblyReference(ZipEntry entry)
            : base(entry)
        {        
            var directoryName = this.DirectoryName;
            var targetFrameworkString = GetTargetFrameworkString(directoryName);
            this.TargetFramework = VersionUtility.ParseFrameworkName(targetFrameworkString);
            this.Name = System.IO.Path.GetFileName(Path);
        }

        public IEnumerable<FrameworkName> SupportedFrameworks
        {
            get
            {
                if (this.TargetFramework != null)
                {
                    yield return this.TargetFramework;
                }

                yield break;
            }
        }

        public FrameworkName TargetFramework { get; private set; }

        public string Name { get; private set; }

        private string DirectoryName
        {
            get
            {
                var path = this.Path.Substring(3).Trim(System.IO.Path.DirectorySeparatorChar);
                var directoryName = System.IO.Path.GetDirectoryName(path);

                if (string.IsNullOrWhiteSpace(directoryName))
                {
                    throw ExceptionFactory.CreateInvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, Resources.DirectoryNameErrorMessage, path));
                }

                return directoryName;
            }
        }

        private static string GetTargetFrameworkString(string directoryName)
        {
            var targetFrameworkString = directoryName.Split(System.IO.Path.DirectorySeparatorChar).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(targetFrameworkString))
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.TargetFrameworkErrorMessage, directoryName));
            }

            return targetFrameworkString;
        }
    }
}