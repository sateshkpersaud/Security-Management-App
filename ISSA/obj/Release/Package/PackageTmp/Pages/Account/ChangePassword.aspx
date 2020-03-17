<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ISSA.Pages.Account.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-xs-6 col-xs-offset-3">
        <div class="form-horizontal">
            <fieldset>
                <legend>Change Your Password</legend>
            </fieldset>
            <div class="form-group">
                <asp:Label ID="NewPasswordLabel" CssClass="col-xs-3 control-label" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                <div class="col-xs-9">
                    <asp:TextBox ID="NewPassword" CssClass="form-control" runat="server" TextMode="Password" placeholder="6 characters minimum"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword" ErrorMessage="New Password is required." ToolTip="New Password is required." ValidationGroup="ChangePassword1">*</asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-group">
                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                <div class="col-xs-9">
                    <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="6 characters minimum"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword" ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required." ValidationGroup="ChangePassword1">*</asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry." ValidationGroup="ChangePassword1"></asp:CompareValidator>

                </div>
            </div>

            <div class="form-group">
                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
            </div>

            <asp:Button ID="ChangePasswordPushButton" runat="server" CssClass="btn btn-primary" OnClick="ChangePasswordPushButton_Click" CommandName="ChangePassword" Text="Change Password" ValidationGroup="ChangePassword1" />
        </div>
    </div>
</asp:Content>
