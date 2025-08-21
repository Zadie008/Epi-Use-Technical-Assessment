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

                        <div class="form-row">
                            <div class="form-group">
                                <label for="firstNameTextbox">First Name:</label>
                                <asp:TextBox ID="firstNameTextbox" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="lastNameTextbox">Last Name:</label>
                                <asp:TextBox ID="lastNameTextbox" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="dobTextbox">Date of Birth:</label>
                                <asp:TextBox ID="dobTextbox" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="emailTextbox">Email:</label>
                                <asp:TextBox ID="emailTextbox" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
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
                                <asp:TextBox ID="passwordTextbox" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="confirmPassword">Confirm Password:</label>
                                <asp:TextBox ID="confirmPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="salaryTextbox">Salary:</label>
                                <asp:TextBox ID="salaryTextbox" runat="server" TextMode="Number" step="0.01" min="0" CssClass="form-control"></asp:TextBox>
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
                                OnRowDeleting="gvEmployees_RowDeleting" DataKeyNames="EmployeeID">
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
        <asp:UpdatePanel ID="upPositionModal" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlPositionDelete" runat="server" CssClass="modal-panel" Style="display: none;">
                    <div class="modal-panel-content">
                        <div class="modal-header">
                            <h5>Confirm Position Deletion</h5>
                        </div>
                        <div class="modal-body">
                            <div class="alert alert-warning">
                                <strong>Warning!</strong> This position has employees assigned to it.
                            </div>
                            <p>
                                Employees with this position will be moved to: 
                        <strong>
                            <asp:Label ID="lblNextPosition" runat="server" Text=""></asp:Label></strong>
                            </p>
                            <p class="text-danger"><strong>This action cannot be undone!</strong></p>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnCancelPositionDelete" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClientClick="hidePositionDeleteModal(); return false;" />
                            <asp:Button ID="btnConfirmPositionDelete" runat="server" Text="Reassign and Delete" CssClass="btn btn-primary" OnClick="btnConfirmPositionDelete_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>


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

    <!-- Success Modal -->
    <asp:UpdatePanel ID="upSuccessModal" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccessModal" runat="server" CssClass="modal-panel" Style="display: none;">
                <div class="modal-panel-content">
                    <div class="modal-header" style="background-color: #28a745;">
                        <h5 style="color: white;">Success</h5>
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
    </asp:UpdatePanel>

    <!-- Delete Confirmation Panel -->
    <asp:UpdatePanel ID="upDeleteConfirm" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlDeleteConfirm" runat="server" CssClass="modal-panel" Style="display: none;">
                <div class="modal-panel-content">
                    <div class="modal-header">
                        <h5>Confirm Delete</h5>
                    </div>
                    <div class="modal-body">
                        <p>Are you sure you want to delete this employee?</p>
                        <p>
                            Employee ID: 
                            <asp:Label ID="lblEmployeeIDConfirm" runat="server" Text="" Font-Bold="true"></asp:Label>
                        </p>
                        <p class="text-danger"><strong>This action cannot be undone!</strong></p>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnCancelDelete" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClientClick="hideDeletePanel(); return false;" />
                        <asp:Button ID="btnConfirmDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:HiddenField ID="activeTabHidden" runat="server" Value="employeeTab" />

   <script type="text/javascript">
       // Success Modal Functions
       function showSuccessModal() {
           var modal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
           if (modal) {
               modal.style.display = 'block';
               // Add backdrop
               var backdrop = document.createElement('div');
               backdrop.className = 'modal-backdrop fade show';
               backdrop.style.position = 'fixed';
               backdrop.style.top = '0';
               backdrop.style.left = '0';
               backdrop.style.width = '100%';
               backdrop.style.height = '100%';
               backdrop.style.backgroundColor = 'rgba(0,0,0,0.5)';
               backdrop.style.zIndex = '1040';
               document.body.appendChild(backdrop);
               document.body.style.overflow = 'hidden'; // Prevent scrolling
           }
       }

       function hideSuccessModal() {
           var modal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
           if (modal) {
               modal.style.display = 'none';
           }
           // Remove backdrop
           var backdrops = document.querySelectorAll('.modal-backdrop');
           backdrops.forEach(function (backdrop) {
               document.body.removeChild(backdrop);
           });
           document.body.style.overflow = ''; // Re-enable scrolling
       }

       // Delete Confirmation Modal Functions
       function showDeletePanel() {
           var panel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
           if (panel) {
               panel.style.display = 'block';
               // Add backdrop
               var backdrop = document.createElement('div');
               backdrop.className = 'modal-backdrop fade show';
               backdrop.style.position = 'fixed';
               backdrop.style.top = '0';
               backdrop.style.left = '0';
               backdrop.style.width = '100%';
               backdrop.style.height = '100%';
               backdrop.style.backgroundColor = 'rgba(0,0,0,0.5)';
               backdrop.style.zIndex = '1040';
               backdrop.onclick = function () {
                   // Don't hide on backdrop click - force user to make a decision
               };
               document.body.appendChild(backdrop);
               document.body.style.overflow = 'hidden'; // Prevent scrolling
           }
       }

       function hideDeletePanel() {
           var panel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
           if (panel) {
               panel.style.display = 'none';
           }
           // Remove backdrop
           var backdrops = document.querySelectorAll('.modal-backdrop');
           backdrops.forEach(function (backdrop) {
               document.body.removeChild(backdrop);
           });
           document.body.style.overflow = ''; // Re-enable scrolling
       }

       // Department Reassignment Modal Functions
       function showReassignmentModal() {
           var modal = document.getElementById('<%= pnlReassignment.ClientID %>');
           if (modal) {
               modal.style.display = 'block';
               // Add backdrop
               var backdrop = document.createElement('div');
               backdrop.className = 'modal-backdrop fade show';
               backdrop.style.position = 'fixed';
               backdrop.style.top = '0';
               backdrop.style.left = '0';
               backdrop.style.width = '100%';
               backdrop.style.height = '100%';
               backdrop.style.backgroundColor = 'rgba(0,0,0,0.5)';
               backdrop.style.zIndex = '1040';
               document.body.appendChild(backdrop);
               document.body.style.overflow = 'hidden'; // Prevent scrolling
           }
       } execution
               if (callback) {
                   modal.dataset.callback = callback;
               }
           }
       }
             eval(callback);
           }
       }
       function hideReassignmentModal() {
           var modal = document.getElementById('<%= pnlReassignment.ClientID %>');
           if (modal) {
               modal.style.display = 'none';
           }
           // Remove backdrop
           var backdrops = document.querySelectorAll('.modal-backdrop');
           backdrops.forEach(function (backdrop) {
               document.body.removeChild(backdrop);
           });
           document.body.style.overflow = ''; // Re-enable scrolling
       }

       // Position Delete Modal Functions
       function showPositionDeleteModal() {
           var modal = document.getElementById('<%= pnlPositionDelete.ClientID %>');
           if (modal) {
               modal.style.display = 'block';
               // Add backdrop
               var backdrop = document.createElement('div');
               backdrop.className = 'modal-backdrop fade show';
               backdrop.style.position = 'fixed';
               backdrop.style.top = '0';
               backdrop.style.left = '0';
               backdrop.style.width = '100%';
               backdrop.style.height = '100%';
               backdrop.style.backgroundColor = 'rgba(0,0,0,0.5)';
               backdrop.style.zIndex = '1040';
               document.body.appendChild(backdrop);
               document.body.style.overflow = 'hidden'; // Prevent scrolling
           }
       }

       function hidePositionDeleteModal() {
           var modal = document.getElementById('<%= pnlPositionDelete.ClientID %>');
           if (modal) {
               modal.style.display = 'none';
           }
           // Remove backdrop
           var backdrops = document.querySelectorAll('.modal-backdrop');
           backdrops.forEach(function (backdrop) {
               document.body.removeChild(backdrop);
           });
           document.body.style.overflow = ''; // Re-enable scrolling
       }

       // Location Delete Modal Functions
       function showLocationDeleteModal() {
           var modal = document.getElementById('<%= pnlLocationDelete.ClientID %>');
        if (modal) {
            modal.style.display = 'block';
            // Add backdrop
            var backdrop = document.createElement('div');
            backdrop.className = 'modal-backdrop fade show';
            backdrop.style.position = 'fixed';
            backdrop.style.top = '0';
            backdrop.style.left = '0';
            backdrop.style.width = '100%';
            backdrop.style.height = '100%';
            backdrop.style.backgroundColor = 'rgba(0,0,0,0.5)' backdrops.forEach(function (backdrop) {
               document.body.removeChild(backdrop);
           });
           document.body.style.overflow = ''; // Re-enable scrolling
       }

    function hideLocationDeleteModal() {
        var modal = document.getElementById('<%= pnlLocationDelete.ClientID %>');
        if (modal) {
            modal.style.display = 'none';
        }
        // Remove backdrop
        var backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(function(backdrop) {
            document.body.removeChild(backdrop);
        });
        document.body.style.overflow = ''; // Re-enable scrolling
    }

    // Auto-close success panel after 5 seconds
    function autoCloseSuccessPanel() {
        setTimeout(function() {
            hideSuccessModal();
        }, 5000);
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

        // Set initial tab
        var activeTab = $('#<%= activeTabHidden.ClientID %>').val();
        if (activeTab) {
            $('.tab-content').hide();
            $('#' + activeTab).show();
            $('.tab-button').removeClass('active');
            $('.tab-button[data-tab="' + activeTab + '"]').addClass('active');
        }
    }

    // Close modals when clicking outside (except for delete confirmation which requires explicit action)
    document.addEventListener('click', function (e) {
        // Success Modal
        var successModal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
        if (successModal && successModal.style.display === 'block') {
            var successContent = successModal.querySelector('.modal-panel-content');
            if (successContent && !successContent.contains(e.target)) {
                hideSuccessModal();
            }
        }

        // Department Reassignment Modal
        var reassignmentModal = document.getElementById('<%= pnlReassignment.ClientID %>');
        if (reassignmentModal && reassignmentModal.style.display === 'block') {
            var reassignmentContent = reassignmentModal.querySelector('.modal-panel-content');
            if (reassignmentContent && !reassignmentContent.contains(e.target)) {
                hideReassignmentModal();
            }
        }

        // Position Delete Modal
        var positionDeleteModal = document.getElementById('<%= pnlPositionDelete.ClientID %>');
        if (positionDeleteModal && positionDeleteModal.style.display === 'block') {
            var positionDeleteContent = positionDeleteModal.querySelector('.modal-panel-content');
            if (positionDeleteContent && !positionDeleteContent.contains(e.target)) {
                hidePositionDeleteModal();
            }
        }

        // Location Delete Modal
        var locationDeleteModal = document.getElementById('<%= pnlLocationDelete.ClientID %>');
        if (locationDeleteModal && locationDeleteModal.style.display === 'block') {
            var locationDeleteContent = locationDeleteModal.querySelector('.modal-panel-content');
            if (locationDeleteContent && !locationDeleteContent.contains(e.target)) {
                hideLocationDeleteModal();
            }
        }
        
        // Delete confirmation modal intentionally doesn't close on outside click to force user to make a decision
        
    });

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

           // Set up modal close buttons
           $('[data-dismiss="modal"]').on('click', function () {
               var modal = $(this).closest('.modal-panel');
               modal.hide();
               // Remove backdrop
               $('.modal-backdrop').remove();
               document.body.style.overflow = '';
           });
       });

       // Re-bind events after AJAX update
       var prm = Sys.WebForms.PageRequestManager.getInstance();
       prm.add_endRequest(function () {
           bindTabEvents();
           setupNameValidation();
       });
   </script>
</asp:Content>
