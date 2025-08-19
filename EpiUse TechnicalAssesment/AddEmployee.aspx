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
                                    <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" ControlStyle-CssClass="btn btn-danger btn-sm" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
         </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="submitButton" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnDeleteEmployee" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="gvEmployees" EventName="RowDeleting" />
            </Triggers>
        </asp:UpdatePanel>

        <!-- Department Tab -->
        <asp:UpdatePanel ID="upDepartmentTab" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
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
                                <asp:CommandField ShowDeleteButton="True" ButtonType="Button" DeleteText="Delete" ControlStyle-CssClass="btn btn-danger btn-sm" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnAddDepartment" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="gvDepartments" EventName="RowDeleting" />
            </Triggers>
        </asp:UpdatePanel>

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
    </div>

    <!-- Success Panel -->
    <asp:UpdatePanel ID="upSuccess" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-panel" style="display: none;">
                <div class="success-panel-content">
                    <div class="success-header">
                        <span class="success-icon">✓</span>
                        <h5>Success</h5>
                    </div>
                    <div class="success-body">
                        <asp:Label ID="lblSuccessMessage" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="success-footer">
                        <asp:Button ID="btnSuccessOK" runat="server" Text="OK" CssClass="btn btn-primary" OnClientClick="hideSuccessPanel(); return false;" />
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Delete Confirmation Panel -->
    <asp:UpdatePanel ID="upDeleteConfirm" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlDeleteConfirm" runat="server" CssClass="modal-panel" style="display: none;">
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
        function showSuccessPanel() {
            var panel = document.getElementById('<%= pnlSuccess.ClientID %>');
            if (panel) {
                panel.style.display = 'block';
            }
        }

        function hideSuccessPanel() {
            var panel = document.getElementById('<%= pnlSuccess.ClientID %>');
            if (panel) {
                panel.style.display = 'none';
            }
        }

        function showDeletePanel() {
            var panel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
            if (panel) {
                panel.style.display = 'block';
            }
        }

        function hideDeletePanel() {
            var panel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
            if (panel) {
                panel.style.display = 'none';
            }
        }


        // Close panels when clicking outside
        document.addEventListener('click', function (e) {
            // Success Panel
            var successPanel = document.getElementById('<%= pnlSuccess.ClientID %>');
            if (successPanel && successPanel.style.display === 'block') {
                var successContent = successPanel.querySelector('.success-panel-content');
                if (successContent && !successContent.contains(e.target)) {
                    hideSuccessPanel();
                }
            }

            // Delete Panel
            var deletePanel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
            if (deletePanel && deletePanel.style.display === 'block') {
                var deleteContent = deletePanel.querySelector('.modal-panel-content');
                if (deleteContent && !deleteContent.contains(e.target)) {
                    hideDeletePanel();
                }
            }
        });

        // Auto-close success panel after 5 seconds
        function autoCloseSuccessPanel() {
            setTimeout(function () {
                hideSuccessPanel();
            }, 5000);
        }

        // Client-side tab switching
        $(document).ready(function () {
            bindTabEvents();
        });

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

        // Re-bind events after AJAX update
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            bindTabEvents();
        });
    </script>
</asp:Content>