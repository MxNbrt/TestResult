using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Common;
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

        [HttpGet]
        public HttpResponseMessage DeleteRun(string id)
        {
            try
            {
                AppRun run = context.AppRuns.Find(Convert.ToInt32(id));
                if (run == null)
                    throw new EntitySqlException("Der Testlauf mit der Id '" + id + "' wurde nicht gefunden.");

                context.AppRuns.Remove(run);
                context.SaveChanges();

                return CreateResponse("{\"success\": true, \"message\": \"\"}");
            }
            catch (Exception ex)
            {
                return CreateResponse("{\"success\": false, \"message\": \"" + ex.GetBaseException().Message + "\"}");
            }
        }

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
                "SELECT MAX(AppRunId) OVER (partition by AppArea, DbType, Version) MaxAppRunId, * " +
                "from AppRun) r where AppRunId = MaxAppRunId";
            return CreateResponse(ExecuteQuery(sqlQuery));
        }

        public HttpResponseMessage GetAppArea(string id)
        {
            new FileSystemController().checkDirectories();

            string sqlQuery =
                "with TempTable as ( " +
                "    select AppRunId, AppArea, ServerName, Alias, StartTime, EndTime, DbType, Version,  " +
                "    (select count(distinct(e.CaseRunId)) from TestError e where e.CaseRunId in  " +
                "    (select CaseRunId from TestCaseRun c where c.SuiteRunId in   " +
                "        (select SuiteRunId from TestSuiteRun s where s.AppRunId = r.AppRunId) " +
                "    )) as ErrorCount " +
                "    from dbo.AppRun r where lower(AppArea) = '" + id.ToLower() + "' " +
                ") " +

                "select AppRunId, AppArea, ServerName, Alias, StartTime, EndTime,   " +
                "MSSQL55 = case when DbType = 'MSSQL' and Version = '5.5' then ErrorCount else null end,  " +
                "MSSQL54 = case when DbType = 'MSSQL' and Version = '5.4' then ErrorCount else null end,  " +
                "ORACLE55 = case when DbType = 'ORACLE' and Version = '5.5' then ErrorCount else null end, " +
                "ORACLE54 = case when DbType = 'ORACLE' and Version = '5.4' then ErrorCount else null end " +
                "from TempTable";

            return CreateResponse(ExecuteQuery(sqlQuery));
        }

        public HttpResponseMessage GetRunErrors(string id)
        {
            string errors = ExecuteQuery("select s.Name as SuiteName, c.Name as CaseName, c.Duration, e.Message from TestSuiteRun s " +
                "join TestCaseRun c on s.SuiteRunId = c.SuiteRunId " +
                "join TestError e on e.CaseRunId = c.CaseRunId " +
                "where AppRunId = '" + id + "'");
            string appRun = ExecuteQuery("select AppRunId, AppArea + ' ' + Version as AppArea, BuildDate, ServerName, Alias, DbType, StartTime, EndTime, " +
            "convert(varchar(30), (datediff(mi, StartTime, EndTime) / 60)) + ':' + RIGHT('0' + convert(varchar(30), (datediff(mi, StartTime, EndTime) % 60)), 2)" +
            "as Duration from AppRun where AppRunId='" + id + "'");

            string response = appRun.Remove(appRun.Length - 2) + ",\"Errors\":" + errors + "}]";
            return CreateResponse(response);
        }

        /// <summary>
        /// executes the given query and returns the resultset as a json object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string ExecuteQuery(string query)
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

            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// creates a new http response with json content
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public HttpResponseMessage CreateResponse(string jsonObject)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
