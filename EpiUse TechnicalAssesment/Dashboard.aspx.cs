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
                string query = @"SELECT DepartmentID, DepartmentName 
                                FROM DEPARTMENT 
                                WHERE DepartmentID IN (
                                    
                                    GROUP BY DepartmentName
ORDER BY DepartmentName
                                )
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
                string query = @"SELECT PositionName
                                FROM POSITION
                               ";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlPositions.DataSource = cmd.ExecuteReader();
                ddlPositions.DataTextField = "SeniorName";
                ddlPositions.DataValueField = "EmployeeID";
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
                    e.EmployeeID, 
                    e.FirstName, 
                    e.LastName, 
                    e.Email, 
                    p.PositionName AS Role,  -- Now shows proper role names (CEO, Head of Cape Town, etc.)
                    l.LocationName, 
                    d.DepartmentName,
                    m.FirstName + ' ' + m.LastName AS ManagerName,
                    e.Salary
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Locations l ON e.LocationID = l.LocationID
                LEFT JOIN Position p ON e.PositionID = p.PositionID  -- NEW JOIN
                LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                WHERE (@FirstName = '' OR e.FirstName LIKE @FirstNamePattern)
                  AND (@LastName = '' OR e.LastName LIKE @LastNamePattern)
                  AND (@ManagerID = '' OR e.ManagerID = @ManagerID)
                  AND (@DepartmentID = '' OR d.DepartmentID = @DepartmentID)
                  AND (@LocationID = '' OR e.LocationID = @LocationID)
                  AND ((@MinSalary IS NULL OR e.Salary >= @MinSalary)
                      AND (@MaxSalary IS NULL OR e.Salary <= @MaxSalary))
                ORDER BY e.EmployeeID";

                SqlCommand cmd = new SqlCommand(query, conn);

                // Set parameters
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@FirstNamePattern", "%" + txtFirstName.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastNamePattern", "%" + txtLastName.Text.Trim() + "%");

                // Handle dropdown selections
                cmd.Parameters.AddWithValue("@ManagerID",
                    string.IsNullOrEmpty(ddlPositions.SelectedValue) ? "" : ddlPositions.SelectedValue);
                cmd.Parameters.AddWithValue("@DepartmentID",
                    string.IsNullOrEmpty(ddlDepartments.SelectedValue) ? "" : ddlDepartments.SelectedValue);
                cmd.Parameters.AddWithValue("@LocationID",
                    string.IsNullOrEmpty(ddlLocations.SelectedValue) ? "" : ddlLocations.SelectedValue);

                // Handle salary filters
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

        //protected void btnSearch_Click(object sender, EventArgs e)
        //{
        //    LoadEmployees();
        //}

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

        //protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadEmployees();
        //}

        protected void EmployeeGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            EmployeeGridView.PageIndex = e.NewPageIndex;
            LoadEmployees();
        }

        protected void EmployeeGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                string empNumber = e.CommandArgument.ToString();
                Response.Redirect($"ViewEmployee.aspx?empId={empNumber}");
            }
        }
    }
}