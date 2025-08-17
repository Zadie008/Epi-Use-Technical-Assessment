using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["EmployeeNumber"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                LoadSeniors();
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
                // Get distinct department names with the first ID found for each
                string query = @"SELECT DepartmentID, DepartmentName 
                        FROM Departments 
                        WHERE DepartmentID IN (
                            SELECT MIN(DepartmentID) 
                            FROM Departments 
                            GROUP BY DepartmentName
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
                string query = "SELECT LocationID, LocationName FROM Locations ORDER BY LocationName";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlLocations.DataSource = cmd.ExecuteReader();
                ddlLocations.DataTextField = "LocationName";
                ddlLocations.DataValueField = "LocationID";
                ddlLocations.DataBind();
                ddlLocations.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select a location", ""));
            }
        }
        protected void EmployeeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        private void LoadSeniors()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT EmployeeNumber, 
                   FirstName + ' ' + LastName AS SeniorName
            FROM Employees
            WHERE Role LIKE 'Senior%'   -- finds Senior HR, Senior Developer, etc.
            ORDER BY SeniorName";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlSeniors.DataSource = cmd.ExecuteReader();
                ddlSeniors.DataTextField = "SeniorName";
                ddlSeniors.DataValueField = "EmployeeNumber";
                ddlSeniors.DataBind();
                ddlSeniors.Items.Insert(0, new ListItem("Select a senior", ""));
            }
        }
        private void LoadEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
               
string query = @"
SELECT e.EmployeeNumber, 
       e.FirstName, 
       e.LastName, 
       e.Email, 
       e.Role, 
       l.LocationName, 
       d.DepartmentName,
       m.FirstName + ' ' + m.LastName AS ManagerName,
       e.Salary
FROM Employees e
LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
LEFT JOIN Locations l ON e.LocationID = l.LocationID
LEFT JOIN Employees m ON e.ManagerID = m.EmployeeNumber
WHERE(@FirstName = '' OR e.FirstName LIKE @FirstNamePattern)
  AND(@LastName = '' OR e.LastName LIKE @LastNamePattern)
  AND(@ManagerID = '' OR e.ManagerID = @ManagerID)
  AND(@DepartmentID = '' OR d.DepartmentName = (
      SELECT DepartmentName FROM Departments WHERE DepartmentID = @DepartmentID
  ))
  AND(@LocationID = '' OR e.LocationID = @LocationID)
  AND((@MinSalary IS NULL OR e.Salary >= @MinSalary)
      AND(@MaxSalary IS NULL OR e.Salary <= @MaxSalary))
ORDER BY e.EmployeeNumber";

                SqlCommand cmd = new SqlCommand(query, conn);

                // First Name filter
                string firstName = txtFirstName.Text.Trim();
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@FirstNamePattern", "%" + firstName + "%");

                // Last Name filter
                string lastName = txtLastName.Text.Trim();
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@LastNamePattern", "%" + lastName + "%");

                // Manager filter
                cmd.Parameters.AddWithValue("@ManagerID",
                    string.IsNullOrEmpty(ddlSeniors.SelectedValue) ? "" : ddlSeniors.SelectedValue);

                // Department filter
                cmd.Parameters.AddWithValue("@DepartmentID",
                    string.IsNullOrEmpty(ddlDepartments.SelectedValue) ? "" : ddlDepartments.SelectedValue);

                // Location filter
                cmd.Parameters.AddWithValue("@LocationID",
                    string.IsNullOrEmpty(ddlLocations.SelectedValue) ? "" : ddlLocations.SelectedValue);
                // Salary issues
                if (!string.IsNullOrWhiteSpace(txtMinSalary.Text) && decimal.TryParse(txtMinSalary.Text, out decimal minSalary))
                {
                    cmd.Parameters.AddWithValue("@MinSalary", minSalary);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MinSalary", DBNull.Value);
                }

                if (!string.IsNullOrWhiteSpace(txtMaxSalary.Text) && decimal.TryParse(txtMaxSalary.Text, out decimal maxSalary))
                {
                    cmd.Parameters.AddWithValue("@MaxSalary", maxSalary);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MaxSalary", DBNull.Value);
                }

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
            ddlSeniors.SelectedIndex = 0;
            ddlDepartments.SelectedIndex = 0;
            ddlLocations.SelectedIndex = 0;
            lblNoRecords.Visible = false;
            LoadEmployees();
        }

        protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }
        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            EmployeeGridView.PageIndex = e.NewPageIndex;
            LoadEmployees(); // Reload data for the new page
        }
        protected void EmployeeGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                string empNumber = e.CommandArgument.ToString();
                // Correct the query string parameter from "emp" to "empId"
                Response.Redirect("ViewEmployee.aspx?empId=" + empNumber);
            }
        }
    }
}