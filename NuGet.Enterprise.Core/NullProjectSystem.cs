namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using NuGet;

    public class NullProjectSystem : IProjectSystem
    {
        public ILogger Logger
        {
            get
            {
                return new NullLogger();
            }

            set
            {
            }
        }

        public string Root
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
                return VersionUtility.DefaultTargetFramework;
            }
        }

        public string ProjectName
        {
            get
            {
                return string.Empty;
            }
        }

        public void DeleteDirectory(string path, bool recursive)
        {
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetFiles(string path, string filter)
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return Enumerable.Empty<string>();
        }

        public string GetFullPath(string path)
        {
            return string.Empty;
        }

        public void DeleteFile(string path)
        {
        }

        public bool FileExists(string path)
        {
            return false;
        }

        public bool DirectoryExists(string path)
        {
            return false;
        }

        public void AddFile(string path, Stream stream)
        {
        }

        public Stream OpenFile(string path)
        {
            return Stream.Null;
        }

        public DateTimeOffset GetLastModified(string path)
        {
            return DateTimeOffset.MinValue;
        }

        public DateTimeOffset GetCreated(string path)
        {
            return DateTimeOffset.MinValue;
        }

        public dynamic GetPropertyValue(string propertyName)
        {
            return new object();
        }

        public void AddReference(string referencePath, Stream stream)
        {
        }

        public void AddFrameworkReference(string name)
        {
        }

        public bool ReferenceExists(string name)
        {
            return false;
        }

        public void RemoveReference(string name)
        {
        }

        public bool IsSupportedFile(string path)
        {
            return false;
        }

        public string ResolvePath(string path)
        {
            return string.Empty;
        }
    }
}