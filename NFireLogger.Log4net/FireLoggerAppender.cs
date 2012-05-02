using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace NFireLogger.Log4net
{
    class FireLoggerAppender : AppenderSkeleton
    {

        private FireLogger Logger { get { return FLog.Current; } }



        protected override void Append(LoggingEvent loggingEvent)
        {
            var msg = new LogMessage()
                          {
                              Name = loggingEvent.LoggerName,
                              Message = RenderLoggingEvent(loggingEvent),
                              Level = ConvertLog4NetLevels(loggingEvent.Level),
                              Time = loggingEvent.TimeStamp
                          };

            PopulateFileInfo(msg, loggingEvent);

            Logger.Log(msg);    
        }

        private void PopulateFileInfo(LogMessage msg, LoggingEvent loggingEvent)
        {
            msg.PathName = loggingEvent.LocationInformation.FileName;
            msg.LineNo = ParseLineNumber(loggingEvent.LocationInformation.LineNumber);
        }

        private long ParseLineNumber(string lineNumber)
        {
            long n;
            return long.TryParse(lineNumber, out n) ? n : 0;
        }


        private static Level ConvertLog4NetLevels(log4net.Core.Level level)
        {
            if (level >= log4net.Core.Level.Fatal)
            {
                return Level.Critical;
            }
            else if (level == log4net.Core.Level.Error)
            {
                return Level.Error;
            }
            else if (level == log4net.Core.Level.Warn)
            {
                return Level.Warning;
            }
            else if (level == log4net.Core.Level.Info)
            {
                return Level.Info;
            }
            else if (level >= log4net.Core.Level.Debug)
            {
                return Level.Debug;
            }

            return Level.Info;
        }
    }
}
