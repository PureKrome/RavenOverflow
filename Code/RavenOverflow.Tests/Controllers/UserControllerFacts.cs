using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CuttingEdge.Conditions;
using Moq;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.AutoMapper;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.Authentication;
using RavenOverflow.Web.Models.ViewModels;
using Xunit;

namespace RavenOverflow.Tests.Controllers
{
    // ReSharper disable InconsistentNaming

    public class UserControllerFacts : RavenDbFactBase
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