namespace Ninemsn.PackageManager.NuGet.Console
{
    using System;

    using Args;

    using Common.Logging;

    using Ninemsn.PackageManager.NuGet.Application;
    using Ninemsn.PackageManager.NuGet.Console.Properties;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args);
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var program = new Application.Program(arguments, PackageInstallerFactory.CreatePackageInstaller(arguments));
                program.Start();
            }
            catch (Exception e)
            {
                logger.Error(Resources.InvalidExecutionErrorMessage, e);

                throw ExceptionFactory.CreateInvalidOperationException(Resources.InvalidExecutionExceptionMessage);
            }
        }
    }
}
