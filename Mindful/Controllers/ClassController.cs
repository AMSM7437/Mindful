using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;

namespace Mindful.Controllers
{
    public class ClassController : Controller
    {
        private readonly DbHelper _dbHelper;

        public ClassController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            try
            {
                var query = "SELECT id, name FROM classes";
                var dataTable = _dbHelper.ExecuteQuery(query);

                var classes = new List<Class>();

                foreach (DataRow row in dataTable.Rows)
                {
                    classes.Add(new Class
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Name = row["name"].ToString()
                    });
                }

                return View(classes);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Class cls)
        {
            if (!ModelState.IsValid)
            {
                return View(cls);
            }

            try
            {
                var query = "INSERT INTO classes (name) VALUES (@name)";
                var parameters = new[]
                {
                    new SqlParameter("@name", cls.Name)
                };

                _dbHelper.ExecuteNonQuery(query, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return Content($"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var query = "SELECT * FROM classes WHERE id = @id";
                var parameters = new[] { new SqlParameter("@id", id) };
                var table = _dbHelper.ExecuteQuery(query, parameters);

                if (table.Rows.Count == 0)
                    return NotFound();

                var row = table.Rows[0];
                var cls = new Class
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString()
                };

                return View(cls);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Class cls)
        {
            if (!ModelState.IsValid)
            {
                return View(cls);
            }

            try
            {
                var query = "UPDATE classes SET name = @name WHERE id = @id";
                var parameters = new[]
                {
                    new SqlParameter("@name", cls.Name),
                    new SqlParameter("@id", cls.Id)
                };

                _dbHelper.ExecuteNonQuery(query, parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return Content($"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var query = "DELETE FROM classes WHERE id = @id";
                var parameters = new[] { new SqlParameter("@id", id) };
                _dbHelper.ExecuteNonQuery(query, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return Content($"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
