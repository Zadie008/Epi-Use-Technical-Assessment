using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
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
            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
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
                    SetAccessControls(empId);
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
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    e.EmployeeNumber, 
                    e.FirstName, 
                    e.LastName, 
                    e.DOB, 
                    e.Email,
                    e.Salary,
                    p.PositionName as Role,
                    e.ProfilePhotoBase64,
                    e.LocationID, 
                    e.ManagerID,
                    m.FirstName + ' ' + m.LastName AS ManagerName,
                    l.LocationName,
                    d.DepartmentName,
                    e.DepartmentID,
                    p.PositionID
                FROM Employees e
                LEFT JOIN Employees m ON e.ManagerID = m.EmployeeNumber
                LEFT JOIN Locations l ON e.LocationID = l.LocationID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Position p ON e.PositionID = p.PositionID
                WHERE e.EmployeeNumber = @EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", empId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    pnlEmployee.Visible = true;
                    employeeEmail = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                    // Set all field values
                    lblEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                    lblFirstName.Text = reader["FirstName"].ToString();
                    lblLastName.Text = reader["LastName"].ToString();
                    lblBirthDate.Text = reader["DOB"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd")
                        : "";
                    lblEmail.Text = employeeEmail;

                    // Handle salary
                    decimal salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0;
                    lblSalary.Text = string.Format("{0:C}", salary);
                    hdnOriginalSalary.Value = salary.ToString();

                    lblRole.Text = reader["Role"].ToString();
                    lblDepartment.Text = reader["DepartmentName"] != DBNull.Value
                        ? reader["DepartmentName"].ToString()
                        : "No Department";
                    lblManager.Text = reader["ManagerName"] != DBNull.Value
                        ? reader["ManagerName"].ToString()
                        : "No Manager";
                    lblLocation.Text = reader["LocationName"] != DBNull.Value
                        ? reader["LocationName"].ToString()
                        : "No Location";

                    // Set values for edit fields
                    txtEmployeeNumber.Text = lblEmployeeNumber.Text;
                    txtFirstName.Text = lblFirstName.Text;
                    txtLastName.Text = lblLastName.Text;
                    txtBirthDate.Text = lblBirthDate.Text;
                    txtEmail.Text = lblEmail.Text;
                    txtSalary.Text = hdnOriginalSalary.Value;

                    // Handle profile photo
                    string profilePhotoBase64 = reader["ProfilePhotoBase64"] != DBNull.Value
                        ? reader["ProfilePhotoBase64"].ToString()
                        : "";
                    if (!string.IsNullOrEmpty(profilePhotoBase64))
                    {
                        imgGravatar.ImageUrl = "data:image/jpeg;base64," + profilePhotoBase64;
                    }
                    else if (!string.IsNullOrEmpty(employeeEmail))
                    {
                        imgGravatar.ImageUrl = GetGravatarUrl(employeeEmail);
                    }

                    // Store original values
                    ViewState["OriginalDepartmentId"] = reader["DepartmentID"] != DBNull.Value ? reader["DepartmentID"].ToString() : "";
                    ViewState["OriginalPositionId"] = reader["PositionID"] != DBNull.Value ? reader["PositionID"].ToString() : "";
                    ViewState["OriginalLocationId"] = reader["LocationID"] != DBNull.Value ? reader["LocationID"].ToString() : "";

                    // Populate dropdowns
                    PopulateDepartmentDropdown(Convert.ToInt32(ViewState["OriginalLocationId"]));
                    int currentPositionId = reader["PositionID"] != DBNull.Value ? Convert.ToInt32(reader["PositionID"]) : 0;
                    PopulateRoleDropdown(currentPositionId);

                    // Set selected values
                    ddlDepartment.SelectedValue = ViewState["OriginalDepartmentId"].ToString();
                    ddlRole.SelectedValue = ViewState["OriginalPositionId"].ToString();
                }
                else
                {
                    lblMessage.Text = "Employee not found.";
                    pnlEmployee.Visible = false;
                }
            }
        }

        private void PopulateDepartmentDropdown(int locationId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DepartmentID, DepartmentName FROM Departments WHERE LocationID = @LocationID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LocationID", locationId);

                con.Open();
                ddlDepartment.DataSource = cmd.ExecuteReader();
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
            }
        }

        private void PopulateRoleDropdown(int currentPositionId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT p.PositionID, p.PositionName 
                FROM Position p
                WHERE p.PositionID = @CurrentPositionID
                OR p.PositionID = (
                    SELECT CASE 
                        WHEN @CurrentPositionID = 12 THEN 11  -- Intern can become Junior
                        WHEN @CurrentPositionID = 11 THEN 10  -- Junior can become Senior
                        WHEN @CurrentPositionID = 10 THEN 4   -- Senior can become Head of Pretoria
                        WHEN @CurrentPositionID = 4 THEN 3    -- Head of Pretoria can become Head of Cape Town
                        WHEN @CurrentPositionID = 3 THEN 2    -- Head of Cape Town can become Head of Sandton
                        WHEN @CurrentPositionID = 2 THEN 1    -- Head of Sandton can become CEO
                        ELSE @CurrentPositionID
                    END
                )
                ORDER BY PositionID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CurrentPositionID", currentPositionId);

                con.Open();
                ddlRole.DataSource = cmd.ExecuteReader();
                ddlRole.DataTextField = "PositionName";
                ddlRole.DataValueField = "PositionID";
                ddlRole.DataBind();
            }
        }

        private void SetAccessControls(string targetEmployeeNumber)
        {
            string currentUserNumber = Session["EmployeeNumber"].ToString();
            string currentUserRole = Session["Role"]?.ToString();
            int currentUserLocationId = Convert.ToInt32(Session["LocationID"]);

            // Get target employee's location and department
            int targetLocationId = -1;
            int targetDepartmentId = -1;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID, DepartmentID FROM Employees WHERE EmployeeNumber = @EmployeeNumber";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", targetEmployeeNumber);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    targetLocationId = reader["LocationID"] != DBNull.Value ? Convert.ToInt32(reader["LocationID"]) : -1;
                    targetDepartmentId = reader["DepartmentID"] != DBNull.Value ? Convert.ToInt32(reader["DepartmentID"]) : -1;
                }
            }

            bool isSelf = currentUserNumber == targetEmployeeNumber;
            bool isCEO = currentUserRole == "CEO";
            bool isHeadOfLocation = currentUserRole.StartsWith("Head of");
            bool isSeniorHR = currentUserRole == "Senior HR";
            bool sameLocation = currentUserLocationId == targetLocationId;
            bool isHRDepartment = targetDepartmentId == 1;

            bool canViewSalary = isSelf || isCEO || (isHeadOfLocation && sameLocation) || (isSeniorHR && sameLocation && isHRDepartment);
            bool canEditPersonalDetails = isSelf;
            bool canEditRoleDepartment = isCEO || isHeadOfLocation || isSeniorHR;

            if (!canViewSalary)
            {
                lblSalary.Text = "***";
                txtSalary.Visible = false;
            }

            btnEditEmployee.Visible = canEditPersonalDetails || canEditRoleDepartment;

            ViewState["CanEditPersonalDetails"] = canEditPersonalDetails;
            ViewState["CanEditRoleDepartment"] = canEditRoleDepartment;
            ViewState["CanViewSalary"] = canViewSalary;
            ViewState["IsSelf"] = isSelf;
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
            bool canEditPersonalDetails = (bool)ViewState["CanEditPersonalDetails"];
            bool canEditRoleDepartment = (bool)ViewState["CanEditRoleDepartment"];
            bool canViewSalary = (bool)ViewState["CanViewSalary"];
            bool isSelf = (bool)ViewState["IsSelf"];

            // Toggle visibility of fields
            lblEmployeeNumber.Visible = true;
            lblFirstName.Visible = !isEdit || !canEditPersonalDetails;
            lblLastName.Visible = !isEdit || !canEditPersonalDetails;
            lblBirthDate.Visible = !isEdit || !canEditPersonalDetails;
            lblEmail.Visible = !isEdit || !canEditPersonalDetails;
            lblSalary.Visible = true;
            lblRole.Visible = !isEdit || !canEditRoleDepartment;
            lblDepartment.Visible = !isEdit || !canEditRoleDepartment;
            lblManager.Visible = true;
            lblLocation.Visible = true;

            txtFirstName.Visible = isEdit && canEditPersonalDetails;
            txtLastName.Visible = isEdit && canEditPersonalDetails;
            txtBirthDate.Visible = isEdit && canEditPersonalDetails;
            txtEmail.Visible = isEdit && canEditPersonalDetails;
            txtSalary.Visible = isEdit && canViewSalary && canEditRoleDepartment;
            ddlRole.Visible = isEdit && canEditRoleDepartment;
            ddlDepartment.Visible = isEdit && canEditRoleDepartment;

            btnEditEmployee.Visible = !isEdit && (canEditPersonalDetails || canEditRoleDepartment);
            btnSaveEmployee.Visible = isEdit;
            btnCancelEdit.Visible = isEdit;
            btnChangeProfile.Visible = isEdit && isSelf;
            revSalary.Enabled = isEdit && canViewSalary && canEditRoleDepartment;
            rvSalary.Enabled = isEdit && canViewSalary && canEditRoleDepartment;
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            string employeeNumber = lblEmployeeNumber.Text;
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string birthDate = txtBirthDate.Text;
            string email = txtEmail.Text;
            string newEmployeeNumber = txtEmployeeNumber.Text;

            // Handle salary
            decimal salary = 0;
            if ((bool)ViewState["CanViewSalary"] && (bool)ViewState["CanEditRoleDepartment"])
            {
                string salaryString = txtSalary.Text.Trim();

                // Remove any commas and validate
                salaryString = salaryString.Replace(",", "");

                if (!decimal.TryParse(salaryString, out salary))
                {
                    lblMessage.Text = "Salary must be a valid number (e.g. 50000 or 50000.50)";
                    return;
                }

                if (salary < 0)
                {
                    lblMessage.Text = "Salary cannot be negative.";
                    return;
                }
            }
            else
            {
                decimal.TryParse(hdnOriginalSalary.Value, out salary);
            }

            // Get selected department and role
            int departmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
            int positionId = Convert.ToInt32(ddlRole.SelectedValue);

            SaveEmployeeData(employeeNumber, firstName, lastName, birthDate, email,
                           newEmployeeNumber, salary, positionId, departmentId);

            ToggleEditMode(false);
        }

        private void SaveEmployeeData(string employeeNumber, string firstName, string lastName,
                                    string birthDate, string email, string newEmployeeNumber,
                                    decimal salary, int positionId, int departmentId)
        {
            // Get original values
            int originalPositionId = Convert.ToInt32(ViewState["OriginalPositionId"]);
            int originalLocationId = Convert.ToInt32(ViewState["OriginalLocationId"]);

            // Validate department belongs to the location
            int newDepartmentLocationId = GetDepartmentLocation(departmentId);
            if (newDepartmentLocationId != originalLocationId)
            {
                lblMessage.Text = "Selected department doesn't belong to employee's location!";
                return;
            }

            // Validate role transition
            if (!IsValidRoleTransition(originalPositionId, positionId))
            {
                lblMessage.Text = "Invalid role transition!";
                return;
            }

            // Get new manager based on new role
            string newManagerId = GetManagerForNewRole(positionId, departmentId, originalLocationId);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                UPDATE Employees SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DOB = @BirthDate,
                    Email = @Email,
                    EmployeeNumber = @NewEmployeeNumber,
                    Salary = @Salary,
                    PositionID = @PositionID,
                    DepartmentID = @DepartmentID,
                    ManagerID = @ManagerID
                WHERE EmployeeNumber = @EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@NewEmployeeNumber", newEmployeeNumber);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@PositionID", positionId);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                cmd.Parameters.AddWithValue("@ManagerID", newManagerId ?? (object)DBNull.Value);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblMessage.Text = "Employee updated successfully!";
                        LoadEmployee(newEmployeeNumber);
                    }
                    else
                    {
                        lblMessage.Text = "No changes were made to the employee record.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error updating employee: " + ex.Message;
                }
            }
        }

        private int GetDepartmentLocation(int departmentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID FROM Departments WHERE DepartmentID = @DepartmentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);

                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private bool IsValidRoleTransition(int currentPositionId, int newPositionId)
        {
            // Allow keeping the same role
            if (currentPositionId == newPositionId) return true;

            // CEO can't be changed
            if (currentPositionId == 1) return false;

            // Define role hierarchy (most senior first)
            int[] roleHierarchy = { 1, 2, 3, 4, 10, 11, 12 };

            int currentIndex = Array.IndexOf(roleHierarchy, currentPositionId);
            int newIndex = Array.IndexOf(roleHierarchy, newPositionId);

            if (currentIndex == -1 || newIndex == -1) return false;

            // Only allow moving up by exactly one level
            return newIndex == currentIndex - 1;
        }

        private string GetManagerForNewRole(int newPositionId, int departmentId, int locationId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "";
                SqlCommand cmd = new SqlCommand();

                if (newPositionId == 1) // CEO has no manager
                {
                    return null;
                }
                else if (newPositionId >= 2 && newPositionId <= 4) // Heads report to CEO
                {
                    query = "SELECT TOP 1 EmployeeNumber FROM Employees WHERE PositionID = 1";
                }
                else if (newPositionId == 10) // Seniors report to Head of location
                {
                    query = @"SELECT TOP 1 EmployeeNumber FROM Employees 
                             WHERE LocationID = @LocationID 
                             AND PositionID IN (2, 3, 4)";
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                }
                else if (newPositionId == 11) // Juniors report to Senior in same department
                {
                    query = @"SELECT TOP 1 EmployeeNumber FROM Employees 
                             WHERE DepartmentID = @DepartmentID 
                             AND LocationID = @LocationID
                             AND PositionID = 10";
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                }
                else if (newPositionId == 12) // Interns report to Junior in same department
                {
                    query = @"SELECT TOP 1 EmployeeNumber FROM Employees 
                             WHERE DepartmentID = @DepartmentID 
                             AND LocationID = @LocationID
                             AND PositionID = 11";
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                }

                if (string.IsNullOrEmpty(query)) return null;

                cmd.Connection = con;
                cmd.CommandText = query;

                con.Open();
                object result = cmd.ExecuteScalar();
                return result?.ToString();
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }
}