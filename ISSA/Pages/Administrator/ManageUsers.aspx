<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="ISSA.Pages.Administrator.ManageUsers" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc" TagName="UserForm" Src="~/Pages/Administrator/UserForm.ascx" %>
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
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h4>Manage Users</h4>
        <hr />
    </hgroup>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <%-- Filter Users --%>
            <asp:Panel ID="pnlFilter" runat="server">
                <div class="form-inline">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="tbName" CssClass="control-label">Name:</asp:Label>
                        <asp:TextBox ID="tbName" CssClass="form-control" runat="server" ToolTip="Search By FirstName, LastName, OtherName or UserName. Leave blank to return all users." placeholder="Enter a Name"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label" AssociatedControlID="ddlStatus">Status:</asp:Label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">...Select...</asp:ListItem>
                            <asp:ListItem Value="True">Active</asp:ListItem>
                            <asp:ListItem Value="False">Inactive</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="ddlRole" CssClass="control-label">Role:</asp:Label>
                        <asp:DropDownList ID="ddlRole" CssClass="form-control" runat="server"></asp:DropDownList>
                    </div>


                    <asp:LinkButton ID="btnFilter" OnClick="btnFilter_Click" CssClass="btn btn-primary" runat="server"><span aria-hidden="true" class="glyphicon glyphicon-search"></span> Search</asp:LinkButton>

                </div>
            </asp:Panel>


            <br />
            <div class="row">
                <div class="col-xs-6 text-left">
                    <asp:Label ID="lblUsersFound" runat="server" CssClass="text-info" Text=""></asp:Label>
                </div>
                <div class="col-xs-6 text-right">
                    <asp:LinkButton ID="btnNewUser" OnClick="btnNewUser_Click" CssClass="btn btn-default" runat="server"><span aria-hidden="true" class="glyphicon glyphicon-user"></span> New User</asp:LinkButton>
                </div>
            </div>
            <br />

            <%-- User Grid --%>
            <asp:GridView ID="gvUsers"
                CssClass="table table-bordered table-striped table-hover"
                runat="server"
                EmptyDataText="No results found for your search, please try again..."
                ShowHeaderWhenEmpty="true"
                AutoGenerateColumns="false"
                OnRowDataBound="gvUsers_RowDataBound"
                PagerStyle-CssClass="pagination-ys"
                OnPageIndexChanging="gvUsers_PageIndexChanging"
                PagerSettings-Mode="NumericFirstLast"
                PagerStyle-HorizontalAlign="Center"
                AllowPaging="true"
                PageSize="30">
                <Columns>
                    <asp:TemplateField HeaderText="#">
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lblName" runat="server" Text='<%#Eval("Name")%>' CommandName="Select" OnClick="lblName_Click"> </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User Name">
                        <ItemTemplate>
                            <asp:Label ID="lblUserName" runat="server" Text='<%#Eval("UserName")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Status" DataField="Status" />
                    <asp:BoundField HeaderText="Role(s)" DataField="RoleName" />
                    <asp:BoundField HeaderText="Date Added" DataField="CreateDate" />
                    <asp:BoundField HeaderText="Last Activity" DataField="LastActivityDate" />
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblUserID" runat="server" Text='<%#Eval("UserId")%>'> </asp:Label>
                            <asp:Label ID="lblEmpID" runat="server" Text='<%#Eval("EmployeeId")%>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnFilter" />
            <asp:PostBackTrigger ControlID="btnNewUser" />
        </Triggers>
    </asp:UpdatePanel>

    <%-- Add User Modal  --%>
    <div class="modal fade" id="addUserModal" role="dialog">
        <div class="modal-dialog modal-lg">

            <%-- Modal Content --%>
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">New User</h4>
                </div>
                <div class="modal-body">
                    <%-- Create User --%>
                    <uc:UserForm runat="server" ID="addUser" Visible="false"></uc:UserForm>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="usrVal" CssClass="btn btn-primary" OnClick="btnSave_Click"><span aria-hidden="true" class="glyphicon glyphicon-save"></span> Save</asp:LinkButton>
                    <asp:LinkButton ID="btnSaveNext" runat="server" ValidationGroup="srVal" CssClass="btn btn-default" OnClick="btnSaveNext_Click"><span aria-hidden="true" class="glyphicon glyphicon-triangle-right"></span> Save & Next</asp:LinkButton>
                    <asp:LinkButton ID="btnClose" runat="server" CssClass="btn btn-link" OnClick="btnClose_Click">Close</asp:LinkButton>

                    <%--<button type="button" class="btn btn-link" data-dismiss="modal">Close</button>--%>
                </div>
            </div>
        </div>
    </div>

    <%-- Edit User Modal  --%>
    <div class="modal fade" id="editUserModal" role="dialog">
        <div class="modal-dialog modal-lg">

            <%-- Modal Content --%>
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblEditHeader" runat="server" Text=""></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <div class="">
                        <%-- Tabbed Navigation --%>
                        <ul class="nav nav-tabs" role="tablist">
                            <li role="presentation" class="active"><a href="#AccountDetails" aria-controls="AccountDetails" role="tab" data-toggle="tab">Account Details</a></li>
                            <li role="presentation"><a href="#ActivityHistory" aria-controls="ActivityHistory" role="tab" data-toggle="tab">Activity History</a></li>
                        </ul>

                        <%-- Tab panes --%>
                        <div class="tab-content">
                            <div role="tabpanel" class="tab-pane fade in active" id="AccountDetails">
                                <%-- account details content here --%>
                                <div class="content_padding">
                                    <uc:UserForm runat="server" ID="editUser" Visible="false"></uc:UserForm>
                                    <br />
                                    <div class=" text-right">
                                        <asp:LinkButton ID="btnEdit" runat="server" ValidationGroup="usrVal" CssClass="btn btn-default" OnClick="btnEdit_Click"><span aria-hidden="true" class="glyphicon glyphicon-save"></span> Update</asp:LinkButton>
                                    </div>
                                </div>
                            </div>

                            <div role="tabpanel" class="tab-pane fade" id="ActivityHistory">
                                <%-- activity history content here --%>
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <div class="form-inline content_padding">
                                            <div class="form-group">
                                                <asp:Label ID="Label5" AssociatedControlID="tbDateFrom" runat="server" Text="Date From:"></asp:Label>
                                                <asp:TextBox ID="tbDateFrom" AutoPostBack="true" OnTextChanged="tbDateFrom_TextChanged" CssClass="form-control" runat="server" placeholder="mm/dd/yyyy" MaxLength="10"></asp:TextBox>
                                                <asp:ImageButton ID="ibSearchDateFrom" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                                <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbDateFrom" runat="server" PopupButtonID="ibSearchDateFrom" OnClientDateSelectionChanged="noFutureDate" />
                                                <%-- Validation --%>
                                                <asp:CompareValidator ID="CompareValidator1" runat="server" CssClass="text-danger" ControlToValidate="tbDateFrom" ValidationGroup="actHis" Display="Dynamic" ErrorMessage="Date is in incorrect format!" Type="Date" Operator="DataTypeCheck"></asp:CompareValidator>
                                            </div>

                                            <div class="form-group">
                                                <asp:Label ID="Label6" AssociatedControlID="tbDateTo" runat="server" Text="To:"></asp:Label>
                                                <asp:TextBox ID="tbDateTo" Enabled="false" CssClass="form-control" runat="server" placeholder="mm/dd/yyyy" MaxLength="10"></asp:TextBox>
                                                <asp:ImageButton ID="ibSearchDateTo" Enabled="false" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                                <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="tbDateTo" runat="server" PopupButtonID="ibSearchDateTo" OnClientDateSelectionChanged="noFutureDate" />
                                                <%-- Validation --%>
                                                <asp:CompareValidator ID="CompareValidator2" runat="server" CssClass="text-danger" ControlToValidate="tbDateTo" ValidationGroup="actHis" Display="Dynamic" ErrorMessage="Date is in incorrect format!" Type="Date" Operator="DataTypeCheck"></asp:CompareValidator>
                                            </div>

                                            <asp:LinkButton ID="btnSearch" CssClass="btn btn-primary" OnClick="btnSearch_Click" ValidationGroup="actHis" runat="server"><span aria-hidden="true"  class="glyphicon glyphicon-search"></span> Search</asp:LinkButton>

                                            <div class="pull-right">
                                                <asp:LinkButton ID="btnPrint" CssClass="btn btn-default" OnClick="btnPrint_Click" runat="server"><span aria-hidden="true" class="glyphicon glyphicon-print"></span> Print</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="content_padding">
                                                    <asp:Panel ID="pnlHistory" runat="server">
                                                        <asp:GridView
                                                            ID="gvHistory"
                                                            runat="server"
                                                            OnRowDataBound="gvHistory_RowDataBound"
                                                            CssClass="table table-bordered table-striped table-responsive table-hover"
                                                            EmptyDataText="No results found for your search, please try again..."
                                                            ShowHeaderWhenEmpty="true"
                                                            AutoGenerateColumns="false"
                                                            PagerStyle-CssClass="pagination-ys"
                                                            OnPageIndexChanging="gvHistory_PageIndexChanging"
                                                            PagerSettings-Mode="NumericFirstLast"
                                                            PagerStyle-HorizontalAlign="Center"
                                                            AllowPaging="true"
                                                            PageSize="10">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Date/Time" ItemStyle-CssClass="col-xs-3">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("Date")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Date/Time" ItemStyle-CssClass="col-xs-9">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("Activity")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="btnPrint" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <!-- End .form-inline -->
                            </div>
                            <!-- End .activity history tab -->
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-link" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>


</asp:Content>
