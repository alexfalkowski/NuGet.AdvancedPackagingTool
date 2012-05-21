namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;
    using System.Globalization;

    using NuGet;

    public class PackageLogger : ILogger
    {
        private readonly IList<string> logs = new List<string>();

        public IEnumerable<string> Logs
        {
            get
            {
                return this.logs;
            }
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            this.logs.Add(string.Format(CultureInfo.CurrentCulture, message, args));
        }
    }
}