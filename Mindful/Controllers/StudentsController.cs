using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Mindful.Models;
using Mindful.DataAccess;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mindful.Controllers
{
    public class StudentController : Controller
    {
        private readonly DbHelper _dbHelper;

        public StudentController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index(string addressFilter)
        {
            try
            {
                var query = @"
            SELECT 
                s.id, s.classesid, s.first_name, s.last_name, s.father_name, 
                s.mother_name, s.birthdate, s.address, c.name AS class_name
            FROM students s
            LEFT JOIN classes c ON s.classesid = c.id";

                var dataTable = _dbHelper.ExecuteQuery(query);

                var students = new List<Students>();

                foreach (DataRow row in dataTable.Rows)
                {
                    var student = new Students
                    {
                        id = Convert.ToInt32(row["id"]),
                        classesid = Convert.ToInt32(row["classesid"]),
                        first_name = row["first_name"].ToString(),
                        last_name = row["last_name"].ToString(),
                        father_name = row["father_name"].ToString(),
                        mother_name = row["mother_name"].ToString(),
                        birthdate = row["birthdate"] == DBNull.Value ? null : Convert.ToDateTime(row["birthdate"]),
                        address = Convert.ToInt32(row["address"]),
                        class_name = row["class_name"]?.ToString()
                    };

                    if (string.IsNullOrEmpty(addressFilter) || student.address.ToString().Contains(addressFilter))
                    {
                        students.Add(student);
                    }
                }

                return View(students);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }


        public IActionResult Create()
        {
            var student = new Students();

            // Optional: If you're using a dropdown for class selection
            var classData = _dbHelper.ExecuteQuery("SELECT id, name FROM classes");
            student.ClassOptions = classData.AsEnumerable()
                .Select(r => new SelectListItem
                {
                    Value = r["id"].ToString(),
                    Text = r["name"].ToString()
                }).ToList();

            return View(student); // ✅ Model is no longer null
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Students student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            try
            {
                var query = @"
                    INSERT INTO students 
                        (classesid, first_name, last_name, father_name, mother_name, birthdate, address)
                    VALUES 
                        (@classesid, @first_name, @last_name, @father_name, @mother_name, @birthdate, @address)";

                var parameters = new[]
                {
                    new SqlParameter("@classesid", student.classesid),
                    new SqlParameter("@first_name", student.first_name),
                    new SqlParameter("@last_name", student.last_name),
                    new SqlParameter("@father_name", student.father_name),
                    new SqlParameter("@mother_name", student.mother_name),
                    new SqlParameter("@birthdate", (object?)student.birthdate ?? DBNull.Value),
                    new SqlParameter("@address", student.address)
                };
                var classData = _dbHelper.ExecuteQuery("SELECT id, name FROM classes");
                student.ClassOptions = classData.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

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
                var query = "SELECT * FROM students WHERE id = @id";
                var parameters = new[] { new SqlParameter("@id", id) };
                var table = _dbHelper.ExecuteQuery(query, parameters);

                if (table.Rows.Count == 0)
                    return NotFound();

                var row = table.Rows[0];
                var student = new Students
                {
                    id = Convert.ToInt32(row["id"]),
                    classesid = Convert.ToInt32(row["classesid"]),
                    first_name = row["first_name"].ToString(),
                    last_name = row["last_name"].ToString(),
                    father_name = row["father_name"].ToString(),
                    mother_name = row["mother_name"].ToString(),
                    birthdate = row["birthdate"] == DBNull.Value ? null : Convert.ToDateTime(row["birthdate"]),
                    address = Convert.ToInt32(row["address"])
                };
                var classData = _dbHelper.ExecuteQuery("SELECT id, name FROM classes");
                student.ClassOptions = classData.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["id"].ToString(),
                        Text = r["name"].ToString()
                    }).ToList();

                return View(student);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return View("Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Students student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            try
            {
                var query = @"
                    UPDATE students
                    SET classesid = @classesid,
                        first_name = @first_name,
                        last_name = @last_name,
                        father_name = @father_name,
                        mother_name = @mother_name,
                        birthdate = @birthdate,
                        address = @address
                    WHERE id = @id";

                var parameters = new[]
                {
                    new SqlParameter("@classesid", student.classesid),
                    new SqlParameter("@first_name", student.first_name),
                    new SqlParameter("@last_name", student.last_name),
                    new SqlParameter("@father_name", student.father_name),
                    new SqlParameter("@mother_name", student.mother_name),
                    new SqlParameter("@birthdate", (object?)student.birthdate ?? DBNull.Value),
                    new SqlParameter("@address", student.address),
                    new SqlParameter("@id", student.id)
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
                // First delete grades associated with the student
                var deleteGradesQuery = "DELETE FROM grades WHERE studentsid = @id";
                _dbHelper.ExecuteNonQuery(deleteGradesQuery, new[] { new SqlParameter("@id", id) });

                // Then delete the student
                var deleteStudentQuery = "DELETE FROM students WHERE id = @id";
                _dbHelper.ExecuteNonQuery(deleteStudentQuery, new[] { new SqlParameter("@id", id) });

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
