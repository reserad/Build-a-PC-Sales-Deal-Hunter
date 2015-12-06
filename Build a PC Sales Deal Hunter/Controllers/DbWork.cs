using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Build_a_PC_Sales_Deal_Hunter.Models;
using System.Configuration;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class DbWork
    {
        public static string connectionString;
        public DbWork() 
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        }
        public void AddTask(string email, string query, int price)
        {
            using (var cn2 = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO [dbo].[Emails] ([Email], [Query], [Price]) VALUES (@E, @Q, @P)";
                var insertCmd2 = new SqlCommand(sql, cn2);
                insertCmd2.Parameters
                    .Add(new SqlParameter("@E", SqlDbType.VarChar))
                    .Value = email;
                insertCmd2.Parameters
                    .Add(new SqlParameter("@Q", SqlDbType.VarChar))
                    .Value = query;
                insertCmd2.Parameters
                    .Add(new SqlParameter("@P", SqlDbType.Int))
                    .Value = price;
                cn2.Open();
                insertCmd2.ExecuteNonQuery();
                cn2.Close();
            }
        }
        public List<TaskModel> GetTasks() 
        {
            List<TaskModel> task = new List<TaskModel>();
            using (var cn = new SqlConnection(connectionString))
            {
                string _sql = @"SELECT * FROM [dbo].[Emails]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TaskModel tm = new TaskModel();
                        tm.Email = reader["Email"].ToString();
                        tm.Query = reader["Query"].ToString();
                        tm.Price = Convert.ToInt32(reader["Price"].ToString());
                        task.Add(tm);
                    }
                }
                reader.Dispose();
                cmd.Dispose();
            }
            return task;
        }
        public void LogEmailSent(string url, string email) 
        {
            using (var cn2 = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO [dbo].[EmailsSent] ([URL], [Email]) VALUES (@U, @E)";
                var insertCmd2 = new SqlCommand(sql, cn2);
                insertCmd2.Parameters
                    .Add(new SqlParameter("@U", SqlDbType.VarChar))
                    .Value = url;
                insertCmd2.Parameters
                    .Add(new SqlParameter("@E", SqlDbType.VarChar))
                    .Value = email;
                cn2.Open();
                insertCmd2.ExecuteNonQuery();
                cn2.Close();
            }
        }
        public bool CheckIfEmailSent(string url, string email) 
        {
            using (var cn = new SqlConnection(connectionString))
            {
                string _sql = @"SELECT * FROM [dbo].[EmailsSent]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (email.Equals(reader["Email"].ToString()) && url.Equals(reader["URL"].ToString()))
                        {
                            return true;
                        }
                    }
                }
                reader.Dispose();
                cmd.Dispose();
            }
            return false;
        }
        public void LogError(string error) 
        {
            using (var cn2 = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO [dbo].[ErrorLogging] ([Error], [Time]) VALUES (@E, @T)";
                var insertCmd2 = new SqlCommand(sql, cn2);
                insertCmd2.Parameters
                    .Add(new SqlParameter("@E", SqlDbType.VarChar))
                    .Value = error;
                insertCmd2.Parameters
                    .Add(new SqlParameter("@T", SqlDbType.DateTime))
                    .Value = DateTime.Now;
                cn2.Open();
                insertCmd2.ExecuteNonQuery();
                cn2.Close();
            }
        }
        public void RemoveFromEmailService(string email)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                string _sql = @"DELETE FROM [dbo].[EmailsSent] WHERE [Email] = @E";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@E", SqlDbType.VarChar))
                    .Value = email;
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }

            using (var cn = new SqlConnection(connectionString))
            {
                string _sql = @"DELETE FROM [dbo].[Emails] WHERE [Email] = @E";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@E", SqlDbType.VarChar))
                    .Value = email;
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }
        }
    }
}