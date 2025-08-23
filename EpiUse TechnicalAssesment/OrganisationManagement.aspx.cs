using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EpiUse_TechnicalAssesment
{
    public partial class AddEmployee : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Handle postback for department reassign modal - MUST be before !IsPostBack check
            if (IsPostBack)
            {
                System.Diagnostics.Debug.WriteLine("=== PAGE IS POSTBACK ===");

                // Check if we need to rebind the dropdown
                if (ViewState["TargetDepartments"] != null)
                {
                    DataTable targetDepartments = (DataTable)ViewState["TargetDepartments"];
                    System.Diagnostics.Debug.WriteLine("Rebinding dropdown from ViewState. Departments count: " + targetDepartments.Rows.Count);

                    BindTargetDepartmentsDropdown(targetDepartments);

                    // Restore other values
                    if (ViewState["SourceDepartmentId"] != null)
                    {
                        int departmentId = Convert.ToInt32(ViewState["SourceDepartmentId"]);
                        hdnDepartmentToDelete.Value = departmentId.ToString();

                        int locationId = GetDepartmentLocationId(departmentId);
                        string locationName = GetLocationName(locationId);
                        hdnDepartmentLocationId.Value = locationId.ToString();
                        lblDepartmentLocation.Text = locationName;

                        int employeeCount = GetEmployeeCountInDepartment(departmentId);
                        lblEmployeeCount.Text = employeeCount.ToString();

                        System.Diagnostics.Debug.WriteLine("Restored values from ViewState");
                    }
                }

                // DEBUG: Check dropdown state after postback
                System.Diagnostics.Debug.WriteLine("Dropdown items count in Page_Load: " + ddlTargetDepartment.Items.Count);
            }

            if (!IsPostBack)
            {
                DebugDepartmentData();
                PopulateDropdowns();
                BindGridViews();
                activeTabHidden.Value = "employeeTab";
                UpdateTabVisibility();
            }
            else
            {
                UpdateTabVisibility();
            }
        }

        protected void SwitchTab(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            string tabName = button.CommandArgument;
            activeTabHidden.Value = tabName;
        }

        private void UpdateTabVisibility()
        {
            employeeTab.Style["display"] = "none";
            departmentTab.Style["display"] = "none";
            positionTab.Style["display"] = "none";
            locationTab.Style["display"] = "none";

            lbEmployeeTab.CssClass = lbEmployeeTab.CssClass.Replace("active", "").Trim();
            lbDepartmentTab.CssClass = lbDepartmentTab.CssClass.Replace("active", "").Trim();
            lbPositionTab.CssClass = lbPositionTab.CssClass.Replace("active", "").Trim();
            lbLocationTab.CssClass = lbLocationTab.CssClass.Replace("active", "").Trim();

            switch (activeTabHidden.Value)
            {
                case "employeeTab":
                    employeeTab.Style["display"] = "block";
                    lbEmployeeTab.CssClass += " active";
                    break;
                case "departmentTab":
                    departmentTab.Style["display"] = "block";
                    lbDepartmentTab.CssClass += " active";
                    break;
                case "positionTab":
                    positionTab.Style["display"] = "block";
                    lbPositionTab.CssClass += " active";
                    break;
                case "locationTab":
                    locationTab.Style["display"] = "block";
                    lbLocationTab.CssClass += " active";
                    break;
            }
        }

        private void PopulateDropdowns()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Position dropdown
                string positionQuery = "SELECT PositionID, PositionName FROM POSITION";
                using (SqlCommand cmdPosition = new SqlCommand(positionQuery, connection))
                {
                    using (SqlDataReader reader = cmdPosition.ExecuteReader())
                    {
                        positionDropdown.Items.Clear();
                        while (reader.Read())
                        {
                            positionDropdown.Items.Add(new ListItem(reader["PositionName"].ToString(), reader["PositionID"].ToString()));
                        }
                    }
                }

                // Department dropdown (for employee form)
                string departmentQuery = "SELECT DepartmentID, DepartmentName FROM DEPARTMENT ORDER BY DepartmentName";
                using (SqlCommand cmdDepartment = new SqlCommand(departmentQuery, connection))
                {
                    using (SqlDataReader departmentReader = cmdDepartment.ExecuteReader())
                    {
                        departmentDropdown.Items.Clear();
                        departmentDropdown.Items.Add(new ListItem("-- Select Department --", ""));
                        while (departmentReader.Read())
                        {
                            departmentDropdown.Items.Add(new ListItem(
                                departmentReader["DepartmentName"].ToString(),
                                departmentReader["DepartmentID"].ToString()
                            ));
                        }
                    }
                }

                // Location dropdown (for employee form)
                string locationQuery = "SELECT LocationID, LocationName FROM LOCATION ORDER BY LocationName";
                using (SqlCommand cmdLocation = new SqlCommand(locationQuery, connection))
                {
                    using (SqlDataReader locationReader = cmdLocation.ExecuteReader())
                    {
                        locationDropdown.Items.Clear(); // Fix: Populate locationDropdown instead of ddlDepartmentLocation
                        locationDropdown.Items.Add(new ListItem("-- Select Location --", ""));
                        while (locationReader.Read())
                        {
                            locationDropdown.Items.Add(new ListItem(
                                locationReader["LocationName"].ToString(),
                                locationReader["LocationID"].ToString()
                            ));
                        }
                    }
                }

                // Department Location dropdown (for department form - keep this separate)
                string departmentLocationQuery = "SELECT LocationID, LocationName FROM LOCATION ORDER BY LocationName";
                using (SqlCommand cmdDeptLocation = new SqlCommand(departmentLocationQuery, connection))
                {
                    using (SqlDataReader deptLocationReader = cmdDeptLocation.ExecuteReader())
                    {
                        ddlDepartmentLocation.Items.Clear();
                        ddlDepartmentLocation.Items.Add(new ListItem("-- Select Location --", ""));
                        while (deptLocationReader.Read())
                        {
                            ddlDepartmentLocation.Items.Add(new ListItem(
                                deptLocationReader["LocationName"].ToString(),
                                deptLocationReader["LocationID"].ToString()
                            ));
                        }
                    }
                }

                // Manager dropdown
                string managerQuery = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM EMPLOYEES ORDER BY FirstName, LastName";
                using (SqlCommand cmdManager = new SqlCommand(managerQuery, connection))
                {
                    using (SqlDataReader managerReader = cmdManager.ExecuteReader())
                    {
                        managerDropdown.Items.Clear();
                        managerDropdown.Items.Add(new ListItem("-- No Manager --", "0"));
                        while (managerReader.Read())
                        {
                            managerDropdown.Items.Add(new ListItem(managerReader["FullName"].ToString(), managerReader["EmployeeID"].ToString()));
                        }
                    }
                }
            }
        }
        private void BindGridViews()
        {
            BindEmployeesGridView();
            BindDepartmentsGridView();
            BindPositionsGridView();
            BindLocationsGridView();
        }

        private void BindEmployeesGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT e.EmployeeID, e.FirstName, e.LastName, e.Email, 
                                d.DepartmentName, p.PositionName 
                                FROM EMPLOYEES e
                                LEFT JOIN DEPARTMENT d ON e.DepartmentID = d.DepartmentID
                                LEFT JOIN POSITION p ON e.PositionID = p.PositionID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvEmployees.DataSource = dt;
                        gvEmployees.DataBind();
                    }
                }
            }
        }

        private void BindDepartmentsGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT d.DepartmentID, d.DepartmentName, l.LocationName, 
                        COUNT(e.EmployeeID) AS EmployeeCount
                        FROM DEPARTMENT d
                        LEFT JOIN LOCATION l ON d.LocationID = l.LocationID
                        LEFT JOIN EMPLOYEES e ON d.DepartmentID = e.DepartmentID
                        GROUP BY d.DepartmentID, d.DepartmentName, l.LocationName
                        ORDER BY d.DepartmentName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvDepartments.DataSource = dt;
                        gvDepartments.DataBind();
                    }
                }
            }
        }

        private void BindPositionsGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT PositionID, PositionName FROM POSITION";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvPositions.DataSource = dt;
                        gvPositions.DataBind();
                    }
                }
            }
        }

        private void BindLocationsGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID, LocationName FROM LOCATION";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvLocations.DataSource = dt;
                        gvLocations.DataBind();
                    }
                }
            }
        }

        protected void addEmployee(object sender, EventArgs e)
        {
            validationMessage.Text = "";


            string firstName = firstNameTextbox.Text.Trim();
            string lastName = lastNameTextbox.Text.Trim();
            string dobString = dobTextbox.Text.Trim();
            string email = emailTextbox.Text.Trim();
            string password = passwordTextbox.Text;
            string confirmPass = confirmPassword.Text;
            string salaryString = salaryTextbox.Text.Trim();

            // Validation logic
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(dobString) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPass) ||
                string.IsNullOrEmpty(salaryString))
            {
                validationMessage.Text = "All fields are required.<br />";
                return;
            }

            Regex nameRegex = new Regex("^[a-zA-Z\\-\\s]+$");
            if (!nameRegex.IsMatch(firstName) || !nameRegex.IsMatch(lastName))
            {
                validationMessage.Text = "First and last name may not contain special characters or numbers.<br />";
                return;
            }

            DateTime dob;
            if (!DateTime.TryParse(dobString, out dob))
            {
                validationMessage.Text = "Invalid date format.<br />";
                return;
            }
            if (dob >= new DateTime(2025, 1, 1))
            {
                validationMessage.Text = "Date of birth must be before 2025/01/01.<br />";
                return;
            }

            if (!email.Contains("@"))
            {
                validationMessage.Text = "Email must contain an @ symbol.<br />";
                return;
            }

            if (password != confirmPass)
            {
                validationMessage.Text = "Passwords do not match.<br />";
                return;
            }

            decimal salary;
            if (!decimal.TryParse(salaryString, out salary) || salary < 0)
            {
                validationMessage.Text = "Salary may not be negative or have letters.<br />";
                return;
            }

            // Database Insertion
            int positionId = Convert.ToInt32(positionDropdown.SelectedValue);
            int departmentId = Convert.ToInt32(departmentDropdown.SelectedValue);
            int locationId = Convert.ToInt32(locationDropdown.SelectedValue); // Add this line
            int managerId = Convert.ToInt32(managerDropdown.SelectedValue);
            string hashedPassword = HashPassword(password);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    string employeeInsertQuery = "INSERT INTO EMPLOYEES (FirstName, LastName, DateOfBirth, Email, DepartmentID, PositionID) VALUES (@FirstName, @LastName, @DOB, @Email, @DepartmentID, @PositionID); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(employeeInsertQuery, connection, transaction);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@DOB", dob);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    cmd.Parameters.AddWithValue("@PositionID", positionId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);

                    int employeeId = Convert.ToInt32(cmd.ExecuteScalar());

                    string userAuthInsertQuery = "INSERT INTO USER_AUTH (EmployeeID, Password, RoleID) VALUES (@EmployeeID, @Password, @RoleID)";
                    SqlCommand cmdAuth = new SqlCommand(userAuthInsertQuery, connection, transaction);
                    cmdAuth.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmdAuth.Parameters.AddWithValue("@Password", hashedPassword);
                    cmdAuth.Parameters.AddWithValue("@RoleID", 3);
                    cmdAuth.ExecuteNonQuery();

                    string salaryInsertQuery = "INSERT INTO SALARY (EmployeeID, Amount) VALUES (@EmployeeID, @Amount)";
                    SqlCommand cmdSalary = new SqlCommand(salaryInsertQuery, connection, transaction);
                    cmdSalary.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmdSalary.Parameters.AddWithValue("@Amount", salary);
                    cmdSalary.ExecuteNonQuery();

                    if (managerId != 0)
                    {
                        string reportingLineInsertQuery = "INSERT INTO REPORTING_LINE (ManagerEmployeeID, ReportEmployeeID) VALUES (@ManagerEmployeeID, @ReportEmployeeID)";
                        SqlCommand cmdReportingLine = new SqlCommand(reportingLineInsertQuery, connection, transaction);
                        cmdReportingLine.Parameters.AddWithValue("@ManagerEmployeeID", managerId);
                        cmdReportingLine.Parameters.AddWithValue("@ReportEmployeeID", employeeId);
                        cmdReportingLine.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Show success panel
                    lblSuccessMessage.Text = "Employee added succesfully";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                    upSuccessModal.Update();

                    //UpdateEmployeePanel();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                    ClearForm();
                    BindGridViews();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    validationMessage.Text = "An error occurred during employee creation: " + ex.Message + "<br />";
                }
            }
        }

        protected void deleteEmployee_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Delete clicked - EmployeeID: {txtEmpId.Text}, Password entered: {!string.IsNullOrEmpty(password1.Text)}");
            deleteValidationMessage.Text = "";

            if (Session["EmployeeID"] == null)
            {
                deleteValidationMessage.Text = "You must be logged in to delete an employee.";
                upEmployeeTab.Update();
                return;
            }

            // Validate employee ID
            if (string.IsNullOrEmpty(txtEmpId.Text) || !int.TryParse(txtEmpId.Text, out int employeeIdToDelete))
            {
                deleteValidationMessage.Text = "Please enter a valid Employee ID.";
                upEmployeeTab.Update();
                return;
            }

            // Validate password
            string loggedInUserPassword = password1.Text;
            if (string.IsNullOrEmpty(loggedInUserPassword))
            {
                deleteValidationMessage.Text = "Please enter your password.";
                upEmployeeTab.Update();
                return;
            }

            // Verify password
            bool passwordIsValid = VerifyUserPassword(loggedInUserPassword);
            if (!passwordIsValid)
            {
                deleteValidationMessage.Text = "Incorrect password. Please try again.";
                upEmployeeTab.Update();
                return;
            }
            System.Diagnostics.Debug.WriteLine($"Password valid: {passwordIsValid}, Employee exists: {EmployeeExists(employeeIdToDelete)}");
            // Check if employee exists
            if (!EmployeeExists(employeeIdToDelete))
            {
                deleteValidationMessage.Text = "Employee ID does not exist.";
                upEmployeeTab.Update();
                return;
            }

            // Store the employee ID in session for confirmation
            Session["EmployeeToDelete"] = employeeIdToDelete;
            lblEmployeeIDConfirm.Text = employeeIdToDelete.ToString();

            // Check if employee has direct reports
            if (EmployeeHasReports(employeeIdToDelete))
            {
                DataTable managedEmployees = GetManagedEmployees(employeeIdToDelete);
                if (managedEmployees.Rows.Count > 0)
                {
                    gvManagedEmployees.DataSource = managedEmployees;
                    gvManagedEmployees.DataBind();
                    PopulateNewManagerDropdown(employeeIdToDelete);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowReassignManagerModal", "showReassignManagerModal();", true);
                    upReassignManager.Update();
                    return;
                }
            }

            // If no reports, show the normal delete confirmation
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowDeleteConfirm",
     "setTimeout(function() { showDeleteConfirm(); }, 100);", true);
            upDeleteConfirmPanel.Update();

            // Clear the password field for security
            password1.Text = "";
        }
        private bool EmployeeExists(int employeeId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM EMPLOYEES WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    connection.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            if (Session["EmployeeToDelete"] == null || Session["EmployeeID"] == null)
            {
                deleteValidationMessage.Text = "Session expired. Please try again.";
                upEmployeeTab.Update();
                return;
            }

            int employeeIdToDelete = Convert.ToInt32(Session["EmployeeToDelete"]);
            int loggedInEmployeeId = Convert.ToInt32(Session["EmployeeID"]);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Check if employee still exists
                        if (!EmployeeExists(employeeIdToDelete))
                        {
                            transaction.Rollback();
                            deleteValidationMessage.Text = "Employee not found.";
                            upEmployeeTab.Update();
                            return;
                        }

                        // Create log table if it doesn't exist
                        string createLogTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DELETION_LOG' AND xtype='U')
                CREATE TABLE DELETION_LOG (
                    LogID INT IDENTITY(1,1) PRIMARY KEY,
                    DeletedEmployeeID INT,
                    DeletedByEmployeeID INT,
                    DeletionDate DATETIME
                )";
                        SqlCommand cmdCreateLog = new SqlCommand(createLogTableQuery, connection, transaction);
                        cmdCreateLog.ExecuteNonQuery();

                        // Log the deletion
                        string logQuery = "INSERT INTO DELETION_LOG (DeletedEmployeeID, DeletedByEmployeeID, DeletionDate) VALUES (@DeletedEmployeeID, @DeletedByEmployeeID, GETDATE())";
                        SqlCommand cmdLog = new SqlCommand(logQuery, connection, transaction);
                        cmdLog.Parameters.AddWithValue("@DeletedEmployeeID", employeeIdToDelete);
                        cmdLog.Parameters.AddWithValue("@DeletedByEmployeeID", loggedInEmployeeId);
                        cmdLog.ExecuteNonQuery();

                        // Delete from reporting line
                        string deleteReportingLineQuery = "DELETE FROM REPORTING_LINE WHERE ManagerEmployeeID = @EmployeeID OR ReportEmployeeID = @EmployeeID";
                        SqlCommand cmdReportingLine = new SqlCommand(deleteReportingLineQuery, connection, transaction);
                        cmdReportingLine.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdReportingLine.ExecuteNonQuery();

                        // Delete salary
                        string deleteSalaryQuery = "DELETE FROM SALARY WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdSalary = new SqlCommand(deleteSalaryQuery, connection, transaction);
                        cmdSalary.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdSalary.ExecuteNonQuery();

                        // Delete user auth
                        string deleteAuthQuery = "DELETE FROM USER_AUTH WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdAuth = new SqlCommand(deleteAuthQuery, connection, transaction);
                        cmdAuth.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdAuth.ExecuteNonQuery();

                        // Delete employee picture if exists
                        string deletePictureQuery = "DELETE FROM EMPLOYEE_PICTURE WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdPicture = new SqlCommand(deletePictureQuery, connection, transaction);
                        cmdPicture.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdPicture.ExecuteNonQuery();

                        // Finally delete employee
                        string deleteEmployeeQuery = "DELETE FROM EMPLOYEES WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdEmployee = new SqlCommand(deleteEmployeeQuery, connection, transaction);
                        cmdEmployee.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        int rowsAffected = cmdEmployee.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();

                            // Show success message
                            lblSuccessMessage.Text = "Employee deleted successfully!";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);

                            // Clear form fields
                            txtEmpId.Text = "";
                            password1.Text = "";
                            Session.Remove("EmployeeToDelete");

                            // Refresh the grid view
                            BindGridViews();

                            // Update the panels
                            upEmployeeTab.Update();
                            upSuccessModal.Update();

                            // Auto-close success panel
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessModal(); }, 5000);", true);
                        }
                        else
                        {
                            transaction.Rollback();
                            deleteValidationMessage.Text = "Employee could not be deleted.";
                            upEmployeeTab.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        deleteValidationMessage.Text = "An error occurred during deletion: " + ex.Message;
                        upEmployeeTab.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                deleteValidationMessage.Text = "An error occurred: " + ex.Message;
                upEmployeeTab.Update();
            }
        }
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            pnlDeleteConfirm.Style["display"] = "none";
            Session.Remove("EmployeeToDelete");
            //UpdateDeleteConfirmPanel();
        }
        protected void btnAddDepartment_Click(object sender, EventArgs e)
        {
            departmentValidationMessage.Text = "";
            departmentValidationMessage.ForeColor = System.Drawing.Color.Red;

            string departmentName = txtDepartmentName.Text.Trim();

            // Validate department name
            if (string.IsNullOrEmpty(departmentName))
            {
                departmentValidationMessage.Text = "Department name cannot be empty.";
                return;
            }

            // Validate location selection
            if (string.IsNullOrEmpty(ddlDepartmentLocation.SelectedValue))
            {
                departmentValidationMessage.Text = "Please select a location for the department.";
                return;
            }

            int locationId;
            if (!int.TryParse(ddlDepartmentLocation.SelectedValue, out locationId) || locationId <= 0)
            {
                departmentValidationMessage.Text = "Please select a valid location.";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO DEPARTMENT (DepartmentName, LocationID) VALUES (@DepartmentName, @LocationID)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                        cmd.Parameters.AddWithValue("@LocationID", locationId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            departmentValidationMessage.Text = "Department added successfully!";
                            departmentValidationMessage.ForeColor = System.Drawing.Color.Green;
                            txtDepartmentName.Text = "";
                            ddlDepartmentLocation.SelectedIndex = 0;
                            BindDepartmentsGridView();
                            PopulateDropdowns();

                            // Show success message
                            lblSuccessMessage.Text = "Department added successfully!";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                            upSuccessModal.Update();
                        }
                        else
                        {
                            departmentValidationMessage.Text = "Error adding department. Please try again.";
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                departmentValidationMessage.Text = "Database error: " + sqlEx.Message;
            }
            catch (Exception ex)
            {
                departmentValidationMessage.Text = "Error adding department: " + ex.Message;
            }
        }
        private bool DepartmentHasEmployees(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM EMPLOYEES WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();
                    int employeeCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return employeeCount > 0;
                }
            }
        }
        // Method to get departments at the same location (CORRECTED)
        // Simpler alternative query
        // Method to get departments at the same location with detailed debugging
        private DataTable GetDepartmentsAtSameLocation(int departmentId)
        {
            System.Diagnostics.Debug.WriteLine($"=== GetDepartmentsAtSameLocation for DepartmentID: {departmentId} ===");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // First, get the current department's location
                    string currentLocationQuery = "SELECT LocationID, DepartmentName FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                    int currentLocationId = 0;
                    string currentDeptName = "";

                    using (SqlCommand currentCmd = new SqlCommand(currentLocationQuery, connection))
                    {
                        currentCmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                        using (SqlDataReader currentReader = currentCmd.ExecuteReader())
                        {
                            if (currentReader.Read())
                            {
                                currentLocationId = Convert.ToInt32(currentReader["LocationID"]);
                                currentDeptName = currentReader["DepartmentName"].ToString();
                                System.Diagnostics.Debug.WriteLine($"Current department: {currentDeptName}, LocationID: {currentLocationId}");
                            }
                            currentReader.Close();
                        }
                    }

                    // Now get other departments at the same location
                    string query = @"SELECT DepartmentID, DepartmentName 
                    FROM DEPARTMENT 
                    WHERE LocationID = @LocationID 
                    AND DepartmentID != @DepartmentID
                    ORDER BY DepartmentName";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LocationID", currentLocationId);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} departments at the same location:");
                            foreach (DataRow row in dt.Rows)
                            {
                                System.Diagnostics.Debug.WriteLine($"- {row["DepartmentName"]} (ID: {row["DepartmentID"]})");
                            }

                            if (dt.Rows.Count == 0)
                            {
                                System.Diagnostics.Debug.WriteLine("NO OTHER DEPARTMENTS FOUND AT THIS LOCATION!");

                                // Let's see what departments ARE at this location
                                string allAtLocationQuery = "SELECT DepartmentID, DepartmentName FROM DEPARTMENT WHERE LocationID = @LocationID ORDER BY DepartmentName";
                                using (SqlCommand allCmd = new SqlCommand(allAtLocationQuery, connection))
                                {
                                    allCmd.Parameters.AddWithValue("@LocationID", currentLocationId);
                                    using (SqlDataReader allReader = allCmd.ExecuteReader())
                                    {
                                        System.Diagnostics.Debug.WriteLine($"ALL departments at LocationID {currentLocationId}:");
                                        while (allReader.Read())
                                        {
                                            System.Diagnostics.Debug.WriteLine($"- {allReader["DepartmentName"]} (ID: {allReader["DepartmentID"]})");
                                        }
                                        allReader.Close();
                                    }
                                }
                            }

                            return dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in GetDepartmentsAtSameLocation: {ex.Message}");
                    return new DataTable();
                }
            }
        }
        protected void ReassignEmployeesAndDeleteDepartment(int sourceDepartmentId, int targetDepartmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Reassign employees
                    string reassignQuery = "UPDATE EMPLOYEES SET DepartmentID = @TargetDepartmentID WHERE DepartmentID = @SourceDepartmentID";
                    using (SqlCommand cmd = new SqlCommand(reassignQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TargetDepartmentID", targetDepartmentId);
                        cmd.Parameters.AddWithValue("@SourceDepartmentID", sourceDepartmentId);
                        int employeesReassigned = cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Reassigned {employeesReassigned} employees from department {sourceDepartmentId} to {targetDepartmentId}");
                    }

                    // Delete department
                    string deleteQuery = "DELETE FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", sourceDepartmentId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // No need to show success message here - it's handled in the calling method
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw new Exception("Error during department deletion and employee reassignment: " + ex.Message);
                }
            }
        }
        // Confirm reassignment and delete
        protected void btnConfirmDepartmentReassign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnDepartmentToDelete.Value) ||
                ddlTargetDepartment.SelectedValue == "0")
            {
                lblDepartmentReassignError.Text = "Please select a target department.";
                lblDepartmentReassignError.Visible = true;
                upDepartmentReassign.Update();
                return;
            }

            int sourceDepartmentId = Convert.ToInt32(hdnDepartmentToDelete.Value);
            int targetDepartmentId = Convert.ToInt32(ddlTargetDepartment.SelectedValue);

            // Verify that target department is at the same location
            int sourceLocation = GetDepartmentLocationId(sourceDepartmentId);
            int targetLocation = GetDepartmentLocationId(targetDepartmentId);

            if (sourceLocation != targetLocation)
            {
                lblDepartmentReassignError.Text = "Selected department is not at the same location. Please choose a department from the same location.";
                lblDepartmentReassignError.Visible = true;
                upDepartmentReassign.Update();
                return;
            }

            ReassignEmployeesAndDeleteDepartment(sourceDepartmentId, targetDepartmentId);

            // Hide modal and clear fields
            ScriptManager.RegisterStartupScript(this, this.GetType(), "HideDepartmentReassignModal",
                "hideDepartmentReassignModal();", true);

            // Clear ViewState and hidden fields
            ViewState.Remove("TargetDepartments");
            ViewState.Remove("SourceDepartmentId");
            hdnDepartmentToDelete.Value = "0";
            hdnDepartmentLocationId.Value = "0";
            lblDepartmentReassignError.Visible = false;

            // Clear the dropdown
            ddlTargetDepartment.Items.Clear();
            ddlTargetDepartment.Items.Add(new ListItem("-- Select Department --", "0"));
        }

        protected void btnCancelDepartmentReassign_Click(object sender, EventArgs e)
        {
            // Hide modal and clear fields
            ScriptManager.RegisterStartupScript(this, this.GetType(), "HideDepartmentReassignModal",
                "hideDepartmentReassignModal();", true);

            // Clear ViewState and hidden fields
            ViewState.Remove("TargetDepartments");
            ViewState.Remove("SourceDepartmentId");
            hdnDepartmentToDelete.Value = "0";
            hdnDepartmentLocationId.Value = "0";
            lblDepartmentReassignError.Visible = false;

            // Clear the dropdown
            ddlTargetDepartment.Items.Clear();
            ddlTargetDepartment.Items.Add(new ListItem("-- Select Department --", "0"));
        }
        protected void btnAddPosition_Click(object sender, EventArgs e)
        {
            string positionName = txtPositionName.Text.Trim();

            if (string.IsNullOrEmpty(positionName))
            {
                positionValidationMessage.Text = "Position name is required.";
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Remove the manual ID generation and let SQL Server handle it
                string insertQuery = "INSERT INTO POSITION (PositionName) VALUES (@PositionName)";

                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        cmdInsert.Parameters.AddWithValue("@PositionName", positionName);
                        cmdInsert.ExecuteNonQuery();

                        lblSuccessMessage.Text = "Position added successfully!";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        upSuccessModal.Update();

                        txtPositionName.Text = "";
                        BindGridViews();
                        PopulateDropdowns();
                    }
                    catch (Exception ex)
                    {
                        positionValidationMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
        }

        private bool PositionHasEmployees(int positionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CASE WHEN EXISTS (SELECT 1 FROM EMPLOYEES WHERE PositionID = @PositionID) THEN 1 ELSE 0 END";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PositionID", positionId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                }
            }
        }
        private int GetNextPositionId(int currentPositionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Get the next higher position ID, or if it's the highest, get the lowest
                string query = @"
            DECLARE @NextPositionID INT;
            
            -- Try to get the next higher position
            SELECT TOP 1 @NextPositionID = PositionID 
            FROM POSITION 
            WHERE PositionID > @CurrentPositionID 
            ORDER BY PositionID ASC;
            
            -- If no higher position exists, get the lowest position
            IF @NextPositionID IS NULL
                SELECT TOP 1 @NextPositionID = PositionID 
                FROM POSITION 
                WHERE PositionID != @CurrentPositionID 
                ORDER BY PositionID ASC;
                
            SELECT ISNULL(@NextPositionID, 0);";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@CurrentPositionID", currentPositionId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        private void ReassignEmployeesAndDeletePosition(int positionId, int targetPositionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Reassign employees
                    string reassignQuery = "UPDATE EMPLOYEES SET PositionID = @TargetPositionID WHERE PositionID = @PositionID";
                    using (SqlCommand cmd = new SqlCommand(reassignQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TargetPositionID", targetPositionId);
                        cmd.Parameters.AddWithValue("@PositionID", positionId);
                        int employeesReassigned = cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Reassigned {employeesReassigned} employees from position {positionId} to {targetPositionId}");
                    }

                    // Delete position
                    string deleteQuery = "DELETE FROM POSITION WHERE PositionID = @PositionID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", positionId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw new Exception("Error during position deletion and employee reassignment: " + ex.Message);
                }
            }
        }
        protected void gvPositions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int positionId = Convert.ToInt32(gvPositions.DataKeys[e.RowIndex].Value);
            string positionName = GetPositionName(positionId);

            // Check if position has employees
            if (PositionHasEmployees(positionId))
            {
                // Store position ID for later use
                hdnPositionToDelete.Value = positionId.ToString();

                // Get employee count for this position
                int employeeCount = GetEmployeeCountInPosition(positionId);
                lblPositionEmployeeCount.Text = employeeCount.ToString();

                // Populate the target positions dropdown
                PopulateTargetPositionsDropdown(positionId);

                // Show the reassignment modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPositionReassignModal",
                    "showPositionReassignModal();", true);
                upPositionReassignModal.Update();

                // Cancel the row deleting event since we're handling it through the modal
                e.Cancel = true;
            }
            else
            {
                // No employees, just delete the position
                DeletePosition(positionId);
                lblSuccessMessage.Text = $"Position '{positionName}' has been deleted successfully.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                BindGridViews();
                upSuccessModal.Update();
            }
        }
        private int GetEmployeeCountInPosition(int positionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM EMPLOYEES WHERE PositionID = @PositionID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PositionID", positionId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private void PopulateTargetPositionsDropdown(int excludePositionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT PositionID, PositionName 
                        FROM POSITION 
                        WHERE PositionID != @ExcludePositionID
                        ORDER BY PositionName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ExcludePositionID", excludePositionId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlTargetPosition.Items.Clear();
                        ddlTargetPosition.Items.Add(new ListItem("-- Select Position --", "0"));

                        foreach (DataRow row in dt.Rows)
                        {
                            ddlTargetPosition.Items.Add(new ListItem(
                                row["PositionName"].ToString(),
                                row["PositionID"].ToString()
                            ));
                        }
                    }
                }
            }
        }
        protected void btnConfirmPositionReassign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnPositionToDelete.Value) ||
                ddlTargetPosition.SelectedValue == "0")
            {
                lblPositionReassignError.Text = "Please select a target position.";
                lblPositionReassignError.Visible = true;
                upPositionReassignModal.Update();
                return;
            }

            int sourcePositionId = Convert.ToInt32(hdnPositionToDelete.Value);
            int targetPositionId = Convert.ToInt32(ddlTargetPosition.SelectedValue);

            string sourcePositionName = GetPositionName(sourcePositionId);
            string targetPositionName = GetPositionName(targetPositionId);

            try
            {
                // Get employee count BEFORE deletion
                int employeeCount = GetEmployeeCountInPosition(sourcePositionId);

                // Reassign employees and delete position
                ReassignEmployeesAndDeletePosition(sourcePositionId, targetPositionId);

                // Show success message with the stored count
                lblSuccessMessage.Text = $"{employeeCount} employees from '{sourcePositionName}' position have been reassigned to '{targetPositionName}', and the position has been deleted.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);

                // Hide modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HidePositionReassignModal",
                    "hidePositionReassignModal();", true);

                // Refresh data
                BindGridViews();
                PopulateDropdowns();

                // Update panels
                upSuccessModal.Update();
                upPositionTab.Update();
            }
            catch (Exception ex)
            {
                lblPositionReassignError.Text = "An error occurred: " + ex.Message;
                lblPositionReassignError.Visible = true;
                upPositionReassignModal.Update();
            }
        }
        // Method to get employee count in department
        private int GetEmployeeCountInDepartment(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM EMPLOYEES WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Method to get department location
        private int GetDepartmentLocation(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
        private string GetPositionName(int positionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT PositionName FROM POSITION WHERE PositionID = @PositionID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PositionID", positionId);
                    connection.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? "Unknown Position";
                }
            }
        }
        private void DeletePosition(int positionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM POSITION WHERE PositionID = @PositionID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PositionID", positionId);
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccessMessage.Text = "The position was deleted successfully ";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        upSuccessModal.Update();
                        BindGridViews();
                        PopulateDropdowns();
                    }
                    catch (Exception ex)
                    {
                        positionValidationMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
        }
        protected void btnConfirmPositionDelete_Click(object sender, EventArgs e)
        {
            if (Session["PositionToDelete"] == null || Session["NextPositionId"] == null)
            {
                positionValidationMessage.Text = "Invalid request. Please try again.";
                return;
            }

            int positionId = Convert.ToInt32(Session["PositionToDelete"]);
            int nextPositionId = Convert.ToInt32(Session["NextPositionId"]);

            ReassignEmployeesAndDeletePosition(positionId, nextPositionId);

            // Clear session
            Session.Remove("PositionToDelete");
            Session.Remove("NextPositionId");
            Session.Remove("NextPositionName");
        }
        protected void btnAddLocation_Click(object sender, EventArgs e)
        {
            string locationName = txtLocationName.Text.Trim();

            if (string.IsNullOrEmpty(locationName))
            {
                locationValidationMessage.Text = "Please enter a location name.";
                locationValidationMessage.ForeColor = System.Drawing.Color.Red;
                upLocationTab.Update(); // Update panel to show error
                return;
            }

            // Check if connection string exists
            if (ConfigurationManager.ConnectionStrings["MyDbConnection"] == null)
            {
                locationValidationMessage.Text = "Database connection not configured.";
                locationValidationMessage.ForeColor = System.Drawing.Color.Red;
                upLocationTab.Update(); // Update panel to show error
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO LOCATION (LocationName) VALUES (@LocationName)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LocationName", locationName);
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Success - clear form and show success message
                            txtLocationName.Text = "";
                            locationValidationMessage.Text = ""; // Clear any previous error messages

                            // Refresh data
                            BindLocations();
                            PopulateDropdowns();

                            // Show success modal
                            lblSuccessMessage.Text = "Location added successfully!";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);

                            // Update success panel
                            upSuccessModal.Update();
                        }
                        else
                        {
                            locationValidationMessage.Text = "Failed to add location.";
                            locationValidationMessage.ForeColor = System.Drawing.Color.Red;
                            upLocationTab.Update(); // Update panel to show error
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                locationValidationMessage.Text = "Database error: " + sqlEx.Message;
                locationValidationMessage.ForeColor = System.Drawing.Color.Red;
                upLocationTab.Update(); // Update panel to show error
            }
            catch (Exception ex)
            {
                locationValidationMessage.Text = "Error: " + ex.Message;
                locationValidationMessage.ForeColor = System.Drawing.Color.Red;
                upLocationTab.Update(); // Update panel to show error
            }

            // Always update the location tab panel
            upLocationTab.Update();
        }
        private void BindLocations()
        {
            try
            {
                if (ConfigurationManager.ConnectionStrings["MyDbConnection"] == null)
                {
                    throw new Exception("Database connection not configured.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT LocationID, LocationName FROM LOCATION ORDER BY LocationName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Check if controls exist before binding
                        if (gvLocations != null)
                        {
                            gvLocations.DataSource = dt;
                            gvLocations.DataBind();
                        }

                        if (ddlDepartmentLocation != null && dt.Rows.Count > 0)
                        {
                            ddlDepartmentLocation.DataSource = dt;
                            ddlDepartmentLocation.DataTextField = "LocationName";
                            ddlDepartmentLocation.DataValueField = "LocationID";
                            ddlDepartmentLocation.DataBind();
                            ddlDepartmentLocation.Items.Insert(0, new ListItem("-- Select Location --", "0"));
                        }

                        if (locationDropdown != null && dt.Rows.Count > 0)
                        {
                            locationDropdown.DataSource = dt;
                            locationDropdown.DataTextField = "LocationName";
                            locationDropdown.DataValueField = "LocationID";
                            locationDropdown.DataBind();
                            locationDropdown.Items.Insert(0, new ListItem("-- Select Location --", "0"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                System.Diagnostics.Debug.WriteLine("Error in BindLocations: " + ex.Message);
            }
        }
        // Updated RowDeleting method for departments
        protected void gvDepartments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int departmentId = Convert.ToInt32(gvDepartments.DataKeys[e.RowIndex].Value);
            string departmentName = GetDepartmentName(departmentId);

            // Check if department has employees
            int employeeCount = GetEmployeeCountInDepartment(departmentId);

            if (employeeCount > 0)
            {
                // Get the next department in the same location
                int? nextDepartmentId = GetNextDepartmentInSameLocation(departmentId);

                if (nextDepartmentId.HasValue)
                {
                    string nextDepartmentName = GetDepartmentName(nextDepartmentId.Value);

                    // Reassign employees and delete department
                    ReassignEmployeesAndDeleteDepartment(departmentId, nextDepartmentId.Value);

                    // Show success message with reassignment info
                    lblSuccessMessage.Text = $"{employeeCount} employees from '{departmentName}' department have been automatically reassigned to '{nextDepartmentName}' department, and the department has been deleted.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                }
                else
                {
                    // No other departments at same location - cancel delete
                    e.Cancel = true;
                    departmentValidationMessage.Text = $"Cannot delete '{departmentName}' department. There are no other departments at this location to reassign {employeeCount} employees to.";
                    return;
                }
            }
            else
            {
                // No employees, just delete the department
                DeleteDepartment(departmentId);
                lblSuccessMessage.Text = $"Department '{departmentName}' has been deleted successfully.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
            }

            // Update the grid
            BindGridViews();
            upEmployeeTab.Update();
            upSuccessModal.Update();
        }
        private int? GetNextDepartmentInSameLocation(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 1 DepartmentID 
                FROM DEPARTMENT 
                WHERE LocationID = (SELECT LocationID FROM DEPARTMENT WHERE DepartmentID = @DepartmentID)
                AND DepartmentID != @DepartmentID
                ORDER BY DepartmentName"; // Or DepartmentID for consistent ordering

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();

                    object result = cmd.ExecuteScalar();
                    return result != null ? (int?)Convert.ToInt32(result) : null;
                }
            }
        }
        private void BindTargetDepartmentsDropdown(DataTable targetDepartments)
        {
            // Clear existing items first
            ddlTargetDepartment.Items.Clear();

            // Add the default option
            ddlTargetDepartment.Items.Add(new ListItem("-- Select Department --", "0"));

            // Add departments from the DataTable
            foreach (DataRow row in targetDepartments.Rows)
            {
                string departmentId = row["DepartmentID"].ToString();
                string departmentName = row["DepartmentName"].ToString();
                ddlTargetDepartment.Items.Add(new ListItem(departmentName, departmentId));
            }

            // Ensure ViewState is enabled
            ddlTargetDepartment.EnableViewState = true;

            // DEBUG: Log what was added to the dropdown
            System.Diagnostics.Debug.WriteLine("Dropdown items after binding:");
            foreach (ListItem item in ddlTargetDepartment.Items)
            {
                System.Diagnostics.Debug.WriteLine($" - {item.Text} = {item.Value}");
            }
        }
        private string GetDepartmentLocationName(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT l.LocationName 
                FROM DEPARTMENT d
                INNER JOIN LOCATION l ON d.LocationID = l.LocationID
                WHERE d.DepartmentID = @DepartmentID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? "Unknown Location";
                }
            }
        }

        // Method to get department location ID
        private int GetDepartmentLocationId(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private DataTable GetAllDepartments()
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = @"SELECT d.DepartmentID, d.DepartmentName, l.LocationName, l.LocationID
                FROM DEPARTMENT d
                INNER JOIN LOCATION l ON d.LocationID = l.LocationID
                ORDER BY l.LocationName, d.DepartmentName";

        using (SqlCommand cmd = new SqlCommand(query, connection))
        {
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                // DEBUG: Output all departments
                System.Diagnostics.Debug.WriteLine("=== ALL DEPARTMENTS IN DATABASE ===");
                foreach (DataRow row in dt.Rows)
                {
                    System.Diagnostics.Debug.WriteLine($"Department: {row["DepartmentName"]} (ID: {row["DepartmentID"]}) at Location: {row["LocationName"]} (ID: {row["LocationID"]})");
                }
                
                return dt;
            }
        }
    }
}
        // Comprehensive test method to debug department data
        private void DebugDepartmentData()
        {
            System.Diagnostics.Debug.WriteLine("=== COMPREHENSIVE DEPARTMENT DATA DEBUG ===");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // 1. Check all locations
                    string locationQuery = "SELECT LocationID, LocationName FROM LOCATION ORDER BY LocationName";
                    using (SqlCommand locationCmd = new SqlCommand(locationQuery, connection))
                    {
                        using (SqlDataReader locationReader = locationCmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("=== ALL LOCATIONS ===");
                            while (locationReader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"Location: {locationReader["LocationName"]} (ID: {locationReader["LocationID"]})");
                            }
                            locationReader.Close();
                        }
                    }

                    // 2. Check all departments with their locations
                    string departmentQuery = @"SELECT d.DepartmentID, d.DepartmentName, d.LocationID, l.LocationName 
                              FROM DEPARTMENT d 
                              INNER JOIN LOCATION l ON d.LocationID = l.LocationID 
                              ORDER BY l.LocationName, d.DepartmentName";

                    using (SqlCommand deptCmd = new SqlCommand(departmentQuery, connection))
                    {
                        using (SqlDataReader deptReader = deptCmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("=== ALL DEPARTMENTS ===");
                            while (deptReader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"Department: {deptReader["DepartmentName"]} (ID: {deptReader["DepartmentID"]}) at Location: {deptReader["LocationName"]} (ID: {deptReader["LocationID"]})");
                            }
                            deptReader.Close();
                        }
                    }

                    // 3. Check if any locations have multiple departments
                    string multiDeptQuery = @"SELECT l.LocationID, l.LocationName, COUNT(d.DepartmentID) as DepartmentCount
                             FROM LOCATION l
                             LEFT JOIN DEPARTMENT d ON l.LocationID = d.LocationID
                             GROUP BY l.LocationID, l.LocationName
                             HAVING COUNT(d.DepartmentID) > 1
                             ORDER BY l.LocationName";

                    using (SqlCommand multiCmd = new SqlCommand(multiDeptQuery, connection))
                    {
                        using (SqlDataReader multiReader = multiCmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("=== LOCATIONS WITH MULTIPLE DEPARTMENTS ===");
                            if (multiReader.HasRows)
                            {
                                while (multiReader.Read())
                                {
                                    System.Diagnostics.Debug.WriteLine($"Location: {multiReader["LocationName"]} has {multiReader["DepartmentCount"]} departments");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("NO LOCATIONS HAVE MULTIPLE DEPARTMENTS!");
                            }
                            multiReader.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
        }
        private string GetLocationName(int locationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationName FROM LOCATION WHERE LocationID = @LocationID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    connection.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? "Unknown Location";
                }
            }
        }
        private void DeleteLocation(int locationId)
        {
            // Double-check that the location has no departments before deleting
            if (LocationHasDepartments(locationId))
            {
                throw new InvalidOperationException("Cannot delete location that has departments. Use reassignment instead.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM LOCATION WHERE LocationID = @LocationID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    try
                    {
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblSuccessMessage.Text = "Location deleted successfully";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error deleting location: " + ex.Message);
                    }
                }
            }
        }
        protected void btnConfirmLocationDelete_Click(object sender, EventArgs e)
        {
            if (Session["LocationToDelete"] == null || Session["NextLocationId"] == null)
            {
                locationValidationMessage.Text = "Invalid request. Please try again.";
                return;
            }

            int locationId = Convert.ToInt32(Session["LocationToDelete"]);
            int nextLocationId = Convert.ToInt32(Session["NextLocationId"]);

            ReassignDepartmentsAndDeleteLocation(locationId, nextLocationId);

            // Clear session
            Session.Remove("LocationToDelete");
            Session.Remove("NextLocationId");
            Session.Remove("NextLocationName");
        }
        private void ReassignDepartmentsAndDeleteLocation(int sourceLocationId, int targetLocationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Reassign departments to the new location
                    string reassignQuery = "UPDATE DEPARTMENT SET LocationID = @TargetLocationID WHERE LocationID = @SourceLocationID";
                    using (SqlCommand cmd = new SqlCommand(reassignQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TargetLocationID", targetLocationId);
                        cmd.Parameters.AddWithValue("@SourceLocationID", sourceLocationId);
                        int departmentsReassigned = cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Reassigned {departmentsReassigned} departments from location {sourceLocationId} to {targetLocationId}");
                    }

                    // Delete location
                    string deleteQuery = "DELETE FROM LOCATION WHERE LocationID = @LocationID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@LocationID", sourceLocationId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Deleted location {sourceLocationId}, rows affected: {rowsAffected}");
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw new Exception("Error during location deletion and department reassignment: " + ex.Message);
                }
            }
        }

        private void DeleteDepartment(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error deleting department: " + ex.Message);
                    }
                }
            }
        }
        protected void btnConfirmReassignment_Click(object sender, EventArgs e)
        {
            if (Session["DepartmentToDelete"] == null || string.IsNullOrEmpty(ddlTargetDepartment.SelectedValue))
            {
                departmentValidationMessage.Text = "Invalid request. Please try again.";
                return;
            }

            int sourceDepartmentId = Convert.ToInt32(Session["DepartmentToDelete"]);
            int targetDepartmentId = Convert.ToInt32(ddlTargetDepartment.SelectedValue);

            ReassignEmployeesAndDeleteDepartment(sourceDepartmentId, targetDepartmentId);

            // Clear session
            Session.Remove("DepartmentToDelete");
        }
        private bool LocationHasDepartments(int locationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CASE WHEN EXISTS (SELECT 1 FROM DEPARTMENT WHERE LocationID = @LocationID) THEN 1 ELSE 0 END";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                }
            }
        }

        private int GetNextLocationId(int currentLocationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Get the next higher location ID, or if it's the highest, get the lowest
                string query = @"
            DECLARE @NextLocationID INT;
            
            -- Try to get the next higher location
            SELECT TOP 1 @NextLocationID = LocationID 
            FROM LOCATION 
            WHERE LocationID > @CurrentLocationID 
            ORDER BY LocationID ASC;
            
            -- If no higher location exists, get the lowest location
            IF @NextLocationID IS NULL
                SELECT TOP 1 @NextLocationID = LocationID 
                FROM LOCATION 
                WHERE LocationID != @CurrentLocationID 
                ORDER BY LocationID ASC;
                
            SELECT ISNULL(@NextLocationID, 0);";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@CurrentLocationID", currentLocationId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }



        protected void gvLocations_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int locationId = Convert.ToInt32(gvLocations.DataKeys[e.RowIndex].Value);
            string locationName = GetLocationName(locationId);

            // Check if location has departments
            if (LocationHasDepartments(locationId))
            {
                try
                {
                    // Get a random alternative location
                    int? randomLocationId = GetRandomAlternativeLocation(locationId);

                    if (randomLocationId.HasValue)
                    {
                        string randomLocationName = GetLocationName(randomLocationId.Value);

                        // Get department count before reassignment
                        int departmentCount = GetDepartmentCountInLocation(locationId);

                        // Show info panel with reassignment details
                        lblReassignInfo.Text = $"{departmentCount} departments from '{locationName}' are being moved to '{randomLocationName}' location.";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowLocationReassignInfo",
                            "showLocationReassignInfo();", true);
                        upLocationReassignInfo.Update();

                        // Reassign departments to random location and delete
                        ReassignDepartmentsAndDeleteLocation(locationId, randomLocationId.Value);

                        // Update info panel with completion message
                        lblReassignInfo.Text = $"{departmentCount} departments from '{locationName}' have been successfully moved to '{randomLocationName}' location.";
                        upLocationReassignInfo.Update();

                        // Show success message after a brief delay
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessAfterReassign",
                            "setTimeout(function() { hideLocationReassignInfo(); showSuccessModal(); }, 2000);", true);

                        lblSuccessMessage.Text = $"{departmentCount} departments from '{locationName}' have been automatically reassigned to '{randomLocationName}', and the location has been deleted.";
                    }
                    else
                    {
                        // No other locations exist - cancel delete operation
                        e.Cancel = true;
                        locationValidationMessage.Text = $"Cannot delete '{locationName}'. This is the only location in the system.";
                        upLocationTab.Update();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Hide info panel if it was shown
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "HideLocationReassignInfo",
                        "hideLocationReassignInfo();", true);

                    e.Cancel = true;
                    locationValidationMessage.Text = "An error occurred during location deletion: " + ex.Message;
                    upLocationTab.Update();
                    return;
                }
            }
            else
            {
                // No departments, just delete the location
                try
                {
                    DeleteLocation(locationId);
                    lblSuccessMessage.Text = $"Location '{locationName}' has been deleted successfully.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    locationValidationMessage.Text = "An error occurred: " + ex.Message;
                    upLocationTab.Update();
                    return;
                }
            }

            // Refresh data
            BindGridViews();
            PopulateDropdowns();
            upSuccessModal.Update();
            upLocationTab.Update();
        }
        private int? GetRandomAlternativeLocation(int excludeLocationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT LocationID 
                        FROM LOCATION 
                        WHERE LocationID != @ExcludeLocationID
                        ORDER BY NEWID()"; // NEWID() randomizes the order

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ExcludeLocationID", excludeLocationId);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }

            return null; // No alternative locations found
        }
        private int GetDepartmentCountInLocation(int locationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM DEPARTMENT WHERE LocationID = @LocationID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

       
        protected void btnConfirmDepartmentDelete_Click(object sender, EventArgs e)
        {
            int departmentId = Convert.ToInt32(hdnDepartmentToDelete.Value);
            int targetDepartmentId = Convert.ToInt32(ddlTargetDepartment.SelectedValue);

            if (targetDepartmentId == 0)
            {
                departmentValidationMessage.Text = "Please select a valid target department.";
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Step 1: Move employees
                    string updateEmployees = @"UPDATE EMPLOYEES 
                                       SET DepartmentID = @TargetDepartmentID 
                                       WHERE DepartmentID = @DepartmentID";
                    using (SqlCommand cmd = new SqlCommand(updateEmployees, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TargetDepartmentID", targetDepartmentId);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                        cmd.ExecuteNonQuery();
                    }

                    // Step 2: Delete department
                    string deleteDept = "DELETE FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                    using (SqlCommand cmd = new SqlCommand(deleteDept, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    lblSuccessMessage.Text = "Department deleted and employees reassigned successfully.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                    upSuccessModal.Update();

                    BindGridViews();
                    PopulateDropdowns();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    departmentValidationMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyUserPassword(string enteredPassword)
        {
            int loggedInEmployeeId = Convert.ToInt32(Session["EmployeeID"]);
            string hashedEnteredPassword = HashPassword(enteredPassword);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Updated query to match your USER_AUTH table structure
                string query = "SELECT Password FROM USER_AUTH WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", loggedInEmployeeId);
                    connection.Open();
                    string storedPassword = cmd.ExecuteScalar()?.ToString();

                    return storedPassword == hashedEnteredPassword;
                }
            }
        }
        private bool EmployeeHasReports(int employeeId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM REPORTING_LINE WHERE ManagerEmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    connection.Open();
                    int reportCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return reportCount > 0;
                }
            }
        }

        // Get employees managed by the employee being deleted
        private DataTable GetManagedEmployees(int managerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT e.EmployeeID, e.FirstName, e.LastName, p.PositionName
                        FROM EMPLOYEES e
                        INNER JOIN REPORTING_LINE r ON e.EmployeeID = r.ReportEmployeeID
                        LEFT JOIN POSITION p ON e.PositionID = p.PositionID
                        WHERE r.ManagerEmployeeID = @ManagerID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ManagerID", managerId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private void PopulateNewManagerDropdown(int excludeEmployeeId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT EmployeeID, FirstName + ' ' + LastName AS FullName 
                        FROM EMPLOYEES 
                        WHERE EmployeeID != @ExcludeID
                        AND EmployeeID != @SessionEmployeeID
                        ORDER BY FirstName, LastName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ExcludeID", excludeEmployeeId);
                    cmd.Parameters.AddWithValue("@SessionEmployeeID", Session["EmployeeID"]);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlNewManager.Items.Clear();
                        ddlNewManager.Items.Add(new ListItem("-- Select Manager --", "0"));
                        while (reader.Read())
                        {
                            ddlNewManager.Items.Add(new ListItem(
                                reader["FullName"].ToString(),
                                reader["EmployeeID"].ToString()
                            ));
                        }
                    }
                }
            }
        }
        private string GetDepartmentName(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT DepartmentName FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    connection.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? "Unknown Department";
                }
            }
        }

        // Handle the reassignment confirmation
        protected void btnConfirmReassign_Click(object sender, EventArgs e)
        {
            if (Session["EmployeeToDelete"] == null || ddlNewManager.SelectedValue == "0")
            {
                lblReassignError.Text = "Please select a new manager.";
                lblReassignError.Visible = true;
                upReassignManager.Update();
                return;
            }

            int employeeIdToDelete = Convert.ToInt32(Session["EmployeeToDelete"]);
            int newManagerId = Convert.ToInt32(ddlNewManager.SelectedValue);

            // Reassign the employees to new manager
            ReassignEmployeesToNewManager(employeeIdToDelete, newManagerId);

            // Now show the delete confirmation modal
            pnlDeleteConfirm.Style["display"] = "block";
            //UpdateDeleteConfirmPanel();

            // Hide the reassignment modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "HideReassignManagerModal", "hideReassignManagerModal();", true);
        }

        // Reassign employees to new manager
        private void ReassignEmployeesToNewManager(int oldManagerId, int newManagerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE REPORTING_LINE SET ManagerEmployeeID = @NewManagerID WHERE ManagerEmployeeID = @OldManagerID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NewManagerID", newManagerId);
                    cmd.Parameters.AddWithValue("@OldManagerID", oldManagerId);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // You can log this or show a message if needed
                    System.Diagnostics.Debug.WriteLine($"Reassigned {rowsAffected} employees from manager {oldManagerId} to {newManagerId}");
                }
            }
        }

        // Handle cancel reassignment
        protected void btnCancelReassign_Click(object sender, EventArgs e)
        {
            // Clear the session and hide the modal
            Session.Remove("EmployeeToDelete");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "HideReassignManagerModal", "hideReassignManagerModal();", true);
        }

        private void ClearForm()
        {
            firstNameTextbox.Text = "";
            lastNameTextbox.Text = "";
            dobTextbox.Text = "";
            emailTextbox.Text = "";
            passwordTextbox.Text = "";
            confirmPassword.Text = "";
            salaryTextbox.Text = "";

            if (positionDropdown.Items.Count > 0) positionDropdown.SelectedIndex = 0;
            if (departmentDropdown.Items.Count > 0) departmentDropdown.SelectedIndex = 0;
            if (locationDropdown.Items.Count > 0) locationDropdown.SelectedIndex = 0; // Add this line
            if (managerDropdown.Items.Count > 0) managerDropdown.SelectedIndex = 0;
        }

        // UpdatePanel methods
            }
}