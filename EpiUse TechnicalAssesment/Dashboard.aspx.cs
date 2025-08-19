using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm3 : System.Web.UI.Page
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
                LoadPositions();
                LoadLocations();
                LoadDepartments();
                lblNoRecords.Visible = false;
                LoadEmployees();
            }
        }

        private void LoadDepartments()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT DISTINCT DepartmentID, DepartmentName 
                                FROM DEPARTMENT 
                                ORDER BY DepartmentName";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlDepartments.DataSource = cmd.ExecuteReader();
                ddlDepartments.DataTextField = "DepartmentName";
                ddlDepartments.DataValueField = "DepartmentID";
                ddlDepartments.DataBind();
                ddlDepartments.Items.Insert(0, new ListItem("Select a department", ""));
            }
        }

        private void LoadLocations()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT LocationID, LocationName FROM LOCATION ORDER BY LocationName";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlLocations.DataSource = cmd.ExecuteReader();
                ddlLocations.DataTextField = "LocationName";
                ddlLocations.DataValueField = "LocationID";
                ddlLocations.DataBind();
                ddlLocations.Items.Insert(0, new ListItem("Select a location", ""));
            }
        }

        private void LoadPositions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT PositionID, PositionName
                                FROM POSITION
                                ORDER BY PositionName";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlPositions.DataSource = cmd.ExecuteReader();
                ddlPositions.DataTextField = "PositionName";
                ddlPositions.DataValueField = "PositionID";
                ddlPositions.DataBind();
                ddlPositions.Items.Insert(0, new ListItem("Select a position", ""));
            }
        }

        private void LoadEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
       SELECT 
    EMPLOYEES.EmployeeID, 
    EMPLOYEES.FirstName, 
    EMPLOYEES.LastName, 
    EMPLOYEES.Email, 
    POSITION.PositionName AS Role,
    LOCATION.LocationName, 
    DEPARTMENT.DepartmentName,
    MANAGER.FirstName + ' ' + MANAGER.LastName AS ManagerName,
    SALARY.Amount AS Salary
FROM EMPLOYEES
LEFT JOIN DEPARTMENT ON EMPLOYEES.DepartmentID = DEPARTMENT.DepartmentID
LEFT JOIN LOCATION ON DEPARTMENT.LocationID = LOCATION.LocationID
LEFT JOIN POSITION ON EMPLOYEES.PositionID = POSITION.PositionID
LEFT JOIN REPORTING_LINE ON EMPLOYEES.EmployeeID = REPORTING_LINE.ReportEmployeeID
LEFT JOIN EMPLOYEES AS MANAGER ON REPORTING_LINE.ManagerEmployeeID = MANAGER.EmployeeID
LEFT JOIN SALARY ON EMPLOYEES.EmployeeID = SALARY.EmployeeID
WHERE (@FirstName = '' OR EMPLOYEES.FirstName LIKE '%' + @FirstName + '%')
  AND (@LastName = '' OR EMPLOYEES.LastName LIKE '%' + @LastName + '%')
  AND (@PositionID = '' OR EMPLOYEES.PositionID = @PositionID)
  AND (@DepartmentID = '' OR EMPLOYEES.DepartmentID = @DepartmentID)
  AND (@LocationID = '' OR DEPARTMENT.LocationID = @LocationID)
  AND ((@MinSalary IS NULL OR SALARY.Amount >= @MinSalary)
      AND (@MaxSalary IS NULL OR SALARY.Amount <= @MaxSalary))
ORDER BY EMPLOYEES.EmployeeID";

                SqlCommand cmd = new SqlCommand(query, conn);

                // Set parameters (unchanged from your original code)
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@FirstNamePattern", "%" + txtFirstName.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastNamePattern", "%" + txtLastName.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@PositionID",
                    string.IsNullOrEmpty(ddlPositions.SelectedValue) ? "" : ddlPositions.SelectedValue);
                cmd.Parameters.AddWithValue("@DepartmentID",
                    string.IsNullOrEmpty(ddlDepartments.SelectedValue) ? "" : ddlDepartments.SelectedValue);
                cmd.Parameters.AddWithValue("@LocationID",
                    string.IsNullOrEmpty(ddlLocations.SelectedValue) ? "" : ddlLocations.SelectedValue);
                cmd.Parameters.AddWithValue("@MinSalary",
                    decimal.TryParse(txtMinSalary.Text, out decimal minSalary) ? minSalary : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxSalary",
                    decimal.TryParse(txtMaxSalary.Text, out decimal maxSalary) ? maxSalary : (object)DBNull.Value);

                conn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    EmployeeGridView.DataSource = dt;
                    EmployeeGridView.DataBind();
                    EmployeeGridView.Visible = true;
                    lblNoRecords.Visible = false;
                }
                else
                {
                    EmployeeGridView.Visible = false;
                    lblNoRecords.Visible = true;
                    lblNoRecords.Text = "No records found matching your criteria.";
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtMinSalary.Text = "";
            txtMaxSalary.Text = "";
            ddlPositions.SelectedIndex = 0;
            ddlDepartments.SelectedIndex = 0;
            ddlLocations.SelectedIndex = 0;
            lblNoRecords.Visible = false;
            LoadEmployees();
        }

        protected void EmployeeGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            EmployeeGridView.PageIndex = e.NewPageIndex;
            LoadEmployees();
        }

        protected void EmployeeGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                // Check if session is still valid
                if (Session["EmployeeID"] == null)
                {
                    Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.Url.ToString()));
                    return;
                }

                string empId = e.CommandArgument.ToString();
                Response.Redirect($"ViewEmployee.aspx?empId={empId}");
            }
        }

    }
}