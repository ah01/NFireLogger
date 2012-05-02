using log4net.Appender;
using log4net.Core;
using L4N = log4net.Core;

namespace NFireLogger.Log4net
{
    /// <summary>
    /// Log4Net Appender for FireLogger
    /// </summary>
    class FireLoggerAppender : AppenderSkeleton
    {
        /// <summary>
        /// FireLogger for current HttpContext
        /// </summary>
        private FireLogger Logger { get { return FLog.Current; } }


        /// <summary>
        /// Append log message to FireLogger
        /// </summary>
        /// <param name="loggingEvent">log4net event</param>
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


        /// <summary>
        /// Add file and line of event to log message
        /// </summary>
        /// <param name="msg">log message to add info to</param>
        /// <param name="loggingEvent">log. event from log4net</param>
        private void PopulateFileInfo(LogMessage msg, LoggingEvent loggingEvent)
        {
            msg.PathName = loggingEvent.LocationInformation.FileName;
            msg.LineNo = ParseLineNumber(loggingEvent.LocationInformation.LineNumber);
        }


        /// <summary>
        /// Convert line number to long number (log4net retur string)
        /// </summary>
        private long ParseLineNumber(string lineNumber)
        {
            long n;
            return long.TryParse(lineNumber, out n) ? n : 0;
        }


        /// <summary>
        /// Convert Level from log4net to FireLogger level
        /// </summary>
        private static Level ConvertLog4NetLevels(L4N.Level level)
        {
            if (level >= L4N.Level.Fatal)
            {
                return Level.Critical;
            }
            else if (level == L4N.Level.Error)
            {
                return Level.Error;
            }
            else if (level == L4N.Level.Warn)
            {
                return Level.Warning;
            }
            else if (level == L4N.Level.Info)
            {
                return Level.Info;
            }
            else if (level >= L4N.Level.Debug)
            {
                return Level.Debug;
            }

            return Level.Info;
        }
    }
}
