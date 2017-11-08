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
    public class DatabaseController : ApiController
    {
        private UnitTestLogEntities context = new UnitTestLogEntities();

        public HttpResponseMessage GetLatest()
        {
            new FileSystemController().checkDirectories();

            string sqlQuery =
                "select AppRunId, AppArea, BuildDate, ServerName, StartTime, EndTime, Alias, DbType, Version, " +
                "(select count(*) from TestSuiteRun s where s.AppRunId = r.AppRunId) as SuiteCount, " +
                "(select count(*) from TestCaseRun c where c.SuiteRunId in  " +
                "    (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                ") as CaseCount, " +
                "(select count(distinct(e.CaseRunId)) from TestError e where e.CaseRunId in " +
                "(select CaseRunId from TestCaseRun c where c.SuiteRunId in  " +
                "    (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                ")) " +
                "as ErrorCount " +
                "from ( " +
                "SELECT MAX(StartTime) OVER (partition by AppArea, DbType, Version) MaxStartTime, * " +
                "from AppRun) r where StartTime = MaxStartTime";
            return ExecuteQuery(sqlQuery);
        }

        public HttpResponseMessage GetAppArea(string id)
        {
            new FileSystemController().checkDirectories();

            string sqlQuery =
                "select AppRunId, AppArea, BuildDate, ServerName, StartTime, EndTime, Alias, DbType, Version, " +
                "(select count(*) from TestSuiteRun s where s.AppRunId = r.AppRunId) as SuiteCount, " +
                "(select count(*) from TestCaseRun c where c.SuiteRunId in  " +
                "    (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                ") as CaseCount, " +
                "(select count(distinct(e.CaseRunId)) from TestError e where e.CaseRunId in " +
                "(select CaseRunId from TestCaseRun c where c.SuiteRunId in  " +
                "    (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                ")) as ErrorCount " +
                "from dbo.AppRun r where lower(AppArea) = '" + id.ToLower() + "'";

            return ExecuteQuery(sqlQuery);
        }

        public HttpResponseMessage GetRunErrors(string id)
        {
            string sqlQuery =
                "select s.Name as SuiteName, c.Name as CaseName, c.Duration, e.Message from TestSuiteRun s " +
                "join TestCaseRun c on s.SuiteRunId = c.SuiteRunId " +
                "join TestError e on e.CaseRunId = c.CaseRunId " +
                "where AppRunId = '" + id + "'";

            return ExecuteQuery(sqlQuery);
        }

        /// <summary>
        /// executes the given query and returns the resultset as a json object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public HttpResponseMessage ExecuteQuery(string query)
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

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(dt), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
