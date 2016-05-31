using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOLIDApp.Controllers
{
    public class ThreadsController : Controller
    {
        // GET: Threads
        public ActionResult Index() {
            return View();
        }

        public ActionResult ThreadCreate() {
            return View();
        }

        public ActionResult Monitor() {
            return View();
        }

        public ActionResult ThreadPool() {
            return View();
        }

        public ActionResult Mutex() {
            return View();
        }
    }
}