<%@ Page Title="Add Employee" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="EpiUse_TechnicalAssesment.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Editing the Database
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Edit the database</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div class="tab-container">
        <div class="tabs">
            <button class="tab-button active" onclick="openTab('add-employee')">Add Employee</button>
            <button class="tab-button" onclick="openTab('delete-employee')">Delete Employee</button>
            <button class="tab-button" onclick="openTab('add-location')">Locations</button>
            <button class="tab-button" onclick="openTab('add-department')">Departments</button>
            <button class="tab-button" onclick="openTab('add-position')">Positions</button>
        </div>

        <div id="add-employee" class="tab-content active">
            <h2>Add New Employee</h2>

            <!-- Employee Name -->
            <label for="firstName">First Name:</label><br>
            <input type="text" id="firstNameTextbox" runat="server" required /><br>
            <br>

            <label for="lastName">Last Name:</label><br>
            <input type="text" id="lastNameTextbox" runat="server" required /><br>
            <br>

            <!-- Date of Birth -->
            <label for="dob">Date of Birth:</label><br>
            <input type="date" id="dobTextbox" runat="server" required /><br>
            <br>

            <!-- Email -->
            <label for="email">Email:</label><br>
            <input type="email" id="emailTextbox" runat="server" required /><br>
            <br>

            <!-- Position Dropdown -->
            <label for="position">Position:</label><br>
            <asp:DropDownList ID="positionDropdown" runat="server" required />
            <br>
            <br>

            <!-- Department Dropdown -->
            <label for="department">Department:</label><br>
            <asp:DropDownList ID="departmentDropdown" runat="server" required />
            <br>
            <br>

            <!-- Location Dropdown -->
            <label for="location">Location:</label><br>
            <asp:DropDownList ID="locationDropdown" runat="server" required />
            <br>
            <br>

            <!-- Password -->
            <label for="password">Password:</label><br>
            <input type="password" id="passwordTextbox" runat="server" required /><br>
            <br>

            <label for="confirmPassword">Confirm Password:</label><br>
            <input type="password" id="confirmPassword" runat="server" required /><br>
            <br>

            <!-- Salary -->
            <label for="salary">Salary:</label><br>
            <input type="number" id="salaryTextbox" runat="server" required /><br>
            <br>

            <!-- Reporting Line -->
            <label for="manager">Manager:</label><br>
            <asp:DropDownList ID="managerDropdown" runat="server" />
            <br>
            <br>

            <asp:Button ID="submitButton" runat="server" Text="Submit" OnClick="addEmployee" />
        </div>
    </div>

    <script src="script.js"></script>
</asp:Content>
