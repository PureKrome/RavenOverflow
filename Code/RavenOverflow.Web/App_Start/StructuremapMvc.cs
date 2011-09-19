using System.Web.Mvc;
using RavenOverflow.Web.DependencyResolution;
using StructureMap;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RavenOverflow.Web.App_Start.StructuremapMvc), "Start")]

namespace RavenOverflow.Web.App_Start {
    public static class StructuremapMvc {
        public static void Start() {
            var container = (IContainer) IoC.Initialize();
            DependencyResolver.SetResolver(new SmDependencyResolver(container));
        }
    }
}