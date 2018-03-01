﻿using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TestResult.Models;
using System.Data.Entity;
using System.Linq;

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
                int intId = Convert.ToInt32(id);
                var run = context.AppRuns.Where(q => q.AppRunId == intId).First();
                if (run == null)
                    throw new EntitySqlException("Der Testlauf mit der Id '" + id + "' wurde nicht gefunden.");

                string sqlQuery =
                    "delete from TestError where CaseRunId in (" +
                    "	select CaseRunId from TestCaseRun where SuiteRunId in (" +
                    "		select SuiteRunId from TestSuiteRun where AppRunId = '" + id + "'" +
                    "	)" +
                    ")";
                ExecuteQuery(sqlQuery);

                sqlQuery =
                    "delete from TestCaseRun where SuiteRunId in (" +
                    "   select SuiteRunId from TestSuiteRun where AppRunId = '" + id + "'" +
                    ")";
                ExecuteQuery(sqlQuery);

                sqlQuery = "delete from TestSuiteRun where AppRunId = '" + id + "'";
                ExecuteQuery(sqlQuery);

                sqlQuery = "delete from AppRun where AppRunId = '" + id + "'";
                ExecuteQuery(sqlQuery);

                return CreateSuccessJson();
            }
            catch (Exception ex)
            {
                return CreateSuccessJson(false, ex.GetBaseException().Message.Replace(Environment.NewLine, ""));
            }
        }

        public HttpResponseMessage CreateSuccessJson(bool success = true, string msg = null)
        {
            return CreateResponse("{\"success\": " + success.ToString().ToLower() + ", \"message\": \"" + msg + "\"}");
        }

        [HttpGet]
        public HttpResponseMessage RefreshData()
        {
            try
            {
                new FileSystemController().checkDirectories();
                return CreateSuccessJson();
            }
            catch (Exception ex)
            {
                return CreateSuccessJson(false, ex.GetBaseException().Message.Replace(Environment.NewLine, ""));
            }
        }

        public HttpResponseMessage GetLatest()
        {
            string sqlQuery =
                "select r.AppRunId, AppArea, BuildDate, ServerName, StartTime, EndTime, Alias, DbType, Version, " +
                "count(distinct(s.SuiteRunId)) as SuiteCount, " +
                "count(distinct(c.CaseRunId)) as CaseCount, " +
                "count(distinct(e.CaseRunId)) as ErrorCount " +
                
                "from AppRun r " +
                "join TestSuiteRun s on s.AppRunId = r.AppRunId " +
                "join TestCaseRun c on c.SuiteRunId = s.SuiteRunId " +
                "left join TestError e on e.CaseRunId = c.CaseRunId " +
                
                "where r.AppRunId = ( " +
                "	SELECT TOP 1 AppRunId " +
                "	from UnitTestLog.dbo.AppRun " +
                "	where AppArea = r.AppArea and DbType = r.DbType and Version = r.Version " +
                "	order by StartTime desc " +
                ") " +
                
                "group by r.AppRunId, AppArea, BuildDate, ServerName, StartTime, EndTime, Alias, DbType, Version";

            return CreateResponse(ExecuteQuery(sqlQuery));
        }

        public HttpResponseMessage GetAppArea(string id)
        {
            string sqlQuery =
                "with TempTable as (" +
                "   select r.AppRunId, r.AppArea, r.ServerName, r.Alias, r.StartTime, r.EndTime, r.DbType, r.Version, " +
                "   count(distinct(e.CaseRunId)) as ErrorCount" +
                "   from AppRun r " +
                "   join TestSuiteRun s on s.AppRunId = r.AppRunId" +
                "   join TestCaseRun c on c.SuiteRunId = s.SuiteRunId" +
                "   left join TestError e on e.CaseRunId = c.CaseRunId" +
                "   where lower(r.AppArea) = '" + id.ToLower() + "' " +
                "   group by r.AppRunId, r.AppArea, r.ServerName, r.Alias, r.StartTime, r.EndTime, r.DbType, r.Version" +
                ") " +
                
                "select AppRunId, AppArea, ServerName, Alias, StartTime, EndTime, " +
                "MSSQL55 = case when DbType = 'MSSQL' and Version = '5.5' then ErrorCount else null end, " +
                "MSSQL54 = case when DbType = 'MSSQL' and Version = '5.4' then ErrorCount else null end, " +
                "ORACLE55 = case when DbType = 'ORACLE' and Version = '5.5' then ErrorCount else null end, " +
                "ORACLE54 = case when DbType = 'ORACLE' and Version = '5.4' then ErrorCount else null end " +
                
                "from TempTable " +
                "order by StartTime asc";

            return CreateResponse(ExecuteQuery(sqlQuery));
        }

        public HttpResponseMessage GetRunErrors(string id)
        {
            string errors = ExecuteQuery("select s.SuiteRunId as SuiteId, s.Name as SuiteName, c.CaseRunId as CaseId, c.Name as CaseName, " +
                "c.Duration, e.Message from TestSuiteRun s " +
                "join TestCaseRun c on s.SuiteRunId = c.SuiteRunId " +
                "join TestError e on e.CaseRunId = c.CaseRunId " +
                "where AppRunId = '" + id + "'" +
                "order by s.SuiteRunId asc, c.CaseRunId asc");
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
