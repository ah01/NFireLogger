using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace NFireLogger.MvcDemo.Controllers
{
    public class HomeController : Controller
    {
        private static ILogger logger = FLog.GetLogger("DemoController");

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            logger.Info("Action '{0}' executing.", filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            logger.Info("Action executed.");
            base.OnResultExecuted(filterContext);
        }

        public ActionResult Index()
        {
            FLog.Info("Info message");

            return View();
        }

        private void DoSomething()
        {
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
            throw new NotImplementedException("This method can do event nothing.");
        }


        
        public ActionResult Basic()
        {
            FLog.Debug("DEBUG log level");
            FLog.Info("INFO log level");
            FLog.Error("ERROR log level");
            FLog.Warning("WARNING level");
            FLog.Critical("CRITICAL log level");

            FLog.Info("Request from {0} arrived at {1}", Request.UserHostAddress, DateTime.Now);

            return Content("ok", "text/plain");
        }


        public ActionResult Named()
        {
            FLog.Info("Named logger examle");

            var logger = FLog.Current.GetLogger("MyLogger");

            logger.Debug("This is logged as MyLogger");
            logger.Warning("... and also this.");

            return Content("ok", "text/plain");
        }


        public ActionResult Exception()
        {
            FLog.Info("Exception log examle");

            try
            {
                DoSomething();
            }
            catch (Exception ex)
            {
                FLog.Current.Exception(ex);
            }

            return Content("ok", "text/plain");
        }


        public ActionResult AllInOne()
        {
            FLog.Debug("DEBUG log level");
            FLog.Info("INFO log level");
            FLog.Error("ERROR log level");
            FLog.Warning("WARNING level");
            FLog.Critical("CRITICAL log level");


            FLog.Info("Text: {0}", "Lorem Ipsum");


            FLog.Current.Log(Level.Debug, "DEBUG log level");
            FLog.Current.Log(Level.Info, "INFO log level");
            FLog.Current.Log(Level.Error, "ERROR log level");
            FLog.Current.Log(Level.Warning, "WARNING level");
            FLog.Current.Log(Level.Critical, "CRITICAL log level");


            FLog.Current.Debug("DEBUG log level");
            FLog.Current.Info("INFO log level");
            FLog.Current.Error("ERROR log level");
            FLog.Current.Warning("WARNING level");
            FLog.Current.Critical("CRITICAL log level");
            

            FLog.Current.Log(Level.Info, "Žluťoučký kůň úpěl dábelské ódy.");


            var log = FLog.Current.GetLogger("Test");

            log.Debug("DEBUG log level");
            log.Info("INFO log level");
            log.Error("ERROR log level");
            log.Warning("WARNING level");
            log.Critical("CRITICAL log level");

            
            try
            {
                DoSomething();
            }
            catch (Exception ex)
            {
                FLog.Current.Exception(ex);
            }

            return Content("ok");
        }


        public ActionResult Log4net()
        {
            var l = LogManager.GetLogger("log4net");

            l.Info("Info message from log4net");
            l.DebugFormat("and debug message at {0}", DateTime.Now.ToShortTimeString());
            
            return Content("ok");
        }

    }
}
