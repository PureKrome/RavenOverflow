using System.Web.Mvc;
using Raven.Client;
using RavenOverflow.Web.Models.Authentication;
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

        /// <summary>
        /// Override the custom User property with our own -custom- Principal/Identity implimentation.
        /// </summary>
        public new CustomPrincipal User
        {
            get { return base.User as CustomPrincipal; }
        }
    }
}