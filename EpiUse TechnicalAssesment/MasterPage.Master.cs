using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EpiUse_TechnicalAssesment
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Url.AbsolutePath.ToLower().Contains("login.aspx"))
            {
                return;
            }

            if (!IsPostBack)
            {
                // Check if user is logged in (for all pages except login)
                if (Session["Role"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                SetNavigationVisibility();
            }
        }

        private void SetNavigationVisibility()
        {
            // Get the user's role from session
            string userRole = Session["Role"].ToString();

            // Define which roles can add employees
            string[] authorizedRoles = {
            "CEO",
            "Head of Pretoria",
            "Head of Cape Town",
            "Head of Durban",
            "Senior HR",
            "Senior Payroll",
            "Senior Dev",
            "Senior Support"
        };

            // Show Add Employee link only for authorized roles
            phAddEmployee.Visible = authorizedRoles.Contains(userRole);
        }
    }
}