using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Build_a_PC_Sales_Deal_Hunter.Models;
using System.Configuration;
using RedditNotifier.Data;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class DbWork
    {
        /// <summary>
        /// Adds the task to the system.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="query">The query.</param>
        /// <param name="price">The price.</param>
        /// <returns>True if the add was successful, otherwise false.</returns>
        public static bool AddTask(string email, string query, int price)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@Email", email));
            parameterList.Add(new SqlParameter("@Query", query));
            parameterList.Add(new SqlParameter("@Price", price));

            return Database.Execute("dbo.Task_spt", parameterList);
        }
        
        /// <summary>
        /// Gets all tasks from the system.
        /// </summary>
        /// <returns>List of Task Model objects.</returns>
        public static List<TaskModel> GetAllTasks()
        {
            var tasks = new List<TaskModel>();

            using (var dbResult = Database.GetDataTable("dbo.Task_sps", CommandType.StoredProcedure))
            {
                if (dbResult != null && dbResult.Rows != null && dbResult.Rows.Count > 0)
                {
                    foreach (DataRow row in dbResult.Rows)
                    {
                        //TODO: set up to be null or default / check for NULL entries from DB.
                        var tm = new TaskModel();
                        tm.Email = row["Email"].ToString().Trim();
                        tm.Query = row["Query"].ToString().Trim();
                        tm.Price = Convert.ToInt32(row["Price"].ToString());
                        tasks.Add(tm);
                    }
                }
            }

            return tasks;
        }

        /// <summary>
        /// Logs the email sent to the system.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="email">The email.</param>
        /// <returns>True if the logging was successful, otherwise false.</returns>
        public static bool LogEmailSent(string url, string email)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@URL", url));
            parameterList.Add(new SqlParameter("@Email", email));
            parameterList.Add(new SqlParameter("@Time", DateTime.Now));

            return Database.Execute("dbo.SendEmail_spt", parameterList);
        }

        /// <summary>
        /// Checks if a given email was sent.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool CheckIfEmailSentToUser(string url, string email)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@URL", url));
            parameterList.Add(new SqlParameter("@Email", email));

            var dbResult = Database.GetScalar("dbo.SendEmail_sps", CommandType.StoredProcedure, parameterList);

            if (dbResult != null)
            {
                int emailSentCount = 0;
                Int32.TryParse(dbResult.ToString(), out emailSentCount);

                if (emailSentCount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the given email from the system.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>True if the email was removed. Otherwise false.</returns>
        public static bool RemoveFromEmailServiceByEmail(string email)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@Email", email));

            return Database.Execute("dbo.Task_spd", parameterList);
        }

        /// <summary>
        /// Gets information about number of users, emails sent and any errors logged.
        /// </summary>
        /// <returns></returns>
        public static StatsInfoModel GetStats()
        {
            var si = new StatsInfoModel();
            si.UniqueUsers = GetNumberOfUsers();            
            si.Errors = GetListOfErrors();
            si.EmailsSent = GetNumberEmailsSent();
            return si;
        }

        private static int GetNumberEmailsSent()
        {
            var dbResult = Database.GetScalar("dbo.SendEmailCount_sps", CommandType.StoredProcedure);
            if (dbResult != null)
            {
                int emailsSent = 0;
                int.TryParse(dbResult.ToString(), out emailsSent);

                return emailsSent;
            }
            return 0;  
        }

        /// <summary>
        /// Returns a list of all errors in the system.
        /// </summary>
        /// <returns></returns>
        private static List<Error> GetListOfErrors()
        {
            var result = new List<Error>();
            using (var dbResult = Database.GetDataTable("dbo.ErrorLogging_sps", null))
            {
                if(dbResult != null && dbResult.Rows != null && dbResult.Rows.Count > 0)
                {
                    foreach (DataRow row in dbResult.Rows)
                    {
                        var er = new Error();
                        er.ErrorMessage = row["Error"].ToString();
                        er.Time = Convert.ToDateTime(row["Time"].ToString());
                        result.Add(er);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the number of unique users in the system.
        /// </summary>
        /// <returns></returns>
        private static int GetNumberOfUsers()
        {
            var dbResult = Database.GetScalar("dbo.UniqueUsers_sps", CommandType.StoredProcedure);
            if (dbResult != null)
            {
                int uniqueUsers = 0;
                int.TryParse(dbResult.ToString(), out uniqueUsers);

                return uniqueUsers;
            }
            return 0;            
        }

        public static List<TaskModel> GetTasksByIndividual(string email)
        {
            //TODO: consider refactoring this to return a single TaskModel object.
            //Or should this return all tasks for a given email address?
            // Also consider refactoring this to be simply: GetTask . Individual is redundant.
            var task = new List<TaskModel>();
            try
            {
                var parameterList = new List<SqlParameter>();
                parameterList.Add(new SqlParameter("@Email", email));

                using (var dbResult = Database.GetDataTable("IndividualTask_sps", CommandType.StoredProcedure, parameterList))
                {
                    if (dbResult != null && dbResult.Rows != null && dbResult.Rows.Count > 0)
                    {
                        foreach (DataRow row in dbResult.Rows)
                        {
                            var tm = new TaskModel();
                            tm.Email = row["Email"].ToString().Trim();
                            tm.Query = row["Query"].ToString().Trim();
                            tm.Price = Convert.ToInt32(row["Price"].ToString());
                            task.Add(tm);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LogError("[" + e.Message + "] [" + e.TargetSite + "] [" + e.Source + "] [" + e.Data + "]");                
            }
            return task;
        }

        /// <summary>
        /// Deletes the individual task.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="query">The query.</param>
        /// <param name="price">The price.</param>
        /// <returns>True if the task was deleted, otherwise false.</returns>
        public static bool DeleteTaskByIndividual(string email, string query, int price)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@Email", email));
            parameterList.Add(new SqlParameter("@Query", query));
            parameterList.Add(new SqlParameter("@Price", price));

            return Database.Execute("dbo.IndividualTask_spd", parameterList);
        }
        /// <summary>
        /// Logs the URL used.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="shortened">The shortened.</param>
        /// <returns></returns>
        public static bool LogUrlUsed(string original, string shortened)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@OriginalUrl", original));
            parameterList.Add(new SqlParameter("@ShortenedUrl", shortened));

            return Database.Execute("dbo.UrlsUsed_spt", parameterList);            
        }

        /// <summary>
        /// Gets the shortended URL.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns>String - the shortened URL. If it fails, returns an empty string.</returns>
        public static string GetShortendedUrl(string original)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@OriginalUrl", original));

            var dbResult = Database.GetScalar("UrlsUsed_sps", CommandType.StoredProcedure, parameterList);
            if (dbResult != null)
            {
                return dbResult.ToString();
            }
            return string.Empty;
        }


        public static bool AddJson(string json)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@Json", json));

            return Database.Execute("dbo.api_spt", parameterList);
        }

        public static bool UpdateJson(string json)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@Json", json));

            return Database.Execute("dbo.api_spu", parameterList);
        }


        public static string GetJson()
        {
            var dbResult = Database.GetScalar("api_sps", CommandType.StoredProcedure);
            if (dbResult != null)
            {
                return dbResult.ToString();
            }
            return string.Empty;
        }

        public static bool UpdateAndroidDownloadCount()
        {
            var parameterList = new List<SqlParameter>();
            return Database.Execute("dbo.DownloadLog_spu", parameterList);
        }
    }
}