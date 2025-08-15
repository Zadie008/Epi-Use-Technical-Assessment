<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Dashboard
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Employee Management System</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
    <div class="nav-container">
    <a href="Dashboard.aspx" class="nav-card">
        <img src="Images/Dashboard.png" alt="Hierarchy Icon" class="nav-icon" />
        <span class="nav-title">Home</span>
    </a>
    <a href="Hierarchy.aspx" class="nav-card">
        <img src="Images/Hierarchy.png" alt="Hierarchy Icon" class="nav-icon" />
        <span class="nav-title">Hierarchy</span>
    </a>
    <a href="AddEmployee.aspx" class="nav-card">
        <img src="Images/Employee.png" alt="Add employee" class="nav-icon" />
        <span class="nav-title">Add employee</span>
    </a>
    <a href="ReadMe.aspx" class="nav-card">
        <img src="Images/ReadMe.png" alt="Read Me Icon" class="nav-icon" />
        <span class="nav-title">Read Me</span>
    </a>
</div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
   
   <div class="dashboard-content">
   <h2>Employee Table</h2>

   <asp:Panel ID="SearchPanel" runat="server" DefaultButton="btnSearch" CssClass="search-panel">
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblName" runat="server" Text="First Name:" />
            </div>
            <div class="search-input-group">
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revFirstName" runat="server"
                    ControlToValidate="txtFirstName"
                    ValidationExpression="^[a-zA-Z\s'-]*$"
                    ErrorMessage="First name can only contain letters and spaces."
                    Display="None" ForeColor="Red" ValidationGroup="SearchGroup" />
            </div>
        </div>
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblSurename" runat="server" Text="Last Name:" />
            </div>
            <div class="search-input-group">
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revLastName" runat="server"
                    ControlToValidate="txtLastName"
                    ValidationExpression="^[a-zA-Z\s'-]*$"
                    ErrorMessage="Last name can only contain letters and spaces."
                    Display="None" ForeColor="Red" ValidationGroup="SearchGroup" />
            </div>
        </div>
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblManagerName" runat="server" Text="Manager Name:" />
            </div>
            <div class="search-input-group">
                <asp:DropDownList ID="ddlManagers" runat="server">
                    <asp:ListItem Text="All" Value="" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblLocation" runat="server" Text="Location:" />
            </div>
            <div class="search-input-group">
                <asp:DropDownList ID="ddlLocations" runat="server">
                    <asp:ListItem Text="All" Value="" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblDepartment" runat="server" Text="Department:" />
            </div>
            <div class="search-input-group">
                <asp:DropDownList ID="ddlDepartments" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged" />
            </div>
        </div>
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblSalaryRangeMin" runat="server" Text="Minimum Salary:" />
            </div>
            <div class="search-input-group">
                <asp:TextBox ID="txtMinSalary" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revMinSalary" runat="server"
                    ControlToValidate="txtMinSalary"
                    ValidationExpression="^\d*\.?\d*$"
                    ErrorMessage="Please enter a valid number for minimum salary."
                    Display="None" ForeColor="Red" ValidationGroup="SearchGroup" />
                <asp:RangeValidator ID="rvMinSalary" runat="server"
                    ControlToValidate="txtMinSalary"
                    MinimumValue="0" MaximumValue="99999999" Type="Currency"
                    ErrorMessage="Minimum salary must be a non-negative number."
                    Display="None" ForeColor="Red" ValidationGroup="SearchGroup" />
            </div>
        </div>
        <div class="search-form-row">
            <div class="search-label-group">
                <asp:Label ID="lblSalaryRangeMax" runat="server" Text="Maximum Salary:" />
            </div>
            <div class="search-input-group">
                <asp:TextBox ID="txtMaxSalary" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revMaxSalary" runat="server"
                    ControlToValidate="txtMaxSalary"
                    ValidationExpression="^\d*\.?\d*$"
                    ErrorMessage="Please enter a valid number for maximum salary."
                    Display="None" ForeColor="Red" ValidationGroup="SearchGroup" />
                <asp:RangeValidator ID="rvMaxSalary" runat="server"
                    ControlToValidate="txtMaxSalary"
                    MinimumValue="0" MaximumValue="99999999" Type="Currency"
                    ErrorMessage="Maximum salary must be a non-negative number."
                    Display="None" ForeColor="Red" ValidationGroup="SearchGroup" />
            </div>
        </div>
        <div class="search-buttons-group">
            <asp:Button ID="btnSearch" runat="server" Text="Search"
                CssClass="btn btn-green" OnClick="btnSearch_Click"
                ValidationGroup="SearchGroup" />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-yellow" OnClick="btnReset_Click" />
        </div>
        <div class="validation-summary-container">
            <asp:ValidationSummary
                ID="ValidationSummary1"
                runat="server"
                ShowSummary="true"
                ShowMessageBox="false"
                DisplayMode="BulletList"
                ForeColor="Red"
                CssClass="validation-summary"
                ValidationGroup="SearchGroup" />
        </div>
   </asp:Panel>

   <div class="table-container">
    <asp:GridView ID="EmployeeGridView" runat="server" AutoGenerateColumns="false" CssClass="table"
        OnRowCommand="EmployeeGridView_RowCommand"
        DataKeyNames="EmployeeNumber">
        <Columns>
            <asp:BoundField DataField="FirstName" HeaderText="First Name" />
            <asp:BoundField DataField="LastName" HeaderText="Last Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="Role" HeaderText="Role" />
            <asp:BoundField DataField="LocationName" HeaderText="Location" />
            <asp:BoundField DataField="DepartmentName" HeaderText="Department" />

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="View"
                        CommandArgument='<%# Eval("EmployeeNumber") %>' CssClass="btn btn-view-green" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

    </div>
<script type="text/javascript">
    // Ensure table header stays aligned with content during horizontal scroll
    function syncScrollHeaders() {
        const container = document.querySelector('.table-container');
        const header = container.querySelector('thead');

        if (container && header) {
            container.addEventListener('scroll', function () {
                header.style.transform = `translateX(-${this.scrollLeft}px)`;
            });
        }
    }

    // Call on page load and postbacks
    window.onload = function () {
        syncScrollHeaders();
    };

    // For ASP.NET postbacks
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(function () {
        syncScrollHeaders();
    });
</script>
    
</asp:Content>