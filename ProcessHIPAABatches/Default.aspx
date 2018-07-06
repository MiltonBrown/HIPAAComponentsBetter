<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProcessHIPAABatches._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>TestHIPAABuild</h1>
        <p class="lead">This application is the front end for receipt of an comma-delimited file and converting into an X12 837P.</p>
        <p></p>
    </div>

    <div class="row">
        <asp:DataGrid ID="dgInterchanges" runat="server" Width="90%" AllowSorting="True" AutoGenerateColumns="False" BorderStyle="Solid"
            OnSelectedIndexChanged="dgInterchanges_SelectedIndexChanged">
            <Columns>
                <asp:BoundColumn HeaderText="InterchangeID" DataField="InterchangeID" />
                <asp:BoundColumn HeaderText="SenderIDQual" DataField="SenderIDQual" />
                <asp:BoundColumn HeaderText="SenderID" DataField="SenderID" />
                <asp:BoundColumn HeaderText="ReceiverIDQual" DataField="ReceiverIDQual" />
                <asp:BoundColumn HeaderText="ReceiverID" DataField="ReceiverID" />
                <asp:BoundColumn HeaderText="Acknowledgement" DataField="Acknowledgment" />
                <asp:BoundColumn HeaderText="RepetitionSep" DataField="RepetitionSeparator" />
                <asp:BoundColumn HeaderText="ElementSep" DataField="ElementSeparator" />
                <asp:BoundColumn HeaderText="Usage" DataField="Usage" />
                <asp:ButtonColumn HeaderText="Select" Text="Select" CommandName="Select" ButtonType="PushButton"></asp:ButtonColumn>
            </Columns>
            <HeaderStyle BorderStyle="Double" Font-Bold="true" BackColor="SlateGray" />
        </asp:DataGrid>
    </div>
    <br />
    <br />
    <div class="row">
        <asp:DataGrid ID="dgEDIContacts" runat="server" Width="90%" AllowSorting="true" AutoGenerateColumns="false" BorderStyle="Solid" OnSelectedIndexChanged="dgEDIContacts_SelectedIndexChanged">
            <Columns>
                <asp:BoundColumn HeaderText="SubmitterEDIContactID" DataField="SubmitterEDIContactID" />
                <asp:BoundColumn HeaderText="ContactFunction" DataField="ContactFunctionCode" />
                <asp:BoundColumn HeaderText="ContactName" DataField="ContactName" />
                <asp:BoundColumn HeaderText="ContactNumberID" DataField="CommunicationNumberID" />
                <asp:BoundColumn HeaderText="ContactNumber" DataField="CommunicationNumber" />
                <asp:BoundColumn HeaderText="ContactNumberID 2" DataField="CommunicationNumberID2" />
                <asp:BoundColumn HeaderText="ContactNumber 2" DataField="CommunicationNumber2" />
                <asp:BoundColumn HeaderText="ContactNumberID 3" DataField="CommunicationNumberID3" />
                <asp:BoundColumn HeaderText="ContactNumber 3" DataField="CommunicationNumber3" />
                <asp:ButtonColumn HeaderText="Select" Text="Select" CommandName="Select" ButtonType="PushButton" />
            </Columns>
            <HeaderStyle BorderStyle="Double" Font-Bold="true" BackColor="SlateGray" />
        </asp:DataGrid>
    </div>
    <br />
    <br />
    <div style="height: 25px; width: 2541px">
        <table>
            <tr>
                <asp:Button ID="btnBuildBatches" Text="Build Batches" runat="server" OnClick="btnBuildBatches_Click" />
            </tr>
        </table>
    </div>

</asp:Content>
