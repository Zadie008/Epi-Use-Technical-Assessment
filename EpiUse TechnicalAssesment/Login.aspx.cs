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
            // Check if the user is already logged in
            if (Session["EmployeeNumber"] != null)
            {
                // If they are logged in, redirect them to the dashboard
                Response.Redirect("Dashboard.aspx");
            }

            // Only proceed with the login page's normal logic if the user is NOT logged in
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
                ShowErrorPopup("Please enter both employee number and password");
                return;
            }

            string hashedPassword = HashPassword(rawPassword);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT EmployeeNumber, FirstName, LastName, Role, DepartmentID, LocationID, ManagerID 
                                     FROM Employees  
                                     WHERE EmployeeNumber = @EmpNo AND PasswordHash = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmpNo", employeeNumber);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Successful login - store user data in session
                                Session["EmployeeNumber"] = reader["EmployeeNumber"].ToString();
                                Session["FullName"] = $"{reader["FirstName"]} {reader["LastName"]}";
                                Session["Role"] = reader["Role"].ToString();
                                Session["DepartmentID"] = reader["DepartmentID"];
                                Session["LocationID"] = reader["LocationID"];
                                Session["ManagerID"] = reader["ManagerID"].ToString(); // Storing ManagerID

                                Response.Redirect("Dashboard.aspx");
                            }
                            else
                            {
                                ShowErrorPopup("Invalid employee number or password");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogError(sqlEx);
                ShowErrorPopup("System error. Please try again later.");
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowErrorPopup("An unexpected error occurred.");
            }
        }

        private void ShowErrorPopup(string message)
        {
            lblLoginMessage.Text = message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showPopup();", true);
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
            // Clear the error message when OK is clicked
            lblLoginMessage.Text = "";
        }
    }
}