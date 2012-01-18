using System.ComponentModel.DataAnnotations;

namespace RavenOverflow.Web.Areas.Question.Models.ViewModels
{
    public class CreateInputModel
    {
        [Required(ErrorMessage = "A Subject is required.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "A question is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Some tags are required.")]
        public string Tags { get; set; }
    }
}