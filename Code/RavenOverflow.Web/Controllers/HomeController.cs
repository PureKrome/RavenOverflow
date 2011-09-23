using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Views.Home;

namespace RavenOverflow.Web.Controllers
{
    public class HomeController : AbstractController
    {
        public HomeController(IDocumentStore documentStore) : base(documentStore)
        {
        }

        [RavenActionFilter]
        public ActionResult Index()
        {
            var viewModel = new IndexViewModel
                                {
                                    // 1. All the questions, ordered by most recent.
                                    Questions = DocumentSession.Query<Question>()
                                        .OrderByDescending(x => x.CreatedOn)
                                        .Take(20)
                                        .ToList(),
                                    // 2. All the tags, ordered by most recent.
                                    // TODO: I think this will be a facetted index.

                                    // 3. Log in user information.
                                    //AuthenticationViewModel = AuthenticationViewModel
                                };

            ViewBag.UserDetails = AuthenticationViewModel;

            return View(viewModel);
        }
    }
}