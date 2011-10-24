using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace RavenOverflow.Web.DependencyResolution
{
    public class SmDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public SmDependencyResolver(IContainer container)
        {
            _container = container;
        }

        #region IDependencyResolver Members

        public object GetService(Type serviceType)
        {
            if (serviceType == null) return null;

            return serviceType.IsAbstract || serviceType.IsInterface
                       ? _container.TryGetInstance(serviceType)
                       : _container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        #endregion
    }
}