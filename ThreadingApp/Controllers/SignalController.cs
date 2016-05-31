using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOLIDApp.Controllers
{
    public class SignalController : Controller
    {
        public ActionResult Chat() {
            return View();
        }

        public ActionResult ProgressBar() {
            return View();
        }
    }
}