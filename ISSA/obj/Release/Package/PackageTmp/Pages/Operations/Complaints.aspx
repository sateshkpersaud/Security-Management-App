<%@ Page Title="Complaints" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Complaints.aspx.cs" Inherits="ISSA.Pages.Operations.Complaints" MaintainScrollPositionOnPostback="true" %>

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
            <h4>COMPLAINT REPORTS
                    <br />
                <small>Use the form below to manage complaints.
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
                                    <label class="control-label" for="tbSearchDate">Date of Inc.</label>
                                    <asp:ImageButton ID="ibSearchDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbSearchDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>

                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbSearchDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlSearchIncidentType">Mode</label>
                                    <asp:DropDownList ID="ddlSearchMode" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlSearchArea">Area</label>
                                    <asp:DropDownList ID="ddlSearchArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchArea_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-sm-3 col-xs-12">
                                    <label class="control-label" for="ddlSearchLocation">Location</label>
                                    <asp:DropDownList ID="ddlSearchLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6 col-xs-12">
                            <asp:LinkButton ID="btnNewComplaint" runat="server" CssClass="btn btn-primary" Text="New Complaint" OnClick="btnNewComplaint_Click">New Complaint <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6 col-xs-12 text-right">
                            <asp:LinkButton ID="btnSearchComplaint" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchComplaint_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                        </div>
                    </div>
                    <br />
                    <div class="row pull-left">
                        <div class="col-sm-12 col-xs-12">
                            <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVComplaintsHeader" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12 col-xs-12">
                            <asp:GridView ID="gvComplaints"
                                runat="server"
                                AutoGenerateColumns="false"
                                AllowPaging="true" PageSize="30"
                                AllowSorting="false"
                                ShowFooter="true"
                                ShowHeaderWhenEmpty="true"
                                CssClass="table table-bordered table-striped  table-hover"
                                HeaderStyle-CssClass="text-center"
                                OnRowDataBound="gvComplaints_RowDataBound"
                                OnPageIndexChanging="gvComplaints_PageIndexChanging"
                                OnSelectedIndexChanged="gvComplaints_SelectedIndexChanged">
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
                                                <asp:Label ID="lblReportNumber" runat="server" Text='<%#Eval("ComplaintNumber")%>'></asp:Label>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Location">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Date of Incident">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDateofComplaint" runat="server" Text='<%#Eval("DateOfIncident")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTime" runat="server" Text='<%#Eval("TimeOfIncident")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComplaintsMode" runat="server" Text='<%#Eval("Mode")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Person Making Report">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPersonMakingReport" runat="server" Text='<%#Eval("PersonMakingReport")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Received">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDateReceived" runat="server" Text='<%#Eval("DateReceived")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Received By">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Feedback Provided">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFeedbackProvided" runat="server" Text='<%#Eval("feedbackProvided")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ClientComplaint">
                                        <ItemTemplate>
                                            <asp:Label ID="lblShortClientComplaint" runat="server" Text='<%#Eval("ClientComplaint")%>'></asp:Label>
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
                                            <asp:Label ID="lblComplaintID" runat="server" Text='<%#Eval("ComplaintID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAreaID" runat="server" Text='<%#Eval("AreaID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocationID" runat="server" Text='<%#Eval("LocationID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComplaintChannelID" runat="server" Text='<%#Eval("ComplaintChannelID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblClientComplaint" runat="server" Text='<%#Eval("ClientComplaint")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblReceivedByID" runat="server" Text='<%#Eval("ReceivedByID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcknowledgementID" runat="server" Text='<%#Eval("AcknowledgementID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDateOfFeedback" runat="server" Text='<%#Eval("DateOfFeedback")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblActionTaken" runat="server" Text='<%#Eval("ActionTaken")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblContactNumber" runat="server" Text='<%#Eval("ContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlDetails" runat="server" HorizontalAlign="Left">
                <%-- <h4>
                    
                    <br />
                    <small>Use the form below to add or update details for a complaint report.
                    </small>
                </h4>
                <br />--%>
                <div class="panel panel-default" id="div1">
                    <div class="panel-heading" runat="server">
                        <asp:Label ID="lblHeader" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="panel-body">
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear2" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave2" runat="server" ValidationGroup="vgComplaints" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext2" runat="server" ValidationGroup="vgComplaints" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlComplaintMode">Mode*</label>
                                    <asp:DropDownList ID="ddlComplaintMode" CssClass="form-control" ValidationGroup="vgComplaints" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Complaint Mode is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlComplaintMode" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbDate">Date of Inc.*</label>
                                    <asp:ImageButton ID="ibCalendar" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgComplaints" ID="tbDate" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="ceDate" TargetControlID="tbDate" runat="server" PopupButtonID="ibCalendar" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ErrorMessage="Date of Inc. is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbDate" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbTime">Time of Inc.*</label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgComplaints" ID="tbTime" runat="server" placeholder="24 hrs format" />
                                    <asp:RequiredFieldValidator ID="rfvTime" runat="server" ErrorMessage="Time of Inc. is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbtime" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlArea">Area*</label>
                                    <asp:DropDownList ID="ddlArea" CssClass="form-control" ValidationGroup="vgComplaints" runat="server" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvArea" runat="server" ErrorMessage="Area is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlArea" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-12">
                                    <label class="control-label" for="ddlLocation">Location*</label>
                                    <asp:DropDownList ID="ddlLocation" CssClass="form-control" ValidationGroup="vgComplaints" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvLocation" runat="server" ErrorMessage="Location is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlLocation" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12">
                                    <label class="control-label" for="tbPersonMakingReport">Person making report*</label>
                                    <asp:TextBox CssClass="form-control" ID="tbPersonMakingReport" ValidationGroup="vgComplaints" runat="server" MaxLength="100" placeholder="100 characters max"/>
                                    <asp:RequiredFieldValidator ControlToValidate="tbPersonMakingReport" ID="RequiredFieldValidator2" runat="server" ErrorMessage="Person making report is required" ForeColor="Red" Display="Dynamic" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbContactNumber">Contact Number</label>
                                    <asp:TextBox CssClass="form-control" ID="tbContactNumber" ValidationGroup="vgComplaints" runat="server" TextMode="Phone" Placeholder='eg. "999-999-9999"' />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ForeColor="Red" runat="server" ErrorMessage="The phone number is not in the correct format." ControlToValidate="tbContactNumber" ValidationGroup="vgComplaints" Display="Dynamic" ValidationExpression="^\d{3}-\d{3}-\d{4}$" SetFocusOnError="True"></asp:RegularExpressionValidator>
                                </div>
                                 <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlShift">Shift*</label>
                                    <asp:DropDownList ID="ddlShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="ddlShift" runat="server" ErrorMessage="Shift is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12">
                                    <label class="control-label" for="ddlReceivedBy">Received by*</label>
                                    <asp:DropDownList ID="ddlReceivedBy" CssClass="form-control" ValidationGroup="vgComplaints" runat="server" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlReceivedBy" ID="RequiredFieldValidator3" runat="server" ErrorMessage="Received by is required" ForeColor="Red" Display="Dynamic" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <label class="control-label" for="tbDateReceived">Date Received* </label>
                                    <asp:ImageButton ID="ibCalendarDateReceived" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgComplaints" ID="tbDateReceived" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbDateReceived" runat="server" PopupButtonID="ibCalendarDateReceived" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Date received is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbDateReceived" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlAcknowledgement">Acknowledgement</label>
                                    <asp:DropDownList ID="ddlAcknowledgement" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbClientcomplaint">Client Complaint* </label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgComplaints" MaxLength="5000" Height="100px" ID="tbClientcomplaint" runat="server" TextMode="MultiLine" placeholder="5000 characters max" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Client Complaint is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbClientcomplaint" ValidationGroup="vgComplaints"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12">
                                    <label class="control-label">Was feedback provided to client</label>
                                    <br />
                                    <asp:RadioButton ID="rbYes" CssClass="radio-inline" GroupName="Feedback" runat="server" Text="YES" />
                                    <asp:RadioButton ID="rbNo" CssClass="radio-inline" GroupName="Feedback" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <label class="control-label" for="tbDateClientContacted">Date Contacted </label>
                                    <asp:ImageButton ID="ibCalendar3" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgComplaints" ID="tbDateClientContacted" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="tbDateClientContacted" runat="server" PopupButtonID="ibCalendar3" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbActionTaken">Action taken</label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgComplaints" MaxLength="5000" Height="100px" ID="tbActionTaken" runat="server" TextMode="MultiLine" placeholder="5000 characters max" />
                                </div>
                            </div>
                            <br />
                            <hr />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch2" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgComplaints" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgComplaints" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
