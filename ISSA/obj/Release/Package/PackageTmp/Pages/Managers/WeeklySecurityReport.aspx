<%@ Page Title="Weekly Security Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WeeklySecurityReport.aspx.cs" Inherits="ISSA.Pages.Managers.WeeklySecurityReport" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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

        function positiveNumber(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            //if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) { use this to allow period
            if (charCode < 48 || charCode > 57) {
                return false;
            } else {
                return true;
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h4>WEEKLY SECURITY REPORT
                    <br />
                <small>Use the form below to manage weekly security reports.
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
                                    <label class="control-label" for="tbSearchWeekStart">Week Start</label>
                                    <asp:ImageButton ID="ibSearchWeekStart" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbSearchWeekStart" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbSearchWeekStart" runat="server" PopupButtonID="ibSearchWeekStart" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:CompareValidator ValidationGroup="searchVal" ID="CompareValidator1" runat="server" ControlToCompare="tbSearchWeekStart" ControlToValidate="tbSearchWeekEnd" ErrorMessage="Week Start Date must be on or before Week End Date." ForeColor="Red" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbSearchWeekEnd">End</label>
                                    <asp:ImageButton ID="ibSearchWeekEnd" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbSearchWeekEnd" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="tbSearchWeekEnd" runat="server" PopupButtonID="ibSearchWeekEnd" OnClientDateSelectionChanged="noFutureDate" />
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlSearchArea">Area</label>
                                    <asp:DropDownList ID="ddlSearchArea" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6 col-xs-6">
                            <asp:LinkButton ID="btnNew" runat="server" CssClass="btn btn-primary" OnClick="btnNew_Click">New Report <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6 col-xs-6 text-right">
                            <asp:LinkButton ID="btnSearchResults" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchResults_Click" ValidationGroup="searchVal">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                        </div>
                    </div>
                    <br />
                    <div class="row pull-left">
                        <div class="col-sm-12 col-xs-12">
                            <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVResultsHeader" runat="server" Text=""></asp:Label>
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
                                OnRowDataBound="gvResults_RowDataBound"
                                OnSelectedIndexChanged="gvResults_SelectedIndexChanged"
                                OnPageIndexChanging="gvResults_PageIndexChanging">
                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Number">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbReportnumber" runat="server" CommandName="Select">
                                                <asp:Label ID="lblReportnumber" runat="server" Text='<%#Eval("ReportNumber")%>'></asp:Label>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Area">
                                        <ItemTemplate>
                                            <asp:Label ID="lblArea" runat="server" Text='<%#Eval("Area")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Week">
                                        <ItemTemplate>
                                            <asp:Label ID="lblweekPeriod" runat="server" Text='<%#Eval("weekPeriod")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shift Count">
                                        <ItemTemplate>
                                            <asp:Label ID="lblshiftsCount" runat="server" Text='<%#Eval("shiftsCount")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comments">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComments" runat="server" Text='<%#Eval("Comments")%>'></asp:Label>
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
                                            <asp:Label ID="lblSecurityReportID" runat="server" Text='<%#Eval("SecurityReportID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAreaID" runat="server" Text='<%#Eval("AreaID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWeekEnd" runat="server" Text='<%#Eval("WeekEnd")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWeekStart" runat="server" Text='<%#Eval("WeekStart")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
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
                                        <asp:LinkButton ID="btnBackToSearch2" runat="server" CssClass="btn btn-info" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                    </div>
                                    <div class="col-sm-6 col-xs-12 text-right">
                                        <asp:LinkButton ID="btnClear2" runat="server" CssClass="btn btn-default" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                        <asp:LinkButton ID="btnSave2" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="vgDailyLog">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                        <asp:LinkButton ID="btnSaveAndNext2" runat="server" CssClass="btn btn-primary" OnClick="btnSaveAndNext_Click" ValidationGroup="vgDailyLog">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlArea">Area </label>
                                    <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlArea" runat="server" ValidationGroup="vgDailyLog" ErrorMessage="Area is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbWeekStart">Week Start</label>
                                    <asp:ImageButton ID="ibWeekStart" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbWeekStart" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" OnTextChanged="tbWeekStart_TextChanged"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbWeekStart" runat="server" PopupButtonID="ibWeekStart" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tbWeekStart" runat="server" ValidationGroup="vgDailyLog" ErrorMessage="Date is required." ForeColor="Red"></asp:RequiredFieldValidator>
                               <asp:CompareValidator ValidationGroup="vgDailyLog" ID="CompareValidator2" runat="server" ControlToCompare="tbWeekStart" ControlToValidate="tbWeekEnd" ErrorMessage="<br/>Week Start Date must be on or before Week End Date." ForeColor="Red" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                     </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbWeekEnd">End</label>
                                    <asp:ImageButton ID="ibWeekEnd" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbWeekEnd" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" AutoPostBack="true" OnTextChanged="tbWeekEnd_TextChanged"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender4" TargetControlID="tbWeekEnd" runat="server" PopupButtonID="ibWeekEnd" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="tbWeekEnd" runat="server" ValidationGroup="vgDailyLog" ErrorMessage="Date is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                               <%-- <div class="col-sm-5 col-xs-6 text-right">
                                    <label class="control-label" for="tbWeekEnd">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnBackToSearch2" runat="server" CssClass="btn btn-info" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                </div>--%>
                            </div>
                        </div>
                        <asp:Panel ID="pnlReportDetails" runat="server" HorizontalAlign="Left">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-sm-12 col-xs-12">
                                        <asp:GridView
                                            ID="gvShiftReport"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false"
                                            ShowHeader="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            OnRowDataBound="gvShiftReport_RowDataBound"
                                            OnPreRender="gvShiftReport_PreRender"
                                            OnRowCommand="gvShiftReport_RowCommand">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <%-- <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:BoundField HeaderText="Shift" DataField="Shift" ItemStyle-VerticalAlign="Middle" />
                                                <asp:TemplateField HeaderText="CIT in GT" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCashInGT" runat="server" Text='<%#Eval("cashInGT")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CIT out GT" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCashOutGT" runat="server" Text='<%#Eval("cashOutGT")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Other" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOther" runat="server" Text='<%#Eval("other")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Mon" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbMonday" runat="server" CommandName="Monday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblMonday" runat="server" Text='<%#Eval("Monday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbMonday" runat="server" Text='<%#Eval("Monday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tue" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbTuesday" runat="server" CommandName="Tuesday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblTuesday" runat="server" Text='<%#Eval("Tuesday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbTuesday" runat="server" Text='<%#Eval("Tuesday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Wed" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbWednesday" runat="server" CommandName="Wednesday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblWednesday" runat="server" Text='<%#Eval("Wednesday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbWednesday" runat="server" Text='<%#Eval("Wednesday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Thu" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbThursday" runat="server" CommandName="Thursday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblThursday" runat="server" Text='<%#Eval("Thursday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbThursday" runat="server" Text='<%#Eval("Thursday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fri" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbFriday" runat="server" CommandName="Friday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblFriday" runat="server" Text='<%#Eval("Friday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbFriday" runat="server" Text='<%#Eval("Friday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sat" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbSaturday" runat="server" CommandName="Saturday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblSaturday" runat="server" Text='<%#Eval("Saturday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbSaturday" runat="server" Text='<%#Eval("Saturday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sun" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbSunday" runat="server" CommandName="Sunday" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">
                                                            <asp:Label ID="lblSunday" runat="server" Text='<%#Eval("Sunday")%>'></asp:Label>
                                                        </asp:LinkButton>
                                                        <asp:TextBox CssClass="form-control" ID="tbSunday" runat="server" Text='<%#Eval("Sunday")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Present" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPresent" runat="server" Text='<%#Eval("Present")%>'></asp:Label>
                                                        <asp:TextBox CssClass="form-control" ID="tbPresent" runat="server" Text='<%#Eval("Present")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Absent" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAbsent" runat="server" Text='<%#Eval("Absentt")%>'></asp:Label>
                                                        <asp:TextBox CssClass="form-control" ID="tbAbsent" runat="server" Text='<%#Eval("Absentt")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Inc. & Com." ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIncidentsComplaints" runat="server" Text='<%#Eval("IncidentsComplaints")%>'></asp:Label>
                                                        <asp:TextBox CssClass="form-control" ID="tbIncidentsComplaints" runat="server" Text='<%#Eval("IncidentsComplaints")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Switches" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSwitches" runat="server" Text='<%#Eval("Switches")%>'></asp:Label>
                                                        <asp:TextBox CssClass="form-control" ID="tbSwitches" runat="server" Text='<%#Eval("Switches")%>' Width="50px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblType" runat="server" Text='<%#Eval("Type")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblComparisonID" runat="server" Text='<%#Eval("ComparisonID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div class="row text-right">
                                    <div class="col-sm-12 col-xs-12">
                                        <asp:LinkButton ID="btnNewSecurityShiftReport" runat="server" OnClick="btnNewSecurityShiftReport_Click" CssClass="btn btn-default">Add Next Shift <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-12 col-xs-12">
                                        <label class="control-label" for="tbWeekStart">Comments</label><br />
                                        <asp:TextBox ID="tbComments" runat="server" CssClass="form-control" placeholder="5000 characters max" TextMode="MultiLine" Height="100px"></asp:TextBox>
                                    </div>
                                </div>
                                <hr />
                                <div class="row">
                                    <div class="col-sm-6 col-xs-12 text-left">
                                        <asp:LinkButton ID="btnBackToSearch" runat="server" CssClass="btn btn-info" OnClick="btnBackToSearch_Click">Back to Search <span aria-hidden="true" class="glyphicon glyphicon-backward"></span></asp:LinkButton>
                                    </div>
                                    <div class="col-sm-6 col-xs-12 text-right">
                                        <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" OnClick="btnClear_Click">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="vgDailyLog">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                        <asp:LinkButton ID="btnSaveAndNext" runat="server" CssClass="btn btn-primary" OnClick="btnSaveAndNext_Click" ValidationGroup="vgDailyLog">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="gvShiftReport" />
            <asp:PostBackTrigger ControlID="btnNewSecurityShiftReport" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal" id="comparisonModal" role="dialog">
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblEditHeader" runat="server" Text=""></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-2 col-xs-6">
                                        <label class="control-label"></label>
                                        <asp:Label ID="lblTime" runat="server" Text=""></asp:Label>
                                        <asp:Label ID="lblSelectedCallLogID" runat="server" Text="" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12 col-xs-12">
                                        <asp:GridView
                                            ID="gvComparison"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            AllowPaging="false"
                                            AllowSorting="false"
                                            ShowFooter="false"
                                            ShowHeaderWhenEmpty="false"
                                            ShowHeader="true"
                                            CssClass="table table-bordered table-striped  table-hover"
                                            OnRowDataBound="gvComparison_RowDataBound">
                                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Location" DataField="LocationName" />
                                                <asp:BoundField HeaderText="Operator" DataField="operatorPresent" />
                                                <asp:BoundField HeaderText="Supervisor" DataField="supervisorPresent" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-default" OnClick="btnCancel_Click">Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal" id="newShiftModal" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblNewShiftHeader" runat="server" Text="Add New Shift"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label">Shift</label>
                                        <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Shift is required." ForeColor="Red" Display="Dynamic" ControlToValidate="ddlShift" ValidationGroup="vgNewShift"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="col-md-6 col-xs-6">
                                        <label class="control-label"></label>
                                        <asp:TextBox CssClass="form-control" ValidationGroup="vgPersonsAffected" MaxLength="150" ID="tbLastName" Visible="false" runat="server" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Last Name is required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbLastName" ValidationGroup="vgPersonsAffected"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <%-- <asp:LinkButton ID="btnCancelPersonAffected" runat="server" CssClass="btn btn-default" Text="Cancel" >Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>--%>
                    <asp:LinkButton ID="btnAppendClose" runat="server" ValidationGroup="vgNewShift" CssClass="btn btn-default" OnClick="btnAppendClose_Click">Save & Close <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnAppendNext" runat="server" ValidationGroup="vgNewShift" CssClass="btn btn-primary" OnClick="btnAppendNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
