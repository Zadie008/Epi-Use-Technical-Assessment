<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Editing the Database
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Edit the database</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div class="options-panel">
        <h2>Database management</h2>
        <div class="database-buttons btn-group" role="group">
            <asp:Button ID="btnAddEmployee" runat="server" Text="Add Employee"
                CssClass="btn btn-primary active" OnClick="SwitchPanel" CommandArgument="Employee" />
            <asp:Button ID="btnAddDepartment" runat="server" Text="Add Department"
                CssClass="btn btn-primary" OnClick="SwitchPanel" CommandArgument="Department" />
            <asp:Button ID="btnAddLocation" runat="server" Text="Add Location"
                CssClass="btn btn-primary" OnClick="SwitchPanel" CommandArgument="Location" />
        </div>
    </div>
    <!-- Add Employee Form -->
    <asp:Panel ID="pnlEmployee" runat="server" Visible="true">
        <div class="form-panel">
                    <h3>Add New Employee</h3>
        <div class="form-group">
            <label>First Name</label>
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" required="true"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revFirstName" runat="server" 
                ControlToValidate="txtFirstName"
                ValidationExpression="^[a-zA-Z]+$"
                ErrorMessage="Only letters allowed"
                Display="Dynamic"
                CssClass="text-danger"></asp:RegularExpressionValidator>
        </div>
        <div class="form-group">
            <label>Last Name</label>
            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" required="true"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revLastName" runat="server" 
                ControlToValidate="txtLastName"
                ValidationExpression="^[a-zA-Z]+$"
                ErrorMessage="Only letters allowed"
                Display="Dynamic"
                CssClass="text-danger"></asp:RegularExpressionValidator>
        </div>
        <div class="form-group">
            <label>Date of birth</label>
            <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" required="true" TextMode="Date"></asp:TextBox>
            <asp:CompareValidator ID="cvDOB" runat="server"
                ControlToValidate="txtDOB"
                Operator="LessThan"
                ValueToCompare="2025-01-01"
                Type="Date"
                ErrorMessage="Must be before 2025"
                Display="Dynamic"
                CssClass="text-danger"></asp:CompareValidator>
        </div>
        <div class="form-group">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required="true"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revEmail" runat="server"
                ControlToValidate="txtEmail"
                ValidationExpression=".*@.*"
                ErrorMessage="Must contain @ symbol"
                Display="Dynamic"
                CssClass="text-danger"></asp:RegularExpressionValidator>
        </div>
        <div class="form-group">
            <label>Role</label>
            <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control" required="true"></asp:DropDownList>
        </div>
        <div class="form-group">
            <label>Salary</label>
            <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" required="true" TextMode="Number"></asp:TextBox>
            <asp:CompareValidator ID="cvSalary" runat="server"
                ControlToValidate="txtSalary"
                Operator="GreaterThanEqual"
                ValueToCompare="0"
                Type="Double"
                ErrorMessage="Cannot be negative"
                Display="Dynamic"
                CssClass="text-danger"></asp:CompareValidator>
        </div>
        <div class="form-group">
            <label>Department</label>
            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" required="true"></asp:DropDownList>
        </div>
        <div class="form-group">
            <label>Location</label>
            <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control" required="true"></asp:DropDownList>
        </div>
        <div class="form-group">
            <label>Password</label>
            <asp:TextBox ID="txtPassword1" runat="server" CssClass="form-control" required="true" TextMode="Password"></asp:TextBox>
        </div>
        <div class="form-group">
            <label>Confirm Password</label>
            <asp:TextBox ID="txtPassword2" runat="server" CssClass="form-control" required="true" TextMode="Password"></asp:TextBox>
            <asp:CompareValidator ID="cvPasswords" runat="server"
                ControlToValidate="txtPassword2"
                ControlToCompare="txtPassword1"
                ErrorMessage="Passwords don't match"
                Display="Dynamic"
                CssClass="text-danger"></asp:CompareValidator>
        </div>
        <asp:Button ID="btnSubmitEmployee" runat="server" Text="Add Employee" CssClass="btn btn-success" OnClick="AddEmployee" />
    </div>
</asp:Panel>
    <!-- Department Form -->
    <asp:Panel ID="pnlDepartment" runat="server" Visible="false">
        <div class="form-panel">
            <h3>Add New Department</h3>

            <div class="form-group">
                <label>Location</label>
                <asp:DropDownList ID="ddlDeptLocation" runat="server" CssClass="form-control" required="true"></asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Department Name</label>
                <asp:TextBox ID="txtDeptName" runat="server" CssClass="form-control" required="true"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>Employee in charge of the department</label>
                <asp:DropDownList ID="ddlEmployeesDepart" runat="server" CssClass="form-control" required="true"></asp:DropDownList>
            </div>
            <asp:Button ID="btnSubmitDepartment" runat="server" Text="Add Department" CssClass="btn btn-success" OnClick="AddDepartment" />
        </div>
    </asp:Panel>

    <!-- Location Form -->
    <asp:Panel ID="pnlLocation" runat="server" Visible="false">
        <div class="form-panel">
            <h3>Add New Location</h3>
            <div class="form-group">
                <label>Location Name</label>
                <asp:TextBox ID="txtLocationName" runat="server" CssClass="form-control" required="true"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>Senior in charge of location</label>
                <asp:DropDownList ID="ddlEmployees" runat="server" CssClass="form-control" required="true"></asp:DropDownList>
            </div>
            <asp:Button ID="btnSubmitLocation" runat="server" Text="Add Location" CssClass="btn btn-success" OnClick="AddLocation" />
        </div>
    </asp:Panel>

</asp:Content>
