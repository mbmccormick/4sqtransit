using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace _4sqtransit
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Foursquare.Initialize(ConfigurationManager.AppSettings["FoursquareConsumerKey"], ConfigurationManager.AppSettings["FoursquareConsumerSecret"]);
            var accessToken = Foursquare.GetAccessToken();

            Session["accessToken"] = accessToken;

            var returnUrl = Request["returnUrl"];
            if (returnUrl == string.Empty)
            {
                returnUrl = "~/";
            }

            Response.Redirect(returnUrl);
        }
    }
}