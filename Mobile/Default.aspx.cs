using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;
using System.Web.Helpers;
using _4sqtransit.Common;

namespace _4sqtransit.Mobile
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();

            this.ddTransitAgency.Items.Add(new ListItem("Select Transit Agency:"));
            this.ddTransitAgency.Items.Add(new ListItem(""));

            var data = (from a in db.Agencies orderby a.Name select a);
            foreach (Agency a in data)
            {
                this.ddTransitAgency.Items.Add(new ListItem(a.Name, a.AgencyID));
            }
        }

        protected void uxAuthorize_Click(object sender, EventArgs e)
        {
            if (this.ddTransitAgency.SelectedIndex > 1)
            {
                Session["TRANSIT_AGENCY"] = this.ddTransitAgency.SelectedValue;
                Session["TRANSIT_AGENCY_NAME"] = this.ddTransitAgency.SelectedItem.Text;

                Response.Redirect( "~/mobile/authorize.aspx");
            }
            else
            {
                Response.Redirect("~/mobile/default.aspx?error=Please select a Transit Agency and try again.");
            }
        }
    }
}