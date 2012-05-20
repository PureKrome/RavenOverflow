using System.Web.Mvc;
using AutoMapper;
using FizzWare.NBuilder;
using RavenOverflow.Core.Services;
using RavenOverflow.Services;
using RavenOverflow.Web.Areas.Question.Controllers;
using RavenOverflow.Web.Areas.Question.Models;
using RavenOverflow.Web.AutoMapper;
using RavenOverflow.Web.Models.Authentication;
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
        public void GivenAnInvalidQuestion_Create_ReturnsAResultView()
        {
            // Arrange.
            var questionService = new QuestionService(DocumentSession);
            var questionsController = new QuestionsController(DocumentSession, questionService);
            ControllerUtilities.SetUpControllerContext(questionsController);
            var createInputModel = new QuestionInputModel(questionsController.HttpContext.User.Identity as CustomIdentity);

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
            var questionService = new QuestionService(DocumentSession);
            var questionsController = new QuestionsController(DocumentSession, questionService);
            ControllerUtilities.SetUpControllerContext(questionsController);
            var createInputModel = new QuestionInputModel(questionsController.HttpContext.User.Identity as CustomIdentity);

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
            var questionService = new QuestionService(DocumentSession);
            var questionsController = new QuestionsController(DocumentSession, questionService);
            ControllerUtilities.SetUpControllerContext(questionsController, "users/1");
            var createInputModel =
                new QuestionInputModel(questionsController.HttpContext.User.Identity as CustomIdentity)
                {
                    Subject = "aaaad fdds fsd ds",
                    Content = "sdhfskfhksd sd",
                    Tags = "ahdakjdh"
                };

            // Act.
            var result = questionsController.Create(createInputModel) as RedirectToRouteResult;

            // Assert.
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        // ReSharper restore InconsistentNaming
    }
}