using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;

namespace _4sqtransit.Common
{
    public partial class Inbound : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["Body"].ToLower().Trim() == "update")
            {
                DatabaseDataContext db = new DatabaseDataContext();
                _4sqtransit.Common.User u = db.Users.Single(r => r.PhoneNumber == Request["From"].Replace("+1", "").Replace(" 1", "").Trim());

                try
                {
                    Push.SendTextMessageNotifications(u.FoursquareUserID, true);
                }
                catch (Exception ex)
                {
                   
                    
                    var account = new TwilioRest.Account(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                    var url = string.Format("/2010-04-01/Accounts/{0}/SMS/Messages", ConfigurationManager.AppSettings["TwilioAccountSid"]);

                    var values = new Hashtable();

                    values.Add("To", Request["From"]);
                    values.Add("From", ConfigurationManager.AppSettings["TwilioNumber"]);
                    values.Add("Body", "Oops! An error has occurred, please try again later.");

                    account.request(url, "POST", values);
                }
            }
            else
            {
                var account = new TwilioRest.Account(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var url = string.Format("/2010-04-01/Accounts/{0}/SMS/Messages", ConfigurationManager.AppSettings["TwilioAccountSid"]);

                var values = new Hashtable();
                
                values.Add("To", Request["From"]);
                values.Add("From", ConfigurationManager.AppSettings["TwilioNumber"]);
                values.Add("Body", "Hello from 4sqtransit, real-time public transit information on Foursquare! Visit us on the web at http://4sqtransit.com.");

                account.request(url, "POST", values);
            }
        }
    }
}