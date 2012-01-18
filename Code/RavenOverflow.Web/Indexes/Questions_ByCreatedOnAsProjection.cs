using System.Linq;
using Raven.Client.Indexes;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Web.Indexes
{
    public class Questions_ByCreatedOnAsProjection : AbstractIndexCreationTask<Question>
    {
        public Questions_ByCreatedOnAsProjection()
        {
            Map = questions => from q in questions
                               select new { q.CreatedOn };

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