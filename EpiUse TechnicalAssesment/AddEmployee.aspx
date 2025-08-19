<%@ Page Title="Database Management" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="EpiUse_TechnicalAssesment.AddEmployee" %>

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
        <asp:Panel ID="employeeTab" runat="server" CssClass="tab-content active">
            <h2>Employee Management</h2>
            
            <!-- Add Employee Form -->
            <div class="form-section">
                <h3>Add New Employee</h3>
                <div id="validationMessageDiv">
                    <asp:Label ID="validationMessage" runat="server" ForeColor="Red"></asp:Label>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="firstNameTextbox">First Name:</label>
                        <input type="text" id="firstNameTextbox" runat="server" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="lastNameTextbox">Last Name:</label>
                        <input type="text" id="lastNameTextbox" runat="server" class="form-control" />
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="dobTextbox">Date of Birth:</label>
                        <input type="date" id="dobTextbox" runat="server" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="emailTextbox">Email:</label>
                        <input type="email" id="emailTextbox" runat="server" class="form-control" />
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="positionDropdown">Position:</label>
                        <asp:DropDownList ID="positionDropdown" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="departmentDropdown">Department:</label>
                        <asp:DropDownList ID="departmentDropdown" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
                
                <div class="form-row">
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
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="passwordTextbox">Password:</label>
                        <input type="password" id="passwordTextbox" runat="server" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="confirmPassword">Confirm Password:</label>
                        <input type="password" id="confirmPassword" runat="server" class="form-control" />
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="salaryTextbox">Salary:</label>
                        <input type="number" id="salaryTextbox" runat="server" class="form-control" step="0.01" min="0" />
                    </div>
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
                        <input type="password" id="password1" runat="server" class="form-control" />
                    </div>
                </div>
                
                <div class="form-group">
                    <asp:Button ID="btnDeleteEmployee" runat="server" Text="Delete Employee" CssClass="btn btn-danger" OnClick="deleteEmployee_Click" />
                </div>
            </div>
            
            <!-- Employee List -->
            <div class="form-section">
                <h3>Employee List</h3>
                <asp:GridView ID="gvEmployees" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
                    OnRowDeleting="gvEmployees_RowDeleting" DataKeyNames="EmployeeID">
                    <Columns>
                        <asp:BoundField DataField="EmployeeID" HeaderText="ID" />
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                        <asp:BoundField DataField="PositionName" HeaderText="Position" />
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Department Tab -->
        <asp:Panel ID="departmentTab" runat="server" CssClass="tab-content">
            <h2>Department Management</h2>
            
            <!-- Add Department Form -->
            <div class="form-section">
                <h3>Add New Department</h3>
                <div id="departmentValidationMessageDiv">
                    <asp:Label ID="departmentValidationMessage" runat="server" ForeColor="Red"></asp:Label>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtDepartmentName">Department Name:</label>
                        <asp:TextBox ID="txtDepartmentName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-group">
                    <asp:Button ID="btnAddDepartment" runat="server" Text="Add Department" 
                        CssClass="btn btn-primary" OnClick="btnAddDepartment_Click" />
                </div>
            </div>
            
            <!-- Department List -->
            <div class="form-section">
                <h3>Department List</h3>
                <asp:GridView ID="gvDepartments" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
                    OnRowDeleting="gvDepartments_RowDeleting" DataKeyNames="DepartmentID">
                    <Columns>
                        <asp:BoundField DataField="DepartmentID" HeaderText="ID" />
                        <asp:BoundField DataField="DepartmentName" HeaderText="Department Name" />
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
        
        <!-- Position Tab -->
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
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
        
        <!-- Location Tab -->
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
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>

    <!-- Delete Confirmation Modal -->
    <div class="modal fade" id="deleteConfirmModal" tabindex="-1" role="dialog" aria-labelledby="deleteConfirmModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="deleteConfirmModalLabel">Confirm Delete</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to delete this employee?</p>
                    <p>Employee ID: <asp:Label ID="lblEmployeeIDConfirm" runat="server" Text=""></asp:Label></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnConfirmDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Success Message Modal -->
    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="successModalLabel">Success</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblSuccessMessage" runat="server" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // This function will be called from code-behind to show modals
        function showDeleteConfirmModal() {
            $('#deleteConfirmModal').modal('show');
        }

        function showSuccessModal(message) {
            $('#lblSuccessMessage').text(message);
            $('#successModal').modal('show');
        }

        // Set active tab on page load
        document.addEventListener('DOMContentLoaded', function () {
            // Get the active tab from the hidden field
            var activeTab = document.getElementById('<%= activeTabHidden.ClientID %>').value;
            if (activeTab) {
                switchTab(activeTab);
            }
        });

        // Client-side tab switching (optional enhancement)
        function switchTab(tabName) {
            // Hide all tab content
            var tabContents = document.querySelectorAll('.tab-content');
            tabContents.forEach(function (tab) {
                tab.style.display = 'none';
                tab.classList.remove('active');
            });

            // Remove active class from all buttons
            var tabButtons = document.querySelectorAll('.tab-button');
            tabButtons.forEach(function (button) {
                button.classList.remove('active');
            });

            // Show the specific tab content
            document.getElementById(tabName).style.display = 'block';
            document.getElementById(tabName).classList.add('active');

            // Set the hidden field value for postbacks
            document.getElementById('<%= activeTabHidden.ClientID %>').value = tabName;

            // Find and activate the corresponding button
            var buttonId = 'lb' + tabName.charAt(0).toUpperCase() + tabName.slice(1);
            var button = document.getElementById('<%= lbEmployeeTab.ClientID %>'.replace('lbEmployeeTab', buttonId));
            if (button) {
                button.classList.add('active');
            }
        }
    </script>

    <asp:HiddenField ID="activeTabHidden" runat="server" Value="employeeTab" />
    </asp:Content>