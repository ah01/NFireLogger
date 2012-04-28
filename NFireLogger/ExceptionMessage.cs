using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using NFireLogger.Utils;

namespace NFireLogger
{
    class ExceptionMessage : LogMessage
    {

        #region Internal properties

        /// <summary>
        /// Exception to be send info about
        /// </summary>
        [ScriptIgnore]
        public Exception Exception { get; set; }

        #endregion


        #region Public serializable parts

        /* 
            exc_info – array of 3 items (actually Python’s exc_info structure), 
                       the last is list of stack frames, 
                       n ote: again wrapped in py/tuple which is not needed
                       stackframe values: [ file, line, methodName, object ]
          
         
            exc_text – is not used, instead template is used, see again there 
                       is a hack with underscore, expected is string, 
                       but in case of exception underscore also does the work
         
            exc_frames – contains locals for all stack frames listed in exc_info
         */

        public string message
        {
            get { return "[{0}] {1}".FormatWith(Exception.GetType().Name, Exception.Message); }
        }


        public object[] exc_info 
        { 
            get
            {
                return new object[]
                           {
                               Exception.GetType().FullName, 
                               string.Empty, 
                               FormateStackTrace()
                           };
            } 
        }


        public string exception
        {
            get { return Exception.GetType().FullName; }
        }


        #endregion
        

        /// <summary>
        /// Formate StackTrace of exception for FireLogger
        /// </summary>
        /// <returns>array of stack frames</returns>
        private object[] FormateStackTrace()
        {
            var trace = new StackTrace(Exception, 0, true);
            var result = new object[trace.FrameCount];

            for (int i = 0; i < trace.FrameCount; i++)
            {
                var fr = trace.GetFrame(i);

                result[i] = new object[]
                                {
                                    // 1) file name
                                    fr.GetFileName(),

                                    // 2) line number
                                    fr.GetFileLineNumber(),

                                    // 3) method name
                                    fr.GetMethod().ToString(),

                                    // 4) object (instance of object)
                                    //    not supported in .net
                                    string.Empty
                                };
            }

            return result;
        }
    }
}
