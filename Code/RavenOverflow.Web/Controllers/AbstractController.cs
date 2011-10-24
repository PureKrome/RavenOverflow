using System.Web.Mvc;
using Raven.Client;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Controllers
{
    public class AbstractController : Controller
    {
        public AbstractController(IDocumentSession documentSession)
        {
            DocumentSession = documentSession;
        }

        //public IDocumentStore DocumentStore { get; private set; }
        public IDocumentSession DocumentSession { get; set; }

        /// <summary>
        /// Override the custom User property with our own -custom- Principal/Identity implimentation.
        /// </summary>
        public virtual new CustomPrincipal User
        {
            get { return base.User as CustomPrincipal; }
        }
    }
}