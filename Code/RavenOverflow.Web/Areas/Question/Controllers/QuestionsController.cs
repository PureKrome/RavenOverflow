using System;
using System.Web.Mvc;
using AutoMapper;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Services;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;
using RavenOverflow.Web.Controllers;

namespace RavenOverflow.Web.Areas.Question.Controllers
{
    public class QuestionsController : RavenDbController
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IDocumentStore documentStore, IQuestionService questionService)
            : base(documentStore)
        {
            Condition.Requires(questionService).IsNotNull();

            _questionService = questionService;
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateViewModel(User.Identity)
            {
                Header = "Ask a Question"
            };
            return View("Create", viewModel);
        }

        [HttpPost, Authorize]
        public ActionResult Create(CreateInputModel inputModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Core.Entities.Question question = Mapper.Map<CreateInputModel, Core.Entities.Question>(inputModel);
                    question.CreatedByUserId = User.Identity.UserId;

                    _questionService.Create(question);

                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("RuRoh", exception.Message);
            }

            CreateViewModel viewModel = Mapper.Map(inputModel,
                                                   new CreateViewModel(User.Identity) { Header = "Ask a Question" });
            return View("Create", viewModel);
        }
    }
}