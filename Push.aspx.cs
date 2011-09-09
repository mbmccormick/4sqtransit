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
                    this.ddLogMessage.Text = SendTextMessageNotifications(userid, true);
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
                this.ddLogMessage.Text = ex.Message;
            }
        }

        public static string SendTextMessageNotifications(string userid, Boolean forceSend)
        {
            //StringBuilder output = new StringBuilder();

            //DatabaseDataContext db = new DatabaseDataContext();
            //_4sqtransit.Common.User u;
            //try
            //{
            //    u = db.Users.Single(r => r.FoursquareUserID == userid);
            //}
            //catch (Exception ex)
            //{
            //    Global.LogApplicationError(ex);
            //    return "Error: User was not found.";
            //}

            //System.Net.WebClient client = new System.Net.WebClient();
            //var jsonResult1 = client.DownloadString("https://api.foursquare.com/v2/users/self?oauth_token=" + u.FoursquareAccessToken);
            //var result = Json.Decode(jsonResult1);

            //output.AppendFormat("Executing Foursquare account for {0} {1}.<br />", u.FirstName, u.LastName);

            //var account = new TwilioRest.Account(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

            //if (result.response.user.checkins.items[0].id != u.LastCheckinID ||
            //    forceSend == true)
            //{
            //    output.AppendFormat("User has new checkin {0}.<br />", result.response.user.checkins.items[0].id);

                

            //    List<TransitStop> stops = transitAgency.GetStopsByLocation(Convert.ToDouble(result.response.user.checkins.items[0].venue.location.lat), Convert.ToDouble(result.response.user.checkins.items[0].venue.location.lng), Convert.ToDouble(ConfigurationManager.AppSettings["TransitStopRadius"]));

            //    if (stops.Count > 0)
            //    {
            //        output.AppendFormat("{0} is nearby.<br />", stops[0].Name);

            //        StringBuilder msg = new StringBuilder();
            //        msg.AppendFormat("{0}\n\n", stops[0].Name.ToUpper());

            //        List<TransitStopTime> times = transitAgency.GetStopTimes(stops[0].ID.ToString());

            //        foreach (TransitStopTime t in times.OrderBy(t => t.DepartureTime))
            //        {
            //            DateTime d = new DateTime(t.DepartureTime.Ticks);

            //            string line;
            //            if (t.Type == 1)
            //            {
            //                line = string.Format("{0}: {1}\n", t.RouteShortName, d.ToString("t"));
            //            }
            //            else
            //            {
            //                line = string.Format("{0}: {1} (schd)\n", t.RouteShortName, d.ToString("t"));
            //            }

            //            if ((msg.ToString().Length + line.Length) > 160)
            //            {
            //                var values1 = new Hashtable();
            //                values1.Add("To", (string)result.response.user.contact.phone);
            //                values1.Add("From", ConfigurationManager.AppSettings["TwilioNumber"]);
            //                values1.Add("Body", msg.ToString());
            //                account.request(string.Format("/2010-04-01/Accounts/{0}/SMS/Messages", ConfigurationManager.AppSettings["TwilioAccountSid"]), "POST", values1);

            //                msg.Clear();
            //            }

            //            msg.Append(line);
            //        }

            //        if (times.Count == 0)
            //        {
            //            msg.AppendFormat("There are no departures in the next 2 hours.");

            //            if (forceSend != true)
            //                msg.Clear();
            //        }

            //        if (msg.Length > 0)
            //        {
            //            var values2 = new Hashtable();
            //            values2.Add("To", result.response.user.contact.phone);
            //            values2.Add("From", ConfigurationManager.AppSettings["TwilioNumber"]);
            //            values2.Add("Body", msg.ToString());
            //            account.request(string.Format("/2010-04-01/Accounts/{0}/SMS/Messages", ConfigurationManager.AppSettings["TwilioAccountSid"]), "POST", values2);
            //        }

            //        // store this checkin in the database
            //        if (result.response.user.checkins.items[0].id != u.LastCheckinID)
            //        {
            //            try
            //            {
            //                Common.Checkin h = new Checkin();

            //                h.FoursquareUserID = u.FoursquareUserID;
            //                h.FoursquareCheckinID = (string)result.response.user.checkins.items[0].id;
            //                h.FoursquareVenueID = (string)result.response.user.checkins.items[0].venue.id;
            //                h.VenueName = (string)result.response.user.checkins.items[0].venue.name;
            //                h.VenueLatitude = Convert.ToDecimal(result.response.user.checkins.items[0].venue.location.lat);
            //                h.VenueLongitude = Convert.ToDecimal(result.response.user.checkins.items[0].venue.location.lng);
            //                h.CreatedDate = DateTime.UtcNow;

            //                db.Checkins.InsertOnSubmit(h);
            //            }
            //            catch (Exception ex)
            //            {
            //                Global.LogApplicationError(ex);
            //            }
            //        }

            //        output.AppendFormat("Text message sent to {0}.<br />", result.response.user.contact.phone);
            //    }

            //    u.PhoneNumber = result.response.user.contact.phone != null ? (string)result.response.user.contact.phone : "0";
            //    u.LastCheckinID = (string)result.response.user.checkins.items[0].id;
            //}

            //output.AppendFormat("Done.<br /><br />");

            //db.SubmitChanges();

            //return output.ToString();

            return null;
        }

        public static void SendTextMessageNotificationsBackground(string userid, Boolean forceSend)
        {
            try
            {
                SendTextMessageNotifications(userid, forceSend);
            }
            catch (Exception ex)
            {
                // Global.LogApplicationError(ex);
            }
        }

        public static string GetPostData()
        {
            byte[] b = new byte[System.Web.HttpContext.Current.Request.ContentLength];

            System.Web.HttpContext.Current.Request.InputStream.Read(b, 0, System.Web.HttpContext.Current.Request.ContentLength);
            string s = System.Text.UTF8Encoding.UTF8.GetString(b);

            return s;
        }
    }
}


