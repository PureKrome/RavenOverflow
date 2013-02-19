using System.Web.Mvc;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Web.Models;

namespace RavenOverflow.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController(IDocumentSession documentSession)
        {
            Condition.Requires(documentSession).IsNotNull();

            DocumentSession = documentSession;
        }

        protected IDocumentSession DocumentSession { get; private set; }

        protected ClaimsUser ClaimsUser
        {
            get { return User == null ? null : new ClaimsUser(User); }
        }
    }
}