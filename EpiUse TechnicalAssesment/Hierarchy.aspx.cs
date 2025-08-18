using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace EpiUse_TechnicalAssesment
{
    public partial class WebForm7 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetHierarchyData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            var employees = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT e.EmployeeNumber, e.FirstName, e.LastName, e.Role, e.ManagerID,
                   e.Email, e.ProfilePhotoBase64, e.PositionID,
                   p.PositionName, p.Ranking,
                   l.LocationName, d.DepartmentName
            FROM Employees e
            LEFT JOIN Position p ON e.PositionID = p.PositionID
            LEFT JOIN Locations l ON e.LocationID = l.LocationID
            LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
            ORDER BY p.Ranking DESC, e.LastName, e.FirstName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new
                        {
                            id = reader["EmployeeNumber"].ToString(), // This is crucial for D3.js
                            EmployeeNumber = reader["EmployeeNumber"].ToString(),
                            Name = reader["FirstName"].ToString() + " " + reader["LastName"].ToString(),
                            Role = reader["Role"].ToString(),
                            ManagerID = reader["ManagerID"] != DBNull.Value ? reader["ManagerID"].ToString() : null,
                            Email = reader["Email"].ToString(),
                            Photo = reader["ProfilePhotoBase64"].ToString(),
                            Position = reader["PositionName"].ToString(),
                            PositionID = Convert.ToInt32(reader["PositionID"]),
                            Ranking = reader["Ranking"] != DBNull.Value ? Convert.ToInt32(reader["Ranking"]) : 0,
                            Location = reader["LocationName"].ToString(),
                            Department = reader["DepartmentName"].ToString()
                        });
                    }
                }
            }
            return employees;
        }
    }
}
