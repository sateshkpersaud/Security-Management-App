<%@ Page Title="Work Schedule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WorkSchedule.aspx.cs" Inherits="ISSA.Pages.Managers.WorkSchedule" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function validateDateRange(control, startDate1, endDate1, selectedMonth, selectedYear) {
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
                    var startDate = new Date(startDate1).format("MM/dd/yyyy");
                    var endDate = new Date(endDate1).format("MM/dd/yyyy");
                    //ShowMessage(dateSet + " - " + startDate + " - " + endDate, "info");
                    if (dateSet < startDate) {
                        ShowMessage("Invalid date. Date must be within " + selectedMonth + ", " + selectedYear.value, "Warning");
                        control.style.borderColor = "red";
                        control.focus();
                        //This is for firefox as the above focus does not work
                        tempField = control;
                        setTimeout("tempField.focus();", 1);
                        error = 2 //used to reset the border color of the textbox
                    }
                    if (dateSet > endDate) {
                        ShowMessage("Invalid date. Date must be within " + selectedMonth + ", " + selectedYear.value, "Warning");
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

        function verifyLocation(Area, Location) {
            if (Area.value != "") {
                if (Location.value == "") {
                    ShowMessage("Location is required", "Warning");
                    Location.style.borderColor = "red";
                    Location.focus();
                    //This is for firefox as the above focus does not work
                    tempField = Location;
                    setTimeout("tempField.focus();", 1);
                }
                else {
                    Location.style.borderColor = "lightgray";
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h4>WORK SCHEDULE
                    <br />
                <small>Use the form below to create work schedules.
                </small>
            </h4>
            <hr />
            <div>
                <div class="messagealert" id="alert_container">
                </div>
            </div>
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-3 col-xs-6">
                        <asp:RadioButton ID="rbCreate" runat="server" Text="Create" GroupName="WS" CssClass="radio-inline" AutoPostBack="true" OnCheckedChanged="rbCreate_CheckedChanged" />
                         <asp:RadioButton ID="rbModify" runat="server" Text="Modify/View" GroupName="WS" CssClass="radio-inline" AutoPostBack="true" OnCheckedChanged="rbModify_CheckedChanged" />
                        </div>
                    </div>
                <br />
                <div class="row">
                    <div class="panel panel-default">
                            <div class="panel-heading">
                                Parameters
                            </div>
                            <div class="panel-body">
                    <div class="col-sm-2 col-xs-6">
                        <label class="control-label" for="ddlYear">Year: </label>
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddlYear" ForeColor="Red" runat="server" ValidationGroup="WS" ErrorMessage="Year is required."></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-sm-2 col-xs-6">
                        <label class="control-label" runat="server" id="lblMonth">Month: </label>
                        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
                            <asp:ListItem Value="1" Text="January"></asp:ListItem>
                            <asp:ListItem Value="2" Text="February"></asp:ListItem>
                            <asp:ListItem Value="3" Text="March"></asp:ListItem>
                            <asp:ListItem Value="4" Text="April"></asp:ListItem>
                            <asp:ListItem Value="5" Text="May"></asp:ListItem>
                            <asp:ListItem Value="6" Text="June"></asp:ListItem>
                            <asp:ListItem Value="7" Text="July"></asp:ListItem>
                            <asp:ListItem Value="8" Text="August"></asp:ListItem>
                            <asp:ListItem Value="9" Text="September"></asp:ListItem>
                            <asp:ListItem Value="10" Text="October"></asp:ListItem>
                            <asp:ListItem Value="11" Text="November"></asp:ListItem>
                            <asp:ListItem Value="12" Text="December"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="ddlMonth" runat="server" ValidationGroup="WS" ForeColor="Red" ErrorMessage="Month is required."></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-sm-2 col-xs-6">
                        <label class="control-label" for="ddlArea">Area: </label>
                        <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlArea" runat="server" ValidationGroup="WS" ErrorMessage="Area is required." ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-sm-2 col-xs-6">
                        <label class="control-label" for="ddlShift">Shift: </label>
                        <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="ddlShift" runat="server" ValidationGroup="WS" ErrorMessage="Shift is required." ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-sm-4 col-xs-6 text-right">
                        <br />
                        <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>
                         <asp:LinkButton ID="lbGenerate" runat="server" CssClass="btn btn-success" Text="Generate" OnClick="lbGenerate_Click" ValidationGroup="WS">Generate <span aria-hidden="true" class="glyphicon glyphicon-check"></span></asp:LinkButton>
                        <asp:LinkButton ID="lbView" runat="server" CssClass="btn btn-warning" Text="View" OnClick="lbView_Click">View <span aria-hidden="true" class="glyphicon glyphicon-circle-arrow-up"></span></asp:LinkButton>
                    </div>
                </div>
            </div>
                    </div>
            <br />
                <div class="row">
                    <div class="col-sm-6 col-xs-6">
                        <asp:Label ID="lblemployeesFound" runat="server" Font-Bold="true" CssClass="control-label"></asp:Label>
                    </div>
                    <div class="col-sm-6 col-xs-6 text-right">
                        <asp:LinkButton ID="btnUnscheduledEmps" runat="server" CssClass="btn btn-default" OnClick="btnUnscheduledEmps_Click">Uncheduled Employees</asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12" >
                        <asp:GridView ID="gvEmployeesSchedule"
                            runat="server"
                            AutoGenerateColumns="false"
                            AllowPaging="false"
                            AllowSorting="false"
                            ShowFooter="true"
                            ShowHeaderWhenEmpty="true"
                            CssClass="table table-bordered table-striped table-hover"
                            HeaderStyle-CssClass="text-center"
                            OnRowDataBound="gvEmployeesSchedule_RowDataBound"
                            OnSelectedIndexChanged="gvEmployeesSchedule_SelectedIndexChanged">
                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateField HeaderText="" ItemStyle-Width="15px">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbComplete" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Number" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeNumber" runat="server" Text='<%#Eval("employeenumber")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Employee">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbEmployee" runat="server" CommandName="Select">
                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("fullname")%>' CssClass="control-label"></asp:Label>
                                            </asp:linkbutton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Day Off(s)" ItemStyle-Width="280px">
                                    <ItemTemplate>
                                        <div class="form-inline">
                                            <div class="form-group">
                                                <asp:TextBox ID="tbDayOff1" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" Width="120px"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbDayOff1" runat="server" />
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="tbDayOff2" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" Width="120px"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbDayOff2" runat="server" />
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="tbDayOff3" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" Width="120px"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="tbDayOff3" runat="server" />
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="tbDayOff4" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy" Width="120px"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender4" TargetControlID="tbDayOff4" runat="server" />
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Stand By">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlStandByEmployee" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Exclude">
                                    <ItemTemplate>
                                       <asp:CheckBox ID="cbExclude" runat="server"/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfStandbyRequired" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfIsIllegal" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStandyByEmployeeID" runat="server" Text='<%#Eval("StandyByEmployeeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLocationID" runat="server" Text='<%#Eval("locationid")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblWorkScheduleEmployeeID" runat="server" Text='<%#Eval("WorkScheduleEmployeeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsComplete" runat="server" Text='<%#Eval("IsComplete")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblExclude" runat="server" Text='<%#Eval("Exclude")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("createdOn")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12 text-right">
                        <asp:LinkButton ID="btnUnscheduledEmps2" runat="server" CssClass="btn btn-default" OnClick="btnUnscheduledEmps_Click">Uncheduled Employees</asp:LinkButton>
                        <asp:LinkButton ID="btnSave2" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                    </div>
                </div>
                 <div class="row">
                                <div class="col-sm-12 col-xs-12 text-right">
                                    <asp:Label ID="lblAuditTrail" runat="server" Font-Size="X-Small" ForeColor="DarkGray" Text=""></asp:Label>
                                </div>
                            </div>
            </div>

            <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeDupWorkSchedule" DropShadow="True" runat="server" PopupControlID="pnlDupWorkSchedule" TargetControlID="btnShowPopup" CancelControlID="btnHidePopup"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlDupWorkSchedule" runat="server" Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <asp:Label ID="Label9" runat="server" Text="Existing Work Schedule"></asp:Label></h4></div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label" runat="server" id="lblMessage"></label>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="modal-footer">
                                <asp:LinkButton ID="btnviewThisSchedule" runat="server" CssClass="btn btn-info" OnClick="btnviewThisSchedule_Click" >Modify This Schedule <span aria-hidden="true" class="glyphicon glyphicon-ok"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnCreateNewSchedule" runat="server"  CssClass="btn btn-warning" OnClick="btnCreateNewSchedule_Click" >Create Next Schedule <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <%-- Popup for employees not scheduled --%>
             <asp:Button ID="btnShowPopup2" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup2" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeUnscheduledEmps" DropShadow="True" runat="server" PopupControlID="pnlUnscheduledEmps" TargetControlID="btnShowPopup2" CancelControlID="btnHidePopup2"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlUnscheduledEmps" runat="server" Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header"> <h4 class="modal-title">
                            <asp:Label ID="Label8" runat="server" Text="Unscheduled Employees"></asp:Label></h4></div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label" runat="server" id="Label1" for="gvUnscheduledEmps">Employee(s) not yet scheduled:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-xs-12" style="height:300px; overflow:auto">
                                         <asp:GridView ID="gvUnscheduledEmps"
                            runat="server"
                            AutoGenerateColumns="false"
                            AllowPaging="false"
                            AllowSorting="false"
                            ShowFooter="true"
                            ShowHeaderWhenEmpty="true"
                            CssClass="table table-bordered table-striped table-hover"
                            HeaderStyle-CssClass="text-center"
                            OnRowDataBound="gvUnscheduledEmps_RowDataBound">
                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Select" ItemStyle-Width="15px">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Number" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeNumber" runat="server" Text='<%#Eval("employeenumber")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Employee">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("fullname")%>' CssClass="control-label"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control" Width="180px"></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfIsIllegal" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLocationID" runat="server" Text='<%#Eval("locationid")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="modal-footer">
                                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-default" OnClick="btnCancel_Click"  >Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnAppend" runat="server"  CssClass="btn btn-primary" OnClick="btnAppend_Click"  >Append <span aria-hidden="true" class="glyphicon glyphicon-ok"></span></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <%--  --%>
             <asp:Button ID="btnShowPopup3" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup3" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeEmployeeTransfer" DropShadow="True" runat="server" PopupControlID="pnlEmployeeTransfer" TargetControlID="btnShowPopup3" CancelControlID="btnHidePopup3"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlEmployeeTransfer" runat="server" Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header"> <h4 class="modal-title">
                            <asp:Label ID="Label7" runat="server" Text="Employee Transfer"></asp:Label></h4></div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                         <label class="control-label" runat="server" id="lblEmpID" visible="false"></label>
                                        <label class="control-label" runat="server" id="lblEmpName"></label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-5 col-xs-6">
                                        <label class="control-label" id="Label3" for="ddlTransferFor" >For</label>
                                        <asp:DropDownList ID="ddlTransferFor" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="1" Text="This schedule only"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="This and all new schedules"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-7 col-xs-6">
                                        <label class="control-label" id="Label4" for="ddlTransferShift">Shift</label>
                                        <asp:DropDownList ID="ddlTransferShift" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-5 col-xs-6">
                                        <label class="control-label" id="Label2" for="ddlTransferArea">Area</label>
                                        <asp:DropDownList ID="ddlTransferArea" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTransferArea_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-7 col-xs-6" runat="server" id="trLocation">
                                        <label class="control-label" for="ddlTransferLocation">Location</label>
                                        <asp:DropDownList ID="ddlTransferLocation" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <br />
                                 <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                        <label class="control-label" runat="server" id="Label6">Comments</label>
                                        <asp:TextBox ID="tbTransferComments" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <br />
                            <div class="modal-footer">
                                <asp:LinkButton ID="btnCancel2" runat="server" CssClass="btn btn-default" OnClick="btnCancel2_Click" >Cancel <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnTransfer" runat="server"  CssClass="btn btn-primary" OnClick="btnTransfer_Click">Transfer <span aria-hidden="true" class="glyphicon glyphicon-ok"></span></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        
            </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button ID="btnShowPopup4" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup4" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeViewWorkSchedule" DropShadow="True" runat="server" PopupControlID="pnlViewWS" 
                TargetControlID="btnShowPopup4" CancelControlID="btnHidePopup4"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlViewWS" runat="server" Style="display: none">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header"> 
                            <h4 class="modal-title"><asp:Label ID="Label5" runat="server" Text="Work Schedule"></asp:Label></h4></div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-sm-12 col-xs-12" style="height: 450px;">
                        <rsweb:ReportViewer ID="rvWorkSchedule" runat="server" Width="100%" Height="445px" HyperlinkTarget="_blank" BorderStyle="Groove" ShowParameterPrompts="False" ShowPrintButton="true">
                        </rsweb:ReportViewer>
                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:LinkButton ID="btnCancelWS" runat="server" CssClass="btn btn-default" OnClick="btnCancelWS_Click" >Close <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        
</asp:Content>
