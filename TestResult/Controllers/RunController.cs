using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestResult.Controllers
{
    public class RunController : Controller
    {
        //
        // GET: /Run/

        public ActionResult Index(string id)
        {
            ViewBag.RunId = id;
            return View();
        }
    }
}
