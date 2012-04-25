using System.Web.Mvc;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Override the custom User property with our own -custom- Principal/Identity implimentation.
        /// </summary>
        public new virtual CustomPrincipal User
        {
            get { return base.User as CustomPrincipal; }
        }
    }
}