using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections.Generic;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        private string employeeEmail;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;

            if (Session["EmployeeID"] == null || Session["RoleID"] == null || Session["LocationID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string empId = Request.QueryString["empId"] ?? Session["EmployeeID"].ToString();

                if (!string.IsNullOrEmpty(empId))
                {
                    // Set access controls first
                    SetAccessControls(empId);

                    // Then load data
                    PopulateRoleDropdown();
                    PopulateManagerDropdown();
                    LoadEmployee(empId);

                    // Set initial button visibility
                    btnEditEmployee.Visible = (bool)ViewState["ShowEditButton"];
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
                e.EmployeeID,
                e.FirstName, 
                e.LastName, 
                e.DateOfBirth,
                e.Email,
                d.DepartmentName,
                p.PositionName AS PositionTitle,
                l.LocationName AS Location,
                m.FirstName + ' ' + m.LastName AS ManagerName,
                s.Amount AS Salary,
                ep.Picture AS ProfilePicture,
                e.DepartmentID,
                e.PositionID,
                l.LocationID,
                r.RoleDesc,
                ua.RoleID
            FROM EMPLOYEES e
            LEFT JOIN DEPARTMENT d ON e.DepartmentID = d.DepartmentID
            LEFT JOIN LOCATION l ON d.LocationID = l.LocationID
            LEFT JOIN POSITION p ON e.PositionID = p.PositionID
            LEFT JOIN REPORTING_LINE rl ON e.EmployeeID = rl.ReportEmployeeID
            LEFT JOIN EMPLOYEES m ON rl.ManagerEmployeeID = m.EmployeeID
            LEFT JOIN SALARY s ON e.EmployeeID = s.EmployeeID
            LEFT JOIN EMPLOYEE_PICTURE ep ON e.EmployeeID = ep.EmployeeID
            LEFT JOIN USER_AUTH ua ON e.EmployeeID = ua.EmployeeID
            LEFT JOIN ROLE r ON ua.RoleID = r.RoleID
            WHERE e.EmployeeID = @EmployeeID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeID", empId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    pnlEmployee.Visible = true;
                    employeeEmail = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                    // Display all fields
                    lblEmployeeID.Text = reader["EmployeeID"].ToString();
                    lblFirstName.Text = reader["FirstName"].ToString();
                    lblLastName.Text = reader["LastName"].ToString();
                    lblBirthDate.Text = reader["DateOfBirth"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd")
                        : "N/A";
                    lblEmail.Text = employeeEmail;
                    lblDepartment.Text = reader["DepartmentName"] != DBNull.Value
                        ? reader["DepartmentName"].ToString()
                        : "N/A";
                    lblPosition.Text = reader["PositionTitle"] != DBNull.Value
                        ? reader["PositionTitle"].ToString()
                        : "N/A";
                    lblLocation.Text = reader["Location"] != DBNull.Value
                        ? reader["Location"].ToString()
                        : "N/A";
                    lblManager.Text = reader["ManagerName"] != DBNull.Value
                        ? reader["ManagerName"].ToString()
                        : "N/A";
                    lblAccess.Text = reader["RoleDesc"] != DBNull.Value
                        ? reader["RoleDesc"].ToString()
                        : "N/A";

                    ViewState["OriginalRoleId"] = reader["RoleID"] != DBNull.Value
                        ? reader["RoleID"].ToString()
                        : "";

                    // Handle salary display
                    if (reader["Salary"] != DBNull.Value)
                    {
                        decimal salary = Convert.ToDecimal(reader["Salary"]);
                        lblSalary.Text = string.Format("{0:C}", salary);
                        hdnOriginalSalary.Value = salary.ToString();
                    }
                    else
                    {
                        lblSalary.Text = "N/A";
                        hdnOriginalSalary.Value = "0";
                    }

                    // Handle profile picture
                    if (reader["ProfilePicture"] != DBNull.Value && !string.IsNullOrEmpty(reader["ProfilePicture"].ToString()))
                    {
                        byte[] imageBytes = (byte[])reader["ProfilePicture"];
                        string base64String = Convert.ToBase64String(imageBytes);
                        imgGravatar.ImageUrl = "data:image/jpeg;base64," + base64String;
                    }
                    else if (!string.IsNullOrEmpty(employeeEmail))
                    {
                        imgGravatar.ImageUrl = GetGravatarUrl(employeeEmail);
                    }
                    else
                    {
                        imgGravatar.ImageUrl = "~/Images/default-profile.png";
                    }

                    // Store original values for editing
                    ViewState["OriginalDepartmentId"] = reader["DepartmentID"] != DBNull.Value
                        ? reader["DepartmentID"].ToString()
                        : "";
                    ViewState["OriginalPositionId"] = reader["PositionID"] != DBNull.Value
                        ? reader["PositionID"].ToString()
                        : "";
                    ViewState["OriginalLocationId"] = reader["LocationID"] != DBNull.Value
                        ? reader["LocationID"].ToString()
                        : "";

                    // Populate dropdowns
                    PopulateDepartmentDropdown(Convert.ToInt32(ViewState["OriginalLocationId"]));
                    PopulateLocationDropdown();
                    PopulatePositionDropdown(Convert.ToInt32(ViewState["OriginalPositionId"]));

                    // Set selected values
                    if (ddlDepartment.Items.FindByValue(ViewState["OriginalDepartmentId"].ToString()) != null)
                        ddlDepartment.SelectedValue = ViewState["OriginalDepartmentId"].ToString();

                    if (ddlRole.Items.FindByValue(ViewState["OriginalPositionId"].ToString()) != null)
                        ddlRole.SelectedValue = ViewState["OriginalPositionId"].ToString();

                    if (ddlLocation.Items.FindByValue(ViewState["OriginalLocationId"].ToString()) != null)
                        ddlLocation.SelectedValue = ViewState["OriginalLocationId"].ToString();
                }
                else
                {
                    lblMessage.Text = "Employee not found.";
                    pnlEmployee.Visible = false;
                }
            }
        }

        private void PopulateRoleDropdown()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT RoleID, RoleDesc FROM ROLE ORDER BY RoleID";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                ddlAccess.DataSource = cmd.ExecuteReader();
                ddlAccess.DataTextField = "RoleDesc";
                ddlAccess.DataValueField = "RoleID";
                ddlAccess.DataBind();

                if (ViewState["OriginalRoleId"] != null)
                {
                    ddlAccess.SelectedValue = ViewState["OriginalRoleId"].ToString();
                }
            }
        }

        private void PopulatePositionDropdown(int currentPositionId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT PositionID, PositionName 
                               FROM POSITION
                               ORDER BY PositionName";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                ddlRole.DataSource = cmd.ExecuteReader();
                ddlRole.DataTextField = "PositionName";
                ddlRole.DataValueField = "PositionID";
                ddlRole.DataBind();
            }
        }

        private void PopulateDepartmentDropdown(int locationId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DepartmentID, DepartmentName FROM DEPARTMENT WHERE LocationID = @LocationID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LocationID", locationId);

                con.Open();
                ddlDepartment.DataSource = cmd.ExecuteReader();
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
            }
        }

        private void SetAccessControls(string targetEmployeeId)
        {
            if (Session["EmployeeID"] == null || Session["RoleID"] == null || Session["LocationID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string currentUserId = Session["EmployeeID"].ToString();
            int currentUserRoleId = Convert.ToInt32(Session["RoleID"]);
            int currentUserLocationId = Convert.ToInt32(Session["LocationID"]);

            // Get target employee's location
            int targetLocationId = GetEmployeeLocation(targetEmployeeId);
            bool isSelf = currentUserId == targetEmployeeId;
            bool sameLocation = currentUserLocationId == targetLocationId;

            // Initialize all access controls with default values
            ViewState["CanViewSalary"] = false;
            ViewState["CanEditRoleDepartment"] = false;
            ViewState["ShowEditButton"] = false;

            // Set access based on role
            switch (currentUserRoleId)
            {
                case 1: // Admin - full access
                    ViewState["CanViewSalary"] = true;
                    ViewState["CanEditRoleDepartment"] = true;
                    ViewState["ShowEditButton"] = true;
                    break;
                case 2: // Manager - can edit same location
                    ViewState["CanViewSalary"] = sameLocation;
                    ViewState["CanEditRoleDepartment"] = sameLocation;
                    ViewState["ShowEditButton"] = sameLocation;
                    break;
                case 3: // Regular user - can edit only self
                    ViewState["CanViewSalary"] = false;
                    ViewState["CanEditRoleDepartment"] = false;
                    ViewState["ShowEditButton"] = isSelf;
                    break;
            }

            // Store additional access info
            ViewState["IsSelf"] = isSelf;
            ViewState["SameLocation"] = sameLocation;
            ViewState["CurrentUserRoleId"] = currentUserRoleId;
        }

        private int GetEmployeeLocation(string employeeId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT d.LocationID 
                               FROM EMPLOYEES e
                               JOIN DEPARTMENT d ON e.DepartmentID = d.DepartmentID
                               WHERE e.EmployeeID = @EmployeeID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        protected void btnEditEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleEditMode(true);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error entering edit mode: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Edit error: " + ex.ToString());
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ToggleEditMode(false);
        }

        private void ToggleEditMode(bool isEdit)
        {
            bool canViewSalary = ViewState["CanViewSalary"] != null && (bool)ViewState["CanViewSalary"];
            bool canEditRoleDept = ViewState["CanEditRoleDepartment"] != null && (bool)ViewState["CanEditRoleDepartment"];
            bool isSelf = ViewState["IsSelf"] != null && (bool)ViewState["IsSelf"];
            bool sameLocation = ViewState["SameLocation"] != null && (bool)ViewState["SameLocation"];
            int currentUserRoleId = ViewState["CurrentUserRoleId"] != null ? (int)ViewState["CurrentUserRoleId"] : 3;

            // Basic info - always editable if you have edit access
            lblFirstName.Visible = !isEdit;
            txtFirstName.Visible = isEdit;
            txtFirstName.Text = lblFirstName.Text;

            lblLastName.Visible = !isEdit;
            txtLastName.Visible = isEdit;
            txtLastName.Text = lblLastName.Text;

            lblBirthDate.Visible = !isEdit;
            txtBirthDate.Visible = isEdit;
            txtBirthDate.Text = lblBirthDate.Text == "N/A" ? "" : lblBirthDate.Text;

            lblEmail.Visible = !isEdit;
            txtEmail.Visible = isEdit;
            txtEmail.Text = lblEmail.Text;

            // Role/department info
            lblPosition.Visible = !isEdit || !canEditRoleDept;
            ddlRole.Visible = isEdit && canEditRoleDept;
            lblDepartment.Visible = !isEdit || !canEditRoleDept;
            ddlDepartment.Visible = isEdit && canEditRoleDept;
            lblLocation.Visible = !isEdit || !canEditRoleDept;
            ddlLocation.Visible = isEdit && canEditRoleDept;

            // Salary info
            lblSalary.Visible = canEditRoleDept;
            txtSalary.Visible = isEdit && canEditRoleDept;
            if (txtSalary.Visible)
            {
                txtSalary.Text = hdnOriginalSalary.Value == "0" ? "" : hdnOriginalSalary.Value;
            }

            // Manager dropdown
            lblManager.Visible = !isEdit || !canEditRoleDept;
            ddlManager.Visible = isEdit && canEditRoleDept;
            if (ddlManager.Visible && !IsPostBack)
            {
                PopulateManagerDropdown();
            }

            // Access info
            lblAccess.Visible = !isEdit;
            ddlAccess.Visible = isEdit && currentUserRoleId == 1;

            // Buttons
            btnEditEmployee.Visible = !isEdit && (bool)ViewState["ShowEditButton"];
            btnSaveEmployee.Visible = isEdit;
            btnCancelEdit.Visible = isEdit;
            btnChangeProfile.Visible = isEdit;
        }

        private void PopulateManagerDropdown()
        {
            string currentEmployeeId = lblEmployeeID.Text;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT e.EmployeeID, e.FirstName + ' ' + e.LastName AS ManagerName 
                               FROM EMPLOYEES e
                               WHERE e.EmployeeID <> @CurrentEmployeeID
                               ORDER BY ManagerName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CurrentEmployeeID", currentEmployeeId);
                con.Open();

                ddlManager.DataSource = cmd.ExecuteReader();
                ddlManager.DataTextField = "ManagerName";
                ddlManager.DataValueField = "EmployeeID";
                ddlManager.DataBind();

                // Add empty option
                ddlManager.Items.Insert(0, new ListItem("-- No Manager --", ""));

                // Set current manager if exists
                string currentManagerId = GetCurrentManagerId();
                if (!string.IsNullOrEmpty(currentManagerId))
                {
                    ddlManager.SelectedValue = currentManagerId;
                }
            }
        }

        private string GetCurrentManagerId()
        {
            string currentEmployeeId = lblEmployeeID.Text;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT ManagerEmployeeID 
                               FROM REPORTING_LINE 
                               WHERE ReportEmployeeID = @EmployeeID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
                con.Open();

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            bool canViewSalary = ViewState["CanViewSalary"] != null && (bool)ViewState["CanViewSalary"];
            bool canEditRoleDept = ViewState["CanEditRoleDepartment"] != null && (bool)ViewState["CanEditRoleDepartment"];

            string employeeId = lblEmployeeID.Text;
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string birthDate = txtBirthDate.Text;
            string email = txtEmail.Text;

            // Handle salary
            decimal salary = 0;
            if (canViewSalary && canEditRoleDept)
            {
                string salaryString = txtSalary.Text.Trim().Replace(",", "");

                if (!decimal.TryParse(salaryString, NumberStyles.Any, CultureInfo.InvariantCulture, out salary))
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

            SaveEmployeeData(employeeId, firstName, lastName, birthDate, email, salary, positionId, departmentId);

            ToggleEditMode(false);
        }

        private void SaveEmployeeData(string employeeId, string firstName, string lastName,
    string birthDate, string email, decimal salary, int positionId, int departmentId)
        {
            // Get original values
            int originalPositionId = Convert.ToInt32(ViewState["OriginalPositionId"]);
            int originalLocationId = Convert.ToInt32(ViewState["OriginalLocationId"]);
            int newLocationId = ddlLocation.SelectedIndex >= 0 ? Convert.ToInt32(ddlLocation.SelectedValue) : originalLocationId;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Update basic employee info
                    string employeeQuery = @"
                UPDATE EMPLOYEES SET 
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    Email = @Email,
                    DepartmentID = @DepartmentID,
                    PositionID = @PositionID
                WHERE EmployeeID = @EmployeeID";

                    SqlCommand cmd = new SqlCommand(employeeQuery, con, transaction);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", birthDate);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    cmd.Parameters.AddWithValue("@PositionID", positionId);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Update salary if changed
                        if (txtSalary.Visible)
                        {
                            string salaryQuery = @"
                        IF EXISTS (SELECT 1 FROM SALARY WHERE EmployeeID = @EmployeeID)
                            UPDATE SALARY SET Amount = @Amount WHERE EmployeeID = @EmployeeID
                        ELSE
                            INSERT INTO SALARY (EmployeeID, Amount) VALUES (@EmployeeID, @Amount)";

                            cmd = new SqlCommand(salaryQuery, con, transaction);
                            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                            cmd.Parameters.AddWithValue("@Amount", salary);
                            cmd.ExecuteNonQuery();
                        }

                        // Update manager relationship if changed
                        if (ddlManager.Visible && ddlManager.SelectedIndex >= 0)
                        {
                            string newManagerId = ddlManager.SelectedValue;
                            if (!string.IsNullOrEmpty(newManagerId) && newManagerId != employeeId)
                            {
                                UpdateManagerRelationship(con, transaction, employeeId, newManagerId);
                            }
                        }

                        transaction.Commit();
                        lblMessage.Text = "Employee updated successfully!";
                        LoadEmployee(employeeId);
                    }
                    else
                    {
                        transaction.Rollback();
                        lblMessage.Text = "No changes were made to the employee record.";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    lblMessage.Text = "Error updating employee: " + ex.Message;
                }
            }
        }

        private void UpdateManagerRelationship(SqlConnection con, SqlTransaction transaction,
     string employeeId, string newManagerId)
        {
            // Delete existing relationship if any
            string deleteQuery = "DELETE FROM REPORTING_LINE WHERE ReportEmployeeID = @EmployeeID";
            SqlCommand cmd = new SqlCommand(deleteQuery, con, transaction);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.ExecuteNonQuery();

            // Add new relationship if manager selected
            if (!string.IsNullOrEmpty(newManagerId))
            {
                string insertQuery = @"INSERT INTO REPORTING_LINE (ReportEmployeeID, ManagerEmployeeID) 
                            VALUES (@ReportEmployeeID, @ManagerEmployeeID)";
                cmd = new SqlCommand(insertQuery, con, transaction);
                cmd.Parameters.AddWithValue("@ReportEmployeeID", employeeId);
                cmd.Parameters.AddWithValue("@ManagerEmployeeID", newManagerId);
                cmd.ExecuteNonQuery();
            }
        }

        private int GetDepartmentLocation(int departmentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);

                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private bool DepartmentExistsInLocation(int departmentId, int locationId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM DEPARTMENT WHERE DepartmentID = @DepartmentID AND LocationID = @LocationID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private bool IsValidRoleTransition(int currentPositionId, int newPositionId)
        {
            if (currentPositionId == newPositionId) return true;
            if (currentPositionId == 1) return false;

            int[] roleHierarchy = { 1, 2, 3, 4, 10, 11, 12 };

            int currentIndex = Array.IndexOf(roleHierarchy, currentPositionId);
            int newIndex = Array.IndexOf(roleHierarchy, newPositionId);

            if (currentIndex == -1 || newIndex == -1) return false;
            return newIndex == currentIndex - 1;
        }

        private bool IsUniqueRoleAllowed(int positionId, int departmentId, int locationId, string currentEmployeeId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "";
                if (positionId == 1)
                {
                    query = @"SELECT COUNT(*) FROM EMPLOYEES 
                           WHERE PositionID = @PositionID 
                           AND LocationID = @LocationID
                           AND EmployeeID <> @EmployeeID";
                }
                else if (positionId == 2 || positionId == 3 || positionId == 4)
                {
                    query = @"SELECT COUNT(*) FROM EMPLOYEES 
                           WHERE PositionID = @PositionID 
                           AND LocationID = @LocationID
                           AND EmployeeID <> @EmployeeID";
                }
                else
                {
                    return true;
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PositionID", positionId);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count == 0;
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
            string employeeId = lblEmployeeID.Text;
            Response.Redirect($"ChangeProfilePhoto.aspx?empId={employeeId}");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        private void PopulateLocationDropdown()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID, LocationName FROM LOCATION ORDER BY LocationName";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                ddlLocation.DataSource = cmd.ExecuteReader();
                ddlLocation.DataTextField = "LocationName";
                ddlLocation.DataValueField = "LocationID";
                ddlLocation.DataBind();
            }
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLocation.SelectedValue != ViewState["OriginalLocationId"].ToString())
            {
                int newLocationId = Convert.ToInt32(ddlLocation.SelectedValue);
                int currentDepartmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
                GetDepartmentLocation(currentDepartmentId);
                if (!DepartmentExistsInLocation(currentDepartmentId, newLocationId))
                {
                    lblMessage.Text = "Current department doesn't exist in the new location. Please select a different department.";
                    PopulateDepartmentDropdown(newLocationId);
                    ddlDepartment.SelectedIndex = 0;
                }
            }
        }
    }
}