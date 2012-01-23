using System.ComponentModel.DataAnnotations;

namespace RavenOverflow.Web.Areas.Question.Models.ViewModels
{
    public class CreateInputModel
    {
        [Required(ErrorMessage = "A subject is missing.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "A question is missing.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "you need at least one valid tag.")]
        public string Tags { get; set; }
    }
}