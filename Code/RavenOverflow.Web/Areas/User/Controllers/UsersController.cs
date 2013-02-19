using System.Web.Mvc;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Areas.User.Controllers
{
    public class UsersController : RavenDbController
    {
        private readonly ICustomFormsAuthentication _customFormsAuthentication;

        public UsersController(IDocumentSession documentSession,
                               ICustomFormsAuthentication customCustomFormsAuthentication)
            : base(documentSession)
        {
            Condition.Requires(customCustomFormsAuthentication).IsNotNull();

            _customFormsAuthentication = customCustomFormsAuthentication;
        }

        [HttpGet]
        public ActionResult Authenticate()
        {
            _customFormsAuthentication.SignIn("users/1", "Some Fake user", Response);

            return RedirectToAction("Index", "Home", new {area = ""});
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            _customFormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new {area = ""});
        }
    }
}