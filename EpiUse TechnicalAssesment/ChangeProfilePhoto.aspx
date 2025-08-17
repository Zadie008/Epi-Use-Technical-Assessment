<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ChangeProfilePhoto.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Profile photo
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
   <h1>Change the profile photo of employee: <asp:Label ID="lblEmployeeHeaderName" runat="server" /></h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
      <asp:Panel ID="pnlChangePhoto" runat="server" CssClass="photo-panel">
      <h2>Current Profile Photo</h2>
       <asp:Label ID="lblMessage" runat="server" CssClass="message-label" />
      <div class="employee-image-container-inChange">
          <asp:Image ID="imgEmployeePhoto" runat="server" CssClass="employee-large-image" />
      </div>
      
      <div class="upload-controls">
          <asp:Literal ID="litUploadPrompt" runat="server" Text="<p>Select a new profile image:</p>" />
          <asp:FileUpload ID="fuProfileImage" runat="server" />
          <br />
          <asp:Button ID="btnUploadPhoto" runat="server" Text="Upload Photo" OnClick="btnUploadPhoto_Click" CssClass="edit-button" />
      </div>

     
      
      <div class="button-group">
          <asp:Button ID="btnBack" runat="server" Text="Back to Profile" OnClick="btnBack_Click" CssClass="button-cancel" />
      </div>
  </asp:Panel>
</asp:Content>
