using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm6 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        private string employeeNumber;
        private string employeeName;
        private string employeeEmail;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["EmployeeID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                employeeNumber = Request.QueryString["empId"];
                if (string.IsNullOrEmpty(employeeNumber))
                {
                    lblMessage.Text = "No employee ID specified.";
                    pnlChangePhoto.Visible = false;
                    return;
                }

                LoadEmployeePhotoAndInfo(employeeNumber);
            }
        }

        private void LoadEmployeePhotoAndInfo(string employeeNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Changed from @EmployeeID to @EmployeeNumber to match the parameter you're adding
                string query = @"SELECT e.FirstName, e.LastName, e.Email, ep.Picture 
                       FROM EMPLOYEES e
                       LEFT JOIN EMPLOYEE_PICTURE ep ON e.EmployeeID = ep.EmployeeID
                       WHERE e.EmployeeID = @EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    employeeName = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                    lblEmployeeHeaderName.Text = employeeName;
                    employeeEmail = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                    // Handle the picture (stored as byte[] in the database)
                    if (reader["Picture"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])reader["Picture"];
                        string base64String = Convert.ToBase64String(imageBytes);
                        imgEmployeePhoto.ImageUrl = "data:image/jpeg;base64," + base64String;
                    }
                    else if (!string.IsNullOrEmpty(employeeEmail))
                    {
                        imgEmployeePhoto.ImageUrl = GetGravatarUrl(employeeEmail);
                    }
                    else
                    {
                        imgEmployeePhoto.ImageUrl = "https://www.gravatar.com/avatar/?d=identicon";
                    }
                }
                else
                {
                    lblMessage.Text = "Employee not found.";
                    pnlChangePhoto.Visible = false;
                }
                reader.Close();
            }
        }

        protected void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            if (fuProfileImage.HasFile)
            {
                try
                {
                    // Check file type and size
                    if (fuProfileImage.PostedFile.ContentLength > 2 * 1024 * 1024) // Max 2MB
                    {
                        lblMessage.Text = "File size must be 2MB or less.";
                        lblMessage.ForeColor = Color.Red;
                        return;
                    }

                    string fileExtension = Path.GetExtension(fuProfileImage.PostedFile.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        lblMessage.Text = "Only JPG and PNG files are allowed.";
                        lblMessage.ForeColor = Color.Red;
                        return;
                    }

                    // Convert the image to a Base64 string
                    byte[] photoBytes = fuProfileImage.FileBytes;
                    string base64Image = Convert.ToBase64String(photoBytes);

                    employeeNumber = Request.QueryString["empId"];
                    UpdateProfilePhoto(employeeNumber, base64Image);

                    lblMessage.Text = "Profile photo updated successfully!";
                    lblMessage.ForeColor = Color.Green;

                    // Reload the page to display the new photo
                    LoadEmployeePhotoAndInfo(employeeNumber);
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "An error occurred during upload: " + ex.Message;
                    lblMessage.ForeColor = Color.Red;
                }
            }
            else
            {
                lblMessage.Text = "Please select a file to upload.";
                lblMessage.ForeColor = Color.Red;
            }
        }

        private void UpdateProfilePhoto(string employeeNumber, string base64Image)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Convert base64 string back to byte array
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                // Check if the employee already has a picture record
                string checkQuery = "SELECT COUNT(*) FROM EMPLOYEE_PICTURE WHERE EmployeeID = @EmployeeNumber";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);

                con.Open();
                int count = (int)checkCmd.ExecuteScalar();

                string query;
                if (count > 0)
                {
                    // Update existing record
                    query = "UPDATE EMPLOYEE_PICTURE SET Picture = @Picture WHERE EmployeeID = @EmployeeNumber";
                }
                else
                {
                    // Insert new record
                    query = "INSERT INTO EMPLOYEE_PICTURE (EmployeeID, Picture) VALUES (@EmployeeNumber, @Picture)";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                cmd.Parameters.AddWithValue("@Picture", imageBytes);
                cmd.ExecuteNonQuery();
            }
        }

        private string GetGravatarUrl(string email)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(email.Trim().ToLower());
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return $"https://www.gravatar.com/avatar/{sb}?s=200&d=identicon";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            employeeNumber = Request.QueryString["empId"];
            Response.Redirect($"ViewEmployee.aspx?empId={employeeNumber}");
        }
    }
}