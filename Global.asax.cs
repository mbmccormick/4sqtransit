using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using _4sqtransit.Common;

namespace _4sqtransit
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            LogApplicationError(Server.GetLastError());
            HttpContext.Current.ClearError();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        public static void LogApplicationError(Exception ex)
        {
            try
            {
                DatabaseDataContext db = new DatabaseDataContext();

                var r = new Common.Error
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    CreatedDate = DateTime.Now,
                };

                db.Errors.InsertOnSubmit(r);
                db.SubmitChanges();
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
}