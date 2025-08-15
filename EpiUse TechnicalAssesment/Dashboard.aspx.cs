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
            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                LoadManagers();
                LoadLocations();
                LoadDepartments();
                LoadEmployees();
            }
        }
        private void LoadEmployeeData()
        {
            string query = @"
        SELECT FirstName, LastName, Email, Role, 
               L.LocationName, D.DepartmentName, EmployeeNumber
        FROM Employees E
        INNER JOIN Locations L ON E.LocationID = L.LocationID
        INNER JOIN Departments D ON E.DepartmentID = D.DepartmentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                EmployeeGridView.DataSource = reader;
                EmployeeGridView.DataBind();
            }
        }
        private void LoadDepartments()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName";
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
                ddlLocations.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", ""));
            }
        }
        protected void EmployeeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // You can add your logic here.
            // For example, to check the type of row and perform actions.
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Example: Change the background color of the row based on a condition
                // DataRowView rowView = (DataRowView)e.Row.DataItem;
                // if (rowView["SomeValue"].ToString() == "Important")
                // {
                //     e.Row.BackColor = System.Drawing.Color.LightCoral;
                // }

            }
        }

        private void LoadManagers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT DISTINCT m.EmployeeNumber, 
                        m.FirstName + ' ' + m.LastName AS ManagerName
                 FROM Employees e
                 INNER JOIN Employees m ON e.ManagerID = m.EmployeeNumber
                 WHERE e.ManagerID IS NOT NULL
                 ORDER BY ManagerName";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlManagers.DataSource = cmd.ExecuteReader();
                ddlManagers.DataTextField = "ManagerName";
                ddlManagers.DataValueField = "EmployeeNumber";
                ddlManagers.DataBind();
                ddlManagers.Items.Insert(0, new ListItem("All", ""));
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
               m.FirstName + ' ' + m.LastName AS ManagerName
        FROM Employees e
        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
        LEFT JOIN Locations l ON e.LocationID = l.LocationID
        LEFT JOIN Employees m ON e.ManagerID = m.EmployeeNumber
        WHERE (@FirstName = '' OR e.FirstName LIKE @FirstNamePattern)
          AND (@LastName = '' OR e.LastName LIKE @LastNamePattern)
          AND (@ManagerID = '' OR e.ManagerID = @ManagerID)
          AND (@DepartmentID = '' OR e.DepartmentID = @DepartmentID)
          AND (@LocationID = '' OR e.LocationID = @LocationID)
        ORDER BY e.FirstName, e.LastName";

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
                    string.IsNullOrEmpty(ddlManagers.SelectedValue) ? "" : ddlManagers.SelectedValue);

                // Department filter
                cmd.Parameters.AddWithValue("@DepartmentID",
                    string.IsNullOrEmpty(ddlDepartments.SelectedValue) ? "" : ddlDepartments.SelectedValue);

                // Location filter
                cmd.Parameters.AddWithValue("@LocationID",
                    string.IsNullOrEmpty(ddlLocations.SelectedValue) ? "" : ddlLocations.SelectedValue);

                conn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                EmployeeGridView.DataSource = dt;
                EmployeeGridView.DataBind();
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
            ddlManagers.SelectedIndex = 0;
            ddlDepartments.SelectedIndex = 0;
            ddlLocations.SelectedIndex = 0;
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
                Response.Redirect("ViewEmployee.aspx?emp=" + empNumber);
            }
        }
    }
}