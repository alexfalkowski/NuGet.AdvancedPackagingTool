namespace Ninemsn.PackageManager.NuGet.Web
{
    using System.Collections.Generic;
    using System.Globalization;

    using global::NuGet;

    public class ErrorLogger : ILogger
    {
        private readonly IList<string> errors = new List<string>();

        public IEnumerable<string> Errors
        {
            get
            {
                return this.errors;
            }
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            if (level == MessageLevel.Warning)
            {
                this.errors.Add(string.Format(CultureInfo.CurrentCulture, message, args));
            }
        }
    }
}