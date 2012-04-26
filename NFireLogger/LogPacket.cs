using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace NFireLogger
{
    [Serializable]
    internal class LogPacket
    {
        #region Internal properties

        public LogPacket(List<LogMessage> messages)
        {
            Messages = messages;
        }


        /// <summary>
        /// List of all logged messages
        /// </summary>
        [ScriptIgnore]
        public List<LogMessage> Messages { get; set; }

        #endregion

        #region Public serializable parts

        /*
            errors – array of internal exceptions (maybe you don’t want to use this at all)
            logs – array of actual log records
        */

        public List<LogMessage> logs { get { return Messages; } } 

        #endregion
    }
}
