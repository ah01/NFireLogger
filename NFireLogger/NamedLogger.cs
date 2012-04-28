using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFireLogger
{
    public class NamedLogger : ILogger
    {
        public string Name { get; private set; }

        public FireLogger Logger { get; private set; }

        public NamedLogger(string name, FireLogger logger)
        {
            Name = name;
            Logger = logger;
        }

        public NamedLogger(string name)
        {
            Name = name;
            Logger = FLog.Current;
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
