using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestResult.Controllers
{
    public class AppAreaController : Controller
    {
        public ActionResult Index(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
