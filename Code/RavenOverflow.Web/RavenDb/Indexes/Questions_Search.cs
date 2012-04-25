using System.Linq;
using Raven.Client.Indexes;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Web.RavenDb.Indexes
{
    public class Questions_Search : AbstractIndexCreationTask<Question>
    {
        public Questions_Search()
        {
            Map = questions => from q in questions
                               select new
                                      {
                                          q.CreatedOn,
                                          q.Tags
                                      };

            TransformResults = (database, questions) => from q in questions
                                                        let user = database.Load<User>(q.CreatedByUserId)
                                                        select new
                                                                   {
                                                                       q.Answers,
                                                                       q.Comments,
                                                                       q.Content,
                                                                       q.CreatedByUserId,
                                                                       user.DisplayName,
                                                                       q.CreatedOn,
                                                                       q.Id,
                                                                       q.NumberOfViews,
                                                                       q.Subject,
                                                                       q.Tags,
                                                                       q.Vote
                                                                   };
        }
    }
}