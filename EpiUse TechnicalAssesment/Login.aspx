<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Login
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div class="login-page">
        <div class="login-page-container">
            <div class="login-box">
                <h2>Employee Login</h2>

                      <div class="login-form-group">
                            <asp:Label ID="lblUserName" runat="server" Text="Employee Number" CssClass="login-label"></asp:Label>
                            <asp:TextBox ID="txtEmployeeNumber" runat="server" CssClass="login-textbox"></asp:TextBox>
                       </div>
        
                        <div class="login-form-group">
                            <asp:Label ID="lblEmployeePassword" runat="server" Text="Employee Password" CssClass="login-label"></asp:Label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="login-textbox"></asp:TextBox>
                        </div>
        
             <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="login-button" OnClick="btnLogin_Click" />
        
             <asp:Label ID="lblLoginMessage" runat="server" ForeColor="Red" CssClass="login-message"></asp:Label> <%--this displays if an error occurs, Database or user related--%> 

            </div>
        </div>
         <div id="popup" class="simple-popup"> <%--Popup for an iccorect username or password--%>
          <div class="popup-white-box">
          <p>Invalid Employee Number or password</p>
          <img src="Images/EpiUseLogo.png" alt="Company Logo" />
          <div class="buttonSection">
              <asp:Button ID="btnOkay" runat="server" Text="Okay" CssClass="popup-button" 
                  OnClientClick="return hidePopup();" />
          </div>
            </div>
          </div>
    </div>

    <script type="text/javascript">
    function showPopup() {
        document.getElementById('popup').classList.add('show');
    }

    function hidePopup() {
        document.getElementById('popup').classList.remove('show');
        return false; // Prevent postback
    }

    // Show popup if there's an error
    window.onload = function () {
        <% if (lblLoginMessage.Text.Contains("Invalid")) { %>
        showPopup();
        <% } %>
    };
    </script>
</asp:Content>
