<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Hierarchy.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm7" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Hierarchy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Employee Management System</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <h2>Company Hierarchy</h2>
    <div id="hierarchy-container" style="width: 100%; height: 800px;"></div>
    
   <%-- <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />--%>
    <script src="<%= ResolveUrl("~/Scripts/JavaScript.js") %>"></script>
</asp:Content>