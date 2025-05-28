using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                var query = @"
            SELECT t.id, t.first_name, t.last_name, t.email, t.birthdate, s.name AS subject_name
            FROM teachers t
            LEFT JOIN subjects s ON t.id = s.teachersid";

                var dataTable = _dbHelper.ExecuteQuery(query);

                var teacherDict = new Dictionary<int, Teacher>();

                foreach (DataRow row in dataTable.Rows)
                {
                    int teacherId = Convert.ToInt32(row["id"]);

                    if (!teacherDict.ContainsKey(teacherId))
                    {
                        teacherDict[teacherId] = new Teacher
                        {
                            id = teacherId,
                            first_Name = row["first_name"].ToString(),
                            last_Name = row["last_name"].ToString(),
                            email = row["email"].ToString(),
                            birthdate = row["birthdate"] == DBNull.Value ? null : Convert.ToDateTime(row["birthdate"]),
                            SubjectNames = new List<string>()
                        };
                    }

                    if (row["subject_name"] != DBNull.Value)
                    {
                        teacherDict[teacherId].SubjectNames.Add(row["subject_name"].ToString());
                    }
                }

                return View(teacherDict.Values.ToList());
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
            //if (!ModelState.IsValid)
            //{
            //    return View(teacher);
            //}

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

                // Dropdown: All subjects not currently assigned
                var allSubjects = _dbHelper.ExecuteQuery("SELECT id, name FROM subjects WHERE teachersid IS NULL");


                teacher.SubjectOptions = allSubjects.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

                // Multi-select: Currently assigned subjects
                var assignedSubjects = _dbHelper.ExecuteQuery("SELECT id, name FROM subjects WHERE teachersid = @id", new[] {
    new SqlParameter("@id", id)
});


                teacher.AssignedSubjectOptions = assignedSubjects.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

                teacher.AssignedSubjectIds = assignedSubjects.AsEnumerable()
                    .Select(r => Convert.ToInt32(r["id"]))
                    .ToList();

                return View(teacher);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return Content($"Error: {ex.Message}\n\n{ex.StackTrace}");
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
                // Update teacher info
                var updateTeacherQuery = @"
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

                _dbHelper.ExecuteNonQuery(updateTeacherQuery, parameters);

                // Handle removals: set teachersid = NULL for all that were previously assigned but not submitted
                var getCurrentSubjectIdsQuery = "SELECT id FROM subjects WHERE teachersid = @id";
                var currentRows = _dbHelper.ExecuteQuery(getCurrentSubjectIdsQuery, new[] { new SqlParameter("@id", teacher.id) });

                var currentIds = currentRows.AsEnumerable().Select(r => Convert.ToInt32(r["id"])).ToList();
                var submittedIds = teacher.AssignedSubjectIds ?? new List<int>();

                var toUnassign = currentIds.Except(submittedIds).ToList();

                foreach (var sid in toUnassign)
                {
                    _dbHelper.ExecuteNonQuery("UPDATE subjects SET teachersid = NULL WHERE id = @id", new[] {
                new SqlParameter("@id", sid)
            });
                }

                // Reassign selected subjects (in case user reassigned them)
                foreach (var sid in submittedIds)
                {
                    _dbHelper.ExecuteNonQuery("UPDATE subjects SET teachersid = @tid WHERE id = @id", new[] {
                new SqlParameter("@tid", teacher.id),
                new SqlParameter("@id", sid)
            });
                }
            
                // Assign new subject from dropdown if provided and not already assigned
                if (teacher.SelectedSubjectId.HasValue && !submittedIds.Contains(teacher.SelectedSubjectId.Value))
                {
                    _dbHelper.ExecuteNonQuery("UPDATE subjects SET teachersid = @tid WHERE id = @id", new[] {
        new SqlParameter("@tid", teacher.id),
        new SqlParameter("@id", teacher.SelectedSubjectId.Value)
    });
                }


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
