using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{
    public class Grade
    {
        public int studentsid { get; set; }
        public int subjectsid { get; set; }
        public int mark { get; set; }

        public string? StudentName { get; set; }
        public string? SubjectName { get; set; }

        public List<SelectListItem>? StudentOptions { get; set; }
        public List<SelectListItem>? SubjectOptions { get; set; }
    }

}
