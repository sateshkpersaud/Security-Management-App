<%@ Page Title="Employees" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="ISSA.Pages.Supervisors.Employees" MaintainScrollPositionOnPostback="true" %>

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

       </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <h4>EMPLOYEE MANAGEMENT
                    <br />
            <small>Use the form below to manage employee records.
            </small>
        </h4>
        <hr />
        <asp:Panel ID="pnlSearch" runat="server" HorizontalAlign="left">
            <div class="container-fluid">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Search Parameters
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="tbSearchReportNumber">Name</label>
                                <asp:TextBox ID="tbSearchName" runat="server" CssClass="form-control" placeholder="either last or first name"></asp:TextBox>
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="tbSearchDate">Date Hired</label>
                                <asp:ImageButton ID="ibSearchDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                <asp:TextBox ID="tbSearchDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>

                                <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbSearchDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlSearchDepartment">Department</label>
                                <asp:DropDownList ID="ddlSearchDepartment" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="ddlSearchStandByStaff">Is Stand-by</label>
                                <asp:DropDownList ID="ddlSearchStandByStaff" CssClass="form-control" runat="server">
                                    <asp:ListItem Text="...Select..." Value=""></asp:ListItem>
                                    <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="ddlSearchIsActive">Is Active</label>
                                <asp:DropDownList ID="ddlSearchIsActive" CssClass="form-control" runat="server">
                                    <asp:ListItem Text="...Select..." Value=""></asp:ListItem>
                                    <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlSearchPosition">Position</label>
                                <asp:DropDownList ID="ddlSearchPosition" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchPosition_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="ddlSearchShift">Shift</label>
                                <asp:DropDownList ID="ddlSearchShift" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlSearchArea">Area</label>
                                <asp:DropDownList ID="ddlSearchArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchArea_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-sm-4 col-xs-12">
                                <label class="control-label" for="ddlSearchLocation">Location</label>
                                <asp:DropDownList ID="ddlSearchLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6 col-xs-6">
                        <asp:LinkButton ID="btnNewAbsence" runat="server" CssClass="btn btn-primary" Text="New Employee" OnClick="btnNewAbsence_Click">New Employee <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                    </div>
                    <div class="col-sm-6 col-xs-6 text-right">
                        <asp:LinkButton ID="btnSearchEmployee" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchEmployee_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                        <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                    </div>
                </div>
                <br />
                <div class="row pull-left">
                    <div class="col-sm-12 col-xs-12">
                        <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVEmployeesHeader" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12 col-xs-12">
                        <asp:GridView ID="gvEmployees"
                            runat="server"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="30"
                            AllowSorting="false"
                            ShowFooter="true"
                            ShowHeaderWhenEmpty="true"
                            CssClass="table table-bordered table-striped  table-hover"
                            HeaderStyle-CssClass="text-center"
                            OnSelectedIndexChanged="gvEmployees_SelectedIndexChanged"
                            OnPageIndexChanging="gvEmployees_PageIndexChanging"
                            OnRowDataBound="gvEmployees_RowDataBound">
                            <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Number">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbEmployeeNumber" runat="server" CommandName="Select">
                                            <asp:Label ID="lblEmployeeNumber" runat="server" Text='<%#Eval("employeeNumber")%>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFullName" runat="server" Text='<%#Eval("FullName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date Hired">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateHired" runat="server" Text='<%#Eval("DateHired")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLocationName" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Department">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDepartment" runat="server" Text='<%#Eval("Department")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Position">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPositionName" runat="server" Text='<%#Eval("PositionName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shift">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShift" runat="server" Text='<%#Eval("Shift")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Is Stand By">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsStandByStaff" runat="server" Text='<%#Eval("isStandByStaff")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Is Active">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsActive" runat="server" Text='<%#Eval("isActive")%>'></asp:Label>
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
                                        <asp:Label ID="lblEmployeeID" runat="server" Text='<%#Eval("EmployeeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFirstName" runat="server" Text='<%#Eval("FirstName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLastName" runat="server" Text='<%#Eval("LastName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOtherName" runat="server" Text='<%#Eval("OtherName")%>'></asp:Label>
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
                                        <asp:Label ID="lblDepartmentID" runat="server" Text='<%#Eval("DepartmentID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPositionID" runat="server" Text='<%#Eval("PositionID")%>'></asp:Label>
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
            <%--<h4>
                    <asp:Label ID="lblHeader" runat="server" Text=""></asp:Label>
                    <br />
                    <small>Use the form below to add or update details for an employee record.
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
                            <div class="col-sm-12 col-xs-12">
                                <asp:Label ID="lblEmployeeNumber" runat="server" ForeColor="#003a75" Font-Bold="true" Text="" Font-Size="Medium" Font-Underline="true"></asp:Label>
                            </div>
                        </div>
                        <hr />
                        <br />
                        <div class="row">
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="tbFirstName">First Name</label>
                                <asp:TextBox CssClass="form-control" ID="tbFirstName" runat="server" placeholder="50 characters max" MaxLength="50" />
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="tbLastName">Last Name*</label>
                                <asp:TextBox CssClass="form-control" ID="tbLastName" ValidationGroup="vgEmployee" runat="server" placeholder="50 characters max" MaxLength="50"  />
                                <asp:RequiredFieldValidator ControlToValidate="tbLastName" ID="RequiredFieldValidator1" runat="server" ErrorMessage="Last Name is required" ForeColor="Red" Display="Dynamic" ValidationGroup="vgEmployee"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="tbOtherName">Other/Call Name</label>
                                <asp:TextBox CssClass="form-control" ID="tbOtherName" runat="server" placeholder="25 characters max" MaxLength="25"  />
                            </div>
                            <div class="col-sm-2 col-xs-6">
                                <label class="control-label" for="tbDateHired">Date Hired* </label>
                                <asp:ImageButton ID="ibCalendar" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                <asp:TextBox CssClass="form-control" ID="tbDateHired" ValidationGroup="vgEmployee" runat="server" AutoPostBack="true" OnTextChanged="tbDateHired_TextChanged" />
                                <asp:CalendarExtender ID="ceDate" TargetControlID="tbDateHired" runat="server" PopupButtonID="ibCalendar" OnClientDateSelectionChanged="noFutureDate" />
                                <asp:RequiredFieldValidator ID="rfvDate" ControlToValidate="tbDateHired" runat="server" ErrorMessage="Date Hired is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgEmployee"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlArea">Area assigned</label>
                                <asp:DropDownList ID="ddlArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-12">
                                <label class="control-label" for="ddlLocation">Location</label>
                                <asp:DropDownList ID="ddlLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlDepartment">Department</label>
                                <asp:DropDownList ID="ddlDepartment" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlArea">Position</label>
                                <asp:DropDownList ID="ddlPosition" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPosition_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label" for="ddlShiftAssigned">Shift Assigned</label>
                                <asp:DropDownList ID="ddlShiftAssigned" CssClass="form-control" runat="server" AutoPostBack="true"></asp:DropDownList>
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label">Is Stand-By </label>
                                <br />
                                <asp:RadioButton ID="rbStandByYes" CssClass="radio-inline" GroupName="StandBy" runat="server" Text="YES" />
                                <asp:RadioButton ID="rbStandByNo" CssClass="radio-inline" GroupName="StandBy" runat="server" Text="NO" />
                            </div>
                            <div class="col-sm-3 col-xs-6">
                                <label class="control-label">Is Active</label><br />
                                <asp:RadioButton ID="rbActiveYes" CssClass="radio-inline" GroupName="Active" runat="server" Text="YES" />
                                <asp:RadioButton ID="rbActiveNo" CssClass="radio-inline" GroupName="Active" runat="server" Text="NO" />
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
                                <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgEmployee" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgEmployee" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
                                <asp:LinkButton ID="btnDeleteEmployee" runat="server" CssClass="btn btn-danger" OnClick="btnDeleteEmployee_Click" >Delete <span aria-hidden="true" class="glyphicon glyphicon-remove"></span></asp:LinkButton>
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

             <asp:Button ID="btnShowPopup4" runat="server" Style="display: none" />
            <asp:Button ID="btnHidePopup4" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="mpeConfirmDelete" DropShadow="True" runat="server" PopupControlID="pnlViewWS" 
                TargetControlID="btnShowPopup4" CancelControlID="btnHidePopup4"></asp:ModalPopupExtender>
            <asp:Panel ID="pnlViewWS" runat="server" Style="display: none">
                <div class="modal-dialog modal-sm">
                    <div class="modal-content">
                        <div class="modal-header"> 
                            <h4 class="modal-title"><asp:Label ID="Label5" runat="server" Text="Are you sure?"></asp:Label></h4></div>
                        <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12 text-center">
                                       <asp:LinkButton ID="btnYes" runat="server"  CssClass="btn btn-success" OnClick="btnYes_Click" >YES <span aria-hidden="true" class="glyphicon glyphicon-ok-circle"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnNo" runat="server"  CssClass="btn btn-primary" OnClick="btnNo_Click" >NO <span aria-hidden="true" class="glyphicon glyphicon-remove-circle"></span></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>  





    <%-- <div class="modal" id="confirmDelete" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblConfirmHeader" runat="server" Text="Are you sure?"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12 col-xs-12">
                                       <asp:LinkButton ID="btnYes" runat="server"  CssClass="btn btn-success" OnClick="btnYes_Click" >YES <span aria-hidden="true" class="glyphicon glyphicon-ok-circle"></span></asp:LinkButton>
                    <asp:LinkButton ID="btnNo" runat="server"  CssClass="btn btn-primary" OnClick="btnNo_Click" >NO <span aria-hidden="true" class="glyphicon glyphicon-remove-circle"></span></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                 <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>--%>

</asp:Content>
