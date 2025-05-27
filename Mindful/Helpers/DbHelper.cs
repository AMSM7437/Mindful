using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Mindful.DataAccess
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = "Server=localhost;Database=Mindful;Integrated Security=True;TrustServerCertificate=True;";
  }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteScalar();
            }
        }
    }
}