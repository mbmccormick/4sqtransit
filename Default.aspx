<%@ Page Title="" Language="C#" MasterPageFile="~/Common/Layout.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="_4sqtransit.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="ddTransitProviderError" CssClass="notification" Visible="false" runat="server">
        We are currently having issues with our transit provider. As a result, only a limited
        number of Transit Agencies are available at this time.
    </asp:Panel>
    <asp:DropDownList ID="ddTransitAgency" runat="server" title="Transit Agency" Style="width: 100%;" />
    <br />
    <asp:Button ID="uxAuthorize" runat="server" Text="Log In with your Foursquare Account"
        OnClick="uxAuthorize_Click" Style="margin-top: 10px;" />
    <div class="section">
        <b>What is 4sqtransit?</b>
        <p>
            4sqtransit is a simple service that delivers real-time public transit information
            to your phone whenever you check in at a transit stop on Foursquare. When 4sqtransit
            sees that you've checked in at a transit stop, it sends you a text message with
            the schedule for that location.</p>
        <br />
        <b>How does this work?</b>
        <p>
            Select your transit agency above and click the "Log In with your Foursquare Account"
            button to get started. Next, log in on Foursquare and authorize 4sqtransit. After
            that, 4sqtransit will send you a text message anytime you check in at a transit
            stop.</p>
    </div>
</asp:Content>
