using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
            string userRole = Session["Role"].ToString();

            if (!IsUserAuthorized(userRole))
            {
                Response.Redirect("Unauthorized.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadLocations();
                LoadRoles();
               
            }


        }

        private bool IsUserAuthorized(string role)
        {

            if (role == "CEO")
            {
                return true;
            }
            if (role.StartsWith("Head of"))
            {
                return true;
            }
            if (role == "Senior HR")
            {
                return true;
            }
            return false;
        }

        protected void SwitchPanel(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            // Hide all panels first
            pnlEmployee.Visible = false;
            pnlDepartment.Visible = false;
            pnlLocation.Visible = false;

            // Reset button styles
            btnAddEmployee.CssClass = "btn btn-primary";
            btnAddDepartment.CssClass = "btn btn-primary";
            btnAddLocation.CssClass = "btn btn-primary";

            // Show selected panel and update button style
            switch (btn.CommandArgument)
            {
                case "Employee":
                    pnlEmployee.Visible = true;
                    btnAddEmployee.CssClass = "btn btn-primary active";
                    break;
                case "Department":
                    pnlDepartment.Visible = true;
                    btnAddDepartment.CssClass = "btn btn-primary active";
                    break;
                case "Location":
                    pnlLocation.Visible = true;
                    btnAddLocation.CssClass = "btn btn-primary active";
                    break;
            }
        }
        private void determineManger()
        {

        }
        private void LoadDepartments()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("-- Select Department --", "0"));
            }
        }
        private void LoadLocations()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID, LocationName FROM Locations";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlLocation.DataSource = dt;
                ddlLocation.DataTextField = "LocationName";
                ddlLocation.DataValueField = "LocationID";
                ddlLocation.DataBind();
                ddlLocation.Items.Insert(0, new ListItem("-- Select Location --", "0"));
            }
        }
        private void LoadRoles()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT p.PositionID, p.PositionName 
                                FROM Position p
                                ORDER BY p.Ranking";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlRoles.DataSource = dt;
                ddlRoles.DataTextField = "PositionName";
                ddlRoles.DataValueField = "PositionID";
                ddlRoles.DataBind();
                ddlRoles.Items.Insert(0, new ListItem("-- Select Role --", "0"));
            }
        }


        protected void AddEmployee(object sender, EventArgs e)
        {
            if (!ValidateEmployeeInputs())
            {
                return;
            }

            string newEmployeeNumber = GenerateEmployeeNumber();
            int positionId = int.Parse(ddlRoles.SelectedValue);
            int departmentId = int.Parse(ddlDepartment.SelectedValue);
            int locationId = int.Parse(ddlLocation.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Employees 
                            (EmployeeNumber, FirstName, LastName, DOB, Email, Role, Salary, 
                             PasswordHash, ManagerID, DepartmentID, LocationID, PositionID) 
                            VALUES 
                            (@EmployeeNumber, @FirstName, @LastName, @DOB, @Email, @Role, @Salary, 
                             @PasswordHash, @ManagerID, @DepartmentID, @LocationID, @PositionID)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Get role name from position ID
                    string roleName = GetRoleName(positionId);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@EmployeeNumber", newEmployeeNumber);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                    cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(txtDOB.Text));
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Role", roleName);
                    cmd.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text));
                    cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(txtPassword1.Text));
                    cmd.Parameters.AddWithValue("@ManagerID", DetermineManagerID(positionId, departmentId, locationId) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@PositionID", positionId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    ShowSuccessMessage($"Employee {txtFirstName.Text} {txtLastName.Text} added successfully with ID: {newEmployeeNumber}");
                    ClearEmployeeForm();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding employee: " + ex.Message);
            }
        }
        private string GetRoleName(int positionId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT PositionName FROM Positions WHERE PositionID = @PositionID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PositionID", positionId);
                con.Open();
                string roleName = cmd.ExecuteScalar()?.ToString();
                con.Close();
                return roleName;
            }
        }
        private string DetermineManagerID(int positionId, int departmentId, int locationId)
        {
            // CEO doesn't have a manager
            if (positionId == 1) // Assuming 1 is CEO's PositionID
                return null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Get the position level of the new employee
                int newEmployeeLevel = GetPositionLevel(positionId);

                // Find the lowest position level above the new employee in the same department/location
                string query = @"SELECT TOP 1 e.EmployeeNumber
                        FROM Employees e
                        INNER JOIN Positions p ON e.PositionID = p.PositionID
                        WHERE e.DepartmentID = @DepartmentID
                        AND e.LocationID = @LocationID
                        AND p.PositionLevel < @NewEmployeeLevel
                        ORDER BY p.PositionLevel DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@NewEmployeeLevel", newEmployeeLevel);

                con.Open();
                string managerId = cmd.ExecuteScalar()?.ToString();
                con.Close();

                // If no direct manager found in same department/location, look for any manager
                if (string.IsNullOrEmpty(managerId))
                {
                    query = @"SELECT TOP 1 e.EmployeeNumber
                            FROM Employees e
                            INNER JOIN Positions p ON e.PositionID = p.PositionID
                            WHERE p.PositionLevel < @NewEmployeeLevel
                            ORDER BY p.PositionLevel DESC";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NewEmployeeLevel", newEmployeeLevel);

                    con.Open();
                    managerId = cmd.ExecuteScalar()?.ToString();
                    con.Close();
                }

                return managerId;
            }
        }

        private int GetPositionLevel(int positionId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT PositionLevel FROM Positions WHERE PositionID = @PositionID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PositionID", positionId);
                con.Open();
                int level = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                return level;
            }
        }
        private bool ValidateEmployeeInputs()
        {
            // Validate first name (letters only)
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtFirstName.Text.Trim(), @"^[a-zA-Z]+$"))
            {
                ShowErrorMessage("First name can only contain letters");
                return false;
            }

            // Validate last name (letters only)
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtLastName.Text.Trim(), @"^[a-zA-Z]+$"))
            {
                ShowErrorMessage("Last name can only contain letters");
                return false;
            }

            // Validate date of birth
            DateTime dob;
            if (!DateTime.TryParse(txtDOB.Text, out dob))
            {
                ShowErrorMessage("Invalid date of birth format");
                return false;
            }

            if (dob >= new DateTime(2025, 1, 1))
            {
                ShowErrorMessage("Date of birth must be before 2025/01/01");
                return false;
            }

            // Validate email
            if (!txtEmail.Text.Contains("@"))
            {
                ShowErrorMessage("Email must contain an @ symbol");
                return false;
            }

            // Validate salary
            decimal salary;
            if (!decimal.TryParse(txtSalary.Text, out salary))
            {
                ShowErrorMessage("Invalid salary format");
                return false;
            }

            if (salary < 0)
            {
                ShowErrorMessage("Salary cannot be negative");
                return false;
            }

            // Validate password match
            if (txtPassword1.Text != txtPassword2.Text)
            {
                ShowErrorMessage("Passwords do not match");
                return false;
            }

            // Validate department and location selection
            if (ddlDepartment.SelectedValue == "0" || ddlLocation.SelectedValue == "0")
            {
                ShowErrorMessage("Please select both department and location");
                return false;
            }

            return true;
        }

        private string DetermineManagerID(string role, int departmentId)
        {
            // CEO doesn't have a manager
            if (role == "CEO") return null;

            // Head of [Location] reports to CEO
            if (role.StartsWith("Head of"))
            {
                return GetEmployeeIdByRole("CEO");
            }

            // For other roles, find the appropriate manager in their department
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 1 e.EmployeeNumber
                        FROM Employees e
                        WHERE e.DepartmentID = @DepartmentID
                        AND (
                            e.Role = 'Senior HR' OR 
                            e.Role LIKE 'Head of%' OR 
                            e.Role = 'CEO'
                        )
                        ORDER BY CASE 
                            WHEN e.Role = 'CEO' THEN 1
                            WHEN e.Role LIKE 'Head of%' THEN 2
                            WHEN e.Role = 'Senior HR' THEN 3
                            ELSE 4
                        END";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                con.Open();
                string managerId = cmd.ExecuteScalar()?.ToString();
                con.Close();

                return managerId;
            }
        }

        private string GetEmployeeIdByRole(string role)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TOP 1 EmployeeNumber FROM Employees WHERE Role = @Role";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Role", role);
                con.Open();
                string employeeNumber = cmd.ExecuteScalar()?.ToString();
                con.Close();
                return employeeNumber;
            }
        }
        private string GenerateEmployeeNumber()
        {
            // Get the highest current employee number and increment
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT MAX(EmployeeNumber) FROM Employees";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                string maxNumber = cmd.ExecuteScalar()?.ToString();
                con.Close();

                if (string.IsNullOrEmpty(maxNumber))
                {
                    return "EMP001";
                }

                int num = int.Parse(maxNumber.Replace("EMP", ""));
                return "EMP" + (num + 1).ToString("D3");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private int DeterminePositionID(string role)
        {
            
            if (role.Contains("Senior")) return 4;
            if (role.Contains("Junior")) return 5;
            if (role.Contains("Intern")) return 6;
            if (role == "CEO") return 1;
            if (role.StartsWith("Head of")) return 2;
            return 4; 
        }
        private void ShowSuccessMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccess",
                $"alert('{message}');", true);
        }

        private void ShowErrorMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                $"alert('Error: {message}');", true);
        }
        private void ClearEmployeeForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtDOB.Text = "";
            txtEmail.Text = "";
            txtSalary.Text = "";
            txtPassword1.Text = "";
            txtPassword2.Text = "";
            ddlRoles.SelectedIndex = 0;
            ddlDepartment.SelectedIndex = 0;
            ddlLocation.SelectedIndex = 0;
        }
        protected void AddDepartment(object sender, EventArgs e)
        {
            
        }

        protected void AddLocation(object sender, EventArgs e)
        {
            
        }


    }
}