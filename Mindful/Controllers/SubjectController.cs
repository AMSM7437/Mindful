using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;

namespace Mindful.Controllers
{
    public class SubjectController : Controller
    {
        private readonly DbHelper _dbHelper;

        public SubjectController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            try
            {
                var query = @"
                    SELECT 
                        s.id, s.teachersid, s.name, s.passing_grade,
                        t.first_name + ' ' + t.last_name AS teacher_name
                    FROM subjects s
                    LEFT JOIN teachers t ON s.teachersid = t.id";

                var dataTable = _dbHelper.ExecuteQuery(query);

                var subjects = new List<Subject>();

                foreach (DataRow row in dataTable.Rows)
                {
                    subjects.Add(new Subject
                    {
                        id = Convert.ToInt32(row["id"]),
                        teachersid = Convert.ToInt32(row["teachersid"]),
                        name = row["name"].ToString(),
                        passing_Grade = Convert.ToInt32(row["passing_grade"])
                    });
                }

                return View(subjects);
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
        public IActionResult Create(Subject subject)
        {
            if (!ModelState.IsValid)
            {
                return View(subject);
            }

            try
            {
                var query = @"
                    INSERT INTO subjects (teachersid, name, passing_grade)
                    VALUES (@teachersid, @name, @passing_grade)";

                var parameters = new[]
                {
                    new SqlParameter("@teachersid", subject.teachersid),
                    new SqlParameter("@name", subject.name),
                    new SqlParameter("@passing_grade", subject.passing_Grade)
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
                var query = "SELECT * FROM subjects WHERE id = @id";
                var parameters = new[] { new SqlParameter("@id", id) };
                var table = _dbHelper.ExecuteQuery(query, parameters);

                if (table.Rows.Count == 0)
                    return NotFound();

                var row = table.Rows[0];
                var subject = new Subject
                {
                    id = Convert.ToInt32(row["id"]),
                    teachersid = Convert.ToInt32(row["teachersid"]),
                    name = row["name"].ToString(),
                    passing_Grade = Convert.ToInt32(row["passing_grade"])
                };

                return View(subject);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Subject subject)
        {
            if (!ModelState.IsValid)
            {
                return View(subject);
            }

            try
            {
                var query = @"
                    UPDATE subjects
                    SET teachersid = @teachersid,
                        name = @name,
                        passing_grade = @passing_grade
                    WHERE id = @id";

                var parameters = new[]
                {
                    new SqlParameter("@teachersid", subject.teachersid),
                    new SqlParameter("@name", subject.name),
                    new SqlParameter("@passing_grade", subject.passing_Grade),
                    new SqlParameter("@id", subject.id)
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
                var query = "DELETE FROM subjects WHERE id = @id";
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
