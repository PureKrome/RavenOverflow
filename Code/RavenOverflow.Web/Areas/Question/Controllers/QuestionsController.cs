using System.Web.Mvc;
using Raven.Client;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models;

namespace RavenOverflow.Web.Areas.Question.Controllers
{
    public class QuestionsController : AbstractController
    {
        public QuestionsController(IDocumentSession documentSession) : base(documentSession)
        {
        }
    
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new CreateViewModel(User.Identity)
                                {
                                    Header = "Ask a Question"
                                };
            return View(viewModel);
        }

        [HttpPost, RavenActionFilter]
        public ActionResult Create(CreateViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(viewModel);
            }
        }

    }
}