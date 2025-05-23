using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using HelperBasens;

namespace Mindful.Controllers
{
    public class TeacherController : Controller
    {
        private readonly HelperBase _dbHelper;

        public TeacherController(HelperBase dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string query = "SELECT id, first_name, last_name, email, birthdate FROM teachers";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var teachers = new List<Teacher>();

            foreach (DataRow row in dt.Rows)
            {
                teachers.Add(new Teacher
                {
                    id = (int)row["id"],
                    first_name = row["first_name"].ToString(),
                    last_name = row["last_name"].ToString(),
                    email = row["email"].ToString(),
                    birthdate = (DateTime)row["birthdate"]
                });
            }

            return View(teachers);
        }
    }
}
