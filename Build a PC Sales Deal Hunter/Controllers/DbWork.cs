using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Build_a_PC_Sales_Deal_Hunter.Models;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class DbWork
    {
        public static void AddTask(string email, string query, int price)
        {
            using (var cn2 = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
            "=|DataDirectory|\\db.mdf; Integrated Security=True"))
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
        public static List<TaskModel> GetTasks() 
        {
            List<TaskModel> task = new List<TaskModel>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
            "=|DataDirectory|\\db.mdf; Integrated Security=True"))
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
    }
}