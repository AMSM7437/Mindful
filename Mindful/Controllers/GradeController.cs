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
            string query = "SELECT studentsid, subjectsid, mark FROM grades";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var grades = new List<Grade>();

            foreach (DataRow row in dt.Rows)
            {
                grades.Add(new Grade
                {
                    studentsid = (int)row["studentsid"],
                    subjectsid = (int)row["subjectsid"],
                    mark = (int)row["mark"]
                });
            }

            return View(grades);
        }
    }
}
