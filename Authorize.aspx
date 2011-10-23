<%@ Page Title="" Language="C#" MasterPageFile="~/Common/Layout.Master" AutoEventWireup="true"
    CodeBehind="Authorize.aspx.cs" Inherits="_4sqtransit.Authorize" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="welcome">
        <b>
            <asp:Label ID="ddWelcomeMessage" runat="server" /></b>
        <asp:Literal ID="uxMapViewer" runat="server"></asp:Literal>
        <br />
        <br />
        We see that you last checked in at <b>
            <asp:Label ID="ddVenueName" runat="server"></asp:Label></b>. We're monitoring
        your Foursquare account and anytime you check in at a <b>
            <asp:Label ID="ddTransitAgency" runat="server"></asp:Label></b> stop, we'll
        send you the real-time transit schedule for that stop.<br />
        <br />
        <asp:Label ID="ddPhoneMessage" runat="server"></asp:Label><br />
        <br />
        Tired of the notifications? Click
        <asp:HyperLink ID="ddUnsubscribe" runat="server" Text="here" />
        to unsubscribe.
    </div>
</asp:Content>
