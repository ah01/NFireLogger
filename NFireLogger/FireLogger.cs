using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using NFireLogger.Utils;

namespace NFireLogger
{
    public partial class FireLogger : ILogger
    {
        
        /// <summary>
        /// Enable/Disable logging 
        /// (it's automaticaly set according to HTTP headers in request)
        /// </summary>
        public bool Enabled { get; set; }


        /// <summary>
        /// If true, all internal errors of FireLogger is throw away
        /// (otherwise it's logger by standart Trace technique)
        /// </summary>
        public bool Silent { get; set; }


        /// <summary>
        /// Name of logger when none is provided
        /// </summary>
        public const string DEFAULT_NAME = "ROOT";


        /// <summary>
        /// Minimal version of FireLogger client
        /// </summary>
        public const string MINIMAL_VERSION = "1.2";

        /// <summary>
        /// Current HttpContext
        /// </summary>
        private HttpContextBase HttpContext { get; set; }


        private bool _responseClosed = false;


        /// <summary>
        /// Enabled with respect to closenes of response
        /// </summary>
        private bool InternalEnabled
        {
            get { return Enabled && !_responseClosed; }
        }


        private volatile List<LogMessage> _logs = null;
        

        /// <summary>
        /// List of individual messages
        /// </summary>
        private List<LogMessage> Logs
        {
            get
            {
                if (_logs == null)
                {
                    lock (this)
                    {
                        if (_logs == null)
                        {
                            _logs = new List<LogMessage>();
                        }
                    }
                    
                }
                return _logs;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        public FireLogger(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                InternalError("Missing HttpContext");
                return;
            }

            HttpContext = httpContext;

            AutodetectState();
        }


        /// <summary>
        /// Detect enable state from HTTP request
        /// </summary>
        private void AutodetectState()
        {
            // TODO check missing HttpContext

            if (HttpContext.Request.Headers["X-Firelogger"] != null)
            {
                Enabled = true;

                CheckVersion();
            }

            // TODO add password protection
        }


        /// <summary>
        /// Check FireLogger client version number and if it's not sufficient print warning
        /// </summary>
        private void CheckVersion()
        {
            Version version;
            var requiredVersion = new Version(MINIMAL_VERSION);

            if (Version.TryParse(HttpContext.Request.Headers["X-Firelogger"], out version))
            { 
                if (version < requiredVersion)
                {
                    Log(-10, "NFireLogger", Level.Warning,
                        "Your FireLogger version {0} is older then required {1}!".FormatWith(version, requiredVersion));
                }
            }
            else
            {
                Log(-10, "NFireLogger", Level.Error,
                        "Cannot determine your FireLogger version!".FormatWith(version, requiredVersion));
            }
        }


        /// <summary>
        /// Add new message to output
        /// </summary>
        /// <param name="message"></param>
        public virtual void Log(LogMessage message)
        {
            if (InternalEnabled && message != null)
            {
                lock (this)
                {
                    Logs.Add(message);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="text"></param>
        /// <param name="p"></param>
        public void Log(Level level, string text, params object[] p)
        {
            Log(1, DEFAULT_NAME, level, text, p);
        }


        /// <summary>
        /// Add new message to output with info from stacktrace (source file name and line of caller)
        /// </summary>
        /// <param name="stackTraceOffset">Offset of caller in stacktrace (to skip record from FireLogger)</param>
        /// <param name="name">Logger name</param>
        /// <param name="level">Log level</param>
        /// <param name="text">Text of message</param>
        /// <param name="parameters">Parameters of message</param>
        internal void Log(int stackTraceOffset, string name, Level level, string text, params object[] parameters)
        {
            var msg = new LogMessage
                          {
                              Level     = level,
                              Message   = text,
                              Arguments = parameters,
                              Name      = name
                          };

            PopulateStackInfo(msg, stackTraceOffset + 1);

            Log(msg);
        }


        /// <summary>
        /// Return named logger 
        /// </summary>
        /// <param name="name">name of logger</param>
        /// <returns></returns>
        public ILogger GetLogger(string name)
        {
            return new NamedLogger(name, this);
        }


        /// <summary>
        /// Flush all logs until now to HTTP Response
        /// </summary>
        public void Flush()
        {
            if (Enabled)
            {
                lock (this)
                {
                    var logs = new LogPacket(Logs);
                    AddHeadersToResponse(logs);
                    Logs.Clear();
                }
            }
        }


        /// <summary>
        /// Flush all logs and disable next logging
        /// </summary>
        public void Close()
        {
            if (Enabled)
            {
                _responseClosed = true;
                Flush();
            }

        }


        /// <summary>
        /// Add info about source file name and line from stacktrace
        /// </summary>
        /// <param name="msg">Log Message</param>
        /// <param name="stackTraceOffset">Offset of caller in stacktrace (to skip record from FireLogger)</param>
        private void PopulateStackInfo(LogMessage msg, int stackTraceOffset)
        {
            if (stackTraceOffset < 0) return;

            var frame = new StackFrame(stackTraceOffset + 1, true);

            // TODO add option to skip this feature
            // TODO check what happen when PDB file is missing (values should be null)
            // TODO do something with path (it's absolute)

            msg.PathName = frame.GetFileName();
            msg.LineNo   = frame.GetFileLineNumber();
        }


        /// <summary>
        /// Process and add logs to HTTP response
        /// </summary>
        /// <param name="data">POCO object to be send</param>
        private void AddHeadersToResponse(object data)
        {
            if (HttpContext == null || HttpContext.Response == null)
            {
                InternalError("HttpContext or Response is null");
                return;
            }

            var chunks = HttpHeaderEncoder.EncodeObjectIntoChunks(data);
            var id = new Random().Next(16, int.MaxValue);

            for (long n = 0; n < chunks.Length; n++)
            {
                HttpContext.Response.AddHeader("FireLogger-{0:X}-{1}".FormatWith(id, n), chunks[n]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        private void InternalError(string reason)
        {
            if (!Silent)
            {
                Trace.TraceError(reason);
            }
        }
    }
}
