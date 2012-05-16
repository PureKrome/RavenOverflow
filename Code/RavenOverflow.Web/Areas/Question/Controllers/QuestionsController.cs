using System;
using System.Web.Mvc;
using AutoMapper;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Services.Interfaces;
using RavenOverflow.Services.Models;
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
        public ActionResult Create(QuestionInputModel inputModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inputModel.CreatedByUserId = User.Identity.UserId;
                    
                    _questionService.Store(inputModel, DocumentSession);

                    DocumentSession.SaveChanges();

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