using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string[] authorizedRoles = { /* same roles as before */ };
            string userRole = Session["Role"].ToString();

            if (!authorizedRoles.Contains(userRole))
            {
                Response.Redirect("Unauthorized.aspx");
            }

            // Rest of your Page_Load logic
        }
    }
}