using System.Web.Mvc;
using Raven.Client;
using RavenOverflow.Web.Views.Shared;

namespace RavenOverflow.Web.Controllers
{
    public class AbstractController : Controller
    {
        public AbstractController(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public IDocumentStore DocumentStore { get; private set; }
        public IDocumentSession DocumentSession { get; set; }
        protected AuthenticationViewModel AuthenticationViewModel { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AuthenticationViewModel = new AuthenticationViewModel
                                          {
                                              DisplayName =
                                                  string.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name
                                          };
            
            base.OnActionExecuting(filterContext);
        }
    }
}