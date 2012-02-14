using NuGet.Enterprise.Service.DataServices;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NuGetRoutes), "Start")]

namespace NuGet.Enterprise.Service.DataServices 
{
    using System.Data.Services;
    using System.ServiceModel.Activation;
    using System.Web.Routing;

    using NuGet.Server.DataServices;

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
