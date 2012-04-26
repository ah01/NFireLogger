using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NFireLogger
{
    public class FireLoggerHttpModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.EndRequest += ApplicationOnEndRequest;
        }

        void ApplicationOnEndRequest(object sender, EventArgs e)
        {
            if (FLog.IsActive)
            {
                FLog.Current.SendAndClose();
            }
        }

        public void Dispose()
        {
        }
    }
}
