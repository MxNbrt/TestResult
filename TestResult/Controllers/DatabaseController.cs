using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TestResult.Models;

namespace TestResult.Controllers
{
    public class AppRunExtension : AppRun
    {
        public string SuiteCount;
        public string CaseCount;
        public string FailedCaseCount;
    }

    public class DatabaseController : ApiController
    {
        // GET: api/AppRuns
        private UnitTestLogEntities context = new UnitTestLogEntities();
        public HttpResponseMessage Get()
        {
            string sqlQuery =
                "select AppRunId, AppArea, BuildDate, ServerName, StartTime, EndTime, " +
                "(select count(*) from TestSuiteRun s where s.AppRunId = r.AppRunId) as SuiteCount, " +
                "(select count(*) from TestCaseRun c where c.SuiteRunId in  " +
                "    (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                ") as CaseCount, " +
                "(select count(distinct(e.CaseRunId)) from TestError e where e.CaseRunId in " +
                "(select CaseRunId from TestCaseRun c where c.SuiteRunId in  " +
                "    (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                ")) " +
                "as FailedCaseCount " +
                "from ( " +
                "SELECT MAX(StartTime) OVER (partition by AppArea, ServerName) MaxStartTime, * " +
                "from AppRun) r where StartTime = MaxStartTime";
            return ToJson(ExecuteQuery(sqlQuery));
        }

        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// executes the given query and returns the resultset as a json object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            DbConnection conn = context.Database.Connection;
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return dt;
        }
    }
}
