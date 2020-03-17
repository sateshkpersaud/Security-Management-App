<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ISSA.Account.Login" %>

<%--<%@ Register Src="../../Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>--%>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <section id="loginForm">
        <asp:Login runat="server" ID="LoginControl" OnLoginError="LoginControl_LoginError" OnLoggingIn="LoginControl_LoggingIn" ViewStateMode="Disabled" RenderOuterTable="false" OnLoggedIn="Unnamed_LoggedIn">
            <LayoutTemplate>
                <div class="col-xs-6 col-xs-offset-3">
                    <div class="form-horizontal">
                        <fieldset>
                            <legend>Sign in please</legend>

                            <div class="form-group">
                                <asp:Label ID="Label1" CssClass="col-xs-3 control-label" runat="server" AssociatedControlID="UserName">User name</asp:Label>
                                <div class="col-xs-9">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="UserName" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." />
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label ID="Label2" CssClass="col-xs-3 control-label" runat="server" AssociatedControlID="Password">Password</asp:Label>
                                <div class="col-xs-9">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="Password" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." />
                                </div>
                            </div>
                            <%--<p class="validation-summary-errors" style="color: red">--%>
                            <p class="text-danger text-center">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                            <%--                            <div class="form-group">
                                <asp:Label ID="Label3" runat="server" AssociatedControlID="RememberMe" CssClass="control-label col-xs-4 text-right">Remember me?</asp:Label>
                                <div class="col-xs-8">
                                    <asp:CheckBox runat="server" Text="Remember Me?" ID="RememberMe" />
                                </div>
                            </div>--%>
                            <%-- <div class="checkbox">
                                <label class="control-label">
                                    <input type="checkbox" runat="server" id="RememberMe" value="">Remember Me?</labe>
                            </div>--%>
                            <%--<br />--%>
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary btn-block" CommandName="Login" Text="Sign in" />
                        </fieldset>
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>
    </section>
    <div class="row">
        <div class="col-sm-12 col-xs-12 text-right">
            <asp:ImageButton ID="ibCompuServ" runat="server" ImageUrl="~/Images/compuServLogo.png" OnClientClick="window.open('http://compuservgy.com/')" Width="50px" Height="50px" />
        </div>
    </div>
</asp:Content>
