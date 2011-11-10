namespace Ninemsn.PackageManager.NuGet.Console
{
    using Args;

    public class Program
    {
        public static void Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args); 

            if (arguments.Install)
            {
            }
        }
    }
}
