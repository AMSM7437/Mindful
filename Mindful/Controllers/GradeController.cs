using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;

namespace Mindful.Controllers
{
    public class GradeController : Controller
    {
        private readonly DbHelper _dbHelper;

        public GradeController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            try
            {
                var query = @"
                    SELECT 
                        g.studentsid, g.subjectsid, g.mark,
                        s.first_name + ' ' + s.last_name AS student_name,
                        sub.name AS subject_name
                    FROM grades g
                    LEFT JOIN students s ON g.studentsid = s.id
                    LEFT JOIN subjects sub ON g.subjectsid = sub.id";

                var dataTable = _dbHelper.ExecuteQuery(query);

                var grades = new List<Grade>();

                foreach (DataRow row in dataTable.Rows)
                {
                    grades.Add(new Grade
                    {
                        studentsid = Convert.ToInt32(row["studentsid"]),
                        subjectsid = Convert.ToInt32(row["subjectsid"]),
                        mark = Convert.ToInt32(row["mark"])
                    });
                }

                return View(grades);
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
        public IActionResult Create(Grade grade)
        {
            if (!ModelState.IsValid)
            {
                return View(grade);
            }

            try
            {
                var query = @"
                    INSERT INTO grades (studentsid, subjectsid, mark)
                    VALUES (@studentsid, @subjectsid, @mark)";

                var parameters = new[]
                {
                    new SqlParameter("@studentsid", grade.studentsid),
                    new SqlParameter("@subjectsid", grade.subjectsid),
                    new SqlParameter("@mark", grade.mark)
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

        public IActionResult Edit(int studentsid, int subjectsid)
        {
            try
            {
                var query = "SELECT * FROM grades WHERE studentsid = @studentsid AND subjectsid = @subjectsid";
                var parameters = new[]
                {
                    new SqlParameter("@studentsid", studentsid),
                    new SqlParameter("@subjectsid", subjectsid)
                };

                var table = _dbHelper.ExecuteQuery(query, parameters);

                if (table.Rows.Count == 0)
                    return NotFound();

                var row = table.Rows[0];
                var grade = new Grade
                {
                    studentsid = Convert.ToInt32(row["studentsid"]),
                    subjectsid = Convert.ToInt32(row["subjectsid"]),
                    mark = Convert.ToInt32(row["mark"])
                };

                return View(grade);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Grade grade)
        {
            if (!ModelState.IsValid)
            {
                return View(grade);
            }

            try
            {
                var query = @"
                    UPDATE grades
                    SET mark = @mark
                    WHERE studentsid = @studentsid AND subjectsid = @subjectsid";

                var parameters = new[]
                {
                    new SqlParameter("@mark", grade.mark),
                    new SqlParameter("@studentsid", grade.studentsid),
                    new SqlParameter("@subjectsid", grade.subjectsid)
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

        public IActionResult Delete(int studentsid, int subjectsid)
        {
            try
            {
                var query = "DELETE FROM grades WHERE studentsid = @studentsid AND subjectsid = @subjectsid";
                var parameters = new[]
                {
                    new SqlParameter("@studentsid", studentsid),
                    new SqlParameter("@subjectsid", subjectsid)
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
    }
}
