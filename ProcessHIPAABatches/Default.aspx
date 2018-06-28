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
    <div style="height: 25px; width: 2541px">
        <table>
            <tr>
                <asp:Button ID="btnBuildBatches" Text="Build Batches" runat="server" OnClick="btnBuildBatches_Click" />
            </tr>
        </table>
    </div>

</asp:Content>
