<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserForm.ascx.cs" Inherits="ISSA.Pages.Administrator.UserForm" %>

<script>
    //script to validate checkbox 
    function ValidateModuleList(source, args) {
        var chkListModules = document.getElementById('<%= cbkRole.ClientID %>');
        var chkListInputs = chkListModules.getElementsByTagName("input");
        for (var i = 0; i < chkListInputs.length; i++){
            if (chkListInputs[i].checked)
            {
                args.IsValid = true;
                return; 
            }
            args.IsValid = false;
        }
    }
</script>

<div class="form-horizontal">
    <div class="form-group">
        <asp:Label ID="Label2" CssClass="control-label col-xs-2" AssociatedControlID="ddlEmployee" runat="server" Text="Employee Link"></asp:Label>
        <div class="col-xs-4">
            <asp:DropDownList ID="ddlEmployee" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged" runat="server"></asp:DropDownList>
            <asp:Label ID="lblEmployee" runat="server" CssClass="btn control-label" Text="" Visible="false"></asp:Label>
        </div>
    </div>

    <div class="form-group">
        <asp:Label ID="Label4" runat="server" AssociatedControlID="tbFName" CssClass="control-label col-xs-2" Text="First Name"></asp:Label>
        <div class="col-xs-4">
            <asp:TextBox ID="tbFName" MaxLength="25" CssClass="form-control" AutoPostBack="true" OnTextChanged="tbName_TextChanged" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="First Name is Required!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="tbFName" CssClass="text-danger"></asp:RequiredFieldValidator>
        </div>

        <asp:Label ID="Label7" runat="server" AssociatedControlID="tbLName" CssClass="control-label col-xs-2" Text="Last Name"></asp:Label>
        <div class="col-xs-4">
            <asp:TextBox ID="tbLName" MaxLength="25" CssClass="form-control" AutoPostBack="true" OnTextChanged="tbName_TextChanged" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Last Name is Required!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="tbLName" CssClass="text-danger"></asp:RequiredFieldValidator>
        </div>
    </div>

    <div class="form-group">
        <asp:Label ID="Label8" CssClass="control-label col-xs-2" AssociatedControlID="cbkRole" runat="server" Text="Role"></asp:Label>
        <div class="col-xs-4">
            <asp:CheckBoxList ID="cbkRole" RepeatDirection="Vertical" runat="server"></asp:CheckBoxList>
            <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="A role is required!" ClientValidationFunction="ValidateModuleList" Display="Dynamic" ValidationGroup="usrVal" CssClass="text-danger"></asp:CustomValidator>
        </div>

        <asp:Label ID="Label3" runat="server" CssClass="control-label col-xs-2" AssociatedControlID="tbUserName" Text="UserName"></asp:Label>
        <div class="col-xs-4">
            <asp:TextBox ID="tbUserName" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="UserName is Required!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="tbUserName" CssClass="text-danger"></asp:RequiredFieldValidator>
        </div>
    </div>

    <div class="form-group">
        <asp:Label ID="Label9" runat="server" CssClass="control-label col-xs-2" AssociatedControlID="tbEmail" Text="Email"></asp:Label>
        <div class="col-xs-4">
            <asp:TextBox ID="tbEmail" CssClass="form-control" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Email is Required!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="tbEmail" CssClass="text-danger"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Please enter a valid email!" Display="Dynamic" CssClass="text-danger" ValidationGroup="usrVal" ValidationExpression="^\s*(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*\s*$" ControlToValidate="tbEmail"></asp:RegularExpressionValidator>
        </div>

        <asp:Label ID="Label10" CssClass="control-label col-xs-2" AssociatedControlID="ddlStatus" runat="server" Text="Status"></asp:Label>
        <div class="col-xs-4">
            <asp:DropDownList ID="ddlStatus" CssClass="form-control" runat="server">
                <asp:ListItem Value="True">Active</asp:ListItem>
                <asp:ListItem Value="False">Inactive</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="A status is required!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="ddlStatus" CssClass="text-danger"></asp:RequiredFieldValidator>
        </div>
    </div>

    <div class="form-group">
        <asp:Label ID="Label11" runat="server" CssClass="control-label col-xs-2" AssociatedControlID="tbPassword" Text="Password"></asp:Label>
        <div class="col-xs-4">
            <asp:TextBox ID="tbPassword" TextMode="Password" CssClass="form-control" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Password is Required!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="tbPassword" CssClass="text-danger"></asp:RequiredFieldValidator>
        </div>


        <asp:Label ID="Label12" runat="server" CssClass="control-label col-xs-2" AssociatedControlID="tbConfPassword" Text="Confirm Password"></asp:Label>
        <div class="col-xs-4">
            <asp:TextBox ID="tbConfPassword" TextMode="Password" CssClass="form-control" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="Please confirm the password!" Display="Dynamic" ValidationGroup="usrVal" ControlToValidate="tbConfPassword" CssClass="text-danger"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="tbPassword" ErrorMessage="Passwords do not match!" ValidationGroup="usrVal" ControlToValidate="tbConfPassword" CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
        </div>
    </div>

    <div class="form-group">
        <div class="col-xs-offset-8">
            <div class="checkbox">
                <label>
                    <input type="checkbox" runat="server" id="cbkPwdReset" value="">Reset Password on login</label>
            </div>
        </div>
    </div>
</div>
