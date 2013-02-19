using System.Web.Mvc;
using RavenOverflow.Web.App_Start;
using RavenOverflow.Web.DependencyResolution;
using StructureMap;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (StructuremapMvc), "Start")]

namespace RavenOverflow.Web.App_Start
{
    public static class StructuremapMvc
    {
        public static void Start()
        {
            IContainer container = IoC.Initialize();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
    }
}