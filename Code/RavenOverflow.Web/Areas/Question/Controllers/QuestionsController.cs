using System;
using System.Web.Mvc;
using AutoMapper;
using Raven.Client;
using RavenOverflow.Core.Services;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models;

namespace RavenOverflow.Web.Areas.Question.Controllers
{
    public class QuestionsController : AbstractController
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IDocumentSession documentSession, IQuestionService questionService)
            : base(documentSession)
        {
            _questionService = questionService;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateViewModel(User.Identity)
                                {
                                    Header = "Ask a Question"
                                };
            return View("Create", viewModel);
        }

        [HttpPost, RavenActionFilter, Authorize]
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
                    return View("Create", viewModel);
                }

                Core.Entities.Question question = Mapper.Map<CreateInputModel, Core.Entities.Question>(inputModel);
                question.CreatedByUserId = User.Identity.UserId;

                _questionService.Create(question);

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch(Exception exception)
            {
                ModelState.AddModelError("RuRoh", exception.Message);
                CreateViewModel viewModel = Mapper.Map(inputModel, new CreateViewModel(User.Identity)
                                                                       {
                                                                           Header = "Ask a Question"
                                                                       });
                return View("Create", viewModel);
            }
        }
    }
}