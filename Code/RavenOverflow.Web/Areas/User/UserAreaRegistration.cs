using System.Web.Mvc;

namespace RavenOverflow.Web.Areas.User
{
    public class UserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "User";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "User_default",
                "Users/{action}",
                new { controller="users", action = "Index" }
            );
        }
    }
}
