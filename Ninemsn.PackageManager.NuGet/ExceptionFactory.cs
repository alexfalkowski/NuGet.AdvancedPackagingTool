namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Globalization;

    public static class ExceptionFactory
    {
        public static InvalidOperationException CreateInvalidOperationException(
            string message, 
            params object[] parameters)
        {
            return new InvalidOperationException(FormatMessage(message, parameters));
        }

        public static ArgumentException CreateArgumentException(
            string message, 
            params object[] parameters)
        {
            return new ArgumentException(FormatMessage(message, parameters));
        }

        public static ArgumentNullException CreateArgumentNullException(
            string paramName,
            string message,
            params object[] parameters)
        {
            return new ArgumentNullException(paramName, FormatMessage(message, parameters));
        }

        public static ArgumentNullException CreateArgumentNullException(string paramName)
        {
            return new ArgumentNullException(paramName);
        }

        private static string FormatMessage(string message, object[] parameters)
        {
            return string.Format(CultureInfo.CurrentCulture, message, parameters);
        }
    }
}