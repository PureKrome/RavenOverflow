using System.Web.Mvc;
using AutoMapper;
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

        public ActionResult Create()
        {
            var viewModel = new CreateViewModel(User.Identity)
                                {
                                    Header = "Ask a Question"
                                };
            return View(viewModel);
        }

        [HttpPost, RavenActionFilter]
        public ActionResult Create(CreateInputModel inputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CreateViewModel viewModel = Mapper.Map(inputModel, new CreateViewModel(User.Identity)
                                                                           {
                                                                               Header = "Ask a Question"
                                                                           });
                    return View(viewModel);
                }

                Core.Entities.Question question = Mapper.Map<CreateInputModel, Core.Entities.Question>(inputModel);
                question.CreatedByUserId = User.Identity.IsAuthenticated ? "users/" + User.Identity.UserId : "0";

                DocumentSession.Store(question);
                DocumentSession.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                CreateViewModel viewModel = Mapper.Map(inputModel, new CreateViewModel(User.Identity)
                                                                       {
                                                                           Header = "Ask a Question"
                                                                       });
                return View(viewModel);
            }
        }
    }
}