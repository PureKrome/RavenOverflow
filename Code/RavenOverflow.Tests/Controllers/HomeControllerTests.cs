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
using RavenOverflow.Web.Indexes;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.Authentication;
using RavenOverflow.Web.Models.ViewModels;

namespace RavenOverflow.Tests.Controllers
{
    public class HomeControllerTests : TestBase
    {
        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenSomeQuestions_Index_ReturnsTheMostRecentQuestions()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Arrange.
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

        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenSomeQuestions_Index_ReturnsTheMostRecentPopularTagsInTheLast30Days()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Arrange.
                    HomeController homeController = HomeController(documentSession);

                    // Act.
                    var result = homeController.Index(null, null) as ViewResult;

                    // Assert.
                    Assert.IsNotNull(result);

                    var model = result.Model as IndexViewModel;
                    Assert.IsNotNull(model);

                    Assert.IsNotNull(model.RecentPopularTags);
                    Assert.IsTrue(model.RecentPopularTags.Count > 0);
                    CollectionAssert.AllItemsAreNotNull(model.RecentPopularTags);

                    // Make sure all the items are ordered correctly.
                    int? previousCount = null;
                    foreach (var keyValuePair in model.RecentPopularTags)
                    {
                        if (previousCount.HasValue)
                        {
                            Assert.IsTrue(previousCount.Value >= keyValuePair.Value);
                        }

                        previousCount = keyValuePair.Value;
                    }

                    // ToDo: test fixed tags.
                }
            }
        }

        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenAnAuthenticatedUserWithSomeFavouriteTags_Index_ReturnsAFavouriteTagsViewModelWithContent()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Arrange.
                    // Note: we're faking that a user has authenticated.
                    HomeController homeController = HomeController(documentSession, displayName:"Pure.Krome");

                    // Act.
                    var result = homeController.Index(null, null) as ViewResult;

                    // Assert.
                    Assert.IsNotNull(result);

                    var model = result.Model as IndexViewModel;
                    Assert.IsNotNull(model);

                    var userFavoriteTagListViewModel = model.UserFavoriteTagListViewModel;
                    Assert.IsNotNull(userFavoriteTagListViewModel);

                    Assert.AreEqual("Favorite Tags", userFavoriteTagListViewModel.Header);
                    Assert.AreEqual("interesting-tags", userFavoriteTagListViewModel.DivId1);
                    Assert.AreEqual("interestingtags", userFavoriteTagListViewModel.DivId2);
                    Assert.IsNotNull(userFavoriteTagListViewModel.Tags);
                    Assert.AreEqual(3, userFavoriteTagListViewModel.Tags.Count);
                    CollectionAssert.AllItemsAreNotNull(userFavoriteTagListViewModel.Tags);
                }
            }
        }

        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenNoAuthenticatedUser_Index_ReturnsFavouriteTagsViewModelWithNoTags()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Arrange.
                    // Note: we're faking that no user has been authenticated.
                    HomeController homeController = HomeController(documentSession);

                    // Act.
                    var result = homeController.Index(null, null) as ViewResult;

                    // Assert.
                    Assert.IsNotNull(result);

                    var model = result.Model as IndexViewModel;
                    Assert.IsNotNull(model);

                    var userFavoriteTagListViewModel = model.UserFavoriteTagListViewModel;
                    Assert.IsNotNull(userFavoriteTagListViewModel);

                    Assert.AreEqual("Favorite Tags", userFavoriteTagListViewModel.Header);
                    Assert.AreEqual("interesting-tags", userFavoriteTagListViewModel.DivId1);
                    Assert.AreEqual("interestingtags", userFavoriteTagListViewModel.DivId2);
                    Assert.IsNull(userFavoriteTagListViewModel.Tags);
                }
            }
        }

        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenSomeQuestionsAndAnExistingTag_Tags_ReturnsAListOfTaggedQuestions()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Arrange.
                    const string tag = "ravendb";
                    HomeController homeController = HomeController(documentSession);

                    // Act.
                    var result = homeController.Tag(tag) as JsonResult;

                    // Assert.
                    Assert.IsNotNull(result);

                    dynamic model = result.Data;
                    Assert.IsNotNull(model);

                    // At least 5 questions are hardcoded to include the RavenDb tag.
                    Assert.IsNotNull(model.Questions);
                    Assert.IsTrue(model.Questions.Count >= 5);
                    Assert.IsTrue(model.TotalResults >= 5);
                }
            }
        }

        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenSomeQuestionsAndAnExistingTag_Search_ReturnsAListOfTags()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Force the Index to complete.
                    var meh = documentSession
                        .Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .ToList();

                    // Arrange.
                    const string tag = "ravendb";
                    HomeController homeController = HomeController(documentSession);

                    // Act.
                    var result = homeController.Search(tag) as JsonResult;

                    // Assert.
                    Assert.IsNotNull(result);
                    dynamic model = result.Data;
                    Assert.IsNotNull(model);
                    Assert.AreEqual(1, model.Count);
                    Assert.AreEqual("ravendb", model[0]);
                }
            }
        }

        [Test]
        // ReSharper disable InconsistentNaming
        public void GivenSomeQuestionsAndAnExistingPartialTag_Search_ReturnsAListOfTaggedQuestions()
        // ReSharper restore InconsistentNaming
        {
            using (IDocumentStore documentStore = DocumentStore)
            {
                using (IDocumentSession documentSession = documentStore.OpenSession())
                {
                    // Force the Index to complete.
                    var meh = documentSession
                        .Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .ToList();

                    // Arrange.
                    const string tag = "ravne"; // Hardcoded Typo.
                    HomeController homeController = HomeController(documentSession);

                    // Act.
                    var result = homeController.Search(tag) as JsonResult;

                    // Assert.
                    Assert.IsNotNull(result);

                    dynamic model = result.Data;
                    Assert.IsNotNull(model);
                    Assert.AreEqual(1, model.Count);
                    Assert.AreEqual("ravendb", model[0]);
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