﻿using System;
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
            FLog.Current.Log(Level.Debug   , "DEBUG log level");
            FLog.Current.Log(Level.Info    , "INFO log level");
            FLog.Current.Log(Level.Error   , "ERROR log level");
            FLog.Current.Log(Level.Warning , "WARNING level");
            FLog.Current.Log(Level.Critical, "CRITICAL log level");


            /*
            FLog.Current.Debug("DEBUG log level");
            FLog.Current.Info("INFO log level");
            FLog.Current.Error("ERROR log level");
            FLog.Current.Warning("WARNING level");
            FLog.Current.Critical("CRITICAL log level");
            */

            FLog.Debug("DEBUG log level");
            FLog.Info("INFO log level");
            FLog.Error("ERROR log level");
            FLog.Warning("WARNING level");
            FLog.Critical("CRITICAL log level");

            FLog.Current.Log(Level.Info, "Žluťoučký kůň úpěl dábelské ódy.");

            var log = FLog.Current.GetLogger("Test");

            log.Debug("DEBUG log level");
            log.Info("INFO log level");
            log.Error("ERROR log level");
            log.Warning("WARNING level");
            log.Critical("CRITICAL log level");


            FLog.Info("Text: {0}", "Lorem Ipsum");


            FLog.Info("HttpContext", HttpContext.Request.Headers);

            //DoSomethingWrong();

            try
            {
                DoSomethingWrong();
            }
            catch (Exception ex)
            {
                FLog.Current.Exception(ex);
            }



            return View();
        }

        private void DoSomethingWrong()
        {
            throw new ArgumentException("invalid argument", "argName");
        }

    }
}
