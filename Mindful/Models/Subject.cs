using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Models
{
    public class Subject
    {
        public int id { get; set; }
        public int? teachersid { get; set; }
        public string name { get; set; }
        public int passing_Grade { get; set; }

        // For displaying teacher name in the view
        public string? teacher_name { get; set; }

        // For dropdown UI
        public List<SelectListItem>? TeacherOptions { get; set; }
    }

}
