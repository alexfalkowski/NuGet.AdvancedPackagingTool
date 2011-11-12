﻿namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using global::NuGet;

    public abstract class ProjectSystemBase : PhysicalFileSystem, IProjectSystem
    {
        protected ProjectSystemBase(string installationPath)
            : base(installationPath)
        {
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

        protected abstract string ReferencePath { get; }

        public override IEnumerable<string> GetFiles(string path, string filter)
        {
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.GetFiles(path, filter);
        }

        public override IEnumerable<string> GetDirectories(string path)
        {
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.GetDirectories(path);
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

        public object GetPropertyValue(string propertyName)
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