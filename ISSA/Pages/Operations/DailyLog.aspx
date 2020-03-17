<%@ Page Title="Daily Log" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DailyLog.aspx.cs" Inherits="ISSA.Pages.Operations.DailyLog" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function noFutureOrPastDate(sender, args) {
            if (sender._selectedDate > new Date()) {
                ShowMessage("You cannot select a future date", "Warning");
                sender._selectedDate = new Date();
                // set the date back to the current date
                sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
            //else if (sender._selectedDate < new Date()) {
            //    ShowMessage("You cannot select a past date", "Warning");
            //    sender._selectedDate = new Date();
            //    // set the date back to the current date
            //    sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            //}
        }

        function noFutureDate(sender, args) {
            if (sender._selectedDate > new Date()) {
                ShowMessage("You cannot select a future date", "Warning");
                sender._selectedDate = new Date();
                // set the date back to the current date
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
            <h4>DAILY LOG
                    <br />
                <small>Use the form below to manage daily logs.
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
                            <asp:LinkButton ID="btnNew" runat="server" CssClass="btn btn-primary" Text="New Incident" OnClick="btnNew_Click">New Daily Log <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
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
                                        <asp:LinkButton ID="lbLogNumber" runat="server" CommandName="Select">
                                            <asp:Label ID="lblLogNumber" runat="server" Text='<%#Eval("LogNumber")%>'></asp:Label>
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
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shift">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShift" runat="server" Text='<%#Eval("shift")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Present" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPersonsPresent" runat="server" Text='<%#Eval("PersonsPresent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Absent" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPersonsAbsent" runat="server" Text='<%#Eval("PersonsAbsent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Switches"  ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSwitches" runat="server" Text='<%#Eval("Switches")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Doubles" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDoubles" runat="server" Text='<%#Eval("Doubles")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Calls Made" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCallsMade" runat="server" Text='<%#Eval("CallsMade")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Last Check Time" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLastCheckTime" runat="server" Text='<%#Eval("LastCheckTime")%>'></asp:Label>
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
                                        <asp:Label ID="lblDailyLogID" runat="server" Text='<%#Eval("DailyLogID")%>'></asp:Label>
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
                                        <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
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
                                    <asp:LinkButton ID="btnBackToSearch2" runat="server" CssClass="btn btn-info" Text="Back To Search" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear2" runat="server" CssClass="btn btn-default" Text="Clear Form" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave2" runat="server" ValidationGroup="vgDailyLog" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext2" runat="server" ValidationGroup="vgDailyLog" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
                                    <asp:TextBox ID="tbDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" Enabled="false" OnTextChanged="tbDate_TextChanged"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbDate" runat="server" PopupButtonID="ibDate" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tbDate" runat="server" ValidationGroup="vgDailyLog" ErrorMessage="Date is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlArea">Area </label>
                                    <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlArea" runat="server" ValidationGroup="vgDailyLog" ErrorMessage="Area is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="ddlLocation">Location</label>
                                    <asp:DropDownList ID="ddlLocation" CssClass="form-control" ValidationGroup="vgDailyLog" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvLocation" runat="server" ErrorMessage="Location is required" ForeColor="Red" Display="Dynamic" ControlToValidate="ddlLocation" ValidationGroup="vgDailyLog"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlShift">Shift </label>
                                    <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="ddlShift" runat="server" ValidationGroup="vgDailyLog" ErrorMessage="Shift is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            </div>
                        <asp:Panel ID="pnlDLDetails" runat="server" HorizontalAlign="Left">
                            <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-6 col-xs-6">
                                    <label class="control-label" for="lblLastAreaCheckTime">Last check location: </label>
                                    <asp:Label ID="lblLastCheckLocation" runat="server" Text="Label"></asp:Label>
                                </div>
                                <div class="col-sm-6 col-xs-6 text-right">
                                    <label class="control-label" for="lblLastAreaCheckTime">Last check time for area: </label>
                                    <asp:Label ID="lblLastAreaCheckTime" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="gvPersonWorking">Persons Working</label>
                                    <%--<asp:LinkButton ID="btnNewPersonWorking" runat="server" CssClass="btn"> <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>--%>
                                    <div style="height: 200px; overflow: auto; border-style: solid; border-width: thin; border-color: darkgray">
                                        <asp:GridView
                                            ID="gvPersonWorking"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false" ShowHeader="false"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            OnRowDataBound="gvPersonWorking_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="15px">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbPresent" runat="server" OnCheckedChanged="cbPresent_CheckedChanged" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPersonWorkingID" runat="server" Text='<%#Eval("PersonWorkingID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblisStandBy" runat="server" Text='<%#Eval("isStandBy")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIsDayOff" runat="server" Text='<%#Eval("IsDayOff")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIsPresent" runat="server" Text='<%#Eval("IsPresent")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="gvSwitches">Switches </label>
                                    <div style="height: 200px; overflow: auto; border-style: solid; border-width: thin; border-color: darkgray">
                                        <asp:GridView
                                            ID="gvSwitches"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false" ShowHeader="false"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            OnRowDataBound="gvSwitches_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="15px">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbPresent" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblisStandBy" runat="server" Text='<%#Eval("isStandBy")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIsDayOff" runat="server" Text='<%#Eval("IsDayOff")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSwitchID" runat="server" Text='<%#Eval("SwitchID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIsChecked" runat="server" Text='<%#Eval("IsChecked")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <asp:LinkButton ID="btnNewDouble" runat="server" Font-Bold="true" OnClick="btnNewDouble_Click" Text="Double Shifts"> </asp:LinkButton>
                                    <label class="control-label" for="gvDoubles">&nbsp; </label>
                                    <div style="height: 200px; overflow: auto; border-style: solid; border-width: thin; border-color: darkgray">
                                        <asp:GridView
                                            ID="gvDoubles"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false" ShowHeader="false"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            OnRowDataBound="gvDoubles_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="15px">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbPresent" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDoubleID" runat="server" Text='<%#Eval("DoubleID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 pull-left col-xs-12">
                                    <label class="control-label">Call Log</label>
                                    <asp:LinkButton ID="btnNewCallLog" runat="server" CssClass="btn btn-default" OnClick="btnNewCallLog_Click">New <span aria-hidden="true" class="glyphicon glyphicon-phone"></span></asp:LinkButton>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <asp:GridView
                                        ID="gvCallLog"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="5"
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
                                            <asp:TemplateField HeaderText="Time" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbTime" runat="server" CommandName="Select">
                                                        <asp:Label ID="lblTime" runat="server" Text='<%#Eval("TimeOfCall")%>'></asp:Label>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Reports">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReports" runat="server" Text='<%#Eval("Reports")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Sup. Check Location" ItemStyle-Width="180px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSupCheck" runat="server" Text='<%#Eval("SupCheckLocation")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCallLogID" runat="server" Text='<%#Eval("CallLogID")%>'></asp:Label>
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
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgDailyLog" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgDailyLog" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12 text-right">
                                    <asp:Label ID="lblAuditTrail" runat="server" Font-Size="X-Small" ForeColor="DarkGray" Text=""></asp:Label>
                                </div>
                            </div>
                        </div>
                        </asp:Panel>
                    </div>
                </div>
            </asp:Panel>

            
            <%-- New Call Log pop up --%>
            <%-- <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeCallLog" DropShadow="True" runat="server" PopupControlID="pnlCallLog" TargetControlID="btnShowPopup" CancelControlID="btnHidePopup"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlCallLog" runat="server" Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header bg-primary">Call Log</div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-4 col-xs-6">
                                        <label class="control-label">Time</label>
                                        <asp:Label ID="lblTime" runat="server" Text=""></asp:Label>
                                        <asp:Label ID="lblSelectedCallLogID" runat="server" Text="" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-md-8 col-xs-6">
                                        <label class="control-label">Supervisor check Location</label>
                                        <asp:RadioButton ID="rbYes" CssClass="radio-inline" GroupName="SUpCheck" runat="server" Text="YES" Checked="true" />
                                        <asp:RadioButton ID="rbNo" CssClass="radio-inline" GroupName="SUpCheck" runat="server" Text="NO" Checked="false" />
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label">Supervisor last check time at this location: </label>
                                        <asp:Label ID="lblLastCheckTimeAtThisLocation" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label">Reports</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgCallLog" MaxLength="5000" ID="tbReports" runat="server" TextMode="MultiLine" Height="100px" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Reports is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbReports" ValidationGroup="vgCallLog"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="modal-footer">
                                <asp:LinkButton ID="btnCancelCallLog" runat="server" CssClass="btn btn-default" OnClick="btnCancelCallLog_Click">Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnSaveCloseCallLog" runat="server" ValidationGroup="vgCallLog" CssClass="btn btn-primary" OnClick="btnSaveCloseCallLog_Click">Save & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnSaveNextCallLog" runat="server" ValidationGroup="vgCallLog" CssClass="btn btn-primary" OnClick="btnSaveNextCallLog_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>--%>


            <%-- doubles pop up --%>
          <%--  <asp:Button ID="btnShowPopup2" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup2" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeDoubles" DropShadow="True" runat="server" PopupControlID="pnlDouble" TargetControlID="btnShowPopup2" CancelControlID="btnHidePopup2"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlDouble" runat="server" Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header bg-primary">Add New Double</div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-3 col-xs-4 text-right">
                                        <label class="control-label" for="">Search for </label>
                                    </div>
                                    <div class="col-md-3 col-xs-4">
                                        <asp:RadioButton ID="rbPreviousShifts" runat="server" GroupName="Doubles" />
                                        Previous Shift
                                    </div>
                                    <div class="col-md-6 col-xs-4">
                                        <asp:RadioButton ID="rbAnyEmployee" runat="server" GroupName="Doubles" />
                                        Any Employee
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-3 col-xs-3 text-right">
                                        <label class="control-label" for="tbName">Name</label>
                                    </div>
                                    <div class="col-md-6 col-xs-6">
                                        <asp:TextBox ID="tbName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3 col-xs-3">
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default" OnClick="btnSearch_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 200px; overflow: auto">
                                        <asp:GridView
                                            ID="gvDoublesFound"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false" ShowHeader="true"
                                            CssClass="table table-bordered table-striped  table-hover">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="15px">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbSelect" runat="server" />
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
                                                <asp:TemplateField HeaderText="Location">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLocationName" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Shift">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShiftWorked" runat="server" Text='<%#Eval("shiftTime")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                    </div>
                                </div>
                                <br />
                                <div class="modal-footer">
                                    <asp:LinkButton ID="btnCancelDouble" runat="server" CssClass="btn btn-default" OnClick="btnCancelDouble_Click">Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnAddCloseDouble" runat="server" ValidationGroup="vgDouble" CssClass="btn btn-primary" OnClick="btnAddCloseDouble_Click">Add & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnAddNextDouble" runat="server" ValidationGroup="vgDouble" CssClass="btn btn-primary" OnClick="btnAddNextDouble_Click">Add & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
            </asp:Panel>--%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnNewCallLog" />
            <asp:PostBackTrigger ControlID="gvCallLog" />
            <asp:PostBackTrigger ControlID="btnNewDouble" />
            <asp:PostBackTrigger ControlID="gvDoubles" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="callLogModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblEditHeader" runat="server" Text="Call Log"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-2 col-xs-6">
                                        <label class="control-label">Time</label>
                                        <asp:Label ID="lblTime" runat="server" Text=""></asp:Label>
                                        <asp:Label ID="lblSelectedCallLogID" runat="server" Text="" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-md-5 col-xs-6">
                                        <label class="control-label">Sup. check location</label>
                                        <asp:RadioButton ID="rbYes" CssClass="radio-inline" GroupName="SUpCheck" runat="server" Text="YES" Checked="false" />
                                        <asp:RadioButton ID="rbNo" CssClass="radio-inline" GroupName="SUpCheck" runat="server" Text="NO" Checked="true" />
                                    </div>
                                    <div class="col-md-5 col-xs-12">
                                        <label class="control-label">Sup. last check time at this location: </label>
                                        <asp:Label ID="lblLastCheckTimeAtThisLocation" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label">Reports</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgCallLog" MaxLength="5000" ID="tbReports" runat="server" TextMode="MultiLine" Height="100px" placeholder="5000 characters max" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Reports is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbReports" ValidationGroup="vgCallLog"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCancelCallLog" runat="server" CssClass="btn btn-default" OnClick="btnCancelCallLog_Click">Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnSaveCloseCallLog" runat="server" ValidationGroup="vgCallLog" CssClass="btn btn-primary" OnClick="btnSaveCloseCallLog_Click">Save & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnSaveNextCallLog" runat="server" ValidationGroup="vgCallLog" CssClass="btn btn-primary" OnClick="btnSaveNextCallLog_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

     <div class="modal fade" id="doublesModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="Label1" runat="server" Text="Add Double Shift Employee"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-4 col-xs-6">
                                        <label class="control-label" for="">Search for </label> <br />
                                        <asp:RadioButton ID="rbPreviousShifts" runat="server" GroupName="Doubles" />
                                        Previous Shift
                                        <asp:RadioButton ID="rbAnyEmployee" runat="server" GroupName="Doubles" />
                                        Any Employee
                                    </div>
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label" for="tbName">Name</label>
                                        <asp:TextBox ID="tbName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-2 col-xs-6">
                                        <label class="control-label" for="btnSearch">&nbsp;</label> <br />
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default" OnClick="btnSearch_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height: 400px; overflow: auto">
                                        <asp:GridView
                                            ID="gvDoublesFound"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false" ShowHeader="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            OnRowDataBound="gvDoublesFound_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="15px">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbSelect" runat="server" />
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
                                                <asp:TemplateField HeaderText="Location">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLocationName" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Shift">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShiftWorked" runat="server" Text='<%#Eval("shiftTime")%>'></asp:Label>
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
                                    <asp:LinkButton ID="btnCancelDouble" runat="server" CssClass="btn btn-default" OnClick="btnCancelDouble_Click">Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnAddCloseDouble" runat="server" ValidationGroup="vgDouble" CssClass="btn btn-primary" OnClick="btnAddCloseDouble_Click">Add & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnAddNextDouble" runat="server" ValidationGroup="vgDouble" CssClass="btn btn-primary" OnClick="btnAddNextDouble_Click">Add & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                </div>
            </div>
        </div>
    </div>
</asp:Content>
