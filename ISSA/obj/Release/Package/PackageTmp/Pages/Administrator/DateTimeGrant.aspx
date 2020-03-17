<%@ Page Title="Date/Time Grant" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DateTimeGrant.aspx.cs" Inherits="ISSA.Pages.Managers.DateTimeGrant" MaintainScrollPositionOnPostback="true" %>

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
                    //if (dateSet > endDate) {
                    //    ShowMessage("You cannot select a future date", "Warning");
                    //    control.value = endDate;
                    //    control.style.borderColor = "red";
                    //    control.focus();
                    //    error = 2
                    //}
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
    <h4>DATE/TIME GRANT
                    <br />
        <small>Use the form below to grant modification of date/time in the Daily Log, Incident & Complaint Pages
        </small>
    </h4>
    <hr />
    <asp:Panel ID="pnlDetails" runat="server" HorizontalAlign="Left">
        <div class="panel panel-default" id="div1">
            <div id="Div2" class="panel-heading" runat="server">
                <asp:Label ID="lblHeader" runat="server" Text=""></asp:Label>
            </div>
            <div class="panel-body">
                <div class="container-fluid">
                    <div class="row">
                        <div class="col-sm-2 col-xs-6">
                            <label class="control-label" for="tbDateStart">Date Start</label>
                            <asp:ImageButton ID="ibDateStart" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                            <asp:TextBox ID="tbDateStart" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbDateStart" runat="server" PopupButtonID="ibDateStart" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tbDateStart" runat="server" ValidationGroup="DTGrant" ErrorMessage="Required." ForeColor="Red"></asp:RequiredFieldValidator>
                        </div>
                         <div class="col-sm-2 col-xs-6">
                            <label class="control-label" for="tbDateStart">Time Start</label>
                            <asp:TextBox CssClass="form-control" ID="tbTimeStart" runat="server" placeholder="24 hrs format" />
                            <asp:RequiredFieldValidator ID="rfvTime" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbTimeStart" ValidationGroup="DTGrant"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-sm-2 col-xs-6">
                            <label class="control-label" for="tbDateEnd">Date End</label>
                            <asp:ImageButton ID="ibDateEnd" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                            <asp:TextBox ID="tbDateEnd" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbDateEnd" runat="server" PopupButtonID="ibDateEnd" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="tbDateEnd" runat="server" ValidationGroup="DTGrant" ErrorMessage="Required." ForeColor="Red"></asp:RequiredFieldValidator>
                        </div>
                         <div class="col-sm-2 col-xs-6">
                            <label class="control-label" for="tbDateEnd">Time End</label>
                            <asp:TextBox CssClass="form-control" ID="tbTimeEnd" runat="server" placeholder="24 hrs format" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbTimeEnd" ValidationGroup="DTGrant"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12 col-xs-12">
                            <label class="control-label" for="tbComments">Comments</label>
                            <asp:TextBox ID="tbComments" runat="server" CssClass="form-control" placeholder="1000 characters max" TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Required." ForeColor="Red" Display="Dynamic" ControlToValidate="tbComments" ValidationGroup="DTGrant"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12 col-xs-12 text-right">
                            <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default">Clear Form <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                            <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" ValidationGroup="DTGrant" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 col-xs-12">
                            <asp:GridView
                                ID="gvGrants"
                                runat="server"
                                AutoGenerateColumns="false"
                                AllowPaging="true"
                                PageSize="20"
                                AllowSorting="false"
                                ShowFooter="true"
                                ShowHeaderWhenEmpty="false"
                                ShowHeader="true"
                                CssClass="table table-bordered table-striped  table-hover"
                                OnPageIndexChanging="gvGrants_PageIndexChanging">
                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Date Time From" DataField="dtFrom"/>
                                    <asp:BoundField HeaderText="Date Time To" DataField="dtTo"/>
                                    <asp:BoundField HeaderText="Comments" DataField="Comments" />      
                                    <asp:BoundField HeaderText="Created On" DataField="CreatedOn" />
                                    <asp:BoundField HeaderText="Created By" DataField="CreatedBy"/>                             
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
