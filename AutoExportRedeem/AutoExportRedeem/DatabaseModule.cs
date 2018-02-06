using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace AutoExportRedeem
{
    class DatabaseModule
    {
        private SqlConnection connection;

        private string connectionString;

        public DatabaseModule()
            : this("192.168.10.192", "DASDB_TEST", "sa", "P@ssw0rd", 30)
        {

        }

        public DatabaseModule(string dataSource, string databaseName, string username, string password, int timeout)
        {
            StringBuilder connectionStr = new StringBuilder();
            connectionStr.Append("Data Source=" + dataSource + ";");
            connectionStr.Append("Initial Catalog=" + databaseName + ";");
            if (username != "")
            {
                connectionStr.Append("User id=" + username + ";");
                if (password != "")
                {
                    connectionStr.Append("Password=" + password + ";");
                }
            }
            else
            {
                connectionStr.Append("Integrated Security=true;");
            }

            connectionStr.Append("Connection Timeout=" + timeout.ToString() + ";");
            this.connection = new SqlConnection(connectionStr.ToString());
            connectionString = connectionStr.ToString();

            OpenConnection();
            CloseConnection();
        }

        private void OpenConnection()
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (this.connection.State != ConnectionState.Closed)
            {
                this.connection.Close();
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                CloseConnection();
            }


        }

        public void ExecuteQueryNonReturn(string query)
        {
            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't execute sql query");
            }
            finally
            {
                CloseConnection();
            }

        }

        public void ExecuteStoreProcedure(string procedureName)
        {
            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }

        }

        public void ExecuteStoreProcedure(string procedureName,string procedureVariable)
        {
            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MonthOfYear", procedureVariable);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }

        }

        public void WriteLog(System_Type type, string desc, string lineCode)
        {
            using (var tempConnection = new SqlConnection(connectionString))
            {
                tempConnection.Open();
                string rawQuery = "INSERT INTO Redeem_Log VALUES('{0}','{1}','{2}','{3}')";
                string query = string.Format(rawQuery, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), type.ToString(), desc, lineCode);
                using (var command = new SqlCommand(query, tempConnection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            //SqlCommand command = new SqlCommand(query, tempConnection);
            //command.CommandType = CommandType.Text;
            //command.ExecuteNonQuery();
        }
    }

    public enum System_Type
    {
        Process, Retrieve_Data, Adjust_Data, Export_Data, Transfer_Data
    }
}
