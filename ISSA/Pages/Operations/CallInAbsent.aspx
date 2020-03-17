<%@ Page Title="Absence" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CallInAbsent.aspx.cs" Inherits="ISSA.Pages.Operations.CallInAbsent" MaintainScrollPositionOnPostback="true" %>

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

        function verifySickLeaveFor(Reason, SickLeaveFor) {
            if (Reason.value == "5") {
                if (SickLeaveFor.value == "") {
                    ShowMessage("Sick leave for is required", "Warning");
                    SickLeaveFor.style.borderColor = "red";
                    SickLeaveFor.focus();
                    //This is for firefox as the above focus does not work
                    tempField = SickLeaveFor;
                    setTimeout("tempField.focus();", 1);
                }
                else {
                    SickLeaveFor.style.borderColor = "lightgray";
                }
            }
        }

        function verifyAbsenceReasonOther(Reason, tbOther) {
            if (Reason.value == "6") {
                if (tbOther.value == "") {
                    ShowMessage("Absence Reason other description is required", "Warning");
                    tbOther.style.borderColor = "red";
                    tbOther.focus();
                    //This is for firefox as the above focus does not work
                    tempField = tbOther;
                    setTimeout("tempField.focus();", 1);
                }
                else {
                    tbOther.style.borderColor = "lightgray";
                }
            }
        }

        function verifySickLeaveForOther(SickLeaveFor, tbOther) {
            if (SickLeaveFor.value == "5") {
                if (tbOther.value == "") {
                    ShowMessage("Sick Leave For Other description is required", "Warning");
                    tbOther.style.borderColor = "red";
                    tbOther.focus();
                    //This is for firefox as the above focus does not work
                    tempField = tbOther;
                    setTimeout("tempField.focus();", 1);
                }
                else {
                    tbOther.style.borderColor = "lightgray";
                }
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

        function validateDateFormat2(control, endDate1) {
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
            }
            else {
                control.style.borderColor = "lightgrey";
            }

        }

        function validateTime(control) {
            // regular expression to match required time format
            var errorMsg = "";
            var re = /^(\d{1,2}):(\d{2})(:00)?([ap]m)?$/;
            if (control.value != '') {
                if (regs = control.value.match(re)) {
                    if (regs[4]) {
                        // 12-hour time format with am/pm
                        if (regs[1] < 1 || regs[1] > 12) {
                            ShowMessage("Invalid value for hours: " + regs[1], "Warning");
                            errorMsg = "Yes";
                        }
                    } else {
                        // 24-hour time format
                        if (regs[1] > 23) {
                            ShowMessage("Invalid value for hours: " + regs[1], "Warning");
                            errorMsg = "Yes";
                        }
                    }
                    if (!errorMsg && regs[2] > 59) {
                        ShowMessage("Invalid value for minutes: " + regs[2], "Warning");
                        errorMsg = "Yes";
                    }
                }
                else {
                    ShowMessage("Not a valid time " + control.value + ". Kindly enter a time in the 24 hour format. E.g. 13:00", "Warning");
                    errorMsg = "Yes";
                }
            }

            if (errorMsg == "Yes") {
                control.style.borderColor = "red";
                control.value = "";
                control.focus();
                //This is for firefox as the above focus does not work
                tempField = control;
                setTimeout("tempField.focus();", 1);
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
            <h4>CALL-IN/ABSENT REPORTS
                    <br />
                <small>Use the form below to manage call-ins and absence.
                </small>
            </h4>
            <hr />
            <asp:Panel ID="pnlSearch" runat="server" HorizontalAlign="left">
                <div class="container-fluid">
                    <div class="row">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                Search Parameters
                            </div>
                            <div class="panel-body">
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbSearchReportNumber">Number</label>
                                    <asp:TextBox ID="tbSearchReportNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbSearchDate">Date</label>
                                    <asp:ImageButton ID="ibSearchDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbSearchDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbSearchDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlSearchCallTakenBy">Taken By</label>
                                    <asp:DropDownList ID="ddlSearchCallTakenBy" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlSearchCallForwardedTo">Forwarded To</label>
                                    <asp:DropDownList ID="ddlSearchCallForwardedTo" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlSearchEmployee">Employee</label>
                                    <asp:DropDownList ID="ddlSearchEmployee" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <%--     <div class="col-sm-2">
                            <label class="control-label" for="ddlSearchArea">Area</label>
                            <asp:DropDownList ID="ddlSearchArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchArea_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="col-sm-2">
                            <label class="control-label" for="ddlSearchLocation">Location</label>
                            <asp:DropDownList ID="ddlSearchLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>--%>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6  col-xs-6">
                            <asp:LinkButton ID="btnNewAbsence" runat="server" CssClass="btn btn-primary" Text="New Absence" OnClick="btnNewAbsence_Click">New Absence <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6  text-right  col-xs-6">
                            <asp:LinkButton ID="btnSearchAbsence" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchAbsence_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                        </div>
                    </div>
                    <br />
                    <div class="row pull-left">
                        <div class="col-sm-12 col-xs-12">
                            <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVAbsenceHeader" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12 col-xs-12">
                            <asp:GridView ID="gvAbsence"
                                runat="server"
                                AutoGenerateColumns="false"
                                AllowPaging="true" PageSize="30"
                                AllowSorting="false"
                                ShowFooter="true"
                                ShowHeaderWhenEmpty="true"
                                CssClass="table table-bordered table-striped  table-hover"
                                HeaderStyle-CssClass="text-center"
                                OnRowDataBound="gvAbsence_RowDataBound"
                                OnSelectedIndexChanged="gvAbsence_SelectedIndexChanged"
                                OnPageIndexChanging="gvAbsence_PageIndexChanging">
                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Number">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbReportNumber" runat="server" CommandName="Select">
                                                <asp:Label ID="lblReportNumber" runat="server" Text='<%#Eval("ReportNumber")%>'></asp:Label>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Is Call In">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIsCallIn" runat="server" Text='<%#Eval("isCallIn")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Employee">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEmployee" runat="server" Text='<%#Eval("EmpFullName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date of Call">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%#Eval("Date")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Absent For">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDateRange" runat="server" Text='<%#Eval("DateRange")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTime" runat="server" Text='<%#Eval("Time")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Call Taken By">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCallTakenBy" runat="server" Text='<%#Eval("CallTakenBy")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Forwarded To">
                                        <ItemTemplate>
                                            <asp:Label ID="lblForwardedTo" runat="server" Text='<%#Eval("ForwardedTo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAbsentType" runat="server" Text='<%#Eval("AbsentType")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comments">
                                        <ItemTemplate>
                                            <asp:Label ID="lblShortComments" runat="server" Text='<%#Eval("Comments")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
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
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCallInOffID" runat="server" Text='<%#Eval("CallInOffID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCallTakenByID" runat="server" Text='<%#Eval("CallTakenByID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblForwardedToID" runat="server" Text='<%#Eval("ForwardedToID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWorkSchedule" runat="server" Text='<%#Eval("WorkSchedule")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPhoneNumber" runat="server" Text='<%#Eval("PhoneNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAbsentTypeID" runat="server" Text='<%#Eval("AbsentTypeID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSickLeaveForID" runat="server" Text='<%#Eval("SickLeaveForID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComments" runat="server" Text='<%#Eval("Comments")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOtherTypeDescription" runat="server" Text='<%#Eval("OtherTypeDescription")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOtherSickLeaveForTypeDescription" runat="server" Text='<%#Eval("OtherSickLeaveForTypeDescription")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStartDate" runat="server" Text='<%#Eval("StartDate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEndDate" runat="server" Text='<%#Eval("EndDate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlDetails" runat="server" HorizontalAlign="Left">
                <%--<h4>
                    <asp:Label ID="lblHeader" runat="server" Text=""></asp:Label>
                    <br />
                    <small>Use the form below to add or update details for a call-in or absent report.
                    </small>
                </h4>
                <br />--%>
                <div class="panel panel-default" id="div1">
                    <div id="Div2" class="panel-heading" runat="server">
                        <asp:Label ID="lblHeader" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="panel-body">
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch2" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear2" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave2" runat="server" ValidationGroup="vgAbsence" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext2" runat="server" ValidationGroup="vgAbsence" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                </div>
                            </div>
                            <hr />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Label ID="lblReportNumber" runat="server" ForeColor="#003a75" Font-Bold="true" Text="" Font-Size="Medium" Font-Underline="true"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label">Is Call-In </label>
                                    <asp:RadioButton ID="rbYes" CssClass="radio-inline" GroupName="Feedback" runat="server" Text="YES" />
                                    <asp:RadioButton ID="rbNo" CssClass="radio-inline" GroupName="Feedback" runat="server" Text="NO" />
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-6 col-xs-6">
                                    <label class="control-label" for="ddlEmployee">Employee*</label>
                                    <asp:DropDownList ID="ddlEmployee" CssClass="form-control" ValidationGroup="vgAbsence" runat="server" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlEmployee" ID="RequiredFieldValidator1" runat="server" ErrorMessage="Employee is required" ForeColor="Red" Display="Dynamic" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbDate">Date of Call* </label>
                                    <asp:ImageButton ID="ibCalendar" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgAbsence" ID="tbDate" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="ceDate" TargetControlID="tbDate" runat="server" PopupButtonID="ibCalendar" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ErrorMessage="Date is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbDate" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbTime">Time of Call*</label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgAbsence" ID="tbTime" runat="server" placeholder="24 hrs format"/>
                                    <asp:RequiredFieldValidator ID="rfvTime" runat="server" ErrorMessage="Time is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbtime" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                </div>

                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbContactNumber">Contact Number</label>
                                    <asp:TextBox CssClass="form-control" ID="tbContactNumber" ValidationGroup="vgAbsence" runat="server" TextMode="Phone" Placeholder='eg. "999-999-9999"' />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ForeColor="Red" runat="server" ErrorMessage="The phone number is not in the correct format." ControlToValidate="tbContactNumber" ValidationGroup="vgAbsence" Display="Dynamic" ValidationExpression="^\d{3}-\d{3}-\d{4}$" SetFocusOnError="True"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="ddlCallTakenBy">Call Taken By</label>
                                    <asp:DropDownList ID="ddlCallTakenBy" CssClass="form-control" ValidationGroup="vgAbsence" runat="server" AutoPostBack="true"></asp:DropDownList>
                                    <%--<asp:RequiredFieldValidator ID="rfvArea" runat="server" ErrorMessage="Call Taken By is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlCallTakenBy" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="ddlCallForwardedTo">Call Forwarded To</label>
                                    <asp:DropDownList ID="ddlCallForwardedTo" CssClass="form-control" ValidationGroup="vgAbsence" runat="server"></asp:DropDownList>
                                </div>
                                 <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbStartDate">Leave Starts On*</label>
                                    <asp:ImageButton ID="ibStartDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgAbsence" ID="tbStartDate" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbStartDate" runat="server" PopupButtonID="ibStartDate"/>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Start Date is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                 </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbEndDate">Leave Ends On*</label>
                                    <asp:ImageButton ID="ibEndDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgAbsence" ID="tbEndDate" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="tbEndDate" runat="server" PopupButtonID="ibEndDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="End Date is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ValidationGroup="vgAbsence" ID="CompareValidator1" runat="server" ControlToCompare="tbStartDate" ControlToValidate="tbEndDate" ErrorMessage="Leave End On must be after Leave Start On." ForeColor="Red" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                </div>
                               <%-- <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="ddlShift">Shift*</label>
                                    <asp:DropDownList ID="ddlShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                     <asp:RequiredFieldValidator ID="rfvShift" ControlToValidate="ddlShift" runat="server" ErrorMessage="Shift is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                </div>--%>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbWorkSchedule">Work Schedule</label>
                                    <asp:TextBox CssClass="form-control" ID="tbWorkSchedule" ValidationGroup="vgAbsence" runat="server" placeholder="1000 characters max" MaxLength="1000" />
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlAbsenceReason">Reason for absence*</label>
                                    <asp:DropDownList ID="ddlAbsenceReason" CssClass="form-control" ValidationGroup="vgAbsence" OnSelectedIndexChanged="ddlAbsenceReason_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlAbsenceReason" ID="RequiredFieldValidator3" runat="server" ErrorMessage="Reason for absence is required" ForeColor="Red" Display="Dynamic" ValidationGroup="vgAbsence"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbOtherAbsenceReason" id="lblOtherAbsenceReason" runat="server">Other*</label>
                                    <asp:TextBox CssClass="form-control" MaxLength="1000" ID="tbOtherAbsenceReason" runat="server" placeholder="1000 characters max" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" id="lblSickLeaveFor" runat="server" for="ddlSickLeaveFor">Sick leave for*</label>
                                    <asp:DropDownList ID="ddlSickLeaveFor" CssClass="form-control" ValidationGroup="vgAbsence" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSickLeaveFor_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbOtherSickLeaveFor" id="lblOthersickLeaveFor" runat="server">Other*</label>
                                    <asp:TextBox CssClass="form-control" MaxLength="1000" ID="tbOtherSickLeaveFor" runat="server" placeholder="1000 characters max"/>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbComments">Comments</label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgAbsence" MaxLength="5000" Height="100px" ID="tbComments" runat="server" TextMode="MultiLine" placeholder="5000 characters max" />
                                </div>
                            </div>
                            <br />
                            <hr />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgAbsence" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgAbsence" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12 text-right">
                                    <asp:Label ID="lblAuditTrail" runat="server" Font-Size="X-Small" ForeColor="DarkGray" Text=""></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
