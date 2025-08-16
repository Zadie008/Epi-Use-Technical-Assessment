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
            string employeeNumber = txtEmployeeNumber.Text.Trim();
            string rawPassword = txtPassword.Text;

            if (string.IsNullOrEmpty(employeeNumber) || string.IsNullOrEmpty(rawPassword))
            {
                lblLoginMessage.Text = "Please enter both employee number and password";
                ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
                return;
            }

            string hashedPassword = HashPassword(rawPassword);

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString))
                {
                    string query = @"SELECT EmployeeNumber, FirstName, LastName, Role, DepartmentID, LocationID, ManagerID 
                            FROM Employees  
                            WHERE EmployeeNumber = @EmpNo AND PasswordHash = @Password";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmpNo", employeeNumber);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Store all user data in session
                            Session["EmployeeNumber"] = reader["EmployeeNumber"].ToString();
                            Session["FullName"] = $"{reader["FirstName"]} {reader["LastName"]}";
                            Session["Role"] = reader["Role"].ToString();
                            Session["DepartmentID"] = reader["DepartmentID"];
                            Session["LocationID"] = reader["LocationID"];
                            Session["ManagerID"] = reader["ManagerID"].ToString();

                            // Clear any existing error message
                            lblLoginMessage.Text = "";

                            // Redirect to dashboard
                            Response.Redirect("Dashboard.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            lblLoginMessage.Text = "Invalid employee number or password";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblLoginMessage.Text = "System error. Please try again later.";
                ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
                LogError(ex);
            }
        }


        private void LogError(Exception ex)
        {
            //logging purposes for error control
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
            // Clear error message when selecting okay on the popup
            lblLoginMessage.Text = "";
        }
    }
}