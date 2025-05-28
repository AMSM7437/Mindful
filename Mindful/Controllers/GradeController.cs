using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Controllers
{
    public class GradeController : Controller
    {
        private readonly DbHelper _dbHelper;

        public GradeController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index(int? classId, int? subjectId)
        {
            try
            {
                var query = @"
            SELECT 
                g.studentsid, g.subjectsid, g.mark,
                s.first_name + ' ' + s.last_name AS student_name,
                sub.name AS subject_name,
                s.classesid
            FROM grades g
            LEFT JOIN students s ON g.studentsid = s.id
            LEFT JOIN subjects sub ON g.subjectsid = sub.id
            WHERE (@classId IS NULL OR s.classesid = @classId)
              AND (@subjectId IS NULL OR g.subjectsid = @subjectId)";

                var parameters = new[]
                {
            new SqlParameter("@classId", (object?)classId ?? DBNull.Value),
            new SqlParameter("@subjectId", (object?)subjectId ?? DBNull.Value)
        };

                var dataTable = _dbHelper.ExecuteQuery(query, parameters);

                var grades = new List<Grade>();
                foreach (DataRow row in dataTable.Rows)
                {
                    grades.Add(new Grade
                    {
                        studentsid = Convert.ToInt32(row["studentsid"]),
                        subjectsid = Convert.ToInt32(row["subjectsid"]),
                        mark = Convert.ToInt32(row["mark"]),
                        StudentName = row["student_name"].ToString(),
                        SubjectName = row["subject_name"].ToString()
                    });
                }
                var classData = _dbHelper.ExecuteQuery("SELECT id, name FROM classes");
                var subjectData = _dbHelper.ExecuteQuery("SELECT id, name FROM subjects");

                ViewBag.ClassFilter = classData.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r["id"].ToString(),
                    Text = r["name"].ToString(),
                    Selected = classId.HasValue && classId.ToString() == r["id"].ToString()
                }).ToList();

                ViewBag.SubjectFilter = subjectData.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r["id"].ToString(),
                    Text = r["name"].ToString(),
                    Selected = subjectId.HasValue && subjectId.ToString() == r["id"].ToString()
                }).ToList();

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
            try
            {
                var students = _dbHelper.ExecuteQuery("SELECT id, first_name + ' ' + last_name AS name FROM students");
                var subjects = _dbHelper.ExecuteQuery("SELECT id, name FROM subjects");

                ViewBag.StudentOptions = students.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

                ViewBag.SubjectOptions = subjects.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

                return View();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
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

                var students = _dbHelper.ExecuteQuery("SELECT id, first_name + ' ' + last_name AS name FROM students");
                var subjects = _dbHelper.ExecuteQuery("SELECT id, name FROM subjects");

                grade.StudentOptions = students.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

                grade.SubjectOptions = subjects.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

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
