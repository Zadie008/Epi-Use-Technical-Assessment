using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblLoginMessage.Text = "";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string employeeId = txtEmployeeNumber.Text.Trim();
            string rawPassword = txtPassword.Text;

            if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(rawPassword))
            {
                lblLoginMessage.Text = "Please enter both employee ID and password";
                ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
                return;
            }

            string hashedPassword = HashPassword(rawPassword);

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString))
                {
                    // DEBUG: Output the values being checked
                    System.Diagnostics.Debug.WriteLine($"Attempting login for EmployeeID: {employeeId}");
                    System.Diagnostics.Debug.WriteLine($"Using hashed password: {hashedPassword}");

                    string query = @"SELECT EmployeeID, RoleID 
                            FROM USER_AUTH 
                            WHERE EmployeeID = @EmpID AND Password = @Password";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // DEBUG: Confirm we found a matching user
                            string foundEmployeeID = reader["EmployeeID"].ToString();
                            string foundRoleID = reader["RoleID"].ToString();
                            System.Diagnostics.Debug.WriteLine($"Login successful for EmployeeID: {foundEmployeeID}, RoleID: {foundRoleID}");

                            // Set session variables
                            Session["EmployeeID"] = foundEmployeeID;
                            Session["RoleID"] = foundRoleID;

                            // DEBUG: Verify session variables are set
                            System.Diagnostics.Debug.WriteLine($"Session variables set - EmployeeID: {Session["EmployeeID"]}, RoleID: {Session["RoleID"]}");

                            // Clear any error message
                            lblLoginMessage.Text = "";

                            // Redirect to Dashboard
                            System.Diagnostics.Debug.WriteLine("Attempting redirect to Dashboard.aspx");
                            Response.Redirect("Dashboard.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                            return; // Explicit return after redirect
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No matching user found");
                            lblLoginMessage.Text = "Invalid employee ID or password";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.ToString()}");
                lblLoginMessage.Text = "System error. Please try again later.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
                LogError(ex);
            }
        }


        private void LogError(Exception ex)
        {
            // for error loggin purposes
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            lblLoginMessage.Text = "";
        }
    }
}
