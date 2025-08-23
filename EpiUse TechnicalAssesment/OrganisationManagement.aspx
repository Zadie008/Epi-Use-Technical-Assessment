<%@ Page Title="Database Management" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OrganisationManagement.aspx.cs" Inherits="EpiUse_TechnicalAssesment.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Database Management
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Database Management System</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

    <div class="tab-container">
        <div class="tabs">
            <asp:LinkButton ID="lbEmployeeTab" runat="server" CssClass="tab-button active" OnClick="SwitchTab" CommandArgument="employeeTab">Employee Management</asp:LinkButton>
            <asp:LinkButton ID="lbDepartmentTab" runat="server" CssClass="tab-button" OnClick="SwitchTab" CommandArgument="departmentTab">Department Management</asp:LinkButton>
            <asp:LinkButton ID="lbPositionTab" runat="server" CssClass="tab-button" OnClick="SwitchTab" CommandArgument="positionTab">Position Management</asp:LinkButton>
            <asp:LinkButton ID="lbLocationTab" runat="server" CssClass="tab-button" OnClick="SwitchTab" CommandArgument="locationTab">Location Management</asp:LinkButton>
        </div>

        <!-- Employee Tab -->
        <asp:UpdatePanel ID="upEmployeeTab" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="employeeTab" runat="server" CssClass="tab-content active">
                    <h2>Employee Management</h2>

                    <!-- Add Employee Form -->
                    <div class="form-section">
                        <h3>Add New Employee</h3>
                        <div id="validationMessageDiv">
                            <asp:Label ID="validationMessage" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                        <div class="form-group">
                            <label for="firstNameTextbox">First Name:</label>
                            <asp:TextBox ID="firstNameTextbox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="lastNameTextbox">Last Name:</label>
                            <asp:TextBox ID="lastNameTextbox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="dobTextbox">Date of Birth:</label>
                            <asp:TextBox ID="dobTextbox" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="emailTextbox">Email:</label>
                            <asp:TextBox ID="emailTextbox" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="positionDropdown">Position:</label>
                            <asp:DropDownList ID="positionDropdown" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="departmentDropdown">Department:</label>
                            <asp:DropDownList ID="departmentDropdown" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="locationDropdown">Location:</label>
                            <asp:DropDownList ID="locationDropdown" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="managerDropdown">Manager:</label>
                            <asp:DropDownList ID="managerDropdown" runat="server" CssClass="form-control">
                                <asp:ListItem Value="0">-- No Manager --</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="passwordTextbox">Password:</label>
                            <asp:TextBox ID="passwordTextbox" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="confirmPassword">Confirm Password:</label>
                            <asp:TextBox ID="confirmPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="salaryTextbox">Salary:</label>
                            <asp:TextBox ID="salaryTextbox" runat="server" TextMode="Number" step="0.01" min="0" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <asp:Button ID="submitButton" runat="server" Text="Add Employee" OnClick="addEmployee" CssClass="btn btn-primary" />
                        </div>
                    </div>
                    <!-- Delete Employee Form -->
                    <div class="form-section">
                        <h3>Delete Employee</h3>
                        <div id="deleteValidationMessageDiv">
                            <asp:Label ID="deleteValidationMessage" runat="server" ForeColor="Red"></asp:Label>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtEmpId">Employee ID:</label>
                                <asp:TextBox ID="txtEmpId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="password1">Confirm Your Password:</label>
                                <asp:TextBox ID="password1" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Button ID="btnDeleteEmployee" runat="server" Text="Delete Employee" CssClass="btn btn-danger" OnClick="deleteEmployee_Click" />
                        </div>
                    </div>
                    <!-- Employee List -->
                    <div class="form-section">
                        <h3>Employee List</h3>
                        <div class="scrollable-gridviewInTabs">
                            <asp:GridView ID="gvEmployees" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
                                DataKeyNames="EmployeeID">
                                <Columns>
                                    <asp:BoundField DataField="EmployeeID" HeaderText="ID" />
                                    <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                                    <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                                    <asp:BoundField DataField="Email" HeaderText="Email" />
                                    <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                                    <asp:BoundField DataField="PositionName" HeaderText="Position" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="submitButton" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnDeleteEmployee" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnConfirmDelete" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnCancelDelete" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
       <!-- Reassign Manager Modal -->
<div id="reassignManagerModal" class="modal" style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h3>Reassign Employees</h3>
        </div>
        <div class="modal-body">
            <asp:UpdatePanel ID="upReassignManager" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="lblReassignMessage" runat="server" Text="The employee you are deleting manages other employees. Please select a new manager for these employees:" />
                    <br /><br />
                    
                    <div class="reassign-gridview">
                        <asp:GridView ID="gvManagedEmployees" runat="server" AutoGenerateColumns="false" 
                            CssClass="table table-striped" Width="100%">
                            <Columns>
                                <asp:BoundField DataField="EmployeeID" HeaderText="ID" />
                                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                                <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                                <asp:BoundField DataField="PositionName" HeaderText="Position" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    
                    <div class="form-group">
                        <label for="ddlNewManager">Select New Manager:</label>
                        <asp:DropDownList ID="ddlNewManager" runat="server" CssClass="form-control">
                            <asp:ListItem Value="0">-- Select Manager --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <asp:Label ID="lblReassignError" runat="server" ForeColor="Red" Visible="false" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="modal-footer">
            <div class="form-row">
                <asp:Button ID="btnConfirmReassign" runat="server" Text="Confirm Reassignment" 
                    CssClass="btn btn-primary" OnClick="btnConfirmReassign_Click" />
                <asp:Button ID="btnCancelReassign" runat="server" Text="Cancel" 
                    CssClass="btn btn-secondary" OnClick="btnCancelReassign_Click" />
            </div>
        </div>
    </div>
</div>

        <!-- Department Tab -->
        <asp:Panel ID="departmentTab" runat="server" CssClass="tab-content">
            <h2>Department Management</h2>

            <div class="form-section">
                <h3>Add New Department</h3>
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtDepartmentName">Department Name:</label>
                        <asp:TextBox ID="txtDepartmentName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="ddlDepartmentLocation">Location:</label>
                        <asp:DropDownList ID="ddlDepartmentLocation" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>

                <div class="form-group">
                    <asp:Button ID="btnAddDepartment" runat="server" Text="Add Department"
                        CssClass="btn btn-primary" OnClick="btnAddDepartment_Click"
                        OnClientClick="return validateDepartmentForm();" />
                </div>

                <div class="form-group">
                    <asp:Label ID="departmentValidationMessage" runat="server" CssClass="validation-message"></asp:Label>
                </div>
            </div>

            <div class="grid-section">
                <h3>Current Departments</h3>
                <asp:GridView ID="gvDepartments" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-striped table-bordered"
                    DataKeyNames="DepartmentID" OnRowDeleting="gvDepartments_RowDeleting">
                    <Columns>

                        <asp:BoundField DataField="DepartmentName" HeaderText="Department Name" SortExpression="DepartmentName" />
                        <asp:BoundField DataField="LocationName" HeaderText="Location" SortExpression="LocationName" />
                        <asp:BoundField DataField="EmployeeCount" HeaderText="Number of Employees" SortExpression="EmployeeCount"
                            ItemStyle-HorizontalAlign="Center" />
                        <asp:CommandField ShowDeleteButton="True" HeaderText="Actions" ButtonType="Button" DeleteText="Delete"
                            ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                </asp:GridView>
            </div>
            <asp:Panel ID="pnlReassignment" runat="server" CssClass="modal-panel" Style="display: none;">
                <div class="modal-panel-content">
                    <div class="modal-header">
                        <h5>Department Has Employees</h5>
                    </div>
                    <div class="modal-body">
                        <p>This department contains employees. Please select where to reassign them:</p>
                        <div class="form-group">
                            <label for="ddlTargetDepartment">Move employees to:</label>
                            <asp:DropDownList ID="ddlTargetDepartment" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnCancelReassignment" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClientClick="hideReassignmentModal(); return false;" />
                        <asp:Button ID="btnConfirmReassignment" runat="server" Text="Reassign and Delete" CssClass="btn btn-primary" OnClick="btnConfirmReassignment_Click" />
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>


        <!-- Position Tab -->
        <asp:UpdatePanel ID="upPositionTab" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="positionTab" runat="server" CssClass="tab-content">
                    <h2>Position Management</h2>

                    <!-- Add Position Form -->
                    <div class="form-section">
                        <h3>Add New Position</h3>
                        <div id="positionValidationMessageDiv">
                            <asp:Label ID="positionValidationMessage" runat="server" ForeColor="Red"></asp:Label>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtPositionName">Position Name:</label>
                                <asp:TextBox ID="txtPositionName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Button ID="btnAddPosition" runat="server" Text="Add Position"
                                CssClass="btn btn-primary" OnClick="btnAddPosition_Click" />
                        </div>
                    </div>

                    <!-- Position List -->
                    <div class="form-section">
                        <h3>Position List</h3>
                        <asp:GridView ID="gvPositions" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
                            OnRowDeleting="gvPositions_RowDeleting" DataKeyNames="PositionID">
                            <Columns>
                                <asp:BoundField DataField="PositionID" HeaderText="ID" />
                                <asp:BoundField DataField="PositionName" HeaderText="Position Name" />
                                <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" ControlStyle-CssClass="btn btn-danger btn-sm" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnAddPosition" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="gvPositions" EventName="RowDeleting" />
            </Triggers>

        </asp:UpdatePanel>
        <!-- Position Delete Modal -->
<asp:UpdatePanel ID="upPositionModal" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlPositionDelete" runat="server" CssClass="modal" Style="display: none;">
            <div class="modal-content">
                <div class="modal-header">
                    <h3>Confirm Position Deletion</h3>
                </div>
                <div class="modal-body">
                    <div class="alert alert-warning">
                        <strong>Warning!</strong> This position has employees assigned to it.
                    </div>
                    <p>
                        Employees with this position will be moved to: 
                        <strong><asp:Label ID="lblNextPosition" runat="server" Text=""></asp:Label></strong>
                    </p>
                    <p class="text-danger"><strong>This action cannot be undone!</strong></p>
                </div>
                <div class="modal-footer">
                    <div class="form-row">
                        <asp:Button ID="btnCancelPositionDelete" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClientClick="hidePositionDeleteModal(); return false;" />
                        <asp:Button ID="btnConfirmPositionDelete" runat="server" Text="Reassign and Delete" CssClass="btn btn-primary" OnClick="btnConfirmPositionDelete_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<!-- Position Reassignment Modal -->
<asp:UpdatePanel ID="upPositionReassignModal" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlPositionReassign" runat="server" CssClass="modal" Style="display: none;">
            <div class="modal-content">
                <div class="modal-header">
                    <h3>Position Has Employees</h3>
                </div>
                <div class="modal-body">
                    <div class="alert alert-warning">
                        <strong>Warning!</strong> This position has 
                        <asp:Label ID="lblPositionEmployeeCount" runat="server" CssClass="employee-count"></asp:Label> 
                        employees assigned to it.
                    </div>
                    <p>Please select a new position for these employees:</p>
                    
                    <div class="form-group">
                        <label for="ddlTargetPosition">Move employees to:</label>
                        <asp:DropDownList ID="ddlTargetPosition" runat="server" CssClass="form-control">
                            <asp:ListItem Value="0">-- Select Position --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <asp:Label ID="lblPositionReassignError" runat="server" ForeColor="Red" Visible="false" />
                    <asp:HiddenField ID="hdnPositionToDelete" runat="server" Value="0" />
                </div>
                <div class="modal-footer">
                    <div class="form-row">
                        <asp:Button ID="btnCancelPositionReassign" runat="server" Text="Cancel" 
                            CssClass="btn btn-secondary" OnClientClick="hidePositionReassignModal(); return false;" />
                        <asp:Button ID="btnConfirmPositionReassign" runat="server" Text="Reassign and Delete" 
                            CssClass="btn btn-primary" OnClick="btnConfirmPositionReassign_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<!-- Department Reassignment Modal -->
<asp:Panel ID="pnlDepartmentReassign" runat="server" CssClass="modal" Style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h3>Department Has Employees</h3>
        </div>
        <div class="modal-body">
            <asp:UpdatePanel ID="upDepartmentReassign" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="alert alert-warning">
                        <strong>Warning!</strong> This department contains 
                        <asp:Label ID="lblEmployeeCount" runat="server" CssClass="employee-count"></asp:Label> 
                        employees at location: 
                        <asp:Label ID="lblDepartmentLocation" runat="server" CssClass="employee-count"></asp:Label>
                    </div>
                    <p>Please select a department at the same location to move these employees to:</p>
                    
                    <div class="form-group">
                        <label for="ddlTargetDepartment">Move employees to:</label>
                        <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control">
                            <asp:ListItem Value="0">-- Select Department --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <asp:Label ID="lblDepartmentReassignError" runat="server" ForeColor="Red" Visible="false" />
                    <asp:HiddenField ID="hdnDepartmentToDelete" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnDepartmentLocationId" runat="server" Value="0" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="modal-footer">
            <div class="form-row">
                <asp:Button ID="btnCancelDepartmentReassign" runat="server" Text="Cancel" 
                    CssClass="btn btn-secondary" OnClientClick="hideDepartmentReassignModal(); return false;" />
                <asp:Button ID="btnConfirmDepartmentReassign" runat="server" Text="Reassign and Delete" 
                    CssClass="btn btn-primary" OnClick="btnConfirmDepartmentReassign_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
        

        <!-- Location Tab -->
        <asp:UpdatePanel ID="upLocationTab" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="locationTab" runat="server" CssClass="tab-content">
                    <h2>Location Management</h2>

                    <!-- Add Location Form -->
                    <div class="form-section">
                        <h3>Add New Location</h3>
                        <div id="locationValidationMessageDiv">
                            <asp:Label ID="locationValidationMessage" runat="server" ForeColor="Red"></asp:Label>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtLocationName">Location Name:</label>
                                <asp:TextBox ID="txtLocationName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Button ID="btnAddLocation" runat="server" Text="Add Location"
                                CssClass="btn btn-primary" OnClick="btnAddLocation_Click" />
                        </div>
                    </div>

                    <!-- Location List -->
                    <div class="form-section">
                        <h3>Location List</h3>
                        <asp:GridView ID="gvLocations" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
                            OnRowDeleting="gvLocations_RowDeleting" DataKeyNames="LocationID">
                            <Columns>
                                <asp:BoundField DataField="LocationID" HeaderText="ID" />
                                <asp:BoundField DataField="LocationName" HeaderText="Location Name" />
                                <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" ControlStyle-CssClass="btn btn-danger btn-sm" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnAddLocation" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="gvLocations" EventName="RowDeleting" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="upLocationModal" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlLocationDelete" runat="server" CssClass="modal-panel" Style="display: none;">
                    <div class="modal-panel-content">
                        <div class="modal-header">
                            <h5>Confirm Location Deletion</h5>
                        </div>
                        <div class="modal-body">
                            <div class="alert alert-warning">
                                <strong>Warning!</strong> This location has departments assigned to it.
                            </div>
                            <p>
                                Departments at this location will be moved to: 
                        <strong>
                            <asp:Label ID="lblNextLocation" runat="server" Text=""></asp:Label></strong>
                            </p>
                            <p class="text-danger"><strong>This action cannot be undone!</strong></p>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnCancelLocationDelete" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClientClick="hideLocationDeleteModal(); return false;" />
                            <asp:Button ID="btnConfirmLocationDelete" runat="server" Text="Reassign and Delete" CssClass="btn btn-primary" OnClick="btnConfirmLocationDelete_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    
    </div>
     <!-- Simpler Location Reassignment Info Panel -->
<asp:UpdatePanel ID="upLocationReassignInfo" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlLocationReassignInfo" runat="server" CssClass="modal" Style="display: none;">
            <div class="modal-content" style="max-width: 500px;">
                <div class="modal-header">
                    <h3>Location Reassignment</h3>
                </div>
                <div class="modal-body">
                    <div class="alert alert-info">
                        <i class="fas fa-info-circle"></i> 
                        <strong>Processing reassignment...</strong>
                    </div>
                    <p>
                        <asp:Label ID="lblReassignInfo" runat="server" Text=""></asp:Label>
                    </p>
                    <div style="text-align: center; margin: 15px 0;">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
    </asp:UpdatePanel>
<!-- Success Modal UpdatePanel -->
<asp:UpdatePanel ID="upSuccessModal" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlSuccessModal" runat="server" CssClass="modal" Style="display: none;">
            <div class="modal-content">
                <div class="modal-header">
                    <h5>Success</h5>
                </div>
                <div class="modal-body">
                    <div class="alert alert-success">
                        <strong>Success!</strong>
                        <asp:Label ID="lblSuccessMessage" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSuccessOK" runat="server" Text="OK" CssClass="btn btn-success" OnClientClick="hideSuccessModal(); return false;" />
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSuccessOK" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

    <!-- Delete Confirmation Modal -->
    <asp:UpdatePanel ID="upDeleteConfirmPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <!-- Delete Confirmation Modal -->
            <asp:Panel ID="pnlDeleteConfirm" runat="server" CssClass="modal" Style="display: none;">
                <div class="modal-content">
                    <h3>Confirm Delete</h3>
                    <p>Are you sure you want to delete employee ID:
                        <asp:Label ID="lblEmployeeIDConfirm" runat="server" Font-Bold="true"></asp:Label>?</p>

                    <div class="form-row">
                        <asp:Button ID="btnConfirmDelete" runat="server" Text="Yes, Delete"
                            CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
                        <asp:Button ID="btnCancelDelete" runat="server" Text="Cancel"
                            CssClass="btn btn-secondary" OnClientClick="hideDeleteConfirm(); return false;" />
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnDeleteEmployee" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnConfirmDelete" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnCancelDelete" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:HiddenField ID="activeTabHidden" runat="server" Value="employeeTab" />

<script type="text/javascript">
    // Add this to your JavaScript section
    console.log('JavaScript loaded successfully');

    // Debug function to check if modal is being called
    function debugModalShow() {
        console.log('showDepartmentReassignModal() called');
        var modal = document.getElementById('<%= pnlDepartmentReassign.ClientID %>');
    console.log('Modal element found:', modal);
    
    if (modal) {
        console.log('Modal current display:', modal.style.display);
        console.log('Modal current visibility:', modal.style.visibility);
        
        // Force show the modal for testing
        modal.style.display = 'flex';
        modal.style.visibility = 'visible';
        
        console.log('Modal after setting display:', modal.style.display);
        
        // Check if modal content is visible
        var modalContent = modal.querySelector('.modal-content');
        console.log('Modal content found:', modalContent);
    }
}
    // Add this function to debug dropdown on client side
    function debugDropdown() {
        var dropdown = document.getElementById('<%= ddlTargetDepartment.ClientID %>');
        console.log('Dropdown debug:');
        console.log('Dropdown element:', dropdown);

        if (dropdown) {
            console.log('Dropdown options length:', dropdown.options.length);
            console.log('Dropdown innerHTML:', dropdown.innerHTML);

            for (var i = 0; i < dropdown.options.length; i++) {
                console.log('Option ' + i + ': ' + dropdown.options[i].text + ' = ' + dropdown.options[i].value);
            }

            // If dropdown is empty, try to see if it's a ViewState issue
            if (dropdown.options.length <= 1) { // Only the default option
                console.warn('Dropdown appears empty! Checking for ViewState issues...');
            }
        } else {
            console.error('Dropdown not found!');
        }
    }

    // Update your showDepartmentReassignModal to include dropdown debug
    function showDepartmentReassignModal() {
        console.log('showDepartmentReassignModal() called');

        var modal = document.getElementById('<%= pnlDepartmentReassign.ClientID %>');
    console.log('Modal element:', modal);

    if (modal) {
        // Debug dropdown first
        debugDropdown();

        // Show modal
        modal.style.display = 'flex';

        // Add backdrop
        var backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        document.body.appendChild(backdrop);
        document.body.classList.add('body-no-scroll');
    }
}
// Replace your current showDepartmentReassignModal function with this:
function showDepartmentReassignModal() {
    console.log('showDepartmentReassignModal() called');
    
    var modal = document.getElementById('<%= pnlDepartmentReassign.ClientID %>');
    console.log('Modal element:', modal);

    if (modal) {
        // Add department reassign modal class for specific styling
        var modalContent = modal.querySelector('.modal-content');
        if (modalContent) {
            modalContent.classList.add('department-reassign-modal');
        }

        // Force the modal to be visible with multiple methods
        modal.style.display = 'flex';
        modal.style.visibility = 'visible';
        modal.style.opacity = '1';
        modal.style.zIndex = '10000';

        console.log('Modal display set to flex');

        // Add backdrop
        var backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        backdrop.style.cssText = 'position:fixed;top:0;left:0;width:100%;height:100%;background-color:rgba(0,0,0,0.5);z-index:9999;';
        document.body.appendChild(backdrop);
        document.body.classList.add('body-no-scroll');

        // Debug: Check dropdown contents
        var dropdown = document.getElementById('<%= ddlTargetDepartment.ClientID %>');
        if (dropdown) {
            console.log('Dropdown options count:', dropdown.options.length);
            for (var i = 0; i < dropdown.options.length; i++) {
                console.log('Option ' + i + ': ' + dropdown.options[i].text + ' = ' + dropdown.options[i].value);
            }
        }

        // Force a reflow to ensure display changes take effect
        modal.offsetHeight;

    } else {
        console.error('Modal not found! Check the ClientID:', '<%= pnlDepartmentReassign.ClientID %>');
    }
}
    // Success Modal Functions
    function showSuccessModal() {
        var modal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
        if (modal) {
            var modalContent = modal.querySelector('.modal-content');
            if (modalContent) {
                modalContent.classList.add('success-modal');
            }

            modal.style.display = 'flex';
            var backdrop = document.createElement('div');
            backdrop.className = 'modal-backdrop';
            document.body.appendChild(backdrop);
            document.body.classList.add('body-no-scroll');
        }
    }

    function hideSuccessModal() {
        var modal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
        if (modal) {
            modal.style.display = 'none';
        }
        var backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(function (backdrop) {
            document.body.removeChild(backdrop);
        });
        document.body.classList.remove('body-no-scroll');
    }

    // Delete Confirmation Modal Functions
    function showDeleteConfirm() {
        var modal = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
        if (modal) {
            var modalContent = modal.querySelector('.modal-content');
            if (modalContent) {
                modalContent.classList.add('delete-modal');
            }

            modal.style.display = 'flex';
            var backdrop = document.createElement('div');
            backdrop.className = 'modal-backdrop';
            document.body.appendChild(backdrop);
            document.body.classList.add('body-no-scroll');
        }
    }

    function hideDeleteConfirm() {
        var modal = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
        if (modal) {
            modal.style.display = 'none';
        }
        var backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(function (backdrop) {
            document.body.removeChild(backdrop);
        });
        document.body.classList.remove('body-no-scroll');
    }

    // Reassign Manager Modal Functions
    function showReassignManagerModal() {
        var modal = document.getElementById('reassignManagerModal');
        if (modal) {
            var modalContent = modal.querySelector('.modal-content');
            if (modalContent) {
                modalContent.classList.add('reassign-modal');
            }

            modal.style.display = 'flex';
            var backdrop = document.createElement('div');
            backdrop.className = 'modal-backdrop';
            document.body.appendChild(backdrop);
            document.body.classList.add('body-no-scroll');
        }
    }

    function hideReassignManagerModal() {
        var modal = document.getElementById('reassignManagerModal');
        if (modal) {
            modal.style.display = 'none';
        }
        var backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(function (backdrop) {
            document.body.removeChild(backdrop);
        });
        document.body.classList.remove('body-no-scroll');
    }

    // Department Reassign Modal Functions
    function showDepartmentReassignModal() {
        console.log('showDepartmentReassignModal() called');

        var modal = document.getElementById('<%= pnlDepartmentReassign.ClientID %>');
    console.log('Modal element:', modal);
    
    if (modal) {
        // Add department reassign modal class for specific styling
        var modalContent = modal.querySelector('.modal-content');
        if (modalContent) {
            modalContent.classList.add('department-reassign-modal');
        }
        
        // Force the modal to be visible
        modal.style.display = 'flex';
        modal.style.visibility = 'visible';
        
        console.log('Modal display set to flex');
        
        // Add backdrop
        var backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        document.body.appendChild(backdrop);
        document.body.classList.add('body-no-scroll');
        
        // Debug: Check dropdown contents
           var dropdown = document.getElementById('<%= ddlTargetDepartment.ClientID %>');
           if (dropdown) {
               console.log('Dropdown options count:', dropdown.options.length);
               for (var i = 0; i < dropdown.options.length; i++) {
                   console.log('Option ' + i + ': ' + dropdown.options[i].text + ' = ' + dropdown.options[i].value);
               }
           }
       } else {
           console.error('Modal not found!');
       }
   }

    function hideDepartmentReassignModal() {
        var modal = document.getElementById('<%= pnlDepartmentReassign.ClientID %>');
        if (modal) {
            modal.style.display = 'none';
        }
        var backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(function (backdrop) {
            document.body.removeChild(backdrop);
        });
        document.body.classList.remove('body-no-scroll');
    }

    // Position Reassign Modal Functions
    function showPositionReassignModal() {
        console.log('showPositionReassignModal() called');

        var modal = document.getElementById('<%= pnlPositionReassign.ClientID %>');
    console.log('Modal element:', modal);

    if (modal) {
        // Add position reassign modal class for specific styling
        var modalContent = modal.querySelector('.modal-content');
        if (modalContent) {
            modalContent.classList.add('position-reassign-modal');
        }

        // Force the modal to be visible
        modal.style.display = 'flex';
        modal.style.visibility = 'visible';

        console.log('Modal display set to flex');

        // Add backdrop
        var backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        document.body.appendChild(backdrop);
        document.body.classList.add('body-no-scroll');

        // Debug: Check dropdown contents
        var dropdown = document.getElementById('<%= ddlTargetPosition.ClientID %>');
            if (dropdown) {
                console.log('Dropdown options count:', dropdown.options.length);
                for (var i = 0; i < dropdown.options.length; i++) {
                    console.log('Option ' + i + ': ' + dropdown.options[i].text + ' = ' + dropdown.options[i].value);
                }
            }
        } else {
            console.error('Position reassign modal not found!');
        }
    }

    function hidePositionReassignModal() {
        var modal = document.getElementById('<%= pnlPositionReassign.ClientID %>');
    if (modal) {
        modal.style.display = 'none';
    }
    var backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(function (backdrop) {
        document.body.removeChild(backdrop);
    });
    document.body.classList.remove('body-no-scroll');
}
    // Location Reassign Info Panel Functions
    function showLocationReassignInfo() {
        console.log('showLocationReassignInfo() called');

        var modal = document.getElementById('<%= pnlLocationReassignInfo.ClientID %>');
    console.log('Info panel element:', modal);
    
    if (modal) {
        modal.style.display = 'flex';
        modal.style.visibility = 'visible';
        
        console.log('Info panel display set to flex');
        
        // Add backdrop
        var backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        document.body.appendChild(backdrop);
        document.body.classList.add('body-no-scroll');
    } else {
        console.error('Location reassign info panel not found!');
    }
}

function hideLocationReassignInfo() {
    var modal = document.getElementById('<%= pnlLocationReassignInfo.ClientID %>');
    if (modal) {
        modal.style.display = 'none';
    }
    var backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(function (backdrop) {
        document.body.removeChild(backdrop);
    });
    document.body.classList.remove('body-no-scroll');

    // Also hide the success modal if it's showing
    hideSuccessModal();
}

    // Client-side tab switching
    function bindTabEvents() {
        $('.tab-button').off('click').on('click', function (e) {
            e.preventDefault();
            $('.tab-button').removeClass('active');
            $(this).addClass('active');
            $('.tab-content').hide();
            var tabName = $(this).attr('data-tab');
            $('#' + tabName).show();
            $('#<%= activeTabHidden.ClientID %>').val(tabName);
        });

        var activeTab = $('#<%= activeTabHidden.ClientID %>').val();
        if (activeTab) {
            $('.tab-content').hide();
            $('#' + activeTab).show();
            $('.tab-button').removeClass('active');
            $('.tab-button[data-tab="' + activeTab + '"]').addClass('active');
        }
    }

    // Name validation function
    function validateName(input) {
        var nameRegex = /^[a-zA-Z\-\s]+$/;
        return nameRegex.test(input.value);
    }

    function setupNameValidation() {
        var firstNameInput = document.getElementById('<%= firstNameTextbox.ClientID %>');
        var lastNameInput = document.getElementById('<%= lastNameTextbox.ClientID %>');

        if (firstNameInput) {
            firstNameInput.addEventListener('input', function () {
                if (!validateName(this)) {
                    this.style.borderColor = 'red';
                } else {
                    this.style.borderColor = '';
                }
            });
        }

        if (lastNameInput) {
            lastNameInput.addEventListener('input', function () {
                if (!validateName(this)) {
                    this.style.borderColor = 'red';
                } else {
                    this.style.borderColor = '';
                }
            });
        }
    }

    function validateDepartmentForm() {
        var departmentName = document.getElementById('<%= txtDepartmentName.ClientID %>').value.trim();
        var locationId = document.getElementById('<%= ddlDepartmentLocation.ClientID %>').value;

        if (departmentName === "") {
            alert("Please enter a department name.");
            return false;
        }

        if (locationId === "" || locationId === "0") {
            alert("Please select a location for the department.");
            return false;
        }

        return true;
    }

    // Initialize on page load
    $(document).ready(function () {
        bindTabEvents();
        setupNameValidation();
    });

    // Re-bind events after AJAX update
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(function () {
        bindTabEvents();
        setupNameValidation();
    });
</script>
    </asp:Content>