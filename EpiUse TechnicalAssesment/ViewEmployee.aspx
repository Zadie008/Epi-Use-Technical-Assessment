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
                            <asp:Label ID="lblEmployeeNumber" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtEmployeeNumber" runat="server" Visible="false" CssClass="edit-field" />
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
                            <asp:TextBox ID="txtSalary" runat="server" Visible="false" CssClass="edit-field" />
                            <asp:RegularExpressionValidator 
                                ID="revSalary" 
                                runat="server" 
                                ControlToValidate="txtSalary"
                                ValidationExpression="^\d+(\.\d{1,2})?$"
                                ErrorMessage="Salary must be a number (e.g., 50000 or 50000.50)"
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
                            <asp:Label ID="lblRole" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtRole" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Department:</td>
                        <td>
                            <asp:Label ID="lblDepartment" runat="server" CssClass="view-field" />
                            <asp:TextBox ID="txtDepartment" runat="server" Visible="false" CssClass="edit-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Manager:</td>
                        <td>
                            <asp:Label ID="lblManager" runat="server" CssClass="view-field" />
                        </td>
                    </tr>
                    <tr>
                        <td>Location:</td>
                        <td>
                            <asp:Label ID="lblLocation" runat="server" CssClass="view-field" />
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
</asp:Content>
