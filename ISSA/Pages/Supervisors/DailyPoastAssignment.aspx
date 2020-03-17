<%@ Page Title="Daily Post Assignment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DailyPoastAssignment.aspx.cs" Inherits="ISSA.Pages.Supervisors.DailyPoastAssignment" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript">
         function positiveNumber(evt) {
             var charCode = (evt.which) ? evt.which : event.keyCode;
             //if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) { use this to allow period
             if (charCode < 48 || charCode > 57) {
                 return false;
             } else {
                 return true;
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

         function validateMaxNumber(control) {
             if (control.value != "") {
                 if (control.value > 32766) {
                     
                     ShowMessage("Value cannot be larger then 32766.", "Warning");
                     control.value = "0";
                     control.style.borderColor = "red";
                     control.focus();
                     //This is for firefox as the above focus does not work
                     tempField = control;
                     setTimeout("tempField.focus();", 1);
                 }
                 else {
                     control.style.borderColor = "lightgrey";
                 }
             }
         }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h4>DAILY POST ASSIGNMENT ROSTER
                    <br />
                <small>Use the form below to manage daily post assignments.
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
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbSearchReportNumber">Number</label>
                                    <asp:TextBox ID="tbSearchReportNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbSearchDate">Date</label>
                                    <asp:ImageButton ID="ibSearchDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbSearchDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbSearchDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-4 col-xs-12">
                                    <label class="control-label" for="ddlSearchIncidentType">Shift</label>
                                    <asp:DropDownList ID="ddlSearchShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6 col-xs-6">
                            <asp:LinkButton ID="btnNew" runat="server" CssClass="btn btn-primary" OnClick="btnNew_Click">New Post <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6 col-xs-6 text-right">
                            <asp:LinkButton ID="btnSearchResults" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchResults_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" OnClick="lbClearSearch_Click" >Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

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
                            OnPageIndexChanging="gvResults_PageIndexChanging"
                            OnSelectedIndexChanged="gvResults_SelectedIndexChanged"
                            OnRowDataBound="gvResults_RowDataBound"
                            HeaderStyle-CssClass="text-center">
                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Number" ItemStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbReportNumber" runat="server" CommandName="Select">
                                            <asp:Label ID="lblReportNumber" runat="server" Text='<%#Eval("ReportNumber")%>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date" ItemStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%#Eval("Date")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shift" ItemStyle-Width="110px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShift" runat="server" Text='<%#Eval("shift")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Late Arrivals" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLateArrivals" runat="server" Text='<%#Eval("LateArrivals")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Calls for Assis." ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCallsForAssistance" runat="server" Text='<%#Eval("CallsForAssistance")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Assigned" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOfficersAssigned" runat="server" Text='<%#Eval("OfficersAssigned")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Present" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOfficersPresent" runat="server" Text='<%#Eval("OfficersPresent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Absent" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOfficersAbsent" runat="server" Text='<%#Eval("OfficersAbsent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Call Ins" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOfficersCallInForDayOff" runat="server" Text='<%#Eval("OfficersCallInForDayOff")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Call Outs" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOfficersCalledOut" runat="server" Text='<%#Eval("OfficersCalledOut")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="On day Off" ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOfficersOnDayOff" runat="server" Text='<%#Eval("OfficersOnDayOff")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Not Dress Prop." ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNotDressedProperly" runat="server" Text='<%#Eval("NotDressedProperly")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShiftSummary" runat="server" Text='<%#Eval("ShiftSummary")%>'></asp:Label>
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
                                        <asp:Label ID="lblPostAssignmentID" runat="server" Text='<%#Eval("PostAssignmentID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShiftBriefingNotes" runat="server" Text='<%#Eval("ShiftBriefingNotes")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPassedOnFrom" runat="server" Text='<%#Eval("PassedOnFrom")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPassedOnTo" runat="server" Text='<%#Eval("PassedOnTo")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSummaryOfIncidents" runat="server" Text='<%#Eval("SummaryOfIncidents")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSleeping" runat="server" Text='<%#Eval("Sleeping")%>'></asp:Label>
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
                                    <asp:LinkButton ID="btnSave2" runat="server" ValidationGroup="vgDailyPost" CssClass="btn btn-primary" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext2" runat="server" ValidationGroup="vgDailyPost" CssClass="btn btn-primary" OnClick="btnSaveAndNext_Click" >Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
                                <div class="col-sm-1 col-xs-3 text-right">
                                     <label class="control-label" for="tbDate">Date</label>
                                    <asp:ImageButton ID="ibDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                </div>
                                 <div class="col-sm-2 col-xs-3 text-left">
                                      <asp:TextBox ID="tbDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" OnTextChanged="tbDate_TextChanged" ></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbDate" runat="server" PopupButtonID="ibDate" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tbDate" runat="server" ValidationGroup="vgDailyPost" ErrorMessage="Date is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-1 col-xs-3 text-right">
                                   <label class="control-label" for="ddlShift">Shift </label>
                                </div>
                                 <div class="col-sm-2 col-xs-3 text-left">
                                     <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ControlToValidate="ddlShift" ErrorMessage="Shift is required." ID="RequiredFieldValidator4" runat="server" ValidationGroup="vgDailyPost"  ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                 <div class="col-sm-2 col-xs-3 text-right">
                                    <label class="control-label" for="tbLateArrivals">Late arrivals:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbLateArrivals" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                  <div class="col-sm-2 col-xs-3 text-right">
                                   <label class="control-label" for="tbCallsForAssistance">Calls for assistance:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbCallsForAssistance" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                 <div class="col-sm-3 col-xs-3 text-right">
                                    <label class="control-label" for="tbOfficersAssigned">Total number of officers assigned to shift:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbOfficersAssigned" runat="server" CssClass="form-control"></asp:TextBox>
                                     <asp:RequiredFieldValidator ControlToValidate="tbOfficersAssigned" ErrorMessage="Required." ID="RequiredFieldValidator5" runat="server" ValidationGroup="vgDailyPost"  ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-3 text-right">
                                    <label class="control-label" for="tbOfficersPresent">Total number of officers present for shift:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbOfficersPresent" runat="server" CssClass="form-control"></asp:TextBox>
                                     <asp:RequiredFieldValidator ControlToValidate="tbOfficersPresent" ErrorMessage="Required." ID="RequiredFieldValidator1" runat="server" ValidationGroup="vgDailyPost"  ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                  <div class="col-sm-3 col-xs-3 text-right">
                                   <label class="control-label" for="tbOfficersAbsent">Total number of officers absent from shift:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbOfficersAbsent" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ControlToValidate="tbOfficersAbsent" ErrorMessage="Required." ID="RequiredFieldValidator3" runat="server" ValidationGroup="vgDailyPost"  ForeColor="Red"></asp:RequiredFieldValidator>
                                 </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-3 col-xs-3 text-right">
                                    <label class="control-label" for="tbCallIns">Total number of officers who called in for a day off:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbCallIns" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                  <div class="col-sm-3 col-xs-3 text-right">
                                   <label class="control-label" for="tbCallOuts">Total number of officers called out from day off:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbCallOuts" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                 <div class="col-sm-3 col-xs-3 text-right">
                                    <label class="control-label" for="tbOfficersOnDayOff">Total number of officers on day off:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbOfficersOnDayOff" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                             <div class="row">
                                 <div class="col-sm-3 col-xs-3 text-right">
                                    <label class="control-label" for="tbNotDressedProperly">Not dressed properly:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbNotDressedProperly" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                  <div class="col-sm-3 col-xs-3 text-right">
                                    <label class="control-label" for="tbSleeping">Sleeping:</label>
                                </div>
                                 <div class="col-sm-1 col-xs-3 text-left">
                                     <asp:TextBox ID="tbSleeping" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                             <div class="row">
                                <div class="col-sm-12 pull-left col-xs-12">
                                    <label class="control-label" runat="server" id="lblRoster">Daily master roster record of changes</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-xs-12">
                                    <asp:GridView
                                        ID="gvRoster"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="10"
                                        AllowSorting="false"
                                        ShowFooter="false"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        OnPageIndexChanging="gvRoster_PageIndexChanging"
                                        HeaderStyle-CssClass="text-center">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                           <asp:BoundField HeaderText="Officer Assigned" DataField="fullName" />
                                            <asp:BoundField HeaderText="Reason for Absence" DataField="AbsentReason" />
                                            <asp:BoundField HeaderText="Relieving Officer" DataField="RelievingOfficer" NullDisplayText="-" />
                                            <asp:BoundField HeaderText="Comments" DataField="Comments" NullDisplayText="-" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                       <label class="control-label" for="tbShiftSummary">Shift summary</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgDailyPost" MaxLength="5000" placeholder="5000 characters max" ID="tbShiftSummary" runat="server" TextMode="MultiLine" Height="60px" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbShiftSummary" ValidationGroup="vgDailyPost"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                       <label class="control-label" for="tbShiftBriefingNotes">Shift briefing notes</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgDailyPost" MaxLength="5000" placeholder="5000 characters max" ID="tbShiftBriefingNotes" runat="server" TextMode="MultiLine" Height="60px" />
                                         <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbShiftBriefingNotes" ValidationGroup="vgDailyPost"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <br />
                            <div class="row">
                                    <div class="col-md-6 col-xs-12">
                                       <label class="control-label" for="tbPassedOnFrom">Passed on from last shift</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgDailyPost" MaxLength="1000" placeholder="1000 characters max" ID="tbPassedOnFrom" runat="server" TextMode="MultiLine" Height="60px" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbPassedOnFrom" ValidationGroup="vgDailyPost"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="col-md-6 col-xs-12">
                                       <label class="control-label" for="tbPassedOnTo">Passed on to next shift</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgDailyPost" MaxLength="1000" placeholder="1000 characters max" ID="tbPassedOnTo" runat="server" TextMode="MultiLine" Height="60px" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbPassedOnTo" ValidationGroup="vgDailyPost"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <br />
                             <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                       <label class="control-label" for="tbIncidentsSummary">Summary of incidents</label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgDailyPost" MaxLength="5000" placeholder="5000 characters max" ID="tbIncidentsSummary" runat="server" TextMode="MultiLine" Height="60px" />
                                    </div>
                                </div>
                                <br />
                            <hr />
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 text-left">
                                    <asp:LinkButton ID="btnBackToSearch" runat="server" CssClass="btn btn-info" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-6 col-xs-12 text-right">
                                    <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgDailyPost" CssClass="btn btn-primary" OnClick="btnSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgDailyPost" CssClass="btn btn-primary" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
