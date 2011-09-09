<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/Layout.Master" AutoEventWireup="true"
    CodeBehind="Authorize.aspx.cs" Inherits="_4sqtransit.Mobile.Authorize" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="welcome">
        <b>
            <asp:Label ID="ddWelcomeMessage" runat="server" /></b>
        <asp:Literal ID="uxMapViewer" runat="server"></asp:Literal>
        <br />
        <br />
        We see that you last checked in at "<asp:Label ID="ddVenueName" runat="server"></asp:Label>".
        We're monitoring your Foursquare account and anytime you check in near a
        <asp:Label ID="ddTransitAgency" runat="server"></asp:Label>
        stop, we'll send you the real-time transit schedule for that stop.<br />
        <br />
        <asp:Label ID="ddPhoneMessage" runat="server"></asp:Label><br />
        <br />
        Tired of the notifications? Click
        <asp:HyperLink ID="ddUnsubscribe" runat="server" Text="here" />
        to unsubscribe.
    </div>
</asp:Content>
