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
                        <td>Employee ID:</td>
                        <td><asp:Label ID="lblEmployeeID" runat="server" /></td>
                        <td><asp:TextBox ID="txtEmployeeID" runat="server" Visible="false" Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>First Name:</td>
                        <td><asp:Label ID="lblFirstName" runat="server" /></td>
                        <td><asp:TextBox ID="txtFirstName" runat="server" Visible="false" /></td>
                    </tr>
                    <tr>
                        <td>Last Name:</td>
                        <td><asp:Label ID="lblLastName" runat="server" /></td>
                        <td><asp:TextBox ID="txtLastName" runat="server" Visible="false" /></td>
                    </tr>
                    <tr>
                        <td>Birth Date:</td>
                        <td><asp:Label ID="lblBirthDate" runat="server" /></td>
                        <td><asp:TextBox ID="txtBirthDate" runat="server" Visible="false" TextMode="Date" /></td>
                    </tr>
                    <tr>
                        <td>Employee Number:</td>
                        <td><asp:Label ID="lblEmployeeNumber" runat="server" /></td>
                        <td><asp:TextBox ID="txtEmployeeNumber" runat="server" Visible="false" /></td>
                    </tr>
                    <tr>
                        <td>Salary:</td>
                        <td><asp:Label ID="lblSalary" runat="server" /></td>
                        <td><asp:TextBox ID="txtSalary" runat="server" Visible="false" TextMode="Number" /></td>
                    </tr>
                    <tr>
                        <td>Role:</td>
                        <td><asp:Label ID="lblRole" runat="server" /></td>
                        <td><asp:TextBox ID="txtRole" runat="server" Visible="false" /></td>
                    </tr>
                    <tr>
                        <td>Manager:</td>
                        <td><asp:Label ID="lblManager" runat="server" /></td>
                        <td><asp:TextBox ID="txtManager" runat="server" Visible="false" /></td>
                    </tr>
                    <tr>
                        <td>Location:</td>
                        <td><asp:Label ID="lblLocation" runat="server" /></td>
                        <td><asp:TextBox ID="txtLocation" runat="server" Visible="false" /></td>
                    </tr>
                    <tr>
                        <td>Department:</td>
                        <td><asp:Label ID="lblDepartment" runat="server" /></td>
                        <td><asp:TextBox ID="txtDepartment" runat="server" Visible="false" /></td>
                    </tr>
                </table>
            </div>
            
            <div class="employee-image-container">
                <asp:Image ID="imgGravatar" runat="server" CssClass="employee-large-image" />
            </div>
        </div>
        
        <div class="employee-button-group">
            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="employee-button blue" OnClick="btnChangeProfile_Click" />
            <asp:Button ID="btnEditEmployee" runat="server" Text="Edit" CssClass="employee-button" OnClick="btnEditEmployee_Click" />
            <asp:Button ID="btnSaveEmployee" runat="server" Text="Save" CssClass="employee-button save" OnClick="btnSaveEmployee_Click" Visible="false" />
            <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="employee-button cancel" OnClick="btnCancelEdit_Click" Visible="false" />
            <asp:Button ID="btnChangeProfile" runat="server" Text="Change Profile Image" CssClass="employee-button save" OnClick="btnChangeProfile_Click" Visible="false" />
        </div>
    </asp:Panel>
    <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
</asp:Content>
