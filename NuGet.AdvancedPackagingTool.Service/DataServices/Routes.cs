using NuGet.AdvancedPackagingTool.Service.DataServices;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NuGetRoutes), "Start")]

namespace NuGet.AdvancedPackagingTool.Service.DataServices 
{
    using System.Data.Services;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel.Activation;
    using System.Web.Routing;

    using NuGet.Server.DataServices;

    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Nu", Justification = "NuGet is the company.")]
    public static class NuGetRoutes 
    {
        public static void Start() 
        {
            MapRoutes(RouteTable.Routes);
        }

        private static void MapRoutes(RouteCollection routes) 
        {
            // The default route is http://{root}/nuget/Packages
            var factory = new DataServiceHostFactory();
            var serviceRoute = new ServiceRoute("nuget", factory, typeof(Packages))
                {
                    Defaults = new RouteValueDictionary { { "serviceType", "odata" } },
                    Constraints = new RouteValueDictionary { { "serviceType", "odata" } }
                };
            routes.Add("nuget", serviceRoute);
        }
    }
}
