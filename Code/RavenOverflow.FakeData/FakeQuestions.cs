using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.FakeData
{
    public static class FakeQuestions
    {
        private static IList<Question> _fakeQuestions;

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
            if (userIds == null || userIds.Count <= 0)
            {
                throw new ArgumentNullException("userIds");
            }

            if (_fakeQuestions == null)
            {
                var fakeQuestions = new List<Question>();
                for (int i = 0; i < GetRandom.Int(20, 100); i++)
                {
                    fakeQuestions.Add(CreateAFakeQuestion(userIds.ToRandomList(1).Single(), userIds));
                }

                _fakeQuestions = fakeQuestions;
            }

            return _fakeQuestions;
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

            for (int i = 0; i < (GetRandom.Int(1, 20) <= 10 ? GetRandom.PositiveInt(6) : 0); i++)
            {
                if (fakeQuestion.Answers == null)
                {
                    fakeQuestion.Answers = new List<Answer>();
                }

                fakeQuestion.Answers.Add(CreateAFakeAnswer(fakeQuestion.CreatedOn,
                                                           answerUserIds.ToRandomList(1).Single()));
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
            return Builder<Vote>
                .CreateNew()
                .With(y => y.UpVoteCount = GetRandom.PositiveInt(20))
                .And(y => y.DownVoteCount = GetRandom.PositiveInt(3))
                .And(y => y.FavoriteCount = GetRandom.PositiveInt(10))
                .Build();

        }

    }
}