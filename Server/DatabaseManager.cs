using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Server
{
    public class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddRecord(string tableName, Record record)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"INSERT INTO {tableName} VALUES (@Param1, @Param2, @Param3, ...)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Param1", record.Field1);
                command.Parameters.AddWithValue("@Param2", record.Field2);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateRecord(string tableName, Record record)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"UPDATE {tableName} SET Field1 = @Param1, Field2 = @Param2, ... WHERE ID = @RecordID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Param1", record.Field1);
                command.Parameters.AddWithValue("@Param2", record.Field2);
                command.Parameters.AddWithValue("@RecordID", record.ID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteRecord(string tableName, int recordID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"DELETE FROM {tableName} WHERE ID = @RecordID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RecordID", recordID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

}
