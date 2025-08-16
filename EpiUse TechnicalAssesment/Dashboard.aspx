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
    <div class="search-row">
        <div class="search-group">
            <asp:Label ID="lblFirstName" runat="server" Text="First Name:" AssociatedControlID="txtFirstName" CssClass="search-label" />
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="search-input" placeholder="e.g. John" />
        </div>
        
        <div class="search-group">
            <asp:Label ID="lblLastName" runat="server" Text="Last Name:" AssociatedControlID="txtLastName" CssClass="search-label" />
            <asp:TextBox ID="txtLastName" runat="server" CssClass="search-input" placeholder="e.g. Smith" />
        </div>
        
        <div class="search-group">
            <asp:Label ID="lblManager" runat="server" Text="Manager:" AssociatedControlID="ddlManagers" CssClass="search-label" />
            <asp:DropDownList ID="ddlManagers" runat="server" CssClass="search-input"></asp:DropDownList>
        </div>
        
        <div class="search-group">
            <asp:Label ID="lblDepartment" runat="server" Text="Department:" AssociatedControlID="ddlDepartments" CssClass="search-label" />
            <asp:DropDownList ID="ddlDepartments" runat="server" CssClass="search-input" 
                AutoPostBack="true" OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged"></asp:DropDownList>
        </div>
    </div>
    
    <div class="search-row">
        <div class="search-group">
            <asp:Label ID="lblLocation" runat="server" Text="Location:" AssociatedControlID="ddlLocations" CssClass="search-label" />
            <asp:DropDownList ID="ddlLocations" runat="server" CssClass="search-input"></asp:DropDownList>
        </div>
        
        <div class="search-group">
            <asp:Label ID="lblMinSalary" runat="server" Text="Minimum Salary:" AssociatedControlID="txtMinSalary" CssClass="search-label" />
            <asp:TextBox ID="txtMinSalary" runat="server" CssClass="search-input" placeholder="e.g. 10000" />
            <asp:RegularExpressionValidator ID="revMinSalary" runat="server" 
                ControlToValidate="txtMinSalary"
                ValidationExpression="^\d*\.?\d+$"
                ErrorMessage="Minimum salary must be a positive number"
                Display="Dynamic"
                ForeColor="Red"
                ValidationGroup="SearchGroup" />
            <asp:CompareValidator ID="cvMinSalary" runat="server"
                ControlToValidate="txtMinSalary"
                Operator="GreaterThanEqual"
                Type="Double"
                ValueToCompare="0"
                ErrorMessage="Salary cannot be negative"
                Display="Dynamic"
                ForeColor="Red"
                ValidationGroup="SearchGroup" />
        </div>
        
        <div class="search-group">
            <asp:Label ID="lblMaxSalary" runat="server" Text="Maximum Salary:" AssociatedControlID="txtMaxSalary" CssClass="search-label" />
            <asp:TextBox ID="txtMaxSalary" runat="server" CssClass="search-input" placeholder="e.g. 50000" />
            <asp:RegularExpressionValidator ID="revMaxSalary" runat="server" 
                ControlToValidate="txtMaxSalary"
                ValidationExpression="^\d*\.?\d+$"
                ErrorMessage="Maximum salary must be a positive number"
                Display="Dynamic"
                ForeColor="Red"
                ValidationGroup="SearchGroup" />
            <asp:CompareValidator ID="cvMaxSalary" runat="server"
                ControlToValidate="txtMaxSalary"
                Operator="GreaterThanEqual"
                Type="Double"
                ValueToCompare="0"
                ErrorMessage="Salary cannot be negative"
                Display="Dynamic"
                ForeColor="Red"
                ValidationGroup="SearchGroup" />
        </div>
    </div>
    
    <div class="search-group button-group">
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-green" 
            OnClick="btnSearch_Click" ValidationGroup="SearchGroup" />
        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-yellow" 
            OnClick="btnReset_Click" />
    </div>

    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ShowSummary="true" ShowMessageBox="false" DisplayMode="BulletList"
        ForeColor="Red" CssClass="validation-summary" ValidationGroup="SearchGroup" />
</asp:Panel>
        <div class="table-container">
            <asp:Label ID="lblNoRecords" runat="server" Text="" CssClass="no-records-message" Visible="false"></asp:Label>
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
                    <asp:BoundField DataField="ManagerName" HeaderText="Manager" />

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnView" runat="server" Text="View" CommandName="View"
                                CommandArgument='<%# Eval("EmployeeNumber") %>' CssClass="btn btn-view-green" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <RowStyle CssClass="grid-row" />
                <AlternatingRowStyle CssClass="grid-alt-row" />
                <SelectedRowStyle CssClass="grid-selected-row" />
                <HeaderStyle CssClass="grid-header" />
                <PagerStyle CssClass="grid-pager" />
            </asp:GridView>
        </div>
    </div>

    <script type="text/javascript">
        // Ensure table header stays aligned with content during scroll
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

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            syncScrollHeaders();
        });
    </script>
</asp:Content>