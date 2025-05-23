namespace Mindful.Models
{
    public class Students
    {
        public int id { get; set; }
        public int classesid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public DateTime? birthdate { get; set; }
        public int address { get; set; }
    }
}
