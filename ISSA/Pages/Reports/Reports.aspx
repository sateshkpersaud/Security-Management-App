<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="ISSA.Pages.Reports.Reports" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function noFutureDate(sender, args) {
            if (sender._selectedDate > new Date()) {
                ShowMessage("You cannot select a future date", "Warning");
                sender._selectedDate = new Date();
                // set the date back to the current date
                sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
        }

        function validateDateFormat(control, endDate1) {
            if (control.value != "") {
                //Validate the date entered
                var date_regex = /^(?:(0[1-9]|1[012])[\- \/.](0[1-9]|[12][0-9]|3[01])[\- \/.](19|20)[0-9]{2})$/;
                //format the selected date to include zeros for the day and month
                var selectedDate = new Date(control.value).format("MM/dd/yyyy");
                if (!(date_regex.test(selectedDate))) {
                    ShowMessage("Not a valid date. Kindly enter a date in the format MM/DD/YYYY. E.g. 9/27/2015", "Warning");
                    control.style.borderColor = "red";
                    control.value = "";
                    control.focus();
                    //This is for firefox as the above focus does not work
                    tempField = control;
                    setTimeout("tempField.focus();", 1);
                }
                else {
                    var error = new Number();
                    //This part ensures a valid date is entered based on the range of dates for the month the work schedule is being created for
                    //convert the dates to the correct format
                    var dateSet = new Date(control.value).format("MM/dd/yyyy");
                    var endDate = new Date(endDate1).format("MM/dd/yyyy");
                    //ShowMessage(dateSet + " - " + startDate + " - " + endDate, "info");
                    if (dateSet > endDate) {
                        ShowMessage("You cannot select a future date", "Warning");
                        control.value = endDate;
                        control.style.borderColor = "red";
                        control.focus();
                        //This is for firefox as the above focus does not work
                        tempField = control;
                        setTimeout("tempField.focus();", 1);
                        error = 2
                    }
                    if (error != 2) {
                        control.style.borderColor = "lightgrey";
                    }
                }
            }
            else {
                control.style.borderColor = "lightgrey";
            }

        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h4>REPORTS
                    <br />
                <small>Use the form below to generate, export and print reports.
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
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-sm-3 col-xs-6">
                                        <label class="control-label" for="ddlReports">Report: </label>
                                        <asp:DropDownList ID="ddlReports" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlReports_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddlReports" ForeColor="Red" runat="server" ValidationGroup="Reports" ErrorMessage="Required."></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="col-sm-2 col-xs-6">
                                        <label class="control-label" for="dtpStartDate">Start Date</label>
                                        <asp:ImageButton ID="ibDateStart" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                        <asp:TextBox ID="dtpStartDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" OnTextChanged="dtpStartDate_TextChanged"></asp:TextBox>
                                        <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="dtpStartDate" runat="server" PopupButtonID="ibDateStart" />
                                         <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="dtpStartDate" ForeColor="Red" runat="server" ValidationGroup="Reports" ErrorMessage="Required."></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ValidationGroup="Reports" ID="CompareValidator2" runat="server" ControlToCompare="dtpStartDate" ControlToValidate="dtpEndDate" ErrorMessage="<br/>Start Date must be on or before End Date." ForeColor="Red" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                    </div>
                                    <div class="col-sm-2 col-xs-6">
                                        <label class="control-label" for="dtpEndDate">End Date</label>
                                        <asp:ImageButton ID="ibDateEnd" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                        <asp:TextBox ID="dtpEndDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" OnTextChanged="dtpEndDate_TextChanged"></asp:TextBox>
                                        <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="dtpEndDate" runat="server" PopupButtonID="ibDateEnd" />
                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="dtpEndDate" ForeColor="Red" runat="server" ValidationGroup="Reports" ErrorMessage="Required."></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="col-sm-2 col-xs-6">
                                        <label class="control-label" for="ddlSubParam">Sub Param: </label>
                                        <asp:DropDownList ID="ddlSubParam" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                <div class="col-sm-3 col-xs-6 text-right">
                                     <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnGenerate" runat="server" CssClass="btn btn-success" ValidationGroup="Reports" OnClick="btnGenerate_Click">Generate <span aria-hidden="true" class="glyphicon glyphicon-check"></span></asp:LinkButton>
                            </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbToday" runat="server" Text="Today" GroupName="DateMenu" OnCheckedChanged="rbToday_CheckedChanged"
                                            AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to Today." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbThisWeek" runat="server" Text="This Week" GroupName="DateMenu"
                                            OnCheckedChanged="rbThisWeek_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to this week." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbThisMonth" runat="server" Text="This Month" GroupName="DateMenu"
                                            OnCheckedChanged="rbThisMonth_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to this month." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbThisQuarter" runat="server" Text="This Quarter" GroupName="DateMenu"
                                            OnCheckedChanged="rbThisQuarter_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to this quarter." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbThisYear" runat="server" Text="This Year" GroupName="DateMenu"
                                            OnCheckedChanged="rbThisYear_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to this year." />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbYesterday" runat="server" Text="Yesterday" GroupName="DateMenu"
                                            OnCheckedChanged="rbYesterday_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to Yesterday." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbLastWeek" runat="server" Text="Last Week" GroupName="DateMenu"
                                            OnCheckedChanged="rbLastWeek_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to last week." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbLastMonth" runat="server" Text="Last Month" GroupName="DateMenu"
                                            OnCheckedChanged="rbLastMonth_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to last month." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbLastQuarter" runat="server" Text="Last Quarter" GroupName="DateMenu"
                                            OnCheckedChanged="rbLastQuarter_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to last quarter." />
                                    </div>
                                    <div class="col-sm-2 col-xs-2">
                                        <asp:RadioButton ID="rbLastYear" runat="server" Text="Last Year" GroupName="DateMenu"
                                            OnCheckedChanged="rbLastYear_CheckedChanged" AutoPostBack="true" Font-Size="Small" ToolTip="Check here to set the Start Date and End Date to last year." />
                                    </div>
                                </div>
                            </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12">
                        <rsweb:ReportViewer ID="rvReports" runat="server" Width="100%" Height="550px" HyperlinkTarget="_blank" BorderStyle="Groove" ShowParameterPrompts="False" ShowPrintButton="true">
                        </rsweb:ReportViewer>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
