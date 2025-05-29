
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Mindful.Models
{
    public class Students
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        [Display(Name = "Class")]
        public int classesid { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string last_name { get; set; }

        [Display(Name = "Father's Name")]
        public string father_name { get; set; }

        [Display(Name = "Mother's Name")]
        public string mother_name { get; set; }

        [Required(ErrorMessage = "Birthdate is required.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? birthdate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address")]
        public int address { get; set; }

        // For Index display
        [Display(Name = "Class Name")]
        public string? class_name { get; set; }

        // For dropdown in the form
        public List<SelectListItem>? ClassOptions { get; set; }
    }
}
