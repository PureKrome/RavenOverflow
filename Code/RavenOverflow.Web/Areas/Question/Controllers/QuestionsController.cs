using System;
using System.Collections.Generic;
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
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                // We have a legit question, so lets save it.
                string[] tags = viewModel.Tags.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var question = new Core.Entities.Question
                                   {
                                       Subject = viewModel.Title,
                                       Content = viewModel.Question,
                                       Tags = new List<string>(tags)
                                   };
                DocumentSession.Store(question);
                DocumentSession.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View(viewModel);
            }
        }
    }
}