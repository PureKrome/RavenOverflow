using System.ComponentModel.DataAnnotations;

namespace RavenOverflow.Web.Areas.Question.Models.ViewModels
{
    public class CreateInputModel
    {
        [Required(ErrorMessage = "A Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A question is required.")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Some tags are required.")]
        public string Tags { get; set; }
    }
}