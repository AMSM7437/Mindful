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
            string query = "SELECT Id, Teachersid, Name, Passing_Grade FROM Subject";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var subjects = new List<Subject>();

            foreach (DataRow row in dt.Rows)
            {
                subjects.Add(new Subject
                {
                    Id = (int)row["Id"],
                    Teachersid = (int)row["Teachersid"],
                    Name = row["Name"].ToString(),
                    Passing_Grade = (int)row["Passing_Grade"]
                });
            }

            return View(subjects);
        }
    }
}
