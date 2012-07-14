namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using NuGet;

    public class NullPackageFile : IPackageFile
    {
        public string Path
        {
            get
            {
                return string.Empty;
            }
        }

        public string EffectivePath
        {
            get
            {
                return string.Empty;
            }
        }

        public FrameworkName TargetFramework
        {
            get
            {
                return new FrameworkName("net40");
            }
        }

        public IEnumerable<FrameworkName> SupportedFrameworks
        {
            get
            {
                return Enumerable.Empty<FrameworkName>();
            }
        }

        public Stream GetStream()
        {
            return Stream.Null;
        }
    }
}