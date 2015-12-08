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
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.Task_spt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters
                    .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                    .Value = email;
                cmd.Parameters
                    .Add(new SqlParameter("@Query", SqlDbType.VarChar))
                    .Value = query;
                cmd.Parameters
                    .Add(new SqlParameter("@Price", SqlDbType.Int))
                    .Value = price;
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<TaskModel> GetTasks() 
        {
            List<TaskModel> task = new List<TaskModel>();
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.Task_sps", cn);
                cmd.CommandType = CommandType.StoredProcedure;
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
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.SendEmail_spt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters
                    .Add(new SqlParameter("@URL", SqlDbType.VarChar))
                    .Value = url;
                cmd.Parameters
                    .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                    .Value = email;
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public bool CheckIfEmailSent(string url, string email) 
        {
            using (var cn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("dbo.SendEmail_sps", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters
                        .Add(new SqlParameter("@URL", SqlDbType.VarChar))
                        .Value = url;
                    cmd.Parameters
                        .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                        .Value = email;
                    cn.Open();
                    var result = cmd.ExecuteReader();
                    cn.Close();
                    if (result != null)
                    {
                        int emailSentCount = 0;
                        int.TryParse(result.ToString(), out emailSentCount);

                        if (emailSentCount > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public void LogError(string error) 
        {
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.LogError_spt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters
                    .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                    .Value = error;
                cmd.Parameters
                    .Add(new SqlParameter("@Time", SqlDbType.DateTime))
                    .Value = DateTime.Now;
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public void RemoveFromEmailService(string email)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.Task_spd", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters
                    .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                    .Value = email;
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }
        }
        public StatusInfoModel GetStatus()
        {
            StatusInfoModel si = new StatusInfoModel();
            using (var cn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("dbo.UniqueUsers_sps", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    var result = cmd.ExecuteScalar();
                    cn.Close();

                    if (result != null)
                    {
                        int UniqueUsers = 0;
                        int.TryParse(result.ToString(), out UniqueUsers);
                        si.UniqueUsers = UniqueUsers;
                    }
                }
            }
            List<Error> Errors = new List<Error>();
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.ErrorLogging_sps", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Error er = new Error();
                        er.ErrorMessage = reader["Error"].ToString();
                        er.Time = Convert.ToDateTime(reader["Time"].ToString());
                        Errors.Add(er);
                    }
                }
                cn.Close();
                reader.Dispose();
                cmd.Dispose();
            }
            si.Errors = Errors;
            return si;
        }
        public List<TaskModel> GetIndividualTask(string email)
        {
            List<TaskModel> task = new List<TaskModel>();
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.IndividualTask_sps", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters
                    .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                    .Value = email;
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
        public void DeleteIndividualTask(string email, string query, int price)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.IndividualTask_spd", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters
                    .Add(new SqlParameter("@Email", SqlDbType.VarChar))
                    .Value = email;
                cmd.Parameters
                    .Add(new SqlParameter("@Query", SqlDbType.VarChar))
                    .Value = query;
                cmd.Parameters
                    .Add(new SqlParameter("@Price", SqlDbType.VarChar))
                    .Value = price;
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }
        }
    }
}