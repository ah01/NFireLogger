using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NFireLogger.MvcDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            FLog.Current.Log(Level.Info, "test");
            FLog.Current.Log(Level.Warning, "Žluťoučký kůň úpěl dábelské ódy.");
            FLog.Current.Log(Level.Error, "Error");

            return View();
        }

    }
}
