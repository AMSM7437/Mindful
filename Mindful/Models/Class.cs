using System.ComponentModel.DataAnnotations;

namespace Mindful.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        public string Name { get; set; }
    }
}
