﻿namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;
    using System.Linq;

    public class WebProjectSystem : ProjectSystemBase
    {
        public WebProjectSystem(string installationPath)
            : base(installationPath)
        {
        }

        protected override string ReferencePath
        {
            get
            {
                return Path.Combine(this.Root, "bin");
            }
        }

        public override void RemoveReference(string name)
        {
            base.RemoveReference(name);

            if (!this.GetFiles("bin").Any())
            {
                this.DeleteDirectory("bin");
            }
        }
    }
}