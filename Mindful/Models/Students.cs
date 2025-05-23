namespace Mindful.Models
{
    public class Students
    {
        public int Id { get; set; }
        public int Classesid { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public DateTime? Birthdate { get; set; }
        public int Address { get; set; }
    }
}
