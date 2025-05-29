using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{
    public class Grade
    {
        [Required(ErrorMessage = "Student selection is required.")]
        [Display(Name = "Student")]
        public int studentsid { get; set; }

        [Required(ErrorMessage = "Subject selection is required.")]
        [Display(Name = "Subject")]
        public int subjectsid { get; set; }

        [Required(ErrorMessage = "Mark is required.")]
        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100.")]
        [Display(Name = "Grade")]
        public int mark { get; set; }

        // Display-only values
        [Display(Name = "Student Name")]
        public string? StudentName { get; set; }

        [Display(Name = "Subject Name")]
        public string? SubjectName { get; set; }

        // Dropdowns
        public List<SelectListItem>? StudentOptions { get; set; }
        public List<SelectListItem>? SubjectOptions { get; set; }
    }
}
