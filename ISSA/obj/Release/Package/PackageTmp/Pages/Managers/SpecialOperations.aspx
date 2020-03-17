<%@ Page Title="Special Operations" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SpecialOperations.aspx.cs" Inherits="ISSA.Pages.Managers.SpecialOperations" MaintainScrollPositionOnPostback="true" %>

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
                        control.style.borderColor = "red";
                        control.value = endDate;
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
            <h4>SPECIAL OPERATIONS LOG
                    <br />
                <small>Use the form below to manage special operations.
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
                                    <label class="control-label" for="ddlSearchArea">Area</label>
                                    <asp:DropDownList ID="ddlSearchArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchArea_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlSearchLocation">Location</label>
                                    <asp:DropDownList ID="ddlSearchLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbSearchDate">Date</label>
                                    <asp:ImageButton ID="ibSearchDate" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox ID="tbSearchDate" runat="server" CssClass="form-control" placeholder="mm/dd/yyyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="tbSearchDate" runat="server" PopupButtonID="ibSearchDate" OnClientDateSelectionChanged="noFutureDate" />
                                     </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlEscortService">Escort Service</label>
                                    <asp:DropDownList ID="ddlEscortService" CssClass="form-control" runat="server">
                                        <asp:ListItem Text="...Select..." Value=""></asp:ListItem>
                                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlCITinGT">Case in Transit</label>
                                    <asp:DropDownList ID="ddlCITinGT" CssClass="form-control" runat="server">
                                        <asp:ListItem Text="...Select..." Value=""></asp:ListItem>
                                        <asp:ListItem Text="In GT" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Out GT" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlExtraOfficers">Extra Officers</label>
                                    <asp:DropDownList ID="ddlExtraOfficers" CssClass="form-control" runat="server">
                                        <asp:ListItem Text="...Select..." Value=""></asp:ListItem>
                                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6 col-xs-6">
                            <asp:LinkButton ID="btnNewSO" runat="server" CssClass="btn btn-primary" Text="Log New SO" OnClick="btnNewSO_Click">New SO <span aria-hidden="true" class="glyphicon glyphicon-plus"></span></asp:LinkButton>
                        </div>
                        <div class="col-sm-6 col-xs-6 text-right">
                            <asp:LinkButton ID="btnSearchSO" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchSO_Click">Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                            <asp:LinkButton ID="lbClearSearch" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="lbClearSearch_Click">Clear <span aria-hidden="true" class="glyphicon glyphicon-erase"></span></asp:LinkButton>

                        </div>
                    </div>
                    <br />
                    <div class="row pull-left">
                        <div class="col-sm-12 col-xs-12">
                            <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVHeader" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12 col-xs-12">
                            <asp:GridView ID="gvSO"
                                runat="server"
                                AutoGenerateColumns="false"
                                AllowPaging="true" PageSize="30"
                                AllowSorting="false"
                                ShowFooter="true"
                                ShowHeaderWhenEmpty="true"
                                CssClass="table table-bordered table-striped  table-hover"
                                HeaderStyle-CssClass="text-center"
                                OnRowDataBound="gvSO_RowDataBound"
                                OnSelectedIndexChanged="gvSO_SelectedIndexChanged"
                                OnPageIndexChanging="gvSO_PageIndexChanging">
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
                                    <asp:TemplateField HeaderText="Location">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocationName" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Escort Service">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEscortService" runat="server" Text='<%#Eval("EscortService")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CIT in GT">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCashInGT" runat="server" Text='<%#Eval("CashInGT")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CIT out GT">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCashOutGT" runat="server" Text='<%#Eval("CashOutGT")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Extra Officers">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExtraOfficersAmount" runat="server" Text='<%#Eval("ExtraOfficersAmount")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comments">
                                        <ItemTemplate>
                                            <asp:Label ID="lblShortCommnets" runat="server" Text='<%#Eval("Comments")%>'></asp:Label>
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
                                            <asp:Label ID="lblSpecialOperationsID" runat="server" Text='<%#Eval("SpecialOperationsID")%>'></asp:Label>
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
                                            <asp:Label ID="lblExtraOfficers" runat="server" Text='<%#Eval("ExtraOfficers")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmountOfficers" runat="server" Text='<%#Eval("AmountOfficers")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComments" runat="server" Text='<%#Eval("Comments")%>'></asp:Label>
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
                <%--                <h4>
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
                                    <asp:Label ID="lblReportNumber" runat="server" ForeColor="#003a75" Font-Bold="true" Text="" Font-Size="Medium" Font-Underline="true"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="tbDate">Date* </label>
                                    <asp:ImageButton ID="ibCalendar" ImageUrl="~/Images/calendar.png" runat="server" AlternateText="Calendar" Width="20px" Height="20px" ToolTip="Click here to change the date" ImageAlign="AbsMiddle" />
                                    <asp:TextBox CssClass="form-control" ID="tbDate" ValidationGroup="vgSO" runat="server" Enabled="false" />
                                    <asp:CalendarExtender ID="ceDate" TargetControlID="tbDate" runat="server" PopupButtonID="ibCalendar" OnClientDateSelectionChanged="noFutureDate" />
                                    <asp:RequiredFieldValidator ID="rfvDate" ControlToValidate="tbDate" runat="server" ErrorMessage="Date is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgSO"></asp:RequiredFieldValidator>
                                </div>
                                 <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlShift">Shift</label>
                                    <asp:DropDownList ID="ddlShift" CssClass="form-control" runat="server"></asp:DropDownList>
                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="ddlShift" runat="server" ErrorMessage="Shift is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgSO"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlArea">Area</label>
                                    <asp:DropDownList ID="ddlArea" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddlArea" runat="server" ErrorMessage="Area is required." ForeColor="Red" Display="Dynamic" ValidationGroup="vgSO"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="ddlLocation">Location</label>
                                    <asp:DropDownList ID="ddlLocation" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-4 col-xs-12">
                                    <label class="control-label">Escort Service </label>
                                    <asp:RadioButton ID="rbESYes" CssClass="radio-inline" GroupName="ES" runat="server" Text="YES" />
                                    <asp:RadioButton ID="rbESNo" CssClass="radio-inline" GroupName="ES" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label">Cash-in-transit In GT</label>
                                    <asp:RadioButton ID="rbCITGTYes" CssClass="radio-inline" GroupName="CITGT" runat="server" Text="YES" />
                                    <asp:RadioButton ID="rbCITGTNo" CssClass="radio-inline" GroupName="CITGT" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label">Cash-in-transit Out GT</label>
                                    <asp:RadioButton ID="rbCIToGTYes" CssClass="radio-inline" GroupName="CIToGT" runat="server" Text="YES" />
                                    <asp:RadioButton ID="rbCIToGTNo" CssClass="radio-inline" GroupName="CIToGT" runat="server" Text="NO" />
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label">Extra Officers </label>
                                    <asp:RadioButton ID="rbEOYes" CssClass="radio-inline" GroupName="EO" runat="server" Text="YES" AutoPostBack="true" OnCheckedChanged="rbEOYes_CheckedChanged" />
                                    <asp:RadioButton ID="rbEONo" CssClass="radio-inline" GroupName="EO" runat="server" Text="NO" AutoPostBack="true" OnCheckedChanged="rbEONo_CheckedChanged" />
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="tbAmount" id="lblAmount" runat="server">Amount</label>
                                    <asp:TextBox CssClass="form-control" ID="tbAmount" runat="server" MaxLength="3" placeholder="254 max" />
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">
                                    <label class="control-label" for="tbComments">Comments </label>
                                    <asp:TextBox CssClass="form-control" MaxLength="2000" Height="100px" ID="tbComments" runat="server" TextMode="MultiLine" placeholder="2000 characters max" />
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
                                    <asp:LinkButton ID="btnSave" runat="server" ValidationGroup="vgSO" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click">Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                    <asp:LinkButton ID="btnSaveAndNext" runat="server" ValidationGroup="vgSO" CssClass="btn btn-primary" Text="Save & Next" OnClick="btnSaveAndNext_Click">Save & Next <span aria-hidden="true" class="glyphicon glyphicon-floppy-open"></span></asp:LinkButton>
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
