using System.Web.Mvc;
using CuttingEdge.Conditions;
using Moq;
using WorldDomination.Security;
using System.Security.Claims;

namespace RavenOverflow.Tests.Controllers
{
    public static class ControllerUtilities
    {
        // Reference: http://nerddinnerbook.s3.amazonaws.com/Part12.htm
        //            Yes .. Nerd Dinner to the rescue! and we come full circle...
        public static void SetUpControllerContext(ControllerBase controller,
                                                  string userId = null,
                                                  string displayName = null)
        {
            Condition.Requires(controller);

            // Some fake Authentication stuff.
            // TODO: Replace with CLAIMS.
            var userData = new UserData
                {
                    UserId = userId,
                    DisplayName = displayName
                };
            var customIdentity = new ClaimsIdentity(userData.ToClaims(), "Forms");
            var customPrincipal = new ClaimsPrincipal(customIdentity);

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(x => x.HttpContext.User).Returns(customPrincipal);

            controller.ControllerContext = mockControllerContext.Object;
        }
    }
}