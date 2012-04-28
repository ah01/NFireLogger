using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace NFireLogger
{
    public static class FLog
    {
        private const string CONTEXT_KEY = "__NFireLogger__";


        /// <summary>
        /// Is in Http Context some FireLogger already
        /// </summary>
        internal static bool IsActive
        {
            get { return IsWeb && HttpContext.Current.Items[CONTEXT_KEY] is FireLogger; }
        }


        /// <summary>
        /// Is this a web app (is HttpContext available)
        /// </summary>
        private static bool IsWeb
        {
            get { return HttpContext.Current != null; }
        }


        /// <summary>
        /// FireLogger instance for current Http Context
        /// </summary>
        public static FireLogger Current
        {
            get
            {
                if (!IsWeb)
                {
                    return null;
                }
                
                var flog = HttpContext.Current.Items[CONTEXT_KEY] as FireLogger;
                if (flog == null)
                {
                    flog = CreateFireLogger();
                    HttpContext.Current.Items[CONTEXT_KEY] = flog;
                }
                return flog;
            }
        }


        /// <summary>
        /// Create new FireLogger instance and setup it according to web.config
        /// </summary>
        /// <returns>New FireLogger instance</returns>
        private static FireLogger CreateFireLogger()
        {
            string password;
            bool silent;

            LoadConfig(out silent, out password);

            var context = new HttpContextWrapper(HttpContext.Current);
            var logger = new FireLogger(context, password);

            logger.Silent = silent;

            return logger;
        }


        /// <summary>
        /// Load configuration from appSettings section in web.config
        /// </summary>
        /// <param name="silent">value of NFireLogger.Silent (true/false)</param>
        /// <param name="password">value of NFireLogger.Password (string)</param>
        private static void LoadConfig(out bool silent, out string password)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;

                password = appSettings["NFireLogger.Password"];

                if (!bool.TryParse(appSettings["NFireLogger.Silent"], out silent))
                {
                    silent = true;
                }
            }
            catch (ConfigurationErrorsException)
            {
                // default values
                password = null;
                silent = true;
            }
        }

        #region Static Logging Helpers


        public static void Log(string name, Level level, string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, name, level, message, parameters);
        }


        public static void Log(Level level, string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, FireLogger.DEFAULT_NAME, level, message, parameters);
        }


        public static void Debug(string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, FireLogger.DEFAULT_NAME, Level.Debug, message, parameters);
        }


        public static void Info(string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, FireLogger.DEFAULT_NAME, Level.Info, message, parameters);
        }


        public static void Warning(string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, FireLogger.DEFAULT_NAME, Level.Warning, message, parameters);
        }


        public static void Error(string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, FireLogger.DEFAULT_NAME, Level.Warning, message, parameters);
        }


        public static void Critical(string message, params object[] parameters)
        {
            if (IsWeb) Current.Log(1, FireLogger.DEFAULT_NAME, Level.Critical, message, parameters);
        }

        public static void Exception(Exception ex)
        {
            if (IsWeb) Current.Exception(1, FireLogger.DEFAULT_NAME, Level.Critical, ex);
        }

        public static void Exception(string name, Exception ex)
        {
            if (IsWeb) Current.Exception(1, name, Level.Critical, ex);
        }

        #endregion


    }
}
