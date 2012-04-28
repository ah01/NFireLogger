using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NFireLogger
{
    /// <summary>
    /// HttpModule for Close (=> send logs) FireLogger instance (via FLog class) at the end of request. 
    /// </summary>
    public class FireLoggerHttpModule : IHttpModule
    {
        /// <summary>
        /// Module initialization
        /// </summary>
        public void Init(HttpApplication application)
        {
            application.EndRequest += ApplicationOnEndRequest;
        }


        /// <summary>
        /// Close FireLogger at the end of request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationOnEndRequest(object sender, EventArgs e)
        {
            if (FLog.IsActive)
            {
                FLog.Current.Close();
            }
        }


        public void Dispose()
        {
            // nothing to dispouse
        }
    }
}
