using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditNotifier.Data
{
    public static class Logging
    {
        /// <summary>
        /// Logs the error to the database.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="optionalMessage">The optional message.</param>
        public static void LogError(string Message)
        {
            //I'm really not sure WHY these parameters exist the way they do... but hey when I know the DB schema I can fix this.
            //TODO: add the exception / stacktrace to the database.
            //Maybe: Type, Message, Inner Exception (if any), and stacktrace.
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@Error", Message));
            parameterList.Add(new SqlParameter("@Time", DateTime.Now)); //TODO: Make this use the CURRENT_TIMESTAMP attribute in SQL Server. This way we don't need to pass in another parameter.

            Database.Execute("dbo.ErrorLogging_spt", parameterList);           
        }
    }
}
