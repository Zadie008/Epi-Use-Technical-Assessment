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
                return; // Skip role checks on login page
            }

            if (!IsPostBack)
            {
                // Check if user is logged in (for all pages except login)
                if (Session["RoleID"] == null) // Changed from Session["Role"]
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                SetNavigationVisibility();
            }
        }

        private void SetNavigationVisibility()
        {
            if (Session["RoleID"] == null) return;

            int roleId = Convert.ToInt32(Session["RoleID"]);

            // RoleID 1 & 2 (CEO, Heads) get full access
            // RoleID 3 (Regular employees) only see Dashboard, Hierarchy, ReadMe
            phAddEmployee.Visible = (roleId == 1 || roleId == 2);
        }
    }
}