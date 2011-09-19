using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using RavenOverflow.Core.Services;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Views.Home;

namespace RavenOverflow.Web.Controllers
{
    public class HomeController : AbstractController
    {
        private readonly IQuestionService _questionService;

        public HomeController(IDocumentStore documentStore, IQuestionService questionService) : base(documentStore)
        {
            _questionService = questionService;
        }

        [RavenActionFilter]
        public ActionResult Index()
        {
            var viewModel = new IndexViewModel
                                {
                                    // 1. All the questions, ordered by most recent.
                                    Questions = _questionService.Query()
                                        .OrderByDescending(x => x.CreatedOn)
                                        .Take(20)
                                        .ToList(),
                                    // 2. All the tags, ordered by most recent.
                                    // TODO: I think this will be a facetted index.

                                    // 3. Log in user information.
                                    AuthenticationViewModel = AuthenticationViewModel
                                };

            return View(viewModel);
        }
    }
}