using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;
using System.Xml.Linq;
using DotNetOpenAuth.OAuth;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using System.Text;
using _4sqtransit.Common;
using _4sqtransit.Data;

namespace _4sqtransit
{
    public partial class Authorize : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Foursquare.Initialize(ConfigurationManager.AppSettings["FoursquareConsumerKey"], ConfigurationManager.AppSettings["FoursquareConsumerSecret"]);

            if (Session["accessToken"] != null)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                var jsonResult = client.DownloadString("https://api.foursquare.com/v2/users/self?oauth_token=" + Session["accessToken"]);
                var result = Json.Decode(jsonResult);

                DatabaseDataContext db = new DatabaseDataContext();

                var userid = (string)result.response.user.id;

                var count = (from r in db.Users where r.FoursquareUserID == userid select r).Count();
                if (count == 0)
                {
                    User u = new User();

                    u.FoursquareUserID = (string)result.response.user.id;
                    u.FoursquareAccessToken = (string)Session["accessToken"];
                    u.FirstName = result.response.user.firstName != null ? (string)result.response.user.firstName : "";
                    u.LastName = result.response.user.lastName != null ? (string)result.response.user.lastName : "";
                    u.PhoneNumber = result.response.user.contact.phone != null ? (string)result.response.user.contact.phone : "0";
                    u.AgencyID = (string)Session["TRANSIT_AGENCY"];
                    u.LastCheckinID = (string)result.response.user.checkins.items[0].id;
                    u.IsEnabled = true;
                    u.CreatedDate = DateTime.UtcNow;

                    db.Users.InsertOnSubmit(u);

                    this.ddWelcomeMessage.Text = string.Format("Welcome to 4sqtransit, {0}!</b><br /><br />\n", u.FirstName);
                }
                else
                {
                    User u = db.Users.Single(r => r.FoursquareUserID == userid);

                    u.FoursquareAccessToken = (string)Session["accessToken"];
                    u.PhoneNumber = result.response.user.contact.phone != null ? (string)result.response.user.contact.phone : "0";
                    u.AgencyID = (string)Session["TRANSIT_AGENCY"];
                    u.IsEnabled = true;

                    this.ddWelcomeMessage.Text = string.Format("<b>Hello, {0}. Welcome back!</b><br /><br />\n", u.FirstName);
                }

                this.ddVenueName.Text = result.response.user.checkins.items[0].venue.name;
                this.ddTransitAgency.Text = Session["TRANSIT_AGENCY_NAME"].ToString();

                if (result.response.user.contact.phone != null)
                {
                    this.ddPhoneMessage.Text = string.Format("Text messages will be sent to <b>{0}-{1}-{2}</b>. You can change this number at any time on Foursquare.\n", result.response.user.contact.phone.Substring(0, 3), result.response.user.contact.phone.Substring(3, 3), result.response.user.contact.phone.Substring(6, 4));
                }
                else
                {
                    this.ddPhoneMessage.Text = string.Format("You do not have a phone number associated with your Foursquare account. You will not receive text messages until you add a phone number on Foursquare.\n");
                }

                this.uxMapViewer.Text = string.Format("<img alt='bingMap' src='http://dev.virtualearth.net/REST/v1/Imagery/Map/road/{0},{1}/15?mapSize=492,250&pushpin={0},{1};34&mapVersion=v1&key={2}'></img>", result.response.user.checkins.items[0].venue.location.lat, result.response.user.checkins.items[0].venue.location.lng, ConfigurationManager.AppSettings["BingMapsKey"]);

                db.SubmitChanges();

                this.ddUnsubscribe.NavigateUrl = string.Format("/unsubscribe.aspx?id={0}", result.response.user.id);
            }
            else
            {
                var link = Foursquare.GetAuthorizationUrl(Page.ResolveUrl("~/authorize.aspx"));
                Response.Redirect(link);
            }
        }
    }
}