<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebImportNhanVien.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" />
            <br />
            &nbsp;<br />
            <br />
            <br />
            <asp:Button ID="btnImport" runat="server" OnClick="btnImport_Click" Text="Import" Width="93px" />
            <br />
            <br />
            <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
