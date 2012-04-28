using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFireLogger
{
    partial class FireLogger : ILogger
    {
        public void Debug(string message, params object[] parameters)
        {
            Log(1, DEFAULT_NAME, Level.Debug, message, parameters);
        }

        public void Info(string message, params object[] parameters)
        {
            Log(1, DEFAULT_NAME, Level.Info, message, parameters);
        }

        public void Warning(string message, params object[] parameters)
        {
            Log(1, DEFAULT_NAME, Level.Warning, message, parameters);
        }

        public void Error(string message, params object[] parameters)
        {
            Log(1, DEFAULT_NAME, Level.Error, message, parameters);
        }

        public void Critical(string message, params object[] parameters)
        {
            Log(1, DEFAULT_NAME, Level.Critical, message, parameters);
        }

        public void Exception(Exception ex)
        {
            Exception(1, DEFAULT_NAME, Level.Critical, ex);
        }
    }
}
