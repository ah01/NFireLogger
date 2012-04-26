using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using NFireLogger.Utils;

namespace NFireLogger
{
    public class FireLogger
    {
        private HttpRequestBase Request { get; set; }
        private HttpResponseBase Response { get; set; }

        public bool Enabled { get; set; }

        private bool _responseClosed;

        private bool InternalEnabled
        {
            get { return Enabled && !_responseClosed; }
        }

        private volatile List<LogMessage> _logs = null;

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

        public FireLogger(HttpContextBase httpContext)
        {
            if (httpContext == null) throw new ArgumentException("HttpContext has to be");

            Request = httpContext.Request;
            Response = httpContext.Response;

            AutodetectState();
        }

        private void AutodetectState()
        {
            if (Request.Headers["X-Firelogger"] != null)
            {
                Enabled = true;
            }
            // TODO add version checker
            // TODO add password protection
        }


        public void Log(Level level, string text, params object[] p)
        {
            var msg = new LogMessage
                          {
                              Level = level,
                              Message = text
                          };

            Log(msg);
        }

        public void Log(LogMessage msg)
        {
            if (InternalEnabled)
            {
                lock (this)
                {
                    Logs.Add(msg);
                }
            }
        }


        public void Send()
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

        internal void SendAndClose()
        {
            if (Enabled)
            {
                _responseClosed = true;
                Send();
            }

        }

        private void AddHeadersToResponse(object data)
        {
            var chunks = HttpHeaderEncoder.EncodeObjectIntoChunks(data);
            var id = new Random().Next(16, int.MaxValue);

            for (long n = 0; n < chunks.Length; n++)
            {
                Response.AddHeader("FireLogger-{0:X}-{1}".FormatWith(id, n), chunks[n]);
            }
        }


    }
}
