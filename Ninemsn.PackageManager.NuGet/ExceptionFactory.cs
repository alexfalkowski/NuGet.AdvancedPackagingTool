namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Globalization;

    public static class ExceptionFactory
    {
        public static InvalidOperationException CreateInvalidOperationException(
            string message, params object[] parameters)
        {
            return new InvalidOperationException(FormatMessage(message, parameters));
        }

        private static string FormatMessage(string message, object[] parameters)
        {
            return string.Format(CultureInfo.CurrentCulture, message, parameters);
        }
    }
}