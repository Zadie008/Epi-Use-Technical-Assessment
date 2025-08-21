<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewEmployee.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Employee Data
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Employee Data</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <asp:Panel ID="pnlEmployee" runat="server" Visible="false">
        <asp:HiddenField ID="hdnOriginalSalary" runat="server" />

        <div class="employee-flex-container">
            <div class="employee-details-container">
                <table class="employee-details-table">
                    <tr>
                        <td>Employee Number:</td>
                        <td>
                            <asp:Label ID="lblEmployeeID" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtEmployeeID" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>First Name:</td>
                        <td>
                            <asp:Label ID="lblFirstName" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtFirstName" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Last Name:</td>
                        <td>
                            <asp:Label ID="lblLastName" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtLastName" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Date of Birth:</td>
                        <td>
                            <asp:Label ID="lblBirthDate" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtBirthDate" runat="server" Visible="false" TextMode="Date" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Email:</td>
                        <td>
                            <asp:Label ID="lblEmail" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtEmail" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Salary:</td>
                        <td>
                            <asp:Label ID="lblSalary" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtSalary" runat="server" Visible="false" CssClass="edit-field" onkeyup="formatSalary(this)" />
                            <asp:RegularExpressionValidator
                                ID="revSalary"
                                runat="server"
                                ControlToValidate="txtSalary"
                                ValidationExpression="^[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{1,2})?$"
                                ErrorMessage="Please enter a valid salary (e.g. 50,000 or 50000.50)"
                                Display="Dynamic"
                                ForeColor="Red"
                                Enabled="false" />

                            <asp:RangeValidator
                                ID="rvSalary"
                                runat="server"
                                ControlToValidate="txtSalary"
                                Type="Double"
                                MinimumValue="0"
                                MaximumValue="99999999999999"
                                ErrorMessage="Salary must be ≥ 0"
                                Display="Dynamic"
                                ForeColor="Red"
                                Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>Role:</td>
                        <td>
                            <asp:Label ID="lblPosition" runat="server" CssClass="view-field" />
                            <asp:DropDownList ID="ddlRole" runat="server" Visible="false" CssClass="edit-field"
                                DataTextField="PositionName" DataValueField="PositionID" />
                        </td>
                    </tr>
                    <tr>
                        <td>Department:</td>
                        <td>
                            <asp:Label ID="lblDepartment" runat="server" CssClass="view-field" />
                            <asp:DropDownList ID="ddlDepartment" runat="server" Visible="false" CssClass="edit-field"
                                DataTextField="DepartmentName" DataValueField="DepartmentID"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td>Manager:</td>
                        <td>
                            <asp:Label ID="lblManager" runat="server" CssClass="view-field" />
                            <asp:DropDownList ID="ddlManager" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Location:</td>
                        <td>
                            <asp:Label ID="lblLocation" runat="server" CssClass="view-field" />
                            <asp:DropDownList ID="ddlLocation" runat="server" Visible="false" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td>Access Level:</td>

                        <td>
                            <asp:Label ID="lblAccess" runat="server" CssClass="view-field" />
                            <asp:DropDownList ID="ddlAccess" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                </table>
            </div>

            <div class="employee-image-container">
                <asp:Image ID="imgGravatar" runat="server" CssClass="employee-large-image" />
            </div>
        </div>

        <div class="employee-button-group">
            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="employee-button blue" OnClick="btnBack_Click" CausesValidation="false" />
            <asp:Button ID="btnEditEmployee" runat="server" Text="Edit" CssClass="employee-button" OnClick="btnEditEmployee_Click" Visible="false" />
            <asp:Button ID="btnSaveEmployee" runat="server" Text="Save" CssClass="employee-button save" OnClick="btnSaveEmployee_Click" Visible="false" />
            <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="employee-button cancel" OnClick="btnCancelEdit_Click" Visible="false" CausesValidation="false" />
            <asp:Button ID="btnChangeProfile" runat="server" Text="Change Profile Image" CssClass="employee-button save" OnClick="btnChangeProfile_Click" Visible="false" />
        </div>
    </asp:Panel>

    <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>

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

    <asp:UpdatePanel ID="upErrorModal" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlErrorModal" runat="server" CssClass="modal-panel" Style="display: none;">
                <div class="modal-panel-content">
                    <div class="modal-header" style="background-color: #dc3545;">
                        <h5 style="color: white;">Error</h5>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-danger">
                            <strong>Error!</strong>
                            <asp:Label ID="lblErrorMessage" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnErrorOK" runat="server" Text="OK" CssClass="btn btn-danger" OnClientClick="hideErrorModal(); return false;" />
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>


    <script type="text/javascript">
        function formatSalary(input) {
            // Remove all non-digit characters except decimal point
            var value = input.value.replace(/[^\d.]/g, '');

            // Split into whole and decimal parts
            var parts = value.split('.');
            if (parts.length > 2) {
                // More than one decimal point - invalid
                parts = [parts[0] + parts[1], parts.slice(2).join('')];
            }

            // Format whole number part with commas
            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");

            // Rejoin and update the input
            input.value = parts[0] + (parts.length > 1 ? '.' + parts[1] : '');
        }

        function showSuccessModal(message) {
            const modal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
            const messageLabel = document.getElementById('<%= lblSuccessMessage.ClientID %>');
            if (messageLabel) {
                messageLabel.innerText = message;
            }
            if (modal) {
                modal.style.display = 'flex';
            }
        }

        function hideSuccessModal() {
            const modal = document.getElementById('<%= pnlSuccessModal.ClientID %>');
            if (modal) {
                modal.style.display = 'none';
            }
        }
        function showErrorModal(message) {
            const modal = document.getElementById('<%= pnlErrorModal.ClientID %>');
            const messageLabel = document.getElementById('<%= lblErrorMessage.ClientID %>');
            if (messageLabel) {
                messageLabel.innerText = message;
            }
            if (modal) {
                modal.style.display = 'flex';
            }
        }

        function hideErrorModal() {
            const modal = document.getElementById('<%= pnlErrorModal.ClientID %>');
            if (modal) {
                modal.style.display = 'none';
            }
        }
    </script>
</asp:Content>
