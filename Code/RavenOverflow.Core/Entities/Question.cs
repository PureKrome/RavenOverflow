using System;
using System.Collections.Generic;

namespace RavenOverflow.Core.Entities
{
    public class Question : RootAggregate
    {
        public Question()
        {
            NumberOfViews = 1;
            CreatedOn = DateTime.UtcNow;
        }

        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; } // This should always be Utc.
        public ICollection<string> Tags { get; set; }
        public string CreatedByUserId { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public int NumberOfViews { get; set; }
        public Vote Vote { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}