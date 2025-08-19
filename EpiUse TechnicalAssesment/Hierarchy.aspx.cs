using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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
        public static string GetHierarchyData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            var employees = new Dictionary<string, dynamic>();
            var reportingLines = new Dictionary<string, List<string>>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Load all reporting relationships
                using (var cmd = new SqlCommand("SELECT ManagerEmployeeID, ReportEmployeeID FROM REPORTING_LINE", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string managerId = reader["ManagerEmployeeID"].ToString();
                        string reportId = reader["ReportEmployeeID"].ToString();

                        if (!reportingLines.ContainsKey(managerId))
                            reportingLines[managerId] = new List<string>();

                        reportingLines[managerId].Add(reportId);
                    }
                }

                // 2. Load all employees
                using (var cmd = new SqlCommand(
                    @"SELECT e.EmployeeID, e.FirstName, e.LastName, p.PositionName, d.DepartmentName 
              FROM Employees e
              LEFT JOIN Position p ON e.PositionID = p.PositionID
              LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string empId = reader["EmployeeID"].ToString();
                        employees[empId] = new
                        {
                            id = empId,
                            EmployeeID = empId,
                            Name = $"{reader["FirstName"]} {reader["LastName"]}",
                            Position = reader["PositionName"]?.ToString() ?? "",
                            Department = reader["DepartmentName"]?.ToString() ?? "",
                            Email = "", // Will be added if needed
                            children = new List<dynamic>() // Initialize empty children list
                        };
                    }
                }
            }

            // 3. Build the hierarchy
            var rootNodes = new List<dynamic>();

            // Find all employees who don't report to anyone (potential CEOs)
            foreach (var employee in employees.Values)
            {
                bool hasManager = reportingLines.Any(x => x.Value.Contains(employee.EmployeeID));
                if (!hasManager)
                {
                    rootNodes.Add(employee);
                }
            }

            // Add all reporting relationships
            foreach (var relationship in reportingLines)
            {
                if (employees.TryGetValue(relationship.Key, out var manager))
                {
                    foreach (var reportId in relationship.Value)
                    {
                        if (employees.TryGetValue(reportId, out var subordinate))
                        {
                            manager.children.Add(subordinate);
                        }
                    }
                }
            }

            // 4. Create the root node
            dynamic hierarchyData;
            if (rootNodes.Count == 1)
            {
                hierarchyData = rootNodes[0];
            }
            else if (rootNodes.Count > 1)
            {
                // If multiple roots (shouldn't happen in proper hierarchy)
                hierarchyData = new
                {
                    id = "org-root",
                    Name = "Organization",
                    Position = "",
                    Department = "",
                    children = rootNodes
                };
            }
            else
            {
                // Fallback if no roots found (shouldn't happen)
                hierarchyData = new
                {
                    id = "org-root",
                    Name = "Organization",
                    Position = "",
                    Department = "",
                    children = employees.Values.ToList()
                };
            }

            return new JavaScriptSerializer().Serialize(hierarchyData);
        }
    }
}