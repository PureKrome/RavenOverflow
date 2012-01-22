using System.Web.Mvc;
using CuttingEdge.Conditions;
using Moq;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Tests.Controllers
{
    public static class ControllerUtilities
    {
        // Reference: http://nerddinnerbook.s3.amazonaws.com/Part12.htm
        //            Yes .. Nerd Dinner to the rescue! and we come full circle...
        public static void SetUpControllerContext(ControllerBase controller,
                                                  string userId = null,
                                                  string displayName = null,
                                                  string[] roles = null)
        {
            Condition.Requires(controller);

            // Some fake Authentication stuff.
            var customIdentity = new CustomIdentity(userId, displayName);
            var customPrincipal = new CustomPrincipal(customIdentity, roles);

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(x => x.HttpContext.User).Returns(customPrincipal);

            controller.ControllerContext = mockControllerContext.Object;
        }
    }
}