namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using NuGet;

    public class DefaultProjectSystem : PhysicalFileSystem, IProjectSystem
    {
        private readonly bool install;

        public DefaultProjectSystem(string installationPath, bool install)
            : base(installationPath)
        {
            this.install = install;
        }

        public string ProjectName
        {
            get
            {
                return this.Root;
            }
        }

        public FrameworkName TargetFramework
        {
            get
            {
                return VersionUtility.DefaultTargetFramework;
            }
        }

        protected virtual string ReferencePath
        {
            get
            {
                return this.Root;
            }
        }

        public override bool FileExists(string path)
        {
            return !this.install && base.FileExists(path);
        }

        public override IEnumerable<string> GetFiles(string path, string filter)
        {
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateFiles(path, filter);
        }

        public override IEnumerable<string> GetDirectories(string path)
        {
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateDirectories(path);
        }

        public void AddFrameworkReference(string name)
        {
            throw new NotSupportedException();
        }

        public void AddReference(string referencePath, Stream stream)
        {
            var fileName = Path.GetFileName(referencePath);
            var fullPath = this.GetFullPath(this.GetReferenceFile(fileName));
            this.AddFile(fullPath, stream);
        }

        public dynamic GetPropertyValue(string propertyName)
        {
            if ((propertyName != null) && propertyName.Equals("RootNamespace", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return null;
        }

        public bool IsSupportedFile(string path)
        {
            return true;
        }

        public string ResolvePath(string path)
        {
            return Path.Combine(this.Root, path);
        }

        public bool ReferenceExists(string name)
        {
            var referencePath = this.GetReferenceFile(name);
            return this.FileExists(referencePath);
        }

        public virtual void RemoveReference(string name)
        {
            this.DeleteFile(this.GetReferenceFile(name));
        }

        protected string GetReferenceFile(string name)
        {
            return Path.Combine(this.ReferencePath, name);
        }
    }
}