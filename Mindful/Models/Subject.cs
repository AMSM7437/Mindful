using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{
    public class Subject
    {
        public int id { get; set; }

        [Display(Name = "Assigned Teacher")]
        public int? teachersid { get; set; }

        [Required(ErrorMessage = "Subject name is required.")]
        [Display(Name = "Subject Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Passing grade is required.")]
        [Range(0, 100, ErrorMessage = "Passing grade must be between 0 and 100.")]
        [Display(Name = "Passing Grade")]
        public int passing_Grade { get; set; }

        // For displaying teacher name in the view
        [Display(Name = "Teacher Name")]
        public string? teacher_name { get; set; }
        public List<SelectListItem>? TeacherOptions { get; set; }
    }
}
