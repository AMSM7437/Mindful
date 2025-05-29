using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{
    public class Teacher
    {
        public int id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string first_Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string last_Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? birthdate { get; set; }

        [Display(Name = "Assigned Subject Name")]
        public string? subject_Name { get; set; }

        [Display(Name = "Assign Subject")]
        public int? SelectedSubjectId { get; set; }

        public List<SelectListItem>? SubjectOptions { get; set; }

        [Display(Name = "Assigned Subject IDs")]
        public List<int>? AssignedSubjectIds { get; set; }

        [Display(Name = "Assigned Subjects")]
        public List<SelectListItem>? AssignedSubjectOptions { get; set; }

        [Display(Name = "Subjects")]
        public List<string>? SubjectNames { get; set; }

        public string SubjectList => SubjectNames != null ? string.Join(", ", SubjectNames) : "None";
    }
}
