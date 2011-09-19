using System.Web.Mvc;
using RavenOverflow.Web.Controllers;

namespace RavenOverflow.Web.Models
{
    public class RavenActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var abstractController = filterContext.Controller as AbstractController;
            if (abstractController == null)
            {
                return;
            }

            abstractController.DocumentSession = abstractController.DocumentStore.OpenSession();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var abstractController = filterContext.Controller as AbstractController;
            if (abstractController == null)
            {
                return;
            }

            if (abstractController.DocumentSession != null)
            {
                abstractController.DocumentSession.Dispose();
            }
        }
    }
}