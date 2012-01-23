using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RavenOverflow.Core.Entities
{
    public class Question : RootAggregate
    {
        public Question()
        {
            NumberOfViews = 1;
            CreatedOn = DateTime.UtcNow;
        }

        [Required]
        public string Subject { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; } // This should always be Utc.
        [Required]
        public ICollection<string> Tags { get; set; }
        [Required(ErrorMessage = "You need to be logged in to ask a question.")]
        public string CreatedByUserId { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public int NumberOfViews { get; set; }
        public Vote Vote { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}