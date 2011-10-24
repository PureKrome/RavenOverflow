using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CuttingEdge.Conditions;
using Moq;
using NUnit.Framework;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Tests.Controllers
{
    public class HomeControllerTests : TestBase
    {
        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenSomeQuestions_Index_ReturnsTheMostRecentQuestions()
            // ReSharper restore InconsistentNaming
        {
            // Arrange.
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    HomeController homeController = HomeController(documentSession);

                    // Act.
                    var result = homeController.Index(null, null) as ViewResult;

                    // Assert.
                    Assert.IsNotNull(result);

                    var model = result.Model as IndexViewModel;
                    Assert.IsNotNull(model);

                    CollectionAssert.AllItemsAreNotNull(model.Questions);
                    Assert.AreEqual(20, model.Questions.Count);

                    // Make sure all the items are ordered correctly.
                    DateTime? previousQuestion = null;
                    foreach (Question question in model.Questions)
                    {
                        if (previousQuestion.HasValue)
                        {
                            Assert.IsTrue(previousQuestion.Value >= question.CreatedOn);
                        }

                        previousQuestion = question.CreatedOn;
                    }

                    // Now lets test that our fixed questions come back correctly.
                    List<Question> fixedQuestions = FakeQuestions.CreateFakeQuestions(null, 5).ToList();
                    for (int i = 0; i < 5; i++)
                    {
                        // Can Assert anything but the CreatedByUserId.
                        Assert.AreEqual(fixedQuestions[i].Id, model.Questions[i].Id);
                        Assert.AreEqual(fixedQuestions[i].Subject, model.Questions[i].Subject);
                        Assert.AreEqual(fixedQuestions[i].Content, model.Questions[i].Content);
                        Assert.AreEqual(fixedQuestions[i].CreatedOn, model.Questions[i].CreatedOn);
                        Assert.AreEqual(fixedQuestions[i].NumberOfViews, model.Questions[i].NumberOfViews);
                        Assert.AreEqual(fixedQuestions[i].Vote.DownVoteCount, model.Questions[i].Vote.DownVoteCount);
                        Assert.AreEqual(fixedQuestions[i].Vote.FavoriteCount, model.Questions[i].Vote.FavoriteCount);
                        Assert.AreEqual(fixedQuestions[i].Vote.UpVoteCount, model.Questions[i].Vote.UpVoteCount);
                    }
                }
            }
        }

        // Reference: http://nerddinnerbook.s3.amazonaws.com/Part12.htm
        //            Yes .. Nerd Dinner to the rescue! and we come full circle...
        private static HomeController HomeController(IDocumentSession documentSession, int userId = 0,
                                                     string displayName = null, string[] roles = null)
        {
            Condition.Requires(documentSession);

            // Some fake Authentication stuff.
            var customIdentity = new CustomIdentity(userId, displayName);
            var customPrincipal = new CustomPrincipal(customIdentity, roles);

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(x => x.HttpContext.User).Returns(customPrincipal);

            var homeController = new HomeController(documentSession) {ControllerContext = mockControllerContext.Object};

            return homeController;
        }
    }
}