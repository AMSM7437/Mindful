using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using HelperBasens;

namespace Mindful.Controllers
{
    public class ClassController : Controller
    {
        private readonly HelperBase _dbHelper;

        public ClassController(HelperBase dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string query = "SELECT id, name FROM classes";
            DataTable dt = _dbHelper.ExecuteSelect(query);

            var classes = new List<Class>();

            foreach (DataRow row in dt.Rows)
            {
                classes.Add(new Class
                {
                    Id = (int)row["id"],
                    Name = row["name"].ToString()
                });
            }

            return View(classes);
        }
    }
}
