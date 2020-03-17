<%@ Page Title="Incidents" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Incidents.aspx.cs" Inherits="ISSA.Pages.Operations.Incidents" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function noFutureDate(sender, args) {
            if (sender._selectedDate > new Date()) {
                ShowMessage("You cannot select a future date", "Warning");
                sender._selectedDate = new Date();
                // set the date back to the current 
                sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
        }

        function verifyIncidentOther(Type, tbOther) {
            if (Type.value == "7") {
                if (tbOther.value == "") {
                    ShowMessage("Other description is required", "Warning");
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

        function verifyPersonAffectedOther(Type, tbOther) {
            if (Type.value == "4") {
                if (tbOther.value == "") {
                    ShowMessage("Other description is required", "Warning");
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

        function openModal() {
            $('#modPersonAffected').modal('show');
        }

        function closeModal() {
            $('#modPersonAffected').modal('hide');
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
            <h4>INCIDENT REPORTS
                    <br />
                <small>Use the form below to manage incidents.
                </small>
            </h4>
            <hr />
            <asp:Panel ID="pnlIncidentSearch" runat="server" HorizontalAlign="left">
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
                                    <label class="control-label" for="ddlSearchIncidentType">Type</label>
                                    <asp:DropDownList ID="ddlSearchIncidentType" CssClass="form-control" runat="server"></asp:DropDownList>
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
                        <div class="col-sm-6 col-xs-6">
                            <asp:LinkButton ID="btnNewIncident" runat="server" CssClass="btn btn-primary" Text="New Incident" OnClick="btnNewIncident_Click">New Incident <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6 col-xs-6 text-right">
                            <asp:LinkButton ID="btnSearchIncident" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchIncident_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                        </div>
                    </div>
                    <br />
                    <div class="row pull-left">
                        <div class="col-sm-12 col-xs-12">
                            <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVIncidentsHeader" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12">
                        <asp:GridView ID="gvIncidents"
                            runat="server"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="30"
                            AllowSorting="false"
                            ShowFooter="true"
                            ShowHeaderWhenEmpty="true"
                            CssClass="table table-bordered table-striped table-hover"
                            HeaderStyle-CssClass="text-center"
                            OnRowDataBound="gvIncidents_RowDataBound"
                            OnPageIndexChanging="gvIncidents_PageIndexChanging"
                            OnSelectedIndexChanged="gvIncidents_SelectedIndexChanged">
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
                                            <asp:Label ID="lblReportNumber" runat="server" Text='<%#Eval("IncidentNumber")%>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date Occured">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateOccured" runat="server" Text='<%#Eval("DateOccured")%>'></asp:Label>
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
                                        <asp:Label ID="lblShortDescription" runat="server" Text='<%#Eval("description")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Action Taken">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShortActionTaken" runat="server" Text='<%#Eval("ActionTaken")%>'></asp:Label>
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
                                        <asp:Label ID="lblIncidentID" runat="server" Text='<%#Eval("IncidentID")%>'></asp:Label>
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
                                        <asp:Label ID="lblIncidentTypeID" runat="server" Text='<%#Eval("IncidentTypeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOtherTypeDescription" runat="server" Text='<%#Eval("OtherTypeDescription")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblActionTaken" runat="server" Text='<%#Eval("ActionTaken")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescription" runat="server" Text='<%#Eval("description")%>'></asp:Label>
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
            </asp:Panel>
            <asp:Panel ID="pnlIncidentDetails" runat="server" HorizontalAlign="Left">
                <%--<h4>
                    <asp:Label ID="lblIncidentHeader" runat="server" Text=""></asp:Label>
                    <br />
                    <small>Use the form below to add or update details for an incident report.
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
                                    <asp:LinkButton ID="btnSave2" runat="server" ValidationGroup="vgIncidents" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext2" runat="server" ValidationGroup="vgIncidents" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
                                    <label class="control-label" for="tbDate">Date*</label><asp:ImageButton ID="ibCalendar" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgIncidents" ID="tbDate" runat="server" placeholder="mm/dd/yyyy" />
                                    <asp:CalendarExtender ID="ceDate" TargetControlID="tbDate" runat="server" PopupButtonID="ibCalendar" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ErrorMessage="Date is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbDate" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbTime">Time*</label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgIncidents" ID="tbTime" runat="server" placeholder="24 hrs format" />
                                    <asp:RequiredFieldValidator ID="rfvTime" runat="server" ErrorMessage="Time is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbtime" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                                 <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlShift">Shift*</label>
                                    <asp:DropDownList ID="ddlShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                     <asp:RequiredFieldValidator ID="rfvShift" ControlToValidate="ddlShift" runat="server" ErrorMessage="Shift is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <label class="control-label" for="ddlArea">Area*</label>
                                    <asp:DropDownList ID="ddlArea" CssClass="form-control" ValidationGroup="vgIncidents" runat="server" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvArea" runat="server" ErrorMessage="Area is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlArea" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-12">
                                    <label class="control-label" for="ddlLocation">Location*</label>
                                    <asp:DropDownList ID="ddlLocation" CssClass="form-control" ValidationGroup="vgIncidents" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvLocation" runat="server" ErrorMessage="Location is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlLocation" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlIncidentType">Type*</label>
                                    <asp:DropDownList ID="ddlIncidentType" CssClass="form-control" ValidationGroup="vgIncidents" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIncidentType_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvIncidentType" runat="server" ErrorMessage="Incident Type is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlIncidentType" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-9 col-xs-12">
                                    <label class="control-label" for="tbOther" id="lblOtherIncidentType" runat="server">Other*</label>
                                    <asp:TextBox CssClass="form-control" MaxLength="500" ID="tbOther" runat="server" placeholder="500 characters max" />
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbDescription">Description*</label>
                                    <asp:TextBox CssClass="form-control" ValidationGroup="vgIncidents" MaxLength="5000" Height="100px" ID="tbDescription" runat="server" TextMode="MultiLine" placeholder="5000 characters max" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Description is required" ForeColor="Red" Display="Dynamic" ControlToValidate="tbDescription" ValidationGroup="vgIncidents"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 pull-left col-xs-12">
                                    <label class="control-label">Person(s) Affected</label>
                                    <asp:LinkButton ID="btnAddNewPersonAffected" runat="server" ValidationGroup="vgIncidents" CssClass="btn btn-default" Text="Add New Person" OnClick="btnAddNewPersonAffected_Click">Add New Person <span aria-hidden="true" class="glyphicon glyphicon-user"></span></asp:LinkButton>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <asp:GridView
                                        ID="gvPersonsAffected"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="10"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnPageIndexChanging="gvPersonsAffected_PageIndexChanging"
                                        OnSelectedIndexChanged="gvPersonsAffected_SelectedIndexChanged"
                                        OnRowDataBound="gvPersonsAffected_RowDataBound">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbName" runat="server" CommandName="Select">
                                                        <asp:Label ID="lblName" runat="server" Text='<%#Eval("fullName")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Phone" ItemStyle-Width="120px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPhoneNumber" runat="server" Text='<%#Eval("phoneNumber")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Address" ItemStyle-Width="200px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAddress" runat="server" Text='<%#Eval("address")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Type" ItemStyle-Width="200px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblType" runat="server" Text='<%#Eval("affectedType")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Description" ItemStyle-Width="200px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%#Eval("description")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPersonAffectedID" runat="server" Text='<%#Eval("personAffectedID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFirstName" runat="server" Text='<%#Eval("firstName")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastName" runat="server" Text='<%#Eval("lastName")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAffectedTypeID" runat="server" Text='<%#Eval("affectedTypeID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOtherPersonAffectedType" runat="server" Text='<%#Eval("otherTypeDescription")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbActionTaken">Action Taken</label>
                                    <asp:TextBox CssClass="form-control" MaxLength="5000" Height="100px" ID="tbActionTaken" runat="server" TextMode="MultiLine" placeholder="5000 characters max" />                                </div>
                            </div>
                            <hr />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgIncidents" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgIncidents" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
            <asp:PostBackTrigger ControlID="btnAddNewPersonAffected" />
             <asp:PostBackTrigger ControlID="gvPersonsAffected" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="personAffectedModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblEditHeader" runat="server" Text="Person Affected"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label">First Name</label>
                                        <asp:TextBox CssClass="form-control" MaxLength="50" ID="tbFirstName" runat="server" placeholder="50 characters max" />
                                    </div>
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label">Last Name*</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgPersonsAffected" MaxLength="50" ID="tbLastName" runat="server" placeholder="50 characters max" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Last Name is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbLastName" ValidationGroup="vgPersonsAffected"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label">Type</label>
                                        <asp:DropDownList ID="ddlPersonAffectedType" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPersonAffectedType_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label" id="lblOtherPersonAffectedType" runat="server">Other*</label>
                                        <asp:TextBox CssClass="form-control" MaxLength="500" ID="tbOtherPersonAffectedType" runat="server" placeholder="500 characters max" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label">Phone</label>
                                        <asp:TextBox CssClass="form-control" MaxLength="100" ID="tbPhone" runat="server" TextMode="Phone" Placeholder='eg. "999-999-9999"' />
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ForeColor="Red" runat="server" ErrorMessage="The phone number is not in the correct format." ControlToValidate="tbPhone" ValidationGroup="vgPersonsAffected" Display="Dynamic" ValidationExpression="^\d{3}-\d{3}-\d{4}$" SetFocusOnError="True"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label">Address</label>
                                        <asp:TextBox CssClass="form-control" MaxLength="1000" ID="tbAddress" runat="server" TextMode="MultiLine" placeholder="1000 characters max" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label">Description*</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgPersonsAffected" MaxLength="5000" ID="tbPersonDescription" runat="server" TextMode="MultiLine" placeholder="5000 characters max"  />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Description is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbPersonDescription" ValidationGroup="vgPersonsAffected"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCancelPersonAffected" runat="server" CssClass="btn btn-default" Text="Cancel" OnClick="btnCancelPersonAffected_Click">Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton> <%-- --%>
                    <asp:LinkButton ID="btnSaveClosePersonAffected" runat="server" ValidationGroup="vgPersonsAffected" CssClass="btn btn-primary" Text="Save & Close" OnClick="btnSaveClosePersonAffected_Click">Save & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnSaveNextPersonAffected" runat="server" ValidationGroup="vgPersonsAffected" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveNextPersonAffected_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                   
                </div>
            </div>
        </div>
    </div>
</asp:Content>
