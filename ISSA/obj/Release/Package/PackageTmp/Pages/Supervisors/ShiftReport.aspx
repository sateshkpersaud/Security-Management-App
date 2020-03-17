<%@ Page Title="Shift Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ShiftReport.aspx.cs" Inherits="ISSA.Pages.Supervisors.ShiftReport" MaintainScrollPositionOnPostback="true" %>

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
            <h4>DAILY CHECK SHEET / SHIFT REPORT
                    <br />
                <small>Use the form below to manage shift reports.
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
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbSearchDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlSearchIncidentType">Shift</label>
                                    <asp:DropDownList ID="ddlSearchShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlSearchArea">Area</label>
                                    <asp:DropDownList ID="ddlSearchArea" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <%--<div class="col-sm-2 col-xs-12">
                                    <label class="control-label" for="ddlSearchLocation">Location</label>
                                    <asp:DropDownList ID="ddlSearchLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>--%>
                                <div class="col-sm-4 col-xs-12">
                                    <label class="control-label" for="ddlSearchLocation">Supervisor</label>
                                    <asp:DropDownList ID="ddlSearchSupervisor" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6 col-xs-6">
                            <asp:LinkButton ID="btnNew" runat="server" CssClass="btn btn-primary" OnClick="btnNew_Click">New Shift Report <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6 col-xs-6 text-right">
                            <asp:LinkButton ID="btnSearchResults" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchResults_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                        </div>
                    </div>
                    <br />
                    <div class="row pull-left">
                        <div class="col-sm-12 col-xs-12">
                            <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVResultsHeader" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12">
                        <asp:GridView ID="gvResults"
                            runat="server"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="30"
                            AllowSorting="false"
                            ShowFooter="true"
                            ShowHeaderWhenEmpty="true"
                            CssClass="table table-bordered table-striped table-hover"
                            HeaderStyle-CssClass="text-center"
                            OnPageIndexChanging="gvResults_PageIndexChanging"
                            OnSelectedIndexChanged="gvResults_SelectedIndexChanged"
                            OnRowDataBound="gvResults_RowDataBound">
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
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%#Eval("Date")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Area">
                                    <ItemTemplate>
                                        <asp:Label ID="lblArea" runat="server" Text='<%#Eval("Area")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Supervisor">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shift">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShift" runat="server" Text='<%#Eval("shift")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Checks">
                                    <ItemTemplate>
                                        <asp:Label ID="lblChecks" runat="server" Text='<%#Eval("Checks")%>'></asp:Label>
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
                                        <asp:Label ID="lblShiftReportID" runat="server" Text='<%#Eval("ShiftReportID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAreaID" runat="server" Text='<%#Eval("AreaID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSupervisorID" runat="server" Text='<%#Eval("SupervisorID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlDetails" runat="server" HorizontalAlign="Left">
                <div class="panel panel-default" id="div1">
                    <div id="Div2" class="panel-heading" runat="server">
                        <asp:Label ID="lblHeader" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="panel-body">
                        <div class="container-fluid">
                              <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch2" runat="server" CssClass="btn btn-info" OnClick="btnBackToSearch_Click" >Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear2" runat="server" CssClass="btn btn-default" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave2" runat="server" ValidationGroup="vgShiftReport" CssClass="btn btn-primary" OnClick="btnSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext2" runat="server" ValidationGroup="vgShiftReport" CssClass="btn btn-primary" OnClick="btnSaveAndNext_Click" >Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
                                    <label class="control-label" for="tbDate">Date</label>
                                    <asp:ImageButton ID="ibDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbDate" runat="server" PopupButtonID="ibDate" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tbDate" runat="server" ValidationGroup="vgShiftReport" ErrorMessage="Date is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlArea">Area </label>
                                    <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlArea" runat="server" ValidationGroup="vgShiftReport" ErrorMessage="Area is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlShift">Shift </label>
                                    <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="ddlShift" runat="server" ValidationGroup="vgShiftReport" ErrorMessage="Shift is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="ddlLocation">Supervisor</label>
                                    <asp:DropDownList ID="ddlSupervisor" CssClass="form-control" ValidationGroup="vgShiftReport" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvLocation" runat="server" ErrorMessage="supervisor is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlSupervisor" ValidationGroup="vgShiftReport"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 pull-left col-xs-12">
                                    <label class="control-label">Checks</label>
                                    <asp:LinkButton ID="btnNewChecks" runat="server" CssClass="btn btn-default" OnClick="btnNewChecks_Click" ValidationGroup="vgShiftReport">New <span aria-hidden="true" class="glyphicon glyphicon-check"></span></asp:LinkButton>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <asp:GridView
                                        ID="gvChecks"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        OnPageIndexChanging="gvChecks_PageIndexChanging"
                                        OnRowDataBound="gvChecks_RowDataBound"
                                        OnSelectedIndexChanged="gvChecks_SelectedIndexChanged"
                                        HeaderStyle-CssClass="text-center">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Location">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbLocation" runat="server" CommandName="Select">
                                                        <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Seq." ItemStyle-Width="50px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSequence" runat="server" Text='<%#Eval("CheckSequence")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField HeaderText="Amt" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo2" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField HeaderText="Officer & Time">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOfficerName" runat="server" Text='<%#Eval("officerName")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%-- <asp:TemplateField HeaderText="Time">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTime" runat="server" Text='<%#Eval("timeClocked")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField HeaderText="General Observations" ItemStyle-Width="250px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblShortGeneralObservations" runat="server" Text='<%#Eval("GeneralObservations")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Corrections Made/Suggested" ItemStyle-Width="250px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblShortCorrectionsMage" runat="server" Text='<%#Eval("CorrectionsMadeSuggested")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblChecksID" runat="server" Text='<%#Eval("ChecksID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLocationID" runat="server" Text='<%#Eval("LocationID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGeneralObservations" runat="server" Text='<%#Eval("GeneralObservations")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCorrectionsMadeSuggested" runat="server" Text='<%#Eval("CorrectionsMadeSuggested")%>'></asp:Label>
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
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <hr />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgShiftReport" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgShiftReport" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
        <Triggers>
            <asp:PostBackTrigger ControlID="btnNewChecks" />
             <asp:PostBackTrigger ControlID="gvChecks" />
        </Triggers>
    </asp:UpdatePanel>

    <%-- Checks pop up --%>
    <%--    <asp:Button ID="btnShowPopup2" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup2" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeChecks" DropShadow="True" runat="server" PopupControlID="pnlChecks" TargetControlID="btnShowPopup2" CancelControlID="btnHidePopup2"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlChecks" runat="server" Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header bg-primary">Checks</div>
                        <div class="modal-body" style="height:500px; overflow: auto">
                            <div class="container-fluid">
                                <div class="row">
                                     <div class="col-md-2 col-xs-2">
                                       <label class="control-label" for="ddlLocation">Location</label>
                                     </div>
                                   <div class="col-md-6 col-xs-6">
                                    <asp:DropDownList ID="ddlLocation" CssClass="form-control" ValidationGroup="vgChecks" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Location is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlLocation" ValidationGroup="vgChecks"></asp:RequiredFieldValidator>
                                       </div>
                                    <div class="col-md-4 col-xs-4">
                                        <label class="control-label" for="lblSequence">Sequence </label>
                                        <asp:Label ID="lblSequence" runat="server" Text="Label"></asp:Label>
                                    </div>
                                </div>
                                <br />
                                   <div class="row">
                                <div class="col-sm-12 pull-left col-xs-12">
                                    <label class="control-label">Officers</label>
                                    <asp:LinkButton ID="btnNewOfficers" runat="server" CssClass="btn btn-default" OnClick="btnNewOfficers_Click">Add <span aria-hidden="true" class="glyphicon glyphicon-user"></span></asp:LinkButton>
                                </div>
                            </div>
                                <br>
                                <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <asp:GridView
                                        ID="gvOfficers"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="false"
                                        AllowSorting="false"
                                        ShowFooter="false"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowDeleting="gvOfficers_RowDeleting"
                                        OnRowDataBound="gvOfficers_RowDataBound">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Officer Name">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlOfficerName" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Time" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                    <asp:TextBox CssClass="form-control" ID="tbTime" runat="server" TextMode="Time" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                           <asp:TemplateField ItemStyle-Width="35px">
                                <ItemTemplate >
                                    <asp:ImageButton ID="btnRemove" runat="server" CommandName="Delete" ImageUrl="~/Images/Delete.gif"
                                        ToolTip="Remove Row" CausesValidation="false" Height="20px" Width="20px" BorderStyle="None" CssClass="delete" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                       <label class="control-label" for="tbGeneralObservations">General Observations</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgChecks" MaxLength="5000" placeholder="5000 characters max" ID="tbGeneralObservations" runat="server" TextMode="MultiLine" Height="60px" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbGeneralObservations" ValidationGroup="vgChecks"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                       <label class="control-label" for="tbCorrectionsMade">Corrections Made / Suggested</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgChecks" MaxLength="5000" placeholder="5000 characters max" ID="tbCorrectionsMade" runat="server" TextMode="MultiLine" Height="60px" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbCorrectionsMade" ValidationGroup="vgChecks"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <br />
                                <div class="modal-footer">
                                    <asp:LinkButton ID="btnCancelOfficer" runat="server" CssClass="btn btn-default" OnClick="btnCancelOfficer_Click" >Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnAddCloseOfficer" runat="server" ValidationGroup="vgChecks" CssClass="btn btn-primary" OnClick="btnAddCloseOfficer_Click">Add & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnAddNextOfficer" runat="server" ValidationGroup="vgChecks" CssClass="btn btn-primary" OnClick="btnAddNextOfficer_Click">Add & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
            </asp:Panel>--%>

    <div class="modal fade" id="checksModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblEditHeader" runat="server" Text="Checks"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                                <div class="container-fluid">
                                    <div class="row">
                                        <div class="col-md-2 col-xs-2">
                                            <label class="control-label" for="ddlLocation">Location</label>
                                        </div>
                                        <div class="col-md-6 col-xs-6">
                                            <asp:DropDownList ID="ddlLocation" CssClass="form-control" ValidationGroup="vgChecks" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Location is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlLocation" ValidationGroup="vgChecks"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="col-md-4 col-xs-4">
                                            <label class="control-label" for="lblSequence">Sequence </label>
                                            <asp:Label ID="lblSequence" runat="server" Text="Label"></asp:Label>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-sm-12 pull-left col-xs-12">
                                            <label class="control-label">Officers</label>
                                            <asp:LinkButton ID="btnNewOfficers" runat="server" CssClass="btn btn-default" OnClick="btnNewOfficers_Click">Add <span aria-hidden="true" class="glyphicon glyphicon-user"></span></asp:LinkButton>
                                        </div>
                                    </div>
                                    <br>
                                    <div class="row">
                                        <div class="col-md-12 col-xs-12">
                                            <asp:GridView
                                                ID="gvOfficers"
                                                runat="server"
                                                AutoGenerateColumns="false"
                                                AllowPaging="false"
                                                AllowSorting="false"
                                                ShowFooter="false"
                                                ShowHeaderWhenEmpty="true"
                                                CssClass="table table-bordered table-striped  table-hover"
                                                HeaderStyle-CssClass="text-center"
                                                OnRowDeleting="gvOfficers_RowDeleting"
                                                OnRowDataBound="gvOfficers_RowDataBound">
                                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Officer Name">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddlOfficerName" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time" ItemStyle-Width="150px">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="form-control" ID="tbTime" runat="server" placeholder="24 hrs format"/>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="35px">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnRemove" runat="server" CommandName="Delete" ImageUrl="~/Images/Delete.gif"
                                                                ToolTip="Remove Row" CausesValidation="false" Height="20px" Width="20px" BorderStyle="None" CssClass="delete" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-xs-12">
                                            <label class="control-label" for="tbGeneralObservations">General Observations</label>
                                            <asp:TextBox CssClass="form-control" ValidationGroup="vgChecks" MaxLength="5000" placeholder="5000 characters max" ID="tbGeneralObservations" runat="server" TextMode="MultiLine" Height="60px" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbGeneralObservations" ValidationGroup="vgChecks"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-md-12 col-xs-12">
                                            <label class="control-label" for="tbCorrectionsMade">Corrections Made / Suggested</label>
                                            <asp:TextBox CssClass="form-control" ValidationGroup="vgChecks" MaxLength="5000" placeholder="5000 characters max" ID="tbCorrectionsMade" runat="server" TextMode="MultiLine" Height="60px" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbCorrectionsMade" ValidationGroup="vgChecks"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCancelOfficer" runat="server" CssClass="btn btn-default" OnClick="btnCancelOfficer_Click">Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnAddCloseOfficer" runat="server" ValidationGroup="vgChecks" CssClass="btn btn-primary" OnClick="btnAddCloseOfficer_Click">Add & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnAddNextOfficer" runat="server" ValidationGroup="vgChecks" CssClass="btn btn-primary" OnClick="btnAddNextOfficer_Click">Add & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
