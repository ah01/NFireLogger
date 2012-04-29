using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFireLogger
{
    /// <summary>
    /// Named logger helper class
    /// </summary>
    public class NamedLogger : ILogger
    {
        /// <summary>
        /// Logger name
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// FireLogger instance for logging into
        /// </summary>
        public FireLogger Logger { get { return _loggetCallback(); } }

        private Func<FireLogger> _loggetCallback;
        
        public NamedLogger(string name, FireLogger logger)
        {
            Name = name;
            _loggetCallback = () => logger;
        }


        public NamedLogger(string name, Func<FireLogger> loggerCallback)
        {
            Name = name;
            _loggetCallback = loggerCallback;
        }

        
        public void Log(Level level, string message, params object[] parameters)
        {
            Logger.Log(1, Name, level, message, parameters);
        }


        public void Debug(string message, params object[] parameters)
        {
            Logger.Log(1, Name, Level.Debug, message, parameters);
        }


        public void Info(string message, params object[] parameters)
        {
            Logger.Log(1, Name, Level.Info, message, parameters);
        }


        public void Warning(string message, params object[] parameters)
        {
            Logger.Log(1, Name, Level.Warning, message, parameters);
        }


        public void Error(string message, params object[] parameters)
        {
            Logger.Log(1, Name, Level.Error, message, parameters);
        }


        public void Critical(string message, params object[] parameters)
        {
            Logger.Log(1, Name, Level.Critical, message, parameters);
        }


        public void Exception(Exception ex)
        {
            Logger.Exception(1, Name, Level.Critical, ex);
        }
    }
}
