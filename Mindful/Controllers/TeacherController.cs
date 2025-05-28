using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;

namespace Mindful.Controllers
{
    public class TeacherController : Controller
    {
        private readonly DbHelper _dbHelper;

        public TeacherController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            try
            {
                var query = "SELECT id, first_name, last_name, email, birthdate FROM teachers";
                var dataTable = _dbHelper.ExecuteQuery(query);

                var teachers = new List<Teacher>();

                foreach (DataRow row in dataTable.Rows)
                {
                    teachers.Add(new Teacher
                    {
                        id = Convert.ToInt32(row["id"]),
                        first_Name = row["first_name"].ToString(),
                        last_Name = row["last_name"].ToString(),
                        email = row["email"].ToString(),
                        birthdate = row["birthdate"] == DBNull.Value ? null : Convert.ToDateTime(row["birthdate"])
                    });
                }

                return View(teachers);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("error.log", ex.ToString());
                return View("Error", ex);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            try
            {
                var query = "INSERT INTO teachers (first_name, last_name, email, birthdate) VALUES (@first_name, @last_name, @email, @birthdate)";
                var parameters = new[]
                {
                    new SqlParameter("@first_name", teacher.first_Name),
                    new SqlParameter("@last_name", teacher.last_Name),
                    new SqlParameter("@email", teacher.email),
                    new SqlParameter("@birthdate", (object?)teacher.birthdate ?? DBNull.Value)
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
                var query = "SELECT * FROM teachers WHERE id = @id";
                var parameters = new[] { new SqlParameter("@id", id) };
                var table = _dbHelper.ExecuteQuery(query, parameters);

                if (table.Rows.Count == 0)
                    return NotFound();

                var row = table.Rows[0];
                var teacher = new Teacher
                {
                    id = Convert.ToInt32(row["id"]),
                    first_Name = row["first_name"].ToString(),
                    last_Name = row["last_name"].ToString(),
                    email = row["email"].ToString(),
                    birthdate = row["birthdate"] == DBNull.Value ? null : Convert.ToDateTime(row["birthdate"])
                };

                return View(teacher);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            try
            {
                var query = @"
                    UPDATE teachers 
                    SET first_name = @first_name, 
                        last_name = @last_name, 
                        email = @email, 
                        birthdate = @birthdate 
                    WHERE id = @id";

                var parameters = new[]
                {
                    new SqlParameter("@first_name", teacher.first_Name),
                    new SqlParameter("@last_name", teacher.last_Name),
                    new SqlParameter("@email", teacher.email),
                    new SqlParameter("@birthdate", (object?)teacher.birthdate ?? DBNull.Value),
                    new SqlParameter("@id", teacher.id)
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
                var query = "DELETE FROM teachers WHERE id = @id";
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
