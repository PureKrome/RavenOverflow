using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CuttingEdge.Conditions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Extensions;
using RavenOverflow.Core.Filters;
using RavenOverflow.Web.Models.ViewModels;
using RavenOverflow.Web.RavenDb.Indexes;
using WorldDomination.Security;

namespace RavenOverflow.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICustomFormsAuthentication _customFormsAuthentication;

        public HomeController(IDocumentSession documentSession, ICustomFormsAuthentication customCustomFormsAuthentication) : base(documentSession)
        {
            Condition.Requires(customCustomFormsAuthentication).IsNotNull();

            _customFormsAuthentication = customCustomFormsAuthentication;
        }

        [HttpGet]
        public ActionResult Authenticate()
        {
            var userData = new UserData
            {
                UserId = "users/1",
                DisplayName = "Leah Culver",
                PictureUri = "/Content/LeahCulverAvatar.png"
            };
            _customFormsAuthentication.SignIn(userData);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            _customFormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public ActionResult Index(string displayName, string tag)
        {
            string header;

            // 1. All the questions, ordered by most recent.
            IQueryable<QuestionWithDisplayName> questionsQuery = QuestionQuery(tag, out header);

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            IQueryable<RecentPopularTags.ReduceResult> recentPopularTags = RecentPopularTagsQuery();

            // 3. Log in user information.
            IQueryable<User> userQuery = UserQuery(displayName);

            var viewModel = new IndexViewModel(ClaimsUser)
                                {
                                    Header = header,
                                    QuestionListViewModel = new QuestionListViewModel
                                                                {
                                                                    Questions = questionsQuery.ToList()
                                                                },
                                    RecentPopularTags = recentPopularTags.ToDictionary(x => x.Tag, x => x.Count),
                                    UserFavoriteTagListViewModel = new UserTagListViewModel
                                                                       {
                                                                           Header = "Favorite Tags",
                                                                           DivId1 = "interesting-tags",
                                                                           DivId2 = "interestingtags",
                                                                           Tags = userQuery == null
                                                                                      ? null
                                                                                      : (userQuery.SingleOrDefault() ??
                                                                                         new User()).FavoriteTags
                                                                       },
                                    UserIgnoredTagList = new UserTagListViewModel
                                                             {
                                                                 Header = "Ignored Tags",
                                                                 DivId1 = "ignored-tags",
                                                                 DivId2 = "ignoredtags",
                                                                 Tags = null
                                                             }
                                };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult BatchedIndex(string displayName, string tag)
        {
            string header;

            // 1. All the questions, ordered by most recent.
            Lazy<IEnumerable<QuestionWithDisplayName>> questionsQuery = QuestionQuery(tag, out header).Lazily();

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            Lazy<IEnumerable<RecentPopularTags.ReduceResult>> recentPopularTags = RecentPopularTagsQuery().Lazily();

            // 3. Log in user information.
            IQueryable<User> userQuery = UserQuery(displayName);
            Lazy<IEnumerable<User>> lazyUserQuery = (userQuery != null ? userQuery.Lazily() : null);

            var viewModel = new IndexViewModel(ClaimsUser)
                                {
                                    Header = header,
                                    QuestionListViewModel = new QuestionListViewModel
                                                                {
                                                                    Questions = questionsQuery.Value.ToList()
                                                                },
                                    RecentPopularTags = recentPopularTags.Value.ToDictionary(x => x.Tag, x => x.Count),
                                    UserFavoriteTagListViewModel = new UserTagListViewModel
                                                                       {
                                                                           Header = "Favorite Tags",
                                                                           DivId1 = "interesting-tags",
                                                                           DivId2 = "interestingtags",
                                                                           Tags = lazyUserQuery == null
                                                                                      ? null
                                                                                      : (lazyUserQuery.Value.
                                                                                             SingleOrDefault() ??
                                                                                         new User()).FavoriteTags
                                                                       },
                                    UserIgnoredTagList = new UserTagListViewModel
                                                             {
                                                                 Header = "Ignored Tags",
                                                                 DivId1 = "ignored-tags",
                                                                 DivId2 = "ignoredtags",
                                                                 Tags = null
                                                             }
                                };

            return View("Index", viewModel);
        }

        [HttpGet]
        public ActionResult AggressiveIndex(string displayName, string tag)
        {
            using (DocumentSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(1)))
            {
                string header;

                // 1. All the questions, ordered by most recent.
                Lazy<IEnumerable<QuestionWithDisplayName>> questionsQuery = QuestionQuery(tag, out header).Lazily();

                // 2. Popular Tags for a time period.
                // StackOverflow calls it 'recent tags'.
                Lazy<IEnumerable<RecentPopularTags.ReduceResult>> recentPopularTags = RecentPopularTagsQuery().Lazily();

                // 3. Log in user information.
                IQueryable<User> userQuery = UserQuery(displayName);
                Lazy<IEnumerable<User>> lazyUserQuery = (userQuery != null ? userQuery.Lazily() : null);

                var viewModel = new IndexViewModel(ClaimsUser)
                                    {
                                        Header = header,
                                        QuestionListViewModel = new QuestionListViewModel
                                                                    {
                                                                        Questions = questionsQuery.Value.ToList()
                                                                    },
                                        RecentPopularTags =
                                            recentPopularTags.Value.ToDictionary(x => x.Tag, x => x.Count),
                                        UserFavoriteTagListViewModel = new UserTagListViewModel
                                                                           {
                                                                               Header = "Favorite Tags",
                                                                               DivId1 = "interesting-tags",
                                                                               DivId2 = "interestingtags",
                                                                               Tags = lazyUserQuery == null
                                                                                          ? null
                                                                                          : (lazyUserQuery.Value.
                                                                                                 SingleOrDefault() ??
                                                                                             new User()).FavoriteTags
                                                                           },
                                        UserIgnoredTagList = new UserTagListViewModel
                                                                 {
                                                                     Header = "Ignored Tags",
                                                                     DivId1 = "ignored-tags",
                                                                     DivId2 = "ignoredtags",
                                                                     Tags = null
                                                                 }
                                    };

                return View("Index", viewModel);
            }
        }

        public ActionResult Tag(string id)
        {
            RavenQueryStatistics stats;
            List<Question> questions = DocumentSession.Query<Question>()
                .Statistics(out stats)
                .OrderByCreatedByDescending()
                .Take(20)
                .Where(x => x.Tags.Any(tag => tag == id))
                .ToList();

            return Json(new
                            {
                                Questions = questions,
                                stats.TotalResults
                            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Facets(string id)
        {
            var facets = DocumentSession.Query
                <RecentPopularTagsMapOnly.ReduceResult, RecentPopularTagsMapOnly>()
                .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                .ToFacets("Raven/Facets/Tags");

            return Json(facets, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string term)
        {
            IRavenQueryable<RecentPopularTags.ReduceResult> query = DocumentSession
                .Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                .Where(x => x.Tag == term);

            // Does this tag exist?
            RecentPopularTags.ReduceResult tag = query.FirstOrDefault();

            var results = new List<string>();

            if (tag != null)
            {
                results.Add(tag.Tag);
            }
            else
            {
                // No exact match .. so lets use Suggest.
                SuggestionQueryResult suggestedTags = query.Suggest();
                if (suggestedTags.Suggestions.Length > 0)
                {
                    results.AddRange(suggestedTags.Suggestions);
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IndexJson(string displayName, string tag)
        {
            string header;
            var questions = QuestionQuery(tag, out header).ToList();
            return Json(questions.Count <= 0 ? null : questions);
        }

        private IQueryable<QuestionWithDisplayName> QuestionQuery(string tag, out string header)
        {
            header = "Top Questions";

            IQueryable<Question> questionsQuery = DocumentSession.Query<Question, Questions_Search>()
                .OrderByCreatedByDescending()
                .Take(20);

            // Filter Questions by Tags?
            if (!string.IsNullOrEmpty(tag))
            {
                header = "Tagged Questions";
                questionsQuery = questionsQuery
                    .WithAnyTag(tag);
            }

            return questionsQuery.As<QuestionWithDisplayName>();
        }

        private IQueryable<RecentPopularTags.ReduceResult> RecentPopularTagsQuery()
        {
            IQueryable<RecentPopularTags.ReduceResult> recentPopularTags =
                DocumentSession.Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                    .WithinTheLastMonth(1)
                    .OrderByCountDescending()
                    .Take(20);

            return recentPopularTags;
        }

        private IQueryable<User> UserQuery(string displayName)
        {
            string name = displayName ?? User.Identity.Name;
            return !string.IsNullOrEmpty(name) ? DocumentSession.Query<User>().WithDisplayName(name) : null;
        }
    }
}