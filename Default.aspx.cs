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
using _4sqtransit.Data;
using _4sqtransit.Resources;

namespace _4sqtransit
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.ddTransitAgency.Items.Add(new ListItem("Select Transit Agency:"));
                this.ddTransitAgency.Items.Add(new ListItem(""));

                var agencies = OneTransitAPI.GetAgencies();

                foreach (var a in agencies)
                {
                    this.ddTransitAgency.Items.Add(new ListItem(a.Name, a.AgencyID));
                }
            }
            catch (Exception ex)
            {
                // do nothing
            }
        }

        protected void uxAuthorize_Click(object sender, EventArgs e)
        {
            if (this.ddTransitAgency.SelectedIndex > 1)
            {
                Session["TRANSIT_AGENCY"] = this.ddTransitAgency.SelectedValue;
                Session["TRANSIT_AGENCY_NAME"] = this.ddTransitAgency.SelectedItem.Text;

                Response.Redirect( "~/authorize.aspx");
            }
            else
            {
                Response.Redirect("~/default.aspx?error=Please select a Transit Agency and try again.");
            }
        }
    }
}