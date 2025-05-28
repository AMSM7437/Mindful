using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{
    public class Teacher
    {
        public int id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string first_Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string last_Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)]
        public DateTime? birthdate { get; set; }

        public string? subject_Name { get; set; }

        public int? SelectedSubjectId { get; set; }
        public List<SelectListItem>? SubjectOptions { get; set; }

        public List<int>? AssignedSubjectIds { get; set; }
        public List<SelectListItem>? AssignedSubjectOptions { get; set; }

        public List<string>? SubjectNames { get; set; }
        public string SubjectList => SubjectNames != null ? string.Join(", ", SubjectNames) : "None";
    }
}
