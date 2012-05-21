using System;
using System.Web.Mvc;
using AutoMapper;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Services;
using RavenOverflow.Web.Areas.Question.Models;
using RavenOverflow.Web.Controllers;

namespace RavenOverflow.Web.Areas.Question.Controllers
{
    public class QuestionsController : RavenDbController
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IDocumentSession documentSession, IQuestionService questionService)
            : base(documentSession)
        {
            Condition.Requires(questionService).IsNotNull();

            _questionService = questionService;
        }

        [HttpGet]
        public ActionResult Create()
        {
            var inputModel = new QuestionViewModel(User.Identity)
                            {
                                Header = "Ask a Question"
                            };
            return View("Create", inputModel);
        }

        [HttpPost, Authorize]
        public ActionResult Create(QuestionInputModel inputModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var question = Mapper.Map<QuestionInputModel, Core.Entities.Question>(inputModel);
                    question.CreatedByUserId = User.Identity.UserId;

                    _questionService.Store(question);

                    DocumentSession.SaveChanges();

                    return RedirectToAction("Index", "Home", new {area = ""});
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("RuRoh", exception.Message);
            }

            var viewModel = new QuestionViewModel(User.Identity);
            Mapper.Map(inputModel, viewModel);

            return View("Create", viewModel);
        }
    }
}