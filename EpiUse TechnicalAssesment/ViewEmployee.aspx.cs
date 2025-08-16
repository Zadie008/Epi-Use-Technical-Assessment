using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        private string employeeEmail;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["EmployeeNumber"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                string empId = Request.QueryString["empId"];
                if (!string.IsNullOrEmpty(empId))
                {
                    LoadEmployee(empId);
                    ControlEditButtonVisibility(empId);
                }
                else
                {
                    lblMessage.Text = "No Employee ID specified.";
                    pnlEmployee.Visible = false;
                }
            }
        }

        private void LoadEmployee(string empId)
        {
            string currentUserNumber = Session["EmployeeNumber"].ToString();
            string currentUserRole = Session["Role"]?.ToString();

            int currentUserLocationId = Session["LocationID"] != null ? Convert.ToInt32(Session["LocationID"]) : -1;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                e.EmployeeNumber, 
                e.FirstName, 
                e.LastName, 
                e.DOB, 
                e.Salary, 
                e.Role, 
                e.Email, 
                e.ProfilePhotoBase64,
                e.LocationID, 
                e.ManagerID,
                m.FirstName + ' ' + m.LastName AS ManagerName,
                l.LocationName,
                d.DepartmentName
            FROM Employees e
            LEFT JOIN Employees m ON e.ManagerID = m.EmployeeNumber
            LEFT JOIN Locations l ON e.LocationID = l.LocationID
            LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
            WHERE e.EmployeeNumber = @EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", empId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    pnlEmployee.Visible = true;
                    employeeEmail = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                    // Basic employee info
                    lblEmployeeID.Text = reader["EmployeeNumber"].ToString();
                    lblFirstName.Text = reader["FirstName"].ToString();
                    lblLastName.Text = reader["LastName"].ToString();

                    lblBirthDate.Text = reader["DOB"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd")
                        : "";

                    lblEmployeeNumber.Text = reader["EmployeeNumber"].ToString();

                    // Salary visibility control
                    string targetEmployeeNumber = reader["EmployeeNumber"].ToString();
                    int targetLocationId = reader["LocationID"] != DBNull.Value ? Convert.ToInt32(reader["LocationID"]) : -1;
                    string targetManagerID = reader["ManagerID"] != DBNull.Value ? reader["ManagerID"].ToString() : "";
                    decimal salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0;

                    hdnOriginalSalary.Value = salary.ToString();

                    bool canViewSalary = CanViewSalary(currentUserNumber, targetEmployeeNumber, currentUserRole, currentUserLocationId, targetLocationId, targetManagerID);

                    lblSalary.Text = canViewSalary ? string.Format("{0:C}", salary) : "***";
                    txtSalary.Text = canViewSalary ? salary.ToString() : "";

                    lblRole.Text = reader["Role"].ToString();
                    lblManager.Text = reader["ManagerName"] != DBNull.Value ? reader["ManagerName"].ToString() : "No Manager";
                    lblLocation.Text = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : "No Location";
                    lblDepartment.Text = reader["DepartmentName"] != DBNull.Value ? reader["DepartmentName"].ToString() : "No Department";

                    string profilePhotoBase64 = reader["ProfilePhotoBase64"] != DBNull.Value ? reader["ProfilePhotoBase64"].ToString() : "";

                    if (!string.IsNullOrEmpty(profilePhotoBase64))
                    {
                        imgGravatar.ImageUrl = "data:image/jpeg;base64," + profilePhotoBase64;
                    }
                    else if (!string.IsNullOrEmpty(employeeEmail))
                    {
                        imgGravatar.ImageUrl = GetGravatarUrl(employeeEmail);
                    }
                }
                else
                {
                    lblMessage.Text = "Employee not found.";
                    pnlEmployee.Visible = false;
                }
            }
        }

        private void ControlEditButtonVisibility(string employeeNumber)
        {
            string currentUserNumber = Session["EmployeeNumber"].ToString();
            string currentUserRole = Session["Role"]?.ToString();
            int currentUserLocationId = Convert.ToInt32(Session["LocationID"]);

            string targetEmployeeNumber = "";
            int targetLocationId = -1;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeNumber, LocationID FROM Employees WHERE EmployeeNumber = @EmployeeNumber";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    targetEmployeeNumber = reader["EmployeeNumber"].ToString();
                    targetLocationId = Convert.ToInt32(reader["LocationID"]);
                }
            }

            bool canEdit = false;

            if (currentUserNumber == targetEmployeeNumber)
            {
                canEdit = true;
            }
            else if (currentUserRole == "HR Manager" && currentUserLocationId == targetLocationId)
            {
                canEdit = true;
            }
            else if (currentUserRole == "CEO")
            {
                canEdit = true;
            }

            btnEditEmployee.Visible = canEdit;
        }

        private bool CanViewSalary(string currentUserNumber, string targetEmployeeNumber, string currentUserRole, int currentUserLocationId, int targetLocationId, string targetManagerID)
        {
            if (currentUserNumber == targetEmployeeNumber)
                return true;

            if ((currentUserRole == "HR Manager" || currentUserRole == "HR") && currentUserLocationId == targetLocationId)
                return true;

            if (currentUserNumber == targetManagerID)
                return true;

            if (currentUserRole == "CEO")
                return true;

            return false;
        }

        protected void btnEditEmployee_Click(object sender, EventArgs e)
        {
            ToggleEditMode(true);
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ToggleEditMode(false);
        }

        private void ToggleEditMode(bool isEdit)
        {
            string currentUserRole = Session["Role"]?.ToString();
            bool isSalaryEditable = (currentUserRole == "HR Manager" || currentUserRole == "CEO");

            lblFirstName.Visible = !isEdit;
            txtFirstName.Visible = isEdit;
            if (isEdit) txtFirstName.Text = lblFirstName.Text;

            lblLastName.Visible = !isEdit;
            txtLastName.Visible = isEdit;
            if (isEdit) txtLastName.Text = lblLastName.Text;

            lblBirthDate.Visible = !isEdit;
            txtBirthDate.Visible = isEdit;
            if (isEdit) txtBirthDate.Text = lblBirthDate.Text;

            lblEmployeeNumber.Visible = !isEdit;
            txtEmployeeNumber.Visible = isEdit;
            if (isEdit) txtEmployeeNumber.Text = lblEmployeeNumber.Text;

            lblSalary.Visible = !isEdit;
            txtSalary.Visible = isEdit && isSalaryEditable;
            if (isEdit && isSalaryEditable)
            {
                txtSalary.Text = hdnOriginalSalary.Value;
            }
            else if (isEdit && !isSalaryEditable)
            {
                lblSalary.Visible = true;
                txtSalary.Visible = false;
            }

            lblRole.Visible = !isEdit;
            txtRole.Visible = isEdit;
            if (isEdit) txtRole.Text = lblRole.Text;

            lblManager.Visible = !isEdit;
            lblLocation.Visible = !isEdit;
            lblDepartment.Visible = !isEdit;
            txtManager.Visible = false;
            txtLocation.Visible = false;
            txtDepartment.Visible = false;

            lblEmployeeID.Visible = !isEdit;
            txtEmployeeID.Visible = isEdit;
            if (isEdit) txtEmployeeID.Text = lblEmployeeID.Text;

            btnEditEmployee.Visible = !isEdit;
            btnSaveEmployee.Visible = isEdit;
            btnCancelEdit.Visible = isEdit;
            btnChangeProfile.Visible = isEdit;
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            string employeeNumber = txtEmployeeID.Text;
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string birthDate = txtBirthDate.Text;
            string newEmployeeNumber = txtEmployeeNumber.Text;

            decimal salary = 0;
            string currentUserRole = Session["Role"]?.ToString();

            if (currentUserRole == "HR Manager" || currentUserRole == "CEO")
            {
                string salaryString = txtSalary.Text.Replace("$", "").Replace(",", "").Trim();
                if (!string.IsNullOrWhiteSpace(salaryString) && decimal.TryParse(salaryString, out salary))
                {
                    // Salary was successfully parsed
                }
                else
                {
                    lblMessage.Text = "Invalid salary format. Please enter a valid number.";
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(hdnOriginalSalary.Value))
                {
                    decimal.TryParse(hdnOriginalSalary.Value, out salary);
                }
            }

            string role = txtRole.Text;

            SaveEmployeeData(employeeNumber, firstName, lastName, birthDate, newEmployeeNumber, salary, role);

            LoadEmployee(employeeNumber);
            ToggleEditMode(false);
            lblMessage.Text = "Changes saved successfully.";
        }

        private void SaveEmployeeData(string employeeNumber, string firstName, string lastName, string birthDate, string newEmployeeNumber, decimal salary, string role)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE Employees SET
            FirstName = @FirstName,
            LastName = @LastName,
            DOB = @BirthDate,
            EmployeeNumber = @NewEmployeeNumber,
            Salary = @Salary,
            Role = @Role
            WHERE EmployeeNumber = @EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                cmd.Parameters.AddWithValue("@NewEmployeeNumber", newEmployeeNumber);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@Role", role);

                con.Open();
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
        protected void btnChangeProfile_Click(object sender, EventArgs e)
        {
            string employeeNumber = lblEmployeeNumber.Text;
            Response.Redirect($"ChangeProfilePhoto.aspx?empId={employeeNumber}");
        }
    }
}