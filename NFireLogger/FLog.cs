using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NFireLogger
{
    public static class FLog
    {
        private const string CONTEXT_KEY = "__FireLogger__";

        internal static bool IsActive
        {
            get { return HttpContext.Current.Items[CONTEXT_KEY] is FireLogger; }
        }

        public static FireLogger Current
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var flog = HttpContext.Current.Items[CONTEXT_KEY] as FireLogger;
                    if (flog == null)
                    {
                            flog = GetInstance();
                            HttpContext.Current.Items[CONTEXT_KEY] = flog;
                    }
                    return flog;
                }
                else
                {
                    // TODO better handler
                    throw new Exception("No HttpContext");
                }
            }
        }



        private static FireLogger GetInstance()
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            return new FireLogger(context);
        }

    }
}
