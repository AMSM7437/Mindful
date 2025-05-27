using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using HelperBasens;

namespace Mindful.Controllers
{
    public class SubjectController : Controller
    {
        private readonly HelperBase _dbHelper;

        public SubjectController(HelperBase dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string query = "SELECT id, classesid, first_name, last_name, father_name, mother_name, birthdate, address FROM students";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var students = new List<Students>();

            foreach (DataRow row in dt.Rows)
            {
                students.Add(new Students
                {
                    id = (int)row["id"],
                    classesid = (int)row["classesid"],
                    first_name = row["first_name"].ToString(),
                    last_name = row["last_name"].ToString(),
                    father_name = row["father_name"].ToString(),
                    mother_name = row["mother_name"].ToString(),
                    birthdate = (DateTime)row["birthdate"],
                    address = (int)row["address"]
                });
            }

            return View(students);
        }
    }
}
