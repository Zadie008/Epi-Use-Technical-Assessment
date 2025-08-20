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

                string departmentQuery = "SELECT DepartmentID, DepartmentName FROM DEPARTMENT ORDER BY DepartmentName";
                using (SqlCommand cmdDepartment = new SqlCommand(departmentQuery, connection))
                {
                    using (SqlDataReader departmentReader = cmdDepartment.ExecuteReader())
                    {
                        departmentDropdown.Items.Clear();
                        // Optional: Add a placeholder item
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

                string locationQuery = "SELECT LocationID, LocationName FROM LOCATION";
                using (SqlCommand cmdLocation = new SqlCommand(locationQuery, connection))
                {
                    using (SqlDataReader locationReader = cmdLocation.ExecuteReader())
                    {
                        ddlDepartmentLocation.Items.Clear();
                        while (locationReader.Read())
                        {
                            ddlDepartmentLocation.Items.Add(new ListItem(
                                locationReader["LocationName"].ToString(),
                                locationReader["LocationID"].ToString()
                            ));
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
                    lblSuccessMessage.Text = "Your success message";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                    upSuccessModal.Update();

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

                            lblSuccessMessage.Text = "Your success message";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                            upSuccessModal.Update();

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
            int locationId = Convert.ToInt32(ddlDepartmentLocation.SelectedValue);

            if (string.IsNullOrEmpty(departmentName))
            {
                departmentValidationMessage.Text = "Department name cannot be empty.";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // First, get the maximum DepartmentID and increment it
                    string getMaxIdQuery = "SELECT ISNULL(MAX(DepartmentID), 0) FROM DEPARTMENT";
                    using (SqlCommand cmdMaxId = new SqlCommand(getMaxIdQuery, connection))
                    {
                        connection.Open();
                        int newDepartmentId = Convert.ToInt32(cmdMaxId.ExecuteScalar()) + 1;

                        string insertQuery = "INSERT INTO DEPARTMENT (DepartmentID, DepartmentName, LocationID) VALUES (@DepartmentID, @DepartmentName, @LocationID)";
                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@DepartmentID", newDepartmentId);
                            cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                            cmd.Parameters.AddWithValue("@LocationID", locationId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    departmentValidationMessage.Text = "Department added successfully!";
                    departmentValidationMessage.ForeColor = System.Drawing.Color.Green;
                    txtDepartmentName.Text = "";
                    BindDepartmentsGridView();
                }
            }
            catch (Exception ex)
            {
                departmentValidationMessage.Text = "Error adding department: " + ex.Message;
                departmentValidationMessage.ForeColor = System.Drawing.Color.Red;
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
        private DataTable GetDepartmentsAtSameLocation(int departmentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Fixed query - changed 'current' to 'curr' to avoid reserved keyword conflict
                string query = @"SELECT d.DepartmentID, d.DepartmentName 
                        FROM DEPARTMENT d
                        INNER JOIN DEPARTMENT curr ON d.LocationID = curr.LocationID
                        WHERE curr.DepartmentID = @DepartmentID AND d.DepartmentID != @DepartmentID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
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
                        cmd.ExecuteNonQuery();
                    }

                    // Delete department
                    string deleteQuery = "DELETE FROM DEPARTMENT WHERE DepartmentID = @DepartmentID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", sourceDepartmentId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    lblSuccessMessage.Text = "Your success message";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                    upSuccessModal.Update();
                    BindGridViews();
                    PopulateDropdowns();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    departmentValidationMessage.Text = "An error occurred: " + ex.Message;
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
                // First get the maximum PositionID
                string maxIdQuery = "SELECT ISNULL(MAX(PositionID), 0) FROM POSITION";
                string insertQuery = "INSERT INTO POSITION (PositionID, PositionName) VALUES (@PositionID, @PositionName)";

                using (SqlCommand cmdMaxId = new SqlCommand(maxIdQuery, connection))
                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        int newPositionId = Convert.ToInt32(cmdMaxId.ExecuteScalar()) + 1;

                        cmdInsert.Parameters.AddWithValue("@PositionID", newPositionId);
                        cmdInsert.Parameters.AddWithValue("@PositionName", positionName);
                        cmdInsert.ExecuteNonQuery();
                        lblSuccessMessage.Text = "Your success message";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        upSuccessModal.Update();

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
        protected void ReassignEmployeesAndDeletePosition(int positionId, int targetPositionId)
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
                        cmd.ExecuteNonQuery();
                    }

                    // Delete position
                    string deleteQuery = "DELETE FROM POSITION WHERE PositionID = @PositionID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", positionId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    lblSuccessMessage.Text = "Your success message";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                    upSuccessModal.Update();
                    BindGridViews();
                    PopulateDropdowns();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    positionValidationMessage.Text = "An error occurred: " + ex.Message;
                }
            }
        }
        protected void gvPositions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int positionId = Convert.ToInt32(gvPositions.DataKeys[e.RowIndex].Value);

            // Check if position has employees
            if (PositionHasEmployees(positionId))
            {
                // Store position ID in session for later use
                Session["PositionToDelete"] = positionId;

                // Get the next position in hierarchy
                int nextPositionId = GetNextPositionId(positionId);

                if (nextPositionId == 0)
                {
                    // Cancel the delete operation - no other positions exist
                    e.Cancel = true;
                    positionValidationMessage.Text = "Cannot delete position. This is the only position in the system.";
                    return;
                }

                // Get position name for display
                string nextPositionName = GetPositionName(nextPositionId);

                // Store next position info in session
                Session["NextPositionId"] = nextPositionId;
                Session["NextPositionName"] = nextPositionName;

                // Update the label in the modal - THIS IS THE KEY FIX
                lblNextPosition.Text = nextPositionName;

                // Show confirmation modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPositionDeleteModal", "showPositionDeleteModal();", true);

                // Update the panel to ensure the label text is rendered
                upPositionTab.Update();
            }
            else
            {
                // No employees, proceed with deletion
                DeletePosition(positionId);
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

                        lblSuccessMessage.Text = "Your success message";
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
                locationValidationMessage.Text = "Location name is required.";
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // First get the maximum LocationID
                string maxIdQuery = "SELECT ISNULL(MAX(LocationID), 0) FROM LOCATION";
                string insertQuery = "INSERT INTO LOCATION (LocationID, LocationName) VALUES (@LocationID, @LocationName)";

                using (SqlCommand cmdMaxId = new SqlCommand(maxIdQuery, connection))
                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        int newLocationId = Convert.ToInt32(cmdMaxId.ExecuteScalar()) + 1;

                        cmdInsert.Parameters.AddWithValue("@LocationID", newLocationId);
                        cmdInsert.Parameters.AddWithValue("@LocationName", locationName);
                        cmdInsert.ExecuteNonQuery();

                        lblSuccessMessage.Text = "Your success message";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        upSuccessModal.Update();

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

            // Check if department has employees
            if (DepartmentHasEmployees(departmentId))
            {
                // Store department ID in session for later use
                Session["DepartmentToDelete"] = departmentId;

                // Get departments at the same location
                DataTable targetDepartments = GetDepartmentsAtSameLocation(departmentId);

                if (targetDepartments.Rows.Count == 0)
                {
                    // Cancel the delete operation
                    e.Cancel = true;
                    departmentValidationMessage.Text = "Cannot delete department. No other departments at this location to reassign employees to.";
                    return;
                }

                // Bind departments to dropdown
                ddlTargetDepartment.DataSource = targetDepartments;
                ddlTargetDepartment.DataTextField = "DepartmentName";
                ddlTargetDepartment.DataValueField = "DepartmentID";
                ddlTargetDepartment.DataBind();

                // Show reassignment modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowReassignmentModal", "showReassignmentModal();", true);
            }
            else
            {
                // No employees, proceed with deletion
                DeleteDepartment(departmentId);
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

                        lblSuccessMessage.Text = "Your success message";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        upSuccessModal.Update();
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

                        lblSuccessMessage.Text = "Your success message";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                        upSuccessModal.Update();
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

        protected void ReassignDepartmentsAndDeleteLocation(int locationId, int targetLocationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Reassign departments
                    string reassignQuery = "UPDATE DEPARTMENT SET LocationID = @TargetLocationID WHERE LocationID = @LocationID";
                    using (SqlCommand cmd = new SqlCommand(reassignQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TargetLocationID", targetLocationId);
                        cmd.Parameters.AddWithValue("@LocationID", locationId);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete location
                    string deleteQuery = "DELETE FROM LOCATION WHERE LocationID = @LocationID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@LocationID", locationId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    lblSuccessMessage.Text = "Your success message";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                    upSuccessModal.Update();
                    BindGridViews();
                    PopulateDropdowns();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    locationValidationMessage.Text = "An error occurred: " + ex.Message;
                }
            }
        }

        protected void gvLocations_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int locationId = Convert.ToInt32(gvLocations.DataKeys[e.RowIndex].Value);

            // Check if location has departments
            if (LocationHasDepartments(locationId))
            {
                // Store location ID in session for later use
                Session["LocationToDelete"] = locationId;

                // Get the next location in hierarchy
                int nextLocationId = GetNextLocationId(locationId);

                if (nextLocationId == 0)
                {
                    // Cancel the delete operation - no other locations exist
                    e.Cancel = true;
                    locationValidationMessage.Text = "Cannot delete location. This is the only location in the system.";
                    return;
                }

                // Get location name for display
                string nextLocationName = GetLocationName(nextLocationId);

                // Store next location info in session
                Session["NextLocationId"] = nextLocationId;
                Session["NextLocationName"] = nextLocationName;

                // Update the label in the modal
                lblNextLocation.Text = nextLocationName;

                // Show confirmation modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowLocationDeleteModal", "showLocationDeleteModal();", true);

                // Update the panel to ensure the label text is rendered
                upLocationTab.Update();
            }
            else
            {
                // No departments, proceed with deletion
                DeleteLocation(locationId);
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

                            lblSuccessMessage.Text = "Your success message";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccessModal", "showSuccessModal();", true);
                            upSuccessModal.Update();

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
          
            upDeleteConfirm.Update();
        }

        private void UpdateDepartmentPanel()
        {
          
           
        }

        private void UpdatePositionPanel()
        {
            upPositionTab.Update();
           
        }

        private void UpdateLocationPanel()
        {
            upLocationTab.Update();
         
        }

        private void UpdateDeleteConfirmPanel()
        {
            upDeleteConfirm.Update();
        }

        private void UpdateSuccessPanel()
        {
           
        }
    }
}