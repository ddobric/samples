<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebRole1._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to ASP.NET!
    </h2>
    <p>
        To learn more about ASP.NET visit <a href="http://www.asp.net" title="ASP.NET Website">www.asp.net</a>.
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" Text="List Drives" 
            onclick="Button1_Click"  />
        <asp:Button ID="Button2" runat="server" Text="Do Write IO" onclick="Button2_Click" 
            />

            <asp:Button ID="Button4" runat="server" Text="Do Read IO" onclick="Button4_Click"  
            />

        <asp:Button ID="Button3" runat="server" Text="Close" onclick="Button3_Click" 
            />
    </p>
</asp:Content>
