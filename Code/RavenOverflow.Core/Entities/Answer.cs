using System;
using System.Collections.Generic;

namespace RavenOverflow.Core.Entities
{
    public class Answer
    {
        public string CreatedByUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastEditedByUserId { get; set; }
        public DateTime? LastEditedOn { get; set; }
        public Vote Vote { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}