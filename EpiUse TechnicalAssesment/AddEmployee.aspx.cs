using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

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

            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                // Populate positions dropdown
                string positionQuery = "SELECT PositionID, PositionName FROM POSITION";
                SqlCommand cmdPosition = new SqlCommand(positionQuery, connection);
                connection.Open();
                SqlDataReader reader = cmdPosition.ExecuteReader();
                while (reader.Read())
                {
                    positionDropdown.Items.Add(new ListItem(reader["PositionName"].ToString(), reader["PositionID"].ToString()));
                }
                reader.Close();
                connection.Close();

                // Populate departments dropdown
                string departmentQuery = "SELECT DepartmentID, DepartmentName FROM DEPARTMENT";
                SqlCommand cmdDepartment = new SqlCommand(departmentQuery, connection);
                connection.Open();
                SqlDataReader departmentReader = cmdDepartment.ExecuteReader();
                while (departmentReader.Read())
                {
                    departmentDropdown.Items.Add(new ListItem(departmentReader["DepartmentName"].ToString(), departmentReader["DepartmentID"].ToString()));
                }
                departmentReader.Close();
                connection.Close();

                // Populate locations dropdown
                string locationQuery = "SELECT LocationID, LocationName FROM LOCATION";
                SqlCommand cmdLocation = new SqlCommand(locationQuery, connection);
                connection.Open();
                SqlDataReader locationReader = cmdLocation.ExecuteReader();
                while (locationReader.Read())
                {
                    locationDropdown.Items.Add(new ListItem(locationReader["LocationName"].ToString(), locationReader["LocationID"].ToString()));
                }
                locationReader.Close();
                connection.Close();

                // Populate manager dropdown (for reporting line)
                string managerQuery = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM EMPLOYEES";
                SqlCommand cmdManager = new SqlCommand(managerQuery, connection);
                connection.Open();
                SqlDataReader managerReader = cmdManager.ExecuteReader();
                while (managerReader.Read())
                {
                    managerDropdown.Items.Add(new ListItem(managerReader["FullName"].ToString(), managerReader["EmployeeID"].ToString()));
                }
                managerReader.Close();
                connection.Close();
            }
        }

        protected void addEmployee(object sender, EventArgs e)
        {
            // Change .Text to .Value for all HTML input controls
            string firstName = firstNameTextbox.Value;
            string lastName = lastNameTextbox.Value;
            DateTime dob = Convert.ToDateTime(dobTextbox.Value);
            string email = emailTextbox.Value;
            string password = passwordTextbox.Value;
            decimal salary = Convert.ToDecimal(salaryTextbox.Value);

            // The rest of your code remains the same as it correctly references the dropdowns
            int positionId = Convert.ToInt32(positionDropdown.SelectedValue);
            int departmentId = Convert.ToInt32(departmentDropdown.SelectedValue);
            int locationId = Convert.ToInt32(locationDropdown.SelectedValue);
            int managerId = Convert.ToInt32(managerDropdown.SelectedValue);

            string hashedPassword = HashPassword(password);

            SqlConnection connection = new SqlConnection(connectionString);

            // Insert into Employees table
            string employeeInsertQuery = "INSERT INTO EMPLOYEES (FirstName, LastName, DateOfBirth, Email, DepartmentID, PositionID) VALUES (@FirstName, @LastName, @DOB, @Email, @DepartmentID, @PositionID)";
            SqlCommand cmd = new SqlCommand(employeeInsertQuery, connection);
            cmd.Parameters.AddWithValue("@FirstName", firstName);
            cmd.Parameters.AddWithValue("@LastName", lastName);
            cmd.Parameters.AddWithValue("@DOB", dob);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
            cmd.Parameters.AddWithValue("@PositionID", positionId);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();

            // Get the last inserted EmployeeID
            int employeeId = GetLastInsertedEmployeeId();

            // Insert into USER_AUTH
            string userAuthInsertQuery = "INSERT INTO USER_AUTH (EmployeeID, Password, RoleID) VALUES (@EmployeeID, @Password, @RoleID)";
            SqlCommand cmdAuth = new SqlCommand(userAuthInsertQuery, connection);
            cmdAuth.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmdAuth.Parameters.AddWithValue("@Password", hashedPassword);
            cmdAuth.Parameters.AddWithValue("@RoleID", 3);  // Assuming 'Role 3' is for new users
            connection.Open();
            cmdAuth.ExecuteNonQuery();
            connection.Close();

            // Insert into SALARY
            string salaryInsertQuery = "INSERT INTO SALARY (EmployeeID, Amount) VALUES (@EmployeeID, @Amount)";
            SqlCommand cmdSalary = new SqlCommand(salaryInsertQuery, connection);
            cmdSalary.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmdSalary.Parameters.AddWithValue("@Amount", salary);
            connection.Open();
            cmdSalary.ExecuteNonQuery();
            connection.Close();

            // Insert into REPORTING_LINE (if manager selected)
            if (managerId != 0)
            {
                string reportingLineInsertQuery = "INSERT INTO REPORTING_LINE (ManagerEmployeeID, ReportEmployeeID) VALUES (@ManagerEmployeeID, @ReportEmployeeID)";
                SqlCommand cmdReportingLine = new SqlCommand(reportingLineInsertQuery, connection);
                cmdReportingLine.Parameters.AddWithValue("@ManagerEmployeeID", managerId);
                cmdReportingLine.Parameters.AddWithValue("@ReportEmployeeID", employeeId);
                connection.Open();
                cmdReportingLine.ExecuteNonQuery();
                connection.Close();
            }

        
        }

        private int GetLastInsertedEmployeeId()
        {
            string query = "SELECT MAX(EmployeeID) FROM EMPLOYEES";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            int lastEmployeeId = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return lastEmployeeId;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
