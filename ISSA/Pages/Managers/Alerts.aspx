<%@ Page Title="Alerts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Alerts.aspx.cs" Inherits="ISSA.Pages.Managers.Alerts" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        //$(document).ready(function () {
        //    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        //        localStorage.setItem('activeTab', $(e.target).attr('href'));
        //    });
        //    var activeTab = localStorage.getItem('activeTab');
        //    if (activeTab) {
        //        ShowMessage(activeTab, Info)
        //        $('#myTab a[href="' + activeTab + '"]').tab('show');
        //    }
        //});

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

        function noFutureDate(sender, args) {
            if (sender._selectedDate > new Date()) {
                ShowMessage("You cannot select a future date", "Warning");
                sender._selectedDate = new Date();
                // set the date back to the current date
                sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h4>ALERTS
                    <br />
        <small>Use the form below to manage operations.
        </small>
    </h4>
    <hr />
    <asp:UpdatePanel ID="upTimer" runat="server">
        <ContentTemplate>
            <div class="text-right">
                <asp:Timer ID="Timer1" runat="server" Interval="1000" OnTick="Timer1_Tick"></asp:Timer>
                <span aria-hidden="true" class="glyphicon glyphicon-time"></span>

                <asp:Label ID="lblCounter" runat="server" Text="" Font-Bold="true" Font-Italic="true" CssClass="control-label"></asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- Header --%>
    <asp:Panel ID="pnlDetails" runat="server" HorizontalAlign="Left">
        <ul class="nav nav-tabs" id="myTab">
            <li class="active"><a data-toggle="tab" href="#Logs">Logs</a></li>
            <li><a data-toggle="tab" href="#Attendance">Attendance</a></li>
            <li><a data-toggle="tab" href="#DailyReport">Daily Operations Report</a></li>
        </ul>

        <%-- Content --%>
        <div class="tab-content">
            <%-- Daily Logs, Incidents and Complaints --%>
            <div id="Logs" class="tab-pane fade in active">
                <br />
                <asp:UpdatePanel ID="upLogs" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbLogDate">Date</label>
                                    <asp:ImageButton ID="ibSearchDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbLogDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbLogDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlLogArea">Area</label>
                                    <asp:DropDownList ID="ddlLogArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLogArea_SelectedIndexChanged"></asp:DropDownList>
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" ForeColor="Red" ControlToValidate="ddlLogArea" ValidationGroup="Logs"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlLogLocation">Location</label>
                                    <asp:DropDownList ID="ddlLogLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-3 col-xs-6 text-left">
                                    <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" ValidationGroup="Logs">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnStopStartTimer" runat="server" CssClass="btn btn-danger" OnClick="btnStopStartTimer_Click">Stop Timer </asp:LinkButton>
                                </div>
                                <div class="col-sm-2 col-xs-6 text-right">
                                    <label class="control-label">&nbsp;</label><br />

                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-8 col-xs-12">
                                    <label class="control-label" for="">Operator's Call Log</label>
                                    <asp:GridView ID="gvCallLog"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnPageIndexChanging="gvCallLog_PageIndexChanging"
                                        OnSelectedIndexChanged="gvCallLog_SelectedIndexChanged"
                                        OnRowDataBound="gvCallLog_RowDataBound">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Location">
                                                <ItemTemplate>
                                                    <%-- <asp:LinkButton ID="lbReportNumber" runat="server" CommandName="Select">--%>
                                                    <asp:Label ID="lblLocationName" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                                    <%-- </asp:LinkButton>--%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Time">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTimeOfCall" runat="server" Text='<%#Eval("TimeOfCall")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Reports">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReports" runat="server" Text='<%#Eval("Reports")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Sup. Check Location" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSupCheckLocation" runat="server" Text='<%#Eval("SupCheckLocation")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Call made by" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedBy" runat="server" Text='<%#Eval("LastModifiedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedOn" runat="server" Text='<%#Eval("LastModifiedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <div class="col-sm-4 col-xs-12">
                                    <label class="control-label" for="gvIncidentscomplaintsSummary">Incident/Complaint Summary</label>
                                    <asp:GridView ID="gvIncidentscomplaintsSummary"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnPageIndexChanging="gvIncidentscomplaintsSummary_PageIndexChanging"
                                        OnRowCommand="gvIncidentscomplaintsSummary_RowCommand"
                                        OnRowDataBound="gvIncidentscomplaintsSummary_RowDataBound">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Location">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLocationName" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Incidents Count" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbIncidentCount" runat="server" CommandName="IncidentView" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblIncidentCount" runat="server" Text='<%#Eval("incidentCount")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Complaints Count" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbComplaintCount" runat="server" CommandName="ComplaintView" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblComplaintCount" runat="server" Text='<%#Eval("complaintCount")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLocationID" runat="server" Text='<%#Eval("LocationID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="gvIncidentscomplaintsSummary" />
                        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
                        <asp:AsyncPostBackTrigger ControlID="btnStopStartTimer" />
                        <asp:AsyncPostBackTrigger ControlID="ddlLogArea" />
                         <asp:AsyncPostBackTrigger ControlID="gvCallLog" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <%-- Daily Operations Report --%>
            <div id="DailyReport" class="tab-pane fade">
                <br />
                <asp:UpdatePanel ID="upDailyReport" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-1 col-xs-2 text-right">
                                    <label class="control-label" for="tbDORDate">Date</label>
                                    <asp:ImageButton ID="ibDORDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                </div>
                                <div class="col-sm-2 col-xs-4 text-left">
                                    <asp:TextBox ID="tbDORDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" OnTextChanged="tbDORDate_TextChanged"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender4" TargetControlID="tbDORDate" runat="server" PopupButtonID="ibDORDate" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-2 text-right">
                                    <label class="control-label" for="lblDayOfWeek">
                                        DoW:
                                    </label>
                                    <asp:Label ID="lblDayOfWeek" runat="server" Text=""></asp:Label>
                                </div>
                                 <div class="col-sm-2 col-xs-2 text-right">
                                    <label class="control-label" for="lblTime">
                                        Time:
                                    </label>
                                    <asp:Label ID="lblTime" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="col-sm-1 col-xs-2 text-right">
                                    <label class="control-label" for="ddlDORShift">Shift</label>
                                </div>
                                <div class="col-sm-2 col-xs-4 text-left">
                                    <asp:DropDownList ID="ddlDORShift" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDORShift_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-2 text-right">
                                    <label class="control-label" for="lblHoursInShift">
                                        Hours is shift: </label> 
                                    <asp:Label ID="lblHoursInShift" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                        </div>
                        <br />
                        <asp:Panel ID="pnlDORDetails" runat="server" HorizontalAlign="Left">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-sm-12 col-xs-12">
                                        <asp:GridView ID="gvShiftSupervisors"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnRowDataBound="gvShiftSupervisors_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Supervisor" DataField="FullName" />
                                                <asp:BoundField HeaderText="Work Hours" DataField="WorkHours" ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField HeaderText="Zone Coverage" DataField="ZoneCoverage" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblLateArrivals">Late arrivals:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                       <asp:Label ID="lblLateArrivals" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblCallsForAssistance">Calls for assistance:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                        <asp:Label ID="lblCallsForAssistance" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblOnDuty">On Duty:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                       <asp:Label ID="lblOnDuty" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblNotDressedProperly">Not dressed properly:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                       <asp:Label ID="lblNotDressedProperly" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblSickCalls">Sick calls:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                        <asp:Label ID="lblSickCalls" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblSleeping">Sleeping:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                        <asp:Label ID="lblSleeping" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblIncidents">Incidents:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                         <asp:Label ID="lblIncidents" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="col-sm-2 col-xs-3 text-right">
                                        <label class="control-label" for="lblComplaints">Complaints:</label>
                                    </div>
                                    <div class="col-sm-1 col-xs-3 text-left">
                                         <asp:Label ID="lblComplaints" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <label class="control-label" for="lblShiftSummary">Shift summary</label><br />
                                    <asp:Label ID="lblShiftSummary" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <label class="control-label" for="lblShiftBriefingNotes">Shift briefing notes</label><br />
                                   <asp:Label ID="lblShiftBriefingNotes" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                 <div class="col-md-12 col-xs-12 text-right">
                                               <%-- <asp:LinkButton ID="btnPrint" OnClick="btnPrint_Click"  CssClass="btn btn-default" runat="server">Print <span aria-hidden="true" class="glyphicon glyphicon-print"></span> </asp:LinkButton>--%>
                                            </div>
                            </div>
                             <div class="row">
                                <div class="col-sm-12 col-xs-12 text-right">
                                    <asp:Label ID="lblAuditTrail" runat="server" Font-Size="X-Small" ForeColor="DarkGray" Text=""></asp:Label>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvShiftSupervisors" />
                        <asp:AsyncPostBackTrigger ControlID="ddlDORShift" />
                        <asp:AsyncPostBackTrigger ControlID="tbDORDate" />
                        <%-- <asp:PostBackTrigger ControlID="btnPrint" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <%-- Attendance --%>
            <div id="Attendance" class="tab-pane fade">
                <br />
                <asp:UpdatePanel ID="upAttendance" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbAttendanceDateFrom">Date From</label>
                                    <asp:ImageButton ID="ibAttendanceDateFrom" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbAttendanceDateFrom" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbAttendanceDateFrom" runat="server" PopupButtonID="ibAttendanceDateFrom" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbAttendanceDateTo">To</label>
                                    <asp:ImageButton ID="ibAttendanceDateto" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbAttendanceDateTo" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="tbAttendanceDateTo" runat="server" PopupButtonID="ibAttendanceDateto" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlAttendanceArea">Area</label>
                                    <asp:DropDownList ID="ddlAttendanceArea" CssClass="form-control" runat="server"></asp:DropDownList>
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" ForeColor="Red" ControlToValidate="ddlLogArea" ValidationGroup="Logs"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlAttendanceShift">Shift</label>
                                    <asp:DropDownList ID="ddlAttendanceShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-3 col-xs-6 text-left">
                                    <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnSearchAttendance" runat="server" CssClass="btn btn-primary" OnClick="btnSearchAttendance_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnStopStartTimer2" runat="server" CssClass="btn btn-danger" OnClick="btnStopStartTimer_Click">Stop Timer </asp:LinkButton>
                                </div>
                                <div class="col-sm-2 col-xs-6 text-right">
                                    <label class="control-label">&nbsp;</label><br />

                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="">Attendance</label>
                                    <asp:GridView ID="gvAttendance"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowCommand="gvAttendance_RowCommand"
                                        OnRowDataBound="gvAttendance_RowDataBound">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Area">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblArea" runat="server" Text='<%#Eval("Area")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Stand-By Used" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbStandByUsed" runat="server" ForeColor="Black" CommandName="StandByUsed" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblStandByUsed" runat="server" Text='<%#Eval("StandByUsed")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Stand-By Unused" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbStandByUnused" runat="server" ForeColor="Black" CommandName="StandByUnused" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblStandByUnused" runat="server" Text='<%#Eval("StandByUnused")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Sent Home" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbSentHome" runat="server" ForeColor="Black" CommandName="SentHome" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblSentHome" runat="server" Text='<%#Eval("SentHome")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Day Off" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbDayOff" runat="server" ForeColor="Black" CommandName="DayOff" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblDayOff" runat="server" Text='<%#Eval("DayOff")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Absent" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbAbsent" runat="server" ForeColor="Black" CommandName="Absent" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblAbsent" runat="server" Text='<%#Eval("Absent")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Shortage" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbShortage" runat="server" ForeColor="Black" CommandName="Shortage" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblShortage" runat="server" Text='<%#Eval("Shortage")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Illegal Employees" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbIllegalEmployees" runat="server" ForeColor="Black" CommandName="IllegalEmployees" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblIllegalEmployees" runat="server" Text='<%#Eval("IllegalEmployees")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doubles" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbDouble" runat="server" ForeColor="Black" CommandName="Doubles" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                        <asp:Label ID="lblDouble" runat="server" Text='<%#Eval("Double")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAreaID" runat="server" Text='<%#Eval("AreaID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvAttendance" />
                        <asp:AsyncPostBackTrigger ControlID="btnSearchAttendance" />
                        <asp:AsyncPostBackTrigger ControlID="btnStopStartTimer2" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

        </div>
    </asp:Panel>

    <%-- Incidents Details Pop Up --%>
    <div class="modal fade" id="incidentsModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header ">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblIncidentsHeader" runat="server" Text="Incident Summary"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label"></label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgPersonsAffected" MaxLength="5000" ID="tbPersonDescription" Visible="false" runat="server" TextMode="MultiLine" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Description is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbPersonDescription" ValidationGroup="vgPersonsAffected"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <asp:GridView ID="gvIncidentsDetails"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnSelectedIndexChanged="gvIncidentsDetails_SelectedIndexChanged"
                                            OnRowDataBound="gvIncidentsDetails_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Number">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbIncidentNumber" runat="server" CommandName="Select">
                                                            <asp:Label ID="lblIncidentNumber" runat="server" Text='<%#Eval("IncidentNumber")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Time Occured">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTimeOccured" runat="server" Text='<%#Eval("TimeOccured")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIncidentType" runat="server" Text='<%#Eval("IncidentType")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDescription" runat="server" Text='<%#Eval("Description")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Person(s) Affected">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPersonsAffected" runat="server" Text='<%#Eval("PersonsAffected")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Created By">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIncidentID" runat="server" Text='<%#Eval("IncidentID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCancelIncidentsDetails" runat="server" CssClass="btn btn-default" OnClick="btnCancelIncidentsDetails_Click">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                    <%-- --%>
                </div>
            </div>
        </div>
    </div>

    <%-- Complaints Details Pop Up --%>
    <div class="modal fade" id="complaintsModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblComplaintsHeader" runat="server" Text="Complaints Summary"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label"></label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgPersonsAffected" MaxLength="5000" ID="TextBox1" Visible="false" runat="server" TextMode="MultiLine" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Description is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbPersonDescription" ValidationGroup="vgPersonsAffected"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <asp:GridView ID="gvComplaintsDetails"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnRowDataBound="gvComplaintsDetails_RowDataBound"
                                            OnSelectedIndexChanged="gvComplaintsDetails_SelectedIndexChanged">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Number">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbComplaintNumber" runat="server" CommandName="Select">
                                                            <asp:Label ID="lblComplaintNumber" runat="server" Text='<%#Eval("ComplaintNumber")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Time Occured">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTimeOfIncident" runat="server" Text='<%#Eval("TimeOfIncident")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Mode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMode" runat="server" Text='<%#Eval("Mode")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Person Reporting">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPersonMakingReport" runat="server" Text='<%#Eval("PersonMakingReport")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Received By">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReceivedBy" runat="server" Text='<%#Eval

("ReceivedBy")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Complaint">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblClientComplaint" runat="server" Text='<%#Eval

("ClientComplaint")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Feedback Provided" ItemStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblfeedbackProvided" runat="server" Text='<%#Eval

("feedbackProvided")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Created By">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCloseComplaintsDetails" runat="server" CssClass="btn btn-default" OnClick="btnCloseComplaintsDetails_Click">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeAbsence" DropShadow="True" runat="server" PopupControlID="pnlPersonAffected" TargetControlID="btnShowPopup" CancelControlID="btnHidePopup"></asp:ModalPopupExtender>
    <asp:Panel ID="pnlPersonAffected" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblAbsentDetails" runat="server" Text="Absence Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvAbsenceDetails"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnPreRender="gvAbsenceDetails_PreRender"
                                            OnRowDataBound="gvAbsenceDetails_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Location Assigned" DataField="LocationAssigned" />
                                                 <asp:BoundField HeaderText="Name" DataField="FullName" />
                                                 <asp:BoundField HeaderText="Was Called In" DataField="calledIn" />
                                                <asp:BoundField HeaderText="Date" DataField="dateAbsent" />
                                                <asp:BoundField HeaderText="Shift" DataField="shift" />
                                                <asp:BoundField HeaderText="Reason" DataField="AbsentType" />
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseAttendanceDetails" OnClick="btnCloseAttendanceDetails_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                            <%-- --%>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseAttendanceDetails" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopup2" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup2" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeDayOffs" DropShadow="True" runat="server" PopupControlID="Panel1" TargetControlID="btnShowPopup2"
        CancelControlID="btnHidePopup2">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel1" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblDayOffHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblDayOffAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvDayOffsDetails"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnPreRender="gvDayOffsDetails_PreRender"
                                            OnRowDataBound="gvDayOffsDetails_RowDataBound"  >
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                 <asp:BoundField HeaderText="Location Assigned" DataField="LocationAssigned" />
                                                 <asp:BoundField HeaderText="Name" DataField="FullName" />
                                                 <asp:BoundField HeaderText="Date" DataField="dayOffDate" />
                                                 <asp:BoundField HeaderText="Shift" DataField="shift" />
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseDayOffs" OnClick="btnCloseDayOffs_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                            <%-- --%>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseDayOffs" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopup3" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup3" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeDoubles" DropShadow="True" runat="server" PopupControlID="Panel2" TargetControlID="btnShowPopup3"
        CancelControlID="btnHidePopup3">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel2" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblDoubleHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblDoublesAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvDoublesDetails"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnPreRender="gvDoublesDetails_PreRender"
                                            OnRowDataBound="gvDoublesDetails_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                               <asp:BoundField HeaderText="Location Worked" DataField="LocationWorked" />
                                                 <asp:BoundField HeaderText="Name" DataField="FullName" />
                                                 <asp:BoundField HeaderText="Date" DataField="dateWorked" />
                                                <asp:BoundField HeaderText="Shift" DataField="shift" />
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btncloseDoubles" OnClick="btncloseDoubles_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btncloseDoubles" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopup4" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup4" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeStandbyUsed" DropShadow="True" runat="server" PopupControlID="Panel3" TargetControlID="btnShowPopup4"
        CancelControlID="btnHidePopup4">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel3" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblStandbyUsedHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblStandByAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvStandbyUsed"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnPreRender="gvStandbyUsed_PreRender"
                                            OnRowDataBound="gvStandbyUsed_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Location Worked" DataField="LocationWorked" />
                                                 <asp:BoundField HeaderText="Name" DataField="FullName" />
                                                 <asp:BoundField HeaderText="Date" DataField="dateWorked" />
                                                <asp:BoundField HeaderText="Shift" DataField="shift" />
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseStandbyUsed" OnClick="btnCloseStandbyUsed_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseStandbyUsed" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopup5" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup5" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeStandbyUnused" DropShadow="True" runat="server" PopupControlID="Panel4" TargetControlID="btnShowPopup5"
        CancelControlID="btnHidePopup5">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel4" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblStandbyUnusedHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblStandbyUnusedAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvStandbyUnused"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnRowDataBound="gvStandbyUnused_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseStandbyUnused" OnClick="btnCloseStandbyUnused_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseStandbyUnused" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnHidePopup6" runat="server" Style="display: none" />
    <asp:Button ID="btnShowPopup6" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeSentHome" DropShadow="True" runat="server" PopupControlID="Panel5" TargetControlID="btnShowPopup6"
        CancelControlID="btnHidePopup6">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel5" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblSentHomeHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblSentHomeAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvSentHome"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnPreRender="gvSentHome_PreRender"
                                            OnRowDataBound="gvSentHome_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Location Assigned" DataField="LocationAssigned" />
                                                 <asp:BoundField HeaderText="Name" DataField="FullName" />
                                                 <asp:BoundField HeaderText="Date Sent Home" DataField="dateSentHome" />
                                                 <asp:BoundField HeaderText="Comments" DataField="Comments" />
                                                <asp:BoundField HeaderText="Shift" DataField="shift" />
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseSentHome" OnClick="btnCloseSentHome_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseSentHome" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopup7" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup7" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeIllegalEmployees" DropShadow="True" runat="server" PopupControlID="Panel6" TargetControlID="btnShowPopup7"
        CancelControlID="btnHidePopup7">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel6" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblIllegalEmployeesHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblIllegalEmployeesAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvIllegalEmployees"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="true"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnPreRender="gvIllegalEmployees_PreRender"
                                            OnRowDataBound="gvIllegalEmployees_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Location" DataField="LocationName" />
                                                 <asp:BoundField HeaderText="Name" DataField="FullName" />
                                                 <asp:BoundField HeaderText="Date Range" DataField="dateRange" />
                                                <asp:BoundField HeaderText="Days Worked" DataField="daysWorked" ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseIllegalEmployees" OnClick="btnCloseIllegalEmployees_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseIllegalEmployees" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

     <asp:Button ID="btnShowPopup8" runat="server" Style="display: none" />
    <asp:Button ID="btnHidePopup8" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mpeShortage" DropShadow="True" runat="server" PopupControlID="Panel7" TargetControlID="btnShowPopup8"
        CancelControlID="btnHidePopup8">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel7" runat="server" Style="display: none">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">
                                <asp:Label ID="lblShortageHeader" runat="server" Text="Details"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:Label ID="lblShortageAreaID" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:GridView ID="gvShortageDetails"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            HeaderStyle-CssClass="text-center"
                                            OnRowDataBound="gvShortageDetails_RowDataBound"
                                            OnPreRender="gvShortageDetails_PreRender">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Location" DataField="LocationName" />
                                                <asp:BoundField HeaderText="Scheduled" DataField="TotalScheduled" />
                                                 <asp:BoundField HeaderText="Present" DataField="totalPresent" />
                                                 <asp:BoundField HeaderText="Absent" DataField="totalAbsent" />
                                                <asp:BoundField HeaderText="Shortage" DataField="totalShort" />
                                                <%-- <asp:TemplateField HeaderText="Shortage">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShortage" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:BoundField DataField="EmployeeID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnCloseShortageDetails" OnClick="btnCloseShortageDetails_Click" runat="server" CssClass="btn btn-default">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCloseShortageDetails" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

</asp:Content>
