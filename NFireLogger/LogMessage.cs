using System;
using System.Web.Script.Serialization;
using NFireLogger.Utils;

namespace NFireLogger
{
    [Serializable]
    public class LogMessage
    {
        #region Internal properties

        public LogMessage()
        {
            Time = DateTime.Now;
        }

        /// <summary>
        /// Plain log message as it would be logged by your text logger
        /// </summary>
        [ScriptIgnore]
        public string Message { get; set; }

        /// <summary>
        /// array of arguments to be replaced in template
        /// </summary>
        [ScriptIgnore]
        public object[] Arguments { get; set; }

        /// <summary>
        /// Debug level, one of debug,info,warning,error,exception
        /// </summary>
        [ScriptIgnore]
        public Level Level { get; set; }

        /// <summary>
        /// Time to be displayed with log record
        /// </summary>
        [ScriptIgnore]
        public DateTime Time { get; set; }

        /// <summary>
        /// Logger name – see green bubbles on the right of each log record
        /// </summary>
        [ScriptIgnore]
        public string Name { get; set; }

        /// <summary>
        /// Log location of source file
        /// </summary>
        [ScriptIgnore]
        public string PathName { get; set; }

        /// <summary>
        /// Log line location in source file
        /// </summary>
        [ScriptIgnore]
        public long LineNo { get; set; }

        #endregion
        
        #region Public serializable parts 

        /*
            message   – plain log message as it would be logged by your text logger
            template  – template version of message, arguments should be marked as %X, 
                        where FireLogger doesn’t care about X
            args      – array of arguments to be replaced in template. it is your hard 
                        work to provide FireLogger with detailed representation of 
                        arguments as structured data so user can drill it down in the 
                        Watches window. n ote: in current example, there is “py/tuple” 
                        structure wrapping actual array. This is specific to Python 
                        jsonpickle library, FireLogger can unwrap it, but you should 
                        send plain array in this case.
            level     – debug level, one of debug,info,warning,error,exception
            timestamp – unix timestamp of log record (sorting)
            time      – user friendly time to be displayed with log record
            name      – logger name – see green bubbles on the right of each log record
            pathname, lineno – log line location in source file
         
            see https://github.com/darwin/firelogger/wiki
        */

        /*
            other files from PHP implementation:
                order
        */

        // TODO add support for template type of logs

        public string message { get { return Message.FormatWith(Arguments); } }

        // if template is not null it has priority over message (icording to PHP implementation)

        // public string template { get { return Message.FormatWith(Arguments); } }
        
        // public object[] args { get { return Arguments; } }

        public string level { get { return Level.ToString().ToLower(); } }

        public long timestamp { get { return Time.ToTimestamp(); } }

        public string time { get { return Time.ToShortTimeString(); } }

        public string name { get { return Name; } }

        public string pathname { get { return PathName; } }

        public long lineno { get { return LineNo; } }

        #endregion
    }
}
