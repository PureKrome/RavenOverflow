using System.ComponentModel.DataAnnotations;

namespace RavenOverflow.Services.Models
{
    public class QuestionInputModel
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "A subject is missing.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "A question is missing.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "you need at least one valid tag.")]
        public string Tags { get; set; }

        //[Required(ErrorMessage = "You need to be logged in to ask a question.")]
        public string CreatedByUserId { get; set; }
    }
}