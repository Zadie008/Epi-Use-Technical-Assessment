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

                // 1. Reporting lines
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

                // 2. Employees with extended information
                using (var cmd = new SqlCommand(@"
                    SELECT e.EmployeeID, e.FirstName, e.LastName, e.Email, p.Picture,  
                           e.PositionID, pos.PositionName
                    FROM EMPLOYEES e
                    LEFT JOIN EMPLOYEE_PICTURE p ON e.EmployeeID = p.EmployeeID
                    LEFT JOIN POSITION pos ON e.PositionID = pos.PositionID", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string empId = reader["EmployeeID"].ToString();
                        string email = reader["Email"].ToString();
                        string picture = reader["Picture"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["Picture"]) : null;
                        string positionName = reader["PositionName"] != DBNull.Value ? reader["PositionName"].ToString() : "No Position";

                        string imageUrl;
                        if (!string.IsNullOrEmpty(picture))
                        {
                            imageUrl = $"data:image/png;base64,{picture}";
                        }
                        else
                        {
                            string emailHash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(email.Trim().ToLower(), "MD5").ToLower();
                            imageUrl = $"https://www.gravatar.com/avatar/{emailHash}?d=identicon";
                        }

                        employees[empId] = new
                        {
                            id = empId,
                            EmployeeID = empId,
                            Name = $"{reader["FirstName"]} {reader["LastName"]}",
                            Email = email,
                            PositionName = positionName,
                            ImageUrl = imageUrl,
                            children = new List<dynamic>()
                        };
                    }
                }
            }

            // 3. Build hierarchy
            var rootNodes = new List<dynamic>();

            foreach (var employee in employees.Values)
            {
                bool hasManager = reportingLines.Any(x => x.Value.Contains(employee.id));
                if (!hasManager) rootNodes.Add(employee);
            }

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

            // 4. Root node
            dynamic hierarchyData = rootNodes.Count == 1 ? rootNodes[0] : new
            {
                id = "org-root",
                Name = "Organization",
                ImageUrl = "",
                children = rootNodes
            };

            return new JavaScriptSerializer().Serialize(hierarchyData);
        }

    }
}