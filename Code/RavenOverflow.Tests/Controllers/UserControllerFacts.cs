using AutoMapper;
using RavenOverflow.Web.AutoMapper;

namespace RavenOverflow.Tests.Controllers
{
    // ReSharper disable InconsistentNaming

    public class UserControllerFacts
    {
        public UserControllerFacts()
        {
            // WebSite requires AutoMapper mappings.
            AutoMapperBootstrapper.ConfigureMappings();
            Mapper.AssertConfigurationIsValid();
        }
    }

    // ReSharper restore InconsistentNaming
}