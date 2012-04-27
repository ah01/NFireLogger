using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFireLogger
{
    public interface ILogger
    {
        void Log(Level level, string message, params object[] parameters);
        
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Warning(string message, params object[] parameters);
        void Error(string message, params object[] parameters);
        void Critical(string message, params object[] parameters);
    }
}
