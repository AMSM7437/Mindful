using System.ComponentModel.DataAnnotations;

namespace Mindful.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required.")]
        [Display(Name = "Class Name")]
        public string Name { get; set; }
    }
}
