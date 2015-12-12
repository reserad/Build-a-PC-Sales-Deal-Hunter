using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RedditNotifier.Data
{
    public static class Database
    {
        /// <summary>
        /// Returns a SQL Server Connection String.
        /// </summary>
        /// <returns>string - the connection string.</returns>
        private static string GetConnectionString()
        {
            try
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString().Trim();                
            }
            catch (Exception ex)
            {
                Logging.LogError(ex, "GetConnectionString");
                return "Unable to determine connection string. Error has been logged.";
            }
        }

        /// <summary>
        /// Gets the data table.
        /// The default command type will be Stored Procedure.
        /// If you need to use a different type:
        /// 1. Re-evaluate your life choices.
        /// 2. Use the GetDataTable(string, CommandType, List<SqlParameter>) method.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterList">The parameter list.</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string command, List<SqlParameter> parameterList = null)
        {
            return GetDataTable(command, CommandType.StoredProcedure, parameterList);
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="type">The type.</param>
        /// <param name="parameterList">The parameter list.</param>
        /// <returns></returns>        
        public static DataTable GetDataTable(string command, CommandType type = CommandType.StoredProcedure, List<SqlParameter> parameterList = null)
        {
            try
            {
                using (var cnn = new SqlConnection(GetConnectionString()))
                {
                    using (var dt = new DataTable())
                    {
                        using (var cmd = new SqlCommand(command, cnn) { CommandType = type })
                        {
                            if (parameterList != null && parameterList.Count > 0)
                            {
                                foreach (var p in parameterList)
                                {
                                    cmd.Parameters.Add(p);
                                }
                            }

                            using (var adapter = new SqlDataAdapter(cmd))
                            {
                                adapter.Fill(dt);
                            }
                            cmd.Parameters.Clear();
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogError(ex, "Database.GetDataTable");
                return new DataTable();
            }
        }

        /// <summary>
        /// Uses the stored procedure information to get the first value returned from that procedure.
        /// </summary>
        /// <param name="command">Name of stored procedure to call. Can also be the SQL text to execute.</param>
        /// <param name="parameterList">List of parameters to include in the stored procedure call</param>
        /// <returns>An object containing the scalar value returned from the stored procedure</returns>
        public static object GetScalar(string command, CommandType type = CommandType.StoredProcedure, List<SqlParameter> parameterList = null)
        {
            try
            {
                using (var cnn = new SqlConnection(GetConnectionString()))
                {
                    using (var cmd = new SqlCommand(command, cnn))
                    {
                        cmd.CommandType = type;

                        if (parameterList != null)
                        {
                            foreach (var p in parameterList)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }

                        cmd.Connection.Open();
                        object obj = cmd.ExecuteScalar();
                        cmd.Connection.Close();
                        cmd.Parameters.Clear();
                        return obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogError(ex, "MSSqlUtility.GetScalar");
                return -1;
            }
        }

        /// <summary>
        /// Executes the specified command.
        /// The default command type will be Stored Procedure.
        /// If you need to use a different type:
        /// 1. Re-evaluate your life choices.
        /// 2. Use the Execute(string, CommandType, List<SqlParameter>) method.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterList">The parameter list.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public static bool Execute(string command, List<SqlParameter> parameterList = null)
        {
            return Execute(command, CommandType.StoredProcedure, parameterList);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">Name of the command. This may be the SQL query or the name of a stored procedure.</param>
        /// <param name="type">The type.</param>
        /// <param name="parameterList">The parameter list.</param>
        /// <returns>True if the run was successful, otherwise false.</returns>
        public static bool Execute(string command, CommandType type = CommandType.StoredProcedure, List<SqlParameter> parameterList = null)
        {
            try
            {
                using (var con = new SqlConnection(GetConnectionString()))
                {
                    using (var cmd = new SqlCommand(command, con))
                    {
                        cmd.CommandType = type;
                        if (parameterList != null)
                        {
                            foreach (var p in parameterList)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                        cmd.Connection.Dispose();
                        cmd.Parameters.Clear();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError(ex, "Database.Execute");
                return false;
            }
        }
    }
}
