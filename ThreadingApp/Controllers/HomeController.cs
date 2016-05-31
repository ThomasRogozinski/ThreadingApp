using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOLIDApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Sample Multithreading Web Application.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Details:";

            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
    }
}