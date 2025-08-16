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
                    e.Role, 
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
                    txtRole.Text = lblRole.Text;
                    txtDepartment.Text = lblDepartment.Text;

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
                }
                else
                {
                    lblMessage.Text = "Employee not found.";
                    pnlEmployee.Visible = false;
                }
            }
        }

        private void SetAccessControls(string targetEmployeeNumber)
        {
            string currentUserNumber = Session["EmployeeNumber"].ToString();
            string currentUserRole = Session["Role"]?.ToString();
            int currentUserLocationId = Convert.ToInt32(Session["LocationID"]);

            // Get target employee's location and department
            int targetLocationId = -1;
            string targetDepartment = "";

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
                    targetDepartment = reader["DepartmentID"] != DBNull.Value ? reader["DepartmentID"].ToString() : "";
                }
            }

            bool isSelf = currentUserNumber == targetEmployeeNumber;
            bool isCEO = currentUserRole == "CEO";
            bool isHeadOfLocation = currentUserRole.StartsWith("Head of");
            bool isSeniorHR = currentUserRole == "Senior HR";
            bool sameLocation = currentUserLocationId == targetLocationId;
            bool isHRDepartment = targetDepartment == "1"; // Assuming 1 is HR department ID

            // Can view salary if self, CEO, Head of Location, or Senior HR
            bool canViewSalary = isSelf || isCEO || (isHeadOfLocation && sameLocation) || (isSeniorHR && sameLocation && isHRDepartment);

            // Can edit personal details if it's their own profile
            bool canEditPersonalDetails = isSelf;

            // Can edit role/department if CEO, Head of Location, or Senior HR
            bool canEditRoleDepartment = isCEO || isHeadOfLocation || isSeniorHR;

            // Apply salary visibility
            if (!canViewSalary)
            {
                lblSalary.Text = "***";
                txtSalary.Visible = false;
            }

            // Enable Edit button only if the user can edit something
            btnEditEmployee.Visible = canEditPersonalDetails || canEditRoleDepartment;

            // Store permissions in ViewState
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
            bool canEditPersonalDetails = ViewState["CanEditPersonalDetails"] != null && (bool)ViewState["CanEditPersonalDetails"];
            bool canEditRoleDepartment = ViewState["CanEditRoleDepartment"] != null && (bool)ViewState["CanEditRoleDepartment"];
            bool canViewSalary = ViewState["CanViewSalary"] != null && (bool)ViewState["CanViewSalary"];
            bool isSelf = ViewState["IsSelf"] != null && (bool)ViewState["IsSelf"];

            // If not in edit mode, show all labels and hide edit fields
            if (!isEdit)
            {
                // Show all labels
                lblEmployeeNumber.Visible = true;
                lblFirstName.Visible = true;
                lblLastName.Visible = true;
                lblBirthDate.Visible = true;
                lblEmail.Visible = true;
                lblSalary.Visible = true;
                lblRole.Visible = true;
                lblDepartment.Visible = true;
                lblManager.Visible = true;
                lblLocation.Visible = true;

                // Hide all edit fields
                txtFirstName.Visible = false;
                txtLastName.Visible = false;
                txtBirthDate.Visible = false;
                txtEmail.Visible = false;
                txtSalary.Visible = false;
                txtRole.Visible = false;
                txtDepartment.Visible = false;

                // Show Edit button if allowed
                btnEditEmployee.Visible = canEditPersonalDetails || canEditRoleDepartment;
            }
            else // In edit mode
            {
                // Always show all labels (view fields)
                lblEmployeeNumber.Visible = true;
                lblFirstName.Visible = false;
                lblLastName.Visible = false;
                lblBirthDate.Visible = false;
                lblEmail.Visible = false;
                lblSalary.Visible = true;
                lblRole.Visible = true;
                lblDepartment.Visible = true;
                lblManager.Visible = true;
                lblLocation.Visible = true;

                // Show edit fields only for allowed fields
                txtFirstName.Visible = canEditPersonalDetails;
                txtLastName.Visible = canEditPersonalDetails;
                txtBirthDate.Visible = canEditPersonalDetails;
                txtEmail.Visible = canEditPersonalDetails;

                // Salary, Role, Department are editable only by admins
                txtSalary.Visible = canViewSalary && (canEditRoleDepartment);
                txtRole.Visible = canEditRoleDepartment;
                txtDepartment.Visible = canEditRoleDepartment;
            }

            // Toggle buttons
            btnEditEmployee.Visible = !isEdit && (canEditPersonalDetails || canEditRoleDepartment);
            btnSaveEmployee.Visible = isEdit;
            btnCancelEdit.Visible = isEdit;
            btnChangeProfile.Visible = isEdit && isSelf; // Only allow changing profile if it's their own profile
            revSalary.Enabled = isEdit && canViewSalary && canEditRoleDepartment;
            rvSalary.Enabled = isEdit && canViewSalary && canEditRoleDepartment;
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            string currentUserRole = Session["Role"]?.ToString();
            bool isCEO = currentUserRole == "CEO";
            bool isHeadOfLocation = currentUserRole.StartsWith("Head of");

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
                string salaryString = txtSalary.Text.Replace("$", "").Replace(",", "").Trim();

                // Check if salary is numeric
                if (!decimal.TryParse(salaryString, out salary))
                {
                    lblMessage.Text = "Salary must be a valid number.";
                    return;
                }

                // Check if salary is non-negative
                if (salary < 0)
                {
                    lblMessage.Text = "Salary cannot be negative.";
                    return;
                }
            }
            else
            {
                lblMessage.Text = "Invalid salary format. Please enter a valid number.";
                    return;
                decimal.TryParse(hdnOriginalSalary.Value, out salary);
            }
                   

            // Handle role - only editable by CEO/Head of Location
            string role = lblRole.Text; // Default to existing
            if ((isCEO || isHeadOfLocation) && txtRole.Visible)
            {
                role = txtRole.Text;
            }

            string department = txtDepartment.Text;

            SaveEmployeeData(employeeNumber, firstName, lastName, birthDate, email, newEmployeeNumber, salary, role, department);

            // Reload the employee data
            LoadEmployee(employeeNumber);
            SetAccessControls(employeeNumber);
            ToggleEditMode(false);
            lblMessage.Text = "Changes saved successfully.";
        }

        private void SaveEmployeeData(string employeeNumber, string firstName, string lastName,
                                    string birthDate, string email, string newEmployeeNumber,
                                    decimal salary, string role, string department)
        {
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
                    Role = @Role
                WHERE EmployeeNumber = @EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@NewEmployeeNumber", newEmployeeNumber);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@Role", role);

                con.Open();
                cmd.ExecuteNonQuery();

                // Note: Department would need additional handling since it's likely a foreign key
                // You might need a separate update for department if it's changeable
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