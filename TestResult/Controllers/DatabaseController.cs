using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        // GET: api/AppRuns
        private UnitTestLogEntities myEntity = new UnitTestLogEntities();
        public HttpResponseMessage Get()
        {
            return ToJson(myEntity.AppRuns.AsEnumerable());
        }

        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
