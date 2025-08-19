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
                    string query = @"SELECT ua.EmployeeID, ua.RoleID, d.LocationID
                      FROM USER_AUTH ua
                      JOIN EMPLOYEES e ON ua.EmployeeID = e.EmployeeID
                      JOIN DEPARTMENT d ON e.DepartmentID = d.DepartmentID
                      WHERE ua.EmployeeID = @EmpID AND ua.Password = @Password";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set session variables
                            Session["EmployeeID"] = reader["EmployeeID"].ToString();
                            Session["RoleID"] = reader["RoleID"].ToString();
                            Session["LocationID"] = reader["LocationID"].ToString();
                            Session.Timeout = 30;

                            Response.Redirect("Dashboard.aspx");
                        }
                        else
                        {
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
