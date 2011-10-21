using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using _4sqtransit.Common;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Web.Helpers;
using System.Configuration;
using System.Collections;
using System.Threading;
using _4sqtransit.Common;
using _4sqtransit.Data;
using _4sqtransit.Resources;

namespace _4sqtransit
{
    public partial class Push : System.Web.UI.Page
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.QueryString["userid"] != null)
                {
                    string userid = System.Web.HttpContext.Current.Request.QueryString["userid"];
                    SendTextMessageNotifications(userid, true);
                }
                else
                {
                    int garbage = 0;
                    int maxThreads = 0;
                    int availThreads = 0;

                    ThreadPool.GetMaxThreads(out maxThreads, out garbage);
                    ThreadPool.GetAvailableThreads(out availThreads, out garbage);

                    if (availThreads > 0)
                    {
                        string userid = Json.Decode(GetPostData()).user.id;
                        ThreadPool.QueueUserWorkItem(cb => SendTextMessageNotificationsBackground(userid, false));
                    }
                }
            }
            catch (Exception ex)
            {
                this.txtResponse.Text = ex.Message + "<br /><br />" + ex.StackTrace;
            }
        }

        public static void SendTextMessageNotifications(string userid, Boolean forceSend)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            User u = db.Users.Single(r => r.FoursquareUserID == userid &&
                                          r.IsEnabled == true);

            System.Net.WebClient client = new System.Net.WebClient();
            var jsonResult1 = client.DownloadString("https://api.foursquare.com/v2/users/self?oauth_token=" + u.FoursquareAccessToken);
            var result = Json.Decode(jsonResult1);

            var account = new TwilioRest.Account(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

            if (result.response.user.checkins.items[0].id != u.LastCheckinID ||
                forceSend == true)
            {
                var stops = OneTransitAPI.GetStopsByLocation(u.AgencyID, Convert.ToDouble(result.response.user.checkins.items[0].venue.location.lat), Convert.ToDouble(result.response.user.checkins.items[0].venue.location.lng), Convert.ToDouble(ConfigurationManager.AppSettings["TransitStopRadius"]));

                if (stops.Length > 0)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendFormat("{0}\n\n", stops[0].Name.ToUpper());

                    var times = OneTransitAPI.GetStopTimes(u.AgencyID, stops[0].ID.ToString());

                    foreach (var t in times)
                    {
                        string line;
                        if (t.Type == "realtime")
                        {
                            line = string.Format("{0}: {1}\n", t.RouteShortName, t.DepartureTime);
                        }
                        else
                        {
                            line = string.Format("{0}: {1} (schd)\n", t.RouteShortName, t.DepartureTime);
                        }

                        if ((msg.ToString().Length + line.Length) > 160)
                        {
                            var values1 = new Hashtable();
                            values1.Add("To", (string)result.response.user.contact.phone);
                            values1.Add("From", ConfigurationManager.AppSettings["TwilioNumber"]);
                            values1.Add("Body", msg.ToString());
                            account.request(string.Format("/2010-04-01/Accounts/{0}/SMS/Messages", ConfigurationManager.AppSettings["TwilioAccountSid"]), "POST", values1);

                            msg.Clear();
                        }

                        msg.Append(line);
                    }

                    if (times.Length == 0)
                    {
                        msg.AppendFormat("There are no departures in the next 2 hours.");

                        if (forceSend != true)
                            msg.Clear();
                    }

                    if (msg.Length > 0)
                    {
                        var values2 = new Hashtable();
                        values2.Add("To", result.response.user.contact.phone);
                        values2.Add("From", ConfigurationManager.AppSettings["TwilioNumber"]);
                        values2.Add("Body", msg.ToString());
                        account.request(string.Format("/2010-04-01/Accounts/{0}/SMS/Messages", ConfigurationManager.AppSettings["TwilioAccountSid"]), "POST", values2);
                    }
                }

                u.PhoneNumber = result.response.user.contact.phone != null ? (string)result.response.user.contact.phone : "0";
                u.LastCheckinID = (string)result.response.user.checkins.items[0].id;
            }

            db.SubmitChanges();
        }

        public static void SendTextMessageNotificationsBackground(string userid, Boolean forceSend)
        {
            try
            {
                SendTextMessageNotifications(userid, forceSend);
            }
            catch (Exception ex)
            {
            }
        }

        public static string GetPostData()
        {
            byte[] b = new byte[System.Web.HttpContext.Current.Request.ContentLength];

            System.Web.HttpContext.Current.Request.InputStream.Read(b, 0, System.Web.HttpContext.Current.Request.ContentLength);
            string s = System.Text.UTF8Encoding.UTF8.GetString(b);

            if (s.Length < 10)
            {
                s = "{\"checkin\":" + System.Web.HttpContext.Current.Request.Form["checkin"] + "," +
                    "\"user\":" + System.Web.HttpContext.Current.Request.Form["user"] + "}";
            }

            return s;
        }
    }
}


