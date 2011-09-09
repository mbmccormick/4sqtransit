using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using _4sqtransit.Common;
using System.Data.SqlClient;

namespace _4sqtransit.Mobile
{
    public partial class Unsubscribe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();

            _4sqtransit.Common.User u = db.Users.Single(r => r.FoursquareUserID == Request.QueryString["id"]);
            db.Users.DeleteOnSubmit(u);

            db.SubmitChanges();
        }
    }
}