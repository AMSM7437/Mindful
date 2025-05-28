using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{

    public class Teacher
    {
        public int id { get; set; }
        public string first_Name { get; set; }
        public string last_Name { get; set; }
        public string email { get; set; }
        public DateTime? birthdate { get; set; }

        public string? subject_Name { get; set; }

        public int? SelectedSubjectId { get; set; } // optional single-add dropdown (you can remove if redundant)
        public List<SelectListItem>? SubjectOptions { get; set; }

        public List<int>? AssignedSubjectIds { get; set; } // Multi-selected IDs
        public List<SelectListItem>? AssignedSubjectOptions { get; set; }

        public List<string>? SubjectNames { get; set; }
        public string SubjectList => SubjectNames != null ? string.Join(", ", SubjectNames) : "None";
    }

}