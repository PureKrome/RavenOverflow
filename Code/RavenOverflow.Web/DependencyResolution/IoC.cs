using System.Configuration;
using StructureMap;

namespace RavenOverflow.Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry(
                                                 new RavenDbRegistry(ConfigurationManager.ConnectionStrings[0].Name));
                                             x.AddRegistry(
                                                 new AuthenticationRegistry(
                                                     ConfigurationManager.AppSettings["FacebookAppId"],
                                                     ConfigurationManager.AppSettings["FacebookSecret"]
                                                     ));
                                         });
            return ObjectFactory.Container;
        }
    }
}