using System;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.FakeData
{
    public static class FakeQuestions
    {
        private static readonly IList<string> FakeTags = new List<string>
                                                             {
                                                                 "ravendb",
                                                                 "asp.net-mvc",
                                                                 "c#",
                                                                 "linq",
                                                                 "moq",
                                                                 ".net",
                                                                 ".net3.5",
                                                                 "elmah",
                                                                 "yui-compressor",
                                                                 "minify",
                                                                 "mono",
                                                                 "asp.net-mvc3",
                                                                 "ruby-on-rails",
                                                                 "elmah",
                                                                 "rubygems",
                                                                 "pew-pew"
                                                             };

        public static ICollection<Question> CreateFakeQuestions(IList<string> userIds)
        {
            return CreateFakeQuestions(userIds, GetRandom.Int(20, 100));
        }

        public static ICollection<Question> CreateFakeQuestions(IList<string> userIds, int numberOfFakeQuestions)
        {
            Condition.Requires(numberOfFakeQuestions).IsGreaterOrEqual(5); // First 5 questions are fixed.

            // If we don't have any fake userId's, then just create some fake users now.
            // Basically, we don't care what these values are.
            if (userIds == null)
            {
                userIds = FakeUsers.CreateFakeUsers(50).Select(x => x.Id).ToList();
            }

            var fakeQuestions = new List<Question>();

            IList<Question> fixedQuestion = CreateFixedFakeQuestions(userIds);
            Condition.Requires(numberOfFakeQuestions).IsNotLessThan(fixedQuestion.Count);

            fakeQuestions.AddRange(fixedQuestion);
            for (int i = 0; i < numberOfFakeQuestions - fixedQuestion.Count; i++)
            {
                fakeQuestions.Add(CreateAFakeQuestion(userIds.ToRandomList(1).Single(), userIds));
            }

            return fakeQuestions;
        }

        public static Question CreateAFakeQuestion(string userId, IList<string> answerUserIds)
        {
            Question fakeQuestion = Builder<Question>
                .CreateNew()
                .With(x => x.Id = null)
                .With(x => x.Subject = GetRandom.Phrase(GetRandom.Int(10, 50)))
                .With(x => x.Content = GetRandom.Phrase(GetRandom.Int(30, 500)))
                .And(
                    x =>
                    x.CreatedByUserId =
                    string.IsNullOrEmpty(userId) ? null : userId)
                .And(
                    x => x.CreatedOn = GetRandom.DateTime(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddMinutes(-5)))
                .And(x => x.NumberOfViews = GetRandom.PositiveInt(10000))
                .And(x => x.Tags = FakeTags.ToRandomList(GetRandom.Int(1, 5)))
                .And(x => x.Vote = CreateAFakeVote())
                .Build();

            if (answerUserIds != null)
            {
                for (int i = 0; i < (GetRandom.Int(1, 20) <= 10 ? GetRandom.PositiveInt(6) : 0); i++)
                {
                    if (fakeQuestion.Answers == null)
                    {
                        fakeQuestion.Answers = new List<Answer>();
                    }

                    fakeQuestion.Answers.Add(CreateAFakeAnswer(fakeQuestion.CreatedOn,
                                                               answerUserIds.ToRandomList(1).Single()));
                }
            }

            return fakeQuestion;
        }

        private static Answer CreateAFakeAnswer(DateTime questionCreatedOn, string userId)
        {
            Answer answer = Builder<Answer>
                .CreateNew()
                .With(x => x.Content = GetRandom.Phrase(GetRandom.Int(20, 100)))
                .And(x => x.CreatedOn = GetRandom.DateTime(questionCreatedOn.AddMinutes(5), DateTime.UtcNow))
                .And(x => x.CreatedByUserId = userId)
                .And(x => x.Vote = CreateAFakeVote())
                .Build();

            for (int i = 0; i < (GetRandom.PositiveInt(20) <= 10 ? GetRandom.PositiveInt(6) : 0); i++)
            {
                if (answer.Comments == null)
                {
                    answer.Comments = new List<Comment>();
                }
                answer.Comments.Add(CreateAFakeComment(questionCreatedOn));
            }

            return answer;
        }

        private static Comment CreateAFakeComment(DateTime questionCreatedOn)
        {
            return Builder<Comment>
                .CreateNew()
                .With(x => x.Content = GetRandom.Phrase(GetRandom.PositiveInt(20)))
                .And(x => x.CreatedOn = GetRandom.DateTime(questionCreatedOn.AddMinutes(5), DateTime.UtcNow))
                .And(x => x.CreatedByUserId = string.Format("{0}.{1}", GetRandom.FirstName(), GetRandom.LastName()))
                // If Rand number between 1-10 <= 5, then votes = another rand numb between 1-5. else 0.
                .And(x => x.UpVoteCount = (GetRandom.PositiveInt(10) <= 6 ? GetRandom.PositiveInt(5) : 0))
                .Build();
        }

        private static Vote CreateAFakeVote()
        {
            return GetRandom.PositiveInt(5) == 1
                       ? null
                       : Builder<Vote>
                             .CreateNew()
                             .With(y => y.UpVoteCount = GetRandom.PositiveInt(20))
                             .And(y => y.DownVoteCount = GetRandom.PositiveInt(3))
                             .And(y => y.FavoriteCount = GetRandom.PositiveInt(10))
                             .Build();
        }

        private static IList<Question> CreateFixedFakeQuestions(IList<string> userIds)
        {
            Condition.Requires(userIds).IsNotNull().IsLongerOrEqual(1);

            return new List<Question>
                       {
                           new Question
                               {
                                   Subject = "How to query nested information in RavenDB?",
                                   Content =
                                       "I have the following document called Reservation: { \"CustomerId\": 1, \"Items\": [ { \"EmployeeId\": \"employees/1\", \"StartTime\": ...",
                                   CreatedOn = DateTime.UtcNow.AddMinutes(5),
                                   Tags = new List<string> {"ravendb", ".net", "c#"},
                                   CreatedByUserId = userIds.ToRandomList(1).Single(),
                                   NumberOfViews = 50,
                                   Vote = new Vote {DownVoteCount = 5, UpVoteCount = 10, FavoriteCount = 6},
                                   Comments = new List<Comment>
                                                  {
                                                      new Comment
                                                          {
                                                              Content = "Some comment 1",
                                                              CreatedByUserId =
                                                                  userIds.ToRandomList(1).Single(),
                                                              CreatedOn = DateTime.UtcNow.AddMinutes(2),
                                                              UpVoteCount = 2
                                                          },
                                                      new Comment
                                                          {
                                                              Content = "Some comment 2",
                                                              CreatedByUserId =
                                                                  userIds.ToRandomList(1).Single(),
                                                              CreatedOn = DateTime.UtcNow.AddMinutes(1),
                                                          }
                                                  },
                                   Answers = new List<Answer>
                                                 {
                                                     new Answer
                                                         {
                                                             Content = "Answer 1 - blah blah blah",
                                                             CreatedByUserId = userIds.ToRandomList(1).Single(),
                                                             CreatedOn = DateTime.UtcNow.AddMinutes(3),
                                                             Vote = new Vote
                                                                        {
                                                                            DownVoteCount = 5,
                                                                            UpVoteCount = 10,
                                                                            FavoriteCount = 6
                                                                        }
                                                         },
                                                     new Answer
                                                         {
                                                             Content = "Answer 2 - blah blah blah",
                                                             CreatedByUserId = userIds.ToRandomList(1).Single(),
                                                             CreatedOn = DateTime.UtcNow.AddMinutes(2),
                                                             Vote = new Vote
                                                                        {
                                                                            DownVoteCount = 4,
                                                                            UpVoteCount = 8,
                                                                            FavoriteCount = 5
                                                                        }
                                                         },
                                                     new Answer
                                                         {
                                                             Content = "Answer 3 - blah blah blah",
                                                             CreatedByUserId = userIds.ToRandomList(1).Single(),
                                                             CreatedOn = DateTime.UtcNow.AddMinutes(1),
                                                             Vote = new Vote
                                                                        {
                                                                            DownVoteCount = 3,
                                                                            UpVoteCount = 6,
                                                                            FavoriteCount = 4
                                                                        }
                                                         }
                                                 }
                               },
                           new Question
                               {
                                   Subject = "RavenDb MapReduce over subset of Data",
                                   Content =
                                       "Say I have the given document structure in RavenDb public class Car { public string Manufacturer {get;set;} public int BuildYear {get;set;} public string Colour {get;set;} public",
                                   CreatedOn = DateTime.UtcNow.AddMinutes(4),
                                   Tags = new List<string> {"ravendb", ".net", "c#"},
                                   CreatedByUserId = userIds.ToRandomList(1).Single(),
                                   NumberOfViews = 44,
                                   Vote = new Vote {DownVoteCount = 4, UpVoteCount = 8, FavoriteCount = 5}
                               },
                           new Question
                               {
                                   Subject = "RavenDB - retrieving part of document",
                                   Content =
                                       "I am playing with Raven DB for few days and I would like to use it as a storage for my Web chat application. I have document which contains some user data and chat history - which is big collection",
                                   CreatedOn = DateTime.UtcNow.AddMinutes(3),
                                   Tags = new List<string> {"ravendb", ".net", "c#"},
                                   CreatedByUserId = userIds.ToRandomList(1).Single(),
                                   NumberOfViews = 33,
                                   Vote = new Vote {DownVoteCount = 3, UpVoteCount = 6, FavoriteCount = 4}
                               },
                           new Question
                               {
                                   Subject = "RavenDB Index on SubClasses",
                                   Content =
                                       "I am trying to create an indexes for ProviderProfileId, Email, and Address1 I have created queries that work, but not indexes. I know the inheriting from List for the collections might be part of",
                                   CreatedOn = DateTime.UtcNow.AddMinutes(2),
                                   Tags = new List<string> {"ravendb", ".net", "c#"},
                                   CreatedByUserId = userIds.ToRandomList(1).Single(),
                                   NumberOfViews = 22,
                                   Vote = new Vote {DownVoteCount = 2, UpVoteCount = 4, FavoriteCount = 3}
                               },
                           new Question
                               {
                                   Subject =
                                       "given a list of objects using C# push them to ravendb without knowing which ones already exist",
                                   Content =
                                       "Given 1000 documents with a complex data structure. for e.g. a Car class that has three properties, Make and Model and one Id property. What is the most efficient way in C# to push these documents to",
                                   CreatedOn = DateTime.UtcNow.AddMinutes(1),
                                   Tags = new List<string> {"ravendb", ".net", "c#"},
                                   CreatedByUserId = userIds.ToRandomList(1).Single(),
                                   NumberOfViews = 11,
                                   Vote = new Vote {DownVoteCount = 1, UpVoteCount = 2, FavoriteCount = 2}
                               }
                       };
        }
    }
}