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

            if (!IsPostBack)
            {
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

                string departmentQuery = "SELECT DISTINCT DepartmentName FROM DEPARTMENT ORDER BY DepartmentName";
                using (SqlCommand cmdDepartment = new SqlCommand(departmentQuery, connection))
                {
                    using (SqlDataReader departmentReader = cmdDepartment.ExecuteReader())
                    {
                        departmentDropdown.Items.Clear();
                        while (departmentReader.Read())
                        {
                            departmentDropdown.Items.Add(new ListItem(departmentReader["DepartmentName"].ToString(), departmentReader["DepartmentName"].ToString()));
                        }
                    }
                }

                string locationQuery = "SELECT LocationID, LocationName FROM LOCATION";
                using (SqlCommand cmdLocation = new SqlCommand(locationQuery, connection))
                {
                    using (SqlDataReader locationReader = cmdLocation.ExecuteReader())
                    {
                        locationDropdown.Items.Clear();
                        while (locationReader.Read())
                        {
                            locationDropdown.Items.Add(new ListItem(locationReader["LocationName"].ToString(), locationReader["LocationID"].ToString()));
                        }
                    }
                }

                string managerQuery = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM EMPLOYEES";
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
                string query = "SELECT DepartmentID, DepartmentName FROM DEPARTMENT";

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

            // CHANGED: .Value to .Text
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

            Regex nameRegex = new Regex("^[a-zA-Z\\s]+$");
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
                    lblSuccessMessage.Text = $"Employee added successfully! Employee ID: {employeeId}";
                    pnlSuccess.Style["display"] = "block";

                    UpdateEmployeePanel();
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
            deleteValidationMessage.Text = "";

            if (Session["EmployeeID"] == null)
            {
                deleteValidationMessage.Text = "You must be logged in to delete an employee.";
                return;
            }

            // CHANGED: .Value to .Text
            string loggedInUserPassword = password1.Text;

            bool passwordIsValid = VerifyUserPassword(loggedInUserPassword);

            if (!passwordIsValid)
            {
                deleteValidationMessage.Text = "Incorrect password. Please try again.";
                return;
            }

            if (string.IsNullOrEmpty(txtEmpId.Text) || !int.TryParse(txtEmpId.Text, out int employeeIdToDelete))
            {
                deleteValidationMessage.Text = "Please enter a valid Employee ID.";
                return;
            }

            Session["EmployeeToDelete"] = employeeIdToDelete;
            lblEmployeeIDConfirm.Text = employeeIdToDelete.ToString();
            pnlDeleteConfirm.Style["display"] = "block";

            UpdateDeleteConfirmPanel();
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            pnlDeleteConfirm.Style["display"] = "none";

            if (Session["EmployeeToDelete"] == null || Session["EmployeeID"] == null)
            {
                deleteValidationMessage.Text = "Session expired. Please try again.";
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
                        string checkEmployeeQuery = "SELECT COUNT(*) FROM EMPLOYEES WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdCheck = new SqlCommand(checkEmployeeQuery, connection, transaction);
                        cmdCheck.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        int employeeExists = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (employeeExists == 0)
                        {
                            transaction.Rollback();
                            deleteValidationMessage.Text = "Employee not found.";
                            return;
                        }

                        string deleteSalaryQuery = "DELETE FROM SALARY WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdSalary = new SqlCommand(deleteSalaryQuery, connection, transaction);
                        cmdSalary.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdSalary.ExecuteNonQuery();

                        string deleteAuthQuery = "DELETE FROM USER_AUTH WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdAuth = new SqlCommand(deleteAuthQuery, connection, transaction);
                        cmdAuth.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdAuth.ExecuteNonQuery();

                        string deleteReportingLineQuery = "DELETE FROM REPORTING_LINE WHERE ManagerEmployeeID = @EmployeeID OR ReportEmployeeID = @EmployeeID";
                        SqlCommand cmdReportingLine = new SqlCommand(deleteReportingLineQuery, connection, transaction);
                        cmdReportingLine.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdReportingLine.ExecuteNonQuery();

                        string deletePictureQuery = "DELETE FROM EMPLOYEE_PICTURE WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdPicture = new SqlCommand(deletePictureQuery, connection, transaction);
                        cmdPicture.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        cmdPicture.ExecuteNonQuery();

                        string deleteEmployeeQuery = "DELETE FROM EMPLOYEES WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdEmployee = new SqlCommand(deleteEmployeeQuery, connection, transaction);
                        cmdEmployee.Parameters.AddWithValue("@EmployeeID", employeeIdToDelete);
                        int rowsAffected = cmdEmployee.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
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

                            string logQuery = "INSERT INTO DELETION_LOG (DeletedEmployeeID, DeletedByEmployeeID, DeletionDate) VALUES (@DeletedEmployeeID, @DeletedByEmployeeID, GETDATE())";
                            SqlCommand cmdLog = new SqlCommand(logQuery, connection, transaction);
                            cmdLog.Parameters.AddWithValue("@DeletedEmployeeID", employeeIdToDelete);
                            cmdLog.Parameters.AddWithValue("@DeletedByEmployeeID", loggedInEmployeeId);
                            cmdLog.ExecuteNonQuery();

                            transaction.Commit();

                            lblSuccessMessage.Text = "Employee deleted successfully!";
                            pnlSuccess.Style["display"] = "block";

                            UpdateEmployeePanel();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                            txtEmpId.Text = "";
                            password1.Text = "";
                            Session.Remove("EmployeeToDelete");
                            BindGridViews();
                        }
                        else
                        {
                            transaction.Rollback();
                            deleteValidationMessage.Text = "Employee could not be deleted.";
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        deleteValidationMessage.Text = "An error occurred during deletion: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                deleteValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void btnAddDepartment_Click(object sender, EventArgs e)
        {
            string departmentName = txtDepartmentName.Text.Trim();

            if (string.IsNullOrEmpty(departmentName))
            {
                departmentValidationMessage.Text = "Department name is required.";
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO DEPARTMENT (DepartmentName) VALUES (@DepartmentName)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", departmentName);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccessMessage.Text = "Department added successfully!";
                        pnlSuccess.Style["display"] = "block";

                        UpdateDepartmentPanel();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                        txtDepartmentName.Text = "";
                        BindGridViews();
                        PopulateDropdowns();
                    }
                    catch (Exception ex)
                    {
                        departmentValidationMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
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
                string query = "INSERT INTO POSITION (PositionName) VALUES (@PositionName)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PositionName", positionName);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccessMessage.Text = "Position added successfully!";
                        pnlSuccess.Style["display"] = "block";

                        UpdatePositionPanel();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

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

        protected void btnAddLocation_Click(object sender, EventArgs e)
        {
            string locationName = txtLocationName.Text.Trim();

            if (string.IsNullOrEmpty(locationName))
            {
                locationValidationMessage.Text = "Location name is required.";
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO LOCATION (LocationName) VALUES (@LocationName)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LocationName", locationName);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccessMessage.Text = "Location added successfully!";
                        pnlSuccess.Style["display"] = "block";

                        UpdateLocationPanel();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                        txtLocationName.Text = "";
                        BindGridViews();
                        PopulateDropdowns();
                    }
                    catch (Exception ex)
                    {
                        locationValidationMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
        }

        protected void gvDepartments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int departmentId = Convert.ToInt32(gvDepartments.DataKeys[e.RowIndex].Value);

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

                        lblSuccessMessage.Text = "Department deleted successfully!";
                        pnlSuccess.Style["display"] = "block";

                        UpdateDepartmentPanel();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                        BindGridViews();
                        PopulateDropdowns();
                    }
                    catch (Exception ex)
                    {
                        departmentValidationMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
        }

        protected void gvPositions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int positionId = Convert.ToInt32(gvPositions.DataKeys[e.RowIndex].Value);

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

                        lblSuccessMessage.Text = "Position deleted successfully!";
                        pnlSuccess.Style["display"] = "block";

                        UpdatePositionPanel();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

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

        protected void gvLocations_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int locationId = Convert.ToInt32(gvLocations.DataKeys[e.RowIndex].Value);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM LOCATION WHERE LocationID = @LocationID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationId);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccessMessage.Text = "Location deleted successfully!";
                        pnlSuccess.Style["display"] = "block";

                        UpdateLocationPanel();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                        BindGridViews();
                        PopulateDropdowns();
                    }
                    catch (Exception ex)
                    {
                        locationValidationMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
        }

        protected void gvEmployees_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int employeeId = Convert.ToInt32(gvEmployees.DataKeys[e.RowIndex].Value);
            int loggedInEmployeeId = Convert.ToInt32(Session["EmployeeID"]);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        string deleteSalaryQuery = "DELETE FROM SALARY WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdSalary = new SqlCommand(deleteSalaryQuery, connection, transaction);
                        cmdSalary.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmdSalary.ExecuteNonQuery();

                        string deleteAuthQuery = "DELETE FROM USER_AUTH WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdAuth = new SqlCommand(deleteAuthQuery, connection, transaction);
                        cmdAuth.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmdAuth.ExecuteNonQuery();

                        string deleteReportingLineQuery = "DELETE FROM REPORTING_LINE WHERE ManagerEmployeeID = @EmployeeID OR ReportEmployeeID = @EmployeeID";
                        SqlCommand cmdReportingLine = new SqlCommand(deleteReportingLineQuery, connection, transaction);
                        cmdReportingLine.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmdReportingLine.ExecuteNonQuery();

                        string deleteEmployeeQuery = "DELETE FROM EMPLOYEES WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmdEmployee = new SqlCommand(deleteEmployeeQuery, connection, transaction);
                        cmdEmployee.Parameters.AddWithValue("@EmployeeID", employeeId);
                        int rowsAffected = cmdEmployee.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();

                            string logQuery = "INSERT INTO DELETION_LOG (DeletedEmployeeID, DeletedByEmployeeID, DeletionDate) VALUES (@DeletedEmployeeID, @DeletedByEmployeeID, GETDATE())";
                            SqlCommand cmdLog = new SqlCommand(logQuery, connection);
                            cmdLog.Parameters.AddWithValue("@DeletedEmployeeID", employeeId);
                            cmdLog.Parameters.AddWithValue("@DeletedByEmployeeID", loggedInEmployeeId);
                            cmdLog.ExecuteNonQuery();

                            lblSuccessMessage.Text = "Employee deleted successfully!";
                            pnlSuccess.Style["display"] = "block";

                            UpdateEmployeePanel();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "autoCloseSuccess", "setTimeout(function() { hideSuccessPanel(); }, 5000);", true);

                            BindGridViews();
                        }
                        else
                        {
                            transaction.Rollback();
                            validationMessage.Text = "Employee not found or could not be deleted.";
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        validationMessage.Text = "An error occurred during deletion: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                validationMessage.Text = "An error occurred: " + ex.Message;
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
            if (locationDropdown.Items.Count > 0) locationDropdown.SelectedIndex = 0;
            if (managerDropdown.Items.Count > 0) managerDropdown.SelectedIndex = 0;
        }

        // UpdatePanel methods
        private void UpdateEmployeePanel()
        {
            upEmployeeTab.Update();
            upSuccess.Update();
            upDeleteConfirm.Update();
        }

        private void UpdateDepartmentPanel()
        {
            upDepartmentTab.Update();
            upSuccess.Update();
        }

        private void UpdatePositionPanel()
        {
            upPositionTab.Update();
            upSuccess.Update();
        }

        private void UpdateLocationPanel()
        {
            upLocationTab.Update();
            upSuccess.Update();
        }

        private void UpdateDeleteConfirmPanel()
        {
            upDeleteConfirm.Update();
        }

        private void UpdateSuccessPanel()
        {
            upSuccess.Update();
        }
    }
}