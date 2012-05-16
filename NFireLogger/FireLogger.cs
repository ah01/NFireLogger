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
    /// <summary>
    /// Handle all logging stuff for FireLogger extension
    /// </summary>
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
        /// Password protection
        /// (if set, request has to contains valid token)
        /// </summary>
        public string Password { get; private set; }


        /// <summary>
        /// If true, file name and line number is logged
        /// </summary>
        public bool LogFileInfo { get; set; }


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
        /// Initializes a new instance of the FireLogger class.
        /// </summary>
        /// <param name="httpContext">HttpContext to be used for logs.</param>
        public FireLogger(HttpContextBase httpContext) : this(httpContext, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the FireLogger class with password protection.
        /// </summary>
        /// <param name="httpContext">Client password</param>
        /// <param name="password">HttpContext to be used for logs.</param>
        public FireLogger(HttpContextBase httpContext, string password)
        {
            if (httpContext == null)
            {
                InternalError("Missing HttpContext");
                return;
            }

            HttpContext = httpContext;
            Password = password;
            Silent = true;
            LogFileInfo = true;

            AutodetectState();
        }


        #region Http Request Processing


        /// <summary>
        /// Detect enable state from HTTP request
        /// </summary>
        private void AutodetectState()
        {
            if (Request == null)
            {
                InternalError("Request not available");
                return;
            }

            if (Request.Headers["X-Firelogger"] != null)
            {
                Enabled = IsRequestAuthenticated();
                CheckVersion();
            }
        }


        /// <summary>
        /// Evaluate auth. token according to Password (if none return true)
        /// </summary>
        /// <returns>enable state</returns>
        private bool IsRequestAuthenticated()
        {
            if (string.IsNullOrEmpty(Password))
            {
                return true; // no protection
            }

            var clientToken = Request.Headers["X-FireLoggerAuth"];

            if (clientToken == null)
            {
                return false; // no token
            }

            var myToken = "#FireLoggerPassword#{0}#".FormatWith(Password).ToMd5();

            var res = clientToken == myToken;

            if (!res)
            {
                InternalError("FireLogger password do not match. Have you specified correct password FireLogger extension?");
            }

            return res;
        }


        /// <summary>
        /// Check FireLogger client version number and if it's not sufficient print warning
        /// </summary>
        private void CheckVersion()
        {
            Version version;
            var requiredVersion = new Version(MINIMAL_VERSION);

            if (Version.TryParse(Request.Headers["X-Firelogger"], out version))
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


        #endregion


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
        /// Add new message to output
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="text">text of log message</param>
        /// <param name="p">parameters</param>
        public void Log(Level level, string text, params object[] p)
        {
            if (InternalEnabled)
            {
                Log(1, DEFAULT_NAME, level, text, p);
            }
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
            if (!InternalEnabled) return;
            
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
        /// Log exception
        /// </summary>
        /// <param name="stackTraceOffset">Offset of caller in stacktrace (to skip record from FireLogger)</param>
        /// <param name="name">Logger name</param>
        /// <param name="level">Log level</param>
        /// <param name="ex">Exception to be logged</param>
        internal void Exception(int stackTraceOffset, string name, Level level, Exception ex)
        {
            if (!InternalEnabled) return;

            var msg = new ExceptionMessage
                          {
                              Level = level,
                              Name = name,
                              Exception = ex
                          };

            PopulateStackInfo(msg, stackTraceOffset + 1);

            Log(msg);
        }



        /// <summary>
        /// Return named logger for this instance
        /// </summary>
        /// <param name="name">name of logger</param>
        /// <returns>Named Logger conected with this instance of FireLogger</returns>
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
                try
                {
                    lock (this)
                    {
                        var packet = new LogPacket(Logs);
                        AddHeadersToResponse(packet);
                        Logs.Clear();
                    }
                }
                catch (Exception ex)
                {
                    InternalError("Error during serializing to Http Response: [{0}] {1}"
                        .FormatWith(ex.GetType().FullName, ex.Message));
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

            if (!LogFileInfo) return;

            var frame = new StackFrame(stackTraceOffset + 1, true);

            // NOTE when PDB files are missing (e.g. production servers) result values is null, consider some info values
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
            if (HttpContext == null || Response == null)
            {
                InternalError("HttpContext or Response is null");
                return;
            }

            var chunks = HttpHeaderEncoder.EncodeObjectIntoChunks(data);
            var id = new Random().Next(16, int.MaxValue);

            for (long n = 0; n < chunks.Length; n++)
            {
                Response.AddHeader("FireLogger-{0:X}-{1}".FormatWith(id, n), chunks[n]);
            }
        }


        /// <summary>
        /// Current Request or null
        /// </summary>
        private HttpRequestBase Request
        {
            get
            {
                try
                {
                    return HttpContext.Request;
                }
                catch (HttpException)
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Current Response or null
        /// </summary>
        private HttpResponseBase Response
        {
            get
            {
                try
                {
                    return HttpContext.Response;
                }
                catch (HttpException)
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Internal error logging
        /// </summary>
        /// <param name="reason">error message</param>
        [Conditional("TRACE")]
        private void InternalError(string reason)
        {
            if (!Silent)
            {
                Trace.TraceError(reason);
            }
        }
    }
}
