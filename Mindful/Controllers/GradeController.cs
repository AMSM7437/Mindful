using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using HelperBasens;

namespace Mindful.Controllers
{
    public class GradeController : Controller
    {
        private readonly HelperBase _dbHelper;

        public GradeController(HelperBase dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string query = "SELECT Id, First_Name, Last_Name, Email, Birthdate FROM Grades";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var grades = new List<Grade>();

            foreach (DataRow row in dt.Rows)
            {
                grades.Add(new Grade
                {
                    Id = (int)row["Id"],
                    First_Name = row["First_Name"].ToString(),
                    Last_Name = row["Last_Name"].ToString(),
                    Email = row["Email"].ToString(),
                    Birthdate = row["Birthdate"] == DBNull.Value ? null : (DateTime?)row["Birthdate"]
                });
            }

            return View(grades);
        }
    }
}
