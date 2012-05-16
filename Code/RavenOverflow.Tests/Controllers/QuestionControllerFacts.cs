using System.Web.Mvc;
using AutoMapper;
using FizzWare.NBuilder;
using RavenOverflow.Services;
using RavenOverflow.Services.Interfaces;
using RavenOverflow.Services.Models;
using RavenOverflow.Web.Areas.Question.Controllers;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;
using RavenOverflow.Web.AutoMapper;
using Xunit;

namespace RavenOverflow.Tests.Controllers
{
    // ReSharper disable InconsistentNaming

    public class QuestionControllerFacts : RavenDbFactBase
    {
        public QuestionControllerFacts()
        {
            // WebSite requires AutoMapper mappings.
            AutoMapperBootstrapper.ConfigureMappings();
            Mapper.AssertConfigurationIsValid();
        }

        [Fact]
        public void GivenAnInValidQuestion_Create_ReturnsAResultView()
        {
            // Arrange.
            IQuestionService questionService = new QuestionService();
            var questionsController = new QuestionsController(DocumentStore, questionService);
            ControllerUtilities.SetUpControllerContext(questionsController);
            var createInputModel = Builder<QuestionInputModel>.CreateNew().Build();

            // Now pretend the model binding raised an error with the input model.
            questionsController.ModelState.AddModelError("key", "error message");

            // Act.
            var result = questionsController.Create(createInputModel) as ViewResult;

            // Assert.
            Assert.NotNull(result);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void GivenAValidQuestionAndNoOneIsLoggedIn_Create_ReturnsAResultView()
        {
            // Arrange.
            IQuestionService questionService = new QuestionService();
            var questionsController = new QuestionsController(DocumentStore, questionService);
            ControllerUtilities.SetUpControllerContext(questionsController);
            var createInputModel = Builder<QuestionInputModel>.CreateNew().Build();

            // Act.
            var result = questionsController.Create(createInputModel) as ViewResult;

            // Assert.
            Assert.NotNull(result);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void GivenAValidQuestionAndALoggedInUser_Create_AddsTheQuestionAndRedicects()
        {
            // Arrange.
            IQuestionService questionService = new QuestionService();
            var questionsController = new QuestionsController(DocumentStore, questionService);
            ControllerUtilities.SetUpControllerContext(questionsController, "users/1");
            var createInputModel = Builder<QuestionInputModel>.CreateNew().Build();

            // Act.
            var result = questionsController.Create(createInputModel) as RedirectToRouteResult;

            // Assert.
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        // ReSharper restore InconsistentNaming
    }
}