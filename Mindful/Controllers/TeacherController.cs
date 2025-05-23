using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using HelperBase;

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
            string query = "SELECT Id, First_Name, Last_Name, Email, Birthdate FROM Teacher";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var teachers = new List<Teacher>();

            foreach (DataRow row in dt.Rows)
            {
                teachers.Add(new Teacher
                {
                    Id = (int)row["Id"],
                    First_Name = row["First_Name"].ToString(),
                    Last_Name = row["Last_Name"].ToString(),
                    Email = row["Email"].ToString(),
                    Birthdate = row["Birthdate"] == DBNull.Value ? null : (DateTime?)row["Birthdate"]
                });
            }

            return View(teachers);
        }
    }
}
