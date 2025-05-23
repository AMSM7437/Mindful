using System.Data;
using Microsoft.Data.SqlClient;

namespace HelperBase
{
    public class HelperBase
    {
        private readonly string _connectionString;

        public HelperBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteSelect(string query, params SqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }
    }
}