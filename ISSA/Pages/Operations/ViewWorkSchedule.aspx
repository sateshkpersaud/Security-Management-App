<%@ Page Title="Work Schedule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewWorkSchedule.aspx.cs" Inherits="ISSA.Pages.Operations.PrintWorkSchedule" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h4>WORK SCHEDULE
                    <br />
                <small>Use the form below to create work schedules.
                </small>
            </h4>
            <hr />
            <div class="container-fluid">
                <%-- <div class="row">
                    <div class="col-sm-3 col-xs-6">
                        <asp:RadioButton ID="rbCreate" runat="server" Text="Create" GroupName="WS" CssClass="radio-inline" AutoPostBack="true" OnCheckedChanged="rbCreate_CheckedChanged" />
                        <asp:RadioButton ID="rbModify" runat="server" Text="Modify/View" GroupName="WS" CssClass="radio-inline" AutoPostBack="true" OnCheckedChanged="rbModify_CheckedChanged" />
                    </div>
                </div>--%>
                <br />
                <div class="row">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            Parameters
                        </div>
                        <div class="panel-body">
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="ddlYear">Year: </label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddlYear" ForeColor="Red" runat="server" ValidationGroup="WS" ErrorMessage="Year is required."></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" runat="server" id="lblMonth">Month: </label>
                                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
                                    <asp:ListItem Value="1" Text="January"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="February"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="March"></asp:ListItem>
                                    <asp:ListItem Value="4" Text="April"></asp:ListItem>
                                    <asp:ListItem Value="5" Text="May"></asp:ListItem>
                                    <asp:ListItem Value="6" Text="June"></asp:ListItem>
                                    <asp:ListItem Value="7" Text="July"></asp:ListItem>
                                    <asp:ListItem Value="8" Text="August"></asp:ListItem>
                                    <asp:ListItem Value="9" Text="September"></asp:ListItem>
                                    <asp:ListItem Value="10" Text="October"></asp:ListItem>
                                    <asp:ListItem Value="11" Text="November"></asp:ListItem>
                                    <asp:ListItem Value="12" Text="December"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="ddlMonth" runat="server" ValidationGroup="WS" ForeColor="Red" ErrorMessage="Month is required."></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlArea">Area: </label>
                                <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlArea" runat="server" ValidationGroup="WS" ErrorMessage="Area is required." ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="ddlShift">Shift: </label>
                                <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-6 text-right">
                                <br />
                                <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                <asp:LinkButton ID="lbGenerate" runat="server" CssClass="btn btn-success" Text="Generate" ValidationGroup="WS" OnClick="lbGenerate_Click">Generate <span aria-hidden="true" class="glyphicon glyphicon-check"></span></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12">
                        <rsweb:ReportViewer ID="rvWorkSchedule" runat="server" Width="100%" Height="550px" HyperlinkTarget="_blank" BorderStyle="Groove" ShowParameterPrompts="False" ShowPrintButton="true">
                            <%-- <LocalReport EnableHyperlinks="True">
                         </LocalReport>--%>
                        </rsweb:ReportViewer>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
