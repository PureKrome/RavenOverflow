using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CuttingEdge.Conditions;
using Moq;
using Raven.Client;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Tests
{
    public static class ControllerUtilities
    {
        // Reference: http://nerddinnerbook.s3.amazonaws.com/Part12.htm
        //            Yes .. Nerd Dinner to the rescue! and we come full circle...
        public static T HomeController<T>(IDocumentSession documentSession,
                                                     string userId = null,
                                                     string displayName = null,
                                                     string[] roles = null) where T : AbstractController, new()
        {
            Condition.Requires(documentSession);

            // Some fake Authentication stuff.
            var customIdentity = new CustomIdentity(userId, displayName);
            var customPrincipal = new CustomPrincipal(customIdentity, roles);

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(x => x.HttpContext.User).Returns(customPrincipal);

            var homeController = new T(documentSession) { ControllerContext = mockControllerContext.Object };

            return homeController;
        }
    }
}
