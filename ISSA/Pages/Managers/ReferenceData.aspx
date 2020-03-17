<%@ Page Title="Reference Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReferenceData.aspx.cs" Inherits="ISSA.Pages.Managers.ReferenceData" MaintainScrollPositionOnPostback="true" %>
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
             else
             {
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

    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h4>RERFERENCE DATA
                    <br />
        <small>Use the form below to manage reference data.
        </small>
    </h4>
    <hr />
     <asp:Panel ID="pnlDetails" runat="server" HorizontalAlign="Left">
        <ul class="nav nav-tabs" id="myTab">
            <li class="active"><a data-toggle="tab" href="#Area">Area</a></li>
            <li><a data-toggle="tab" href="#Department">Department</a></li>
            <li><a data-toggle="tab" href="#Location">Location</a></li>
            <li><a data-toggle="tab" href="#Position">Position</a></li>
            <li><a data-toggle="tab" href="#Shift">Shift</a></li>
        </ul>

        <%-- Content --%>
        <div class="tab-content">
            <div id="Area" class="tab-pane fade in active">
                <br />
                <asp:UpdatePanel ID="upArea" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbArea">Area:</label>
                                    <asp:TextBox ID="tbArea" runat="server" CssClass="form-control" placeholder="100 characters max" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="tbArea" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Area"  ></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label">Is Active:</label><br />
                                   <asp:RadioButton ID="rbAreaYes" CssClass="radio-inline" GroupName="Area" runat="server" Text="YES"/>
                                    <asp:RadioButton ID="rbAreaNo" CssClass="radio-inline" GroupName="Area" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-3 col-xs-6 text-left">
                                     <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnAreaSearch" runat="server" CssClass="btn btn-default" OnClick="btnAreaSearch_Click" >Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                   <asp:LinkButton ID="btnAreaSave" runat="server" CssClass="btn btn-primary" ValidationGroup="Area" OnClick="btnAreaSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-2 col-xs-6 text-right">
                                    <asp:Label ID="lblAreaLoaded" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblLoadedAreaID" runat="server" Text="" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row pull-left">
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVAreaHeader" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">         
                                    <asp:GridView ID="gvAreas"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowDataBound="gvAreas_RowDataBound"
                                        OnPageIndexChanging="gvAreas_PageIndexChanging"
                                        OnSelectedIndexChanged="gvAreas_SelectedIndexChanged">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Area">
                                                <ItemTemplate>
                                                     <asp:LinkButton ID="lbArea" runat="server" CommandName="Select">
                                                    <asp:Label ID="lblArea" runat="server" Text='<%#Eval("Area")%>'></asp:Label>
                                                     </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Is Active">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIsActive" runat="server" Text='<%#Eval("IsActive")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedBy" runat="server" Text='<%#Eval("LastModifiedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedOn" runat="server" Text='<%#Eval("LastModifiedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAreaID" runat="server" Text='<%#Eval("AreaID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="gvAreas"/>
                        <asp:AsyncPostBackTrigger ControlID="btnAreaSearch" />
                        <asp:AsyncPostBackTrigger ControlID="btnAreaSave" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>


             <div id="Department" class="tab-pane fade in">
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbDepartment">Department:</label>
                                    <asp:TextBox ID="tbDepartment" runat="server" CssClass="form-control" placeholder="30 characters max" MaxLength="30" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tbDepartment" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Department"></asp:RequiredFieldValidator>
                                </div>
                                  <div class="col-sm-3 col-xs-6">
                                    <label class="control-label">Is Active:</label><br />
                                   <asp:RadioButton ID="rbDepartmentYes" CssClass="radio-inline" GroupName="Department" runat="server" Text="YES"/>
                                    <asp:RadioButton ID="rbDepartmentNo" CssClass="radio-inline" GroupName="Department" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-3 col-xs-6 text-left">
                                    <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnDepartmentSearch" runat="server" CssClass="btn btn-default" OnClick="btnDepartmentSearch_Click" >Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                   <asp:LinkButton ID="btnDepartmentSave" runat="server" CssClass="btn btn-primary" ValidationGroup="Department" OnClick="btnDepartmentSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-2 col-xs-6 text-right">
                                    <asp:Label ID="lblDepartmentLoaded" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblLoadedDepartmentID" runat="server" Text="" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row pull-left">
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVDepartmentHeader" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">         
                                    <asp:GridView ID="gvDepartment"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowDataBound="gvDepartment_RowDataBound"
                                        OnPageIndexChanging="gvDepartment_PageIndexChanging"
                                        OnSelectedIndexChanged="gvDepartment_SelectedIndexChanged">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Department">
                                                <ItemTemplate>
                                                     <asp:LinkButton ID="lbDepartment" runat="server" CommandName="Select">
                                                    <asp:Label ID="lblDepartment" runat="server" Text='<%#Eval("Department")%>'></asp:Label>
                                                     </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Is Active">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIsActive" runat="server" Text='<%#Eval("IsActive")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedBy" runat="server" Text='<%#Eval("LastModifiedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedOn" runat="server" Text='<%#Eval("LastModifiedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDepartmentID" runat="server" Text='<%#Eval("DepartmentID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="gvDepartment"/>
                        <asp:AsyncPostBackTrigger ControlID="btnDepartmentSearch" />
                        <asp:AsyncPostBackTrigger ControlID="btnDepartmentSave" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
           
            <div id="Location" class="tab-pane fade in">
                <br />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="ddlArea">Area:</label>
                                   <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="ddlArea" runat="server" ValidationGroup="Location" ErrorMessage="Required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbLocation">Location:</label>
                                    <asp:TextBox ID="tbLocation" runat="server" CssClass="form-control" placeholder="100 characters max" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="tbLocation" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Location"></asp:RequiredFieldValidator>
                                </div>
                                 <div class="col-sm-2 col-xs-6">
                                    <label class="control-label">Is Active:</label><br />
                                   <asp:RadioButton ID="rbLocationYes" CssClass="radio-inline" GroupName="Location" runat="server" Text="YES"/>
                                    <asp:RadioButton ID="rbLocationNo" CssClass="radio-inline" GroupName="Location" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-3 col-xs-6 text-left">
                                    <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnLocationSearch" runat="server" CssClass="btn btn-default" OnClick="btnLocationSearch_Click" >Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                   <asp:LinkButton ID="btnLocationSave" runat="server" CssClass="btn btn-primary" ValidationGroup="Location" OnClick="btnLocationSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-1 col-xs-1 text-right">
                                    <asp:Label ID="lblLocationLoaded" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblLoadedLocationID" runat="server" Text="" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row pull-left">
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVLocationHeader" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">         
                                    <asp:GridView ID="gvLocation"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowDataBound="gvLocation_RowDataBound"
                                        OnPageIndexChanging="gvLocation_PageIndexChanging"
                                        OnSelectedIndexChanged="gvLocation_SelectedIndexChanged">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Area">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblArea" runat="server" Text='<%#Eval("Area")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Location">
                                                <ItemTemplate>
                                                     <asp:LinkButton ID="lbLocation" runat="server" CommandName="Select">
                                                    <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("LocationName")%>'></asp:Label>
                                                     </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Is Active">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIsActive" runat="server" Text='<%#Eval("IsActive")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedBy" runat="server" Text='<%#Eval("LastModifiedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedOn" runat="server" Text='<%#Eval("LastModifiedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLocationID" runat="server" Text='<%#Eval("LocationID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAreaID" runat="server" Text='<%#Eval("AreaID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="gvLocation"/>
                        <asp:AsyncPostBackTrigger ControlID="btnLocationSearch" />
                        <asp:AsyncPostBackTrigger ControlID="btnLocationSave" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

             <div id="Position" class="tab-pane fade in">
                <br />
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-4 col-xs-6">
                                    <label class="control-label" for="tbPosition">Position:</label>
                                    <asp:TextBox ID="tbPosition" runat="server" CssClass="form-control" placeholder="30 characters max" MaxLength="30"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="tbPosition" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Position"></asp:RequiredFieldValidator>
                                </div>
                                  <div class="col-sm-3 col-xs-6">
                                    <label class="control-label">Is Active:</label><br />
                                   <asp:RadioButton ID="rbPositionYes" CssClass="radio-inline" GroupName="Position" runat="server" Text="YES"/>
                                    <asp:RadioButton ID="rbPositionNo" CssClass="radio-inline" GroupName="Position" runat="server" Text="NO" />
                                </div>
                                <div class="col-sm-3 col-xs-6 text-left">
                                    <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnPositionSearch" runat="server" CssClass="btn btn-default" OnClick="btnPositionSearch_Click" >Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                   <asp:LinkButton ID="btnPositionSave" runat="server" CssClass="btn btn-primary" ValidationGroup="Position" OnClick="btnPositionSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                </div>
                                <div class="col-sm-2 col-xs-6 text-right">
                                    <asp:Label ID="lblPositionLoaded" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblLoadedPositionID" runat="server" Text="" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row pull-left">
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVPositionHeader" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">         
                                    <asp:GridView ID="gvPosition"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowDataBound="gvPosition_RowDataBound"
                                        OnPageIndexChanging="gvPosition_PageIndexChanging"
                                        OnSelectedIndexChanged="gvPosition_SelectedIndexChanged">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Position">
                                                <ItemTemplate>
                                                     <asp:LinkButton ID="lbPosition" runat="server" CommandName="Select">
                                                    <asp:Label ID="lblPosition" runat="server" Text='<%#Eval("PositionName")%>'></asp:Label>
                                                     </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Is Active">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIsActive" runat="server" Text='<%#Eval("IsActive")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedBy" runat="server" Text='<%#Eval("LastModifiedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedOn" runat="server" Text='<%#Eval("LastModifiedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPositionID" runat="server" Text='<%#Eval("PositionID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="gvPosition"/>
                        <asp:AsyncPostBackTrigger ControlID="btnPositionSearch" />
                        <asp:AsyncPostBackTrigger ControlID="btnPositionSave" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <div id="Shift" class="tab-pane fade in">
                <br />
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-sm-3 col-xs-6">
                                    <label class="control-label" for="ddlArea">Position:</label>
                                   <asp:DropDownList ID="ddlPosition" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="ddlPosition" runat="server" ValidationGroup="Shift" ErrorMessage="Required." ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbFromTime">From:</label>
                                    <asp:TextBox ID="tbFromTime" runat="server" CssClass="form-control" placeholder="24 hrs format"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ControlToValidate="tbFromTime" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Shift"></asp:RequiredFieldValidator>
                                     </div>
                                <div class="col-sm-2 col-xs-6">
                                    <label class="control-label" for="tbToTime">To:</label>
                                    <asp:TextBox ID="tbToTime" runat="server" CssClass="form-control" placeholder="24 hrs format"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" ControlToValidate="tbToTime" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Shift"></asp:RequiredFieldValidator>
                                </div>
                                 <div class="col-sm-2 col-xs-6">
                                    <label class="control-label">Hours in Shift:</label><br />
                                    <asp:TextBox ID="tbHoursInShift" runat="server" CssClass="form-control" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="tbHoursInShift" runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="Shift"></asp:RequiredFieldValidator>
                                </div>
                                 <div class="col-sm-3 col-xs-6">
                                    <label class="control-label">Is Active:</label><br />
                                   <asp:RadioButton ID="rbShiftYes" CssClass="radio-inline" GroupName="Shift" runat="server" Text="YES"/>
                                    <asp:RadioButton ID="rbShiftNo" CssClass="radio-inline" GroupName="Shift" runat="server" Text="NO" />
                                </div>
                                </div>
                            <div class ="row">
                                 <div class="col-sm-5 col-xs-1 text-right">
                                    <asp:Label ID="lblShiftLoaded" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblLoadedShiftID" runat="server" Text="" Visible="false"></asp:Label>
                                      <asp:Label ID="lblShiftName" runat="server" Text="" Visible="false"></asp:Label>
                                </div>
                                <div class="col-sm-7 col-xs-6 text-right">
                                    <label class="control-label">&nbsp;</label><br />
                                    <asp:LinkButton ID="btnShiftSearch" runat="server" CssClass="btn btn-default" OnClick="btnShiftSearch_Click" >Search <span aria-hidden="true" class="glyphicon glyphicon-search"></span></asp:LinkButton>
                                   <asp:LinkButton ID="btnShiftSave" runat="server" CssClass="btn btn-primary" ValidationGroup="Shift" OnClick="btnShiftSave_Click" >Save <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span></asp:LinkButton>
                                </div>
                            </div>
                            <br />
                            <div class="row pull-left">
                                <div class="col-sm-12 col-xs-12">
                                    <asp:Label CssClass="control-label" Font-Bold="true" Font-Size="Medium" ID="lblGVShiftHeader" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-xs-12">         
                                    <asp:GridView ID="gvShift"
                                        runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true" PageSize="20"
                                        AllowSorting="false"
                                        ShowFooter="true"
                                        ShowHeaderWhenEmpty="true"
                                        CssClass="table table-bordered table-striped  table-hover"
                                        HeaderStyle-CssClass="text-center"
                                        OnRowDataBound="gvShift_RowDataBound"
                                        OnPageIndexChanging="gvShift_PageIndexChanging"
                                        OnSelectedIndexChanged="gvShift_SelectedIndexChanged">
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="#" ItemStyle-Width="25px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNo" runat="server" Text='<%#Container.DataItemIndex + 1%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Position">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPosition" runat="server" Text='<%#Eval("PositionName")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Shift">
                                                <ItemTemplate>
                                                     <asp:LinkButton ID="lbShift" runat="server" CommandName="Select">
                                                    <asp:Label ID="lblShift" runat="server" Text='<%#Eval("Shift")%>'></asp:Label>
                                                     </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Is Active">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIsActive" runat="server" Text='<%#Eval("IsActive")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%#Eval("CreatedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedOn" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedBy" runat="server" Text='<%#Eval("LastModifiedBy")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Modified On">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLastModifiedOn" runat="server" Text='<%#Eval("LastModifiedOn")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblShiftID" runat="server" Text='<%#Eval("ShiftID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPositionID" runat="server" Text='<%#Eval("PositionID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFromTime" runat="server" Text='<%#Eval("FromTime")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblToTime" runat="server" Text='<%#Eval("ToTime")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHoursInShift" runat="server" Text='<%#Eval("HoursInShift")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="gvShift"/>
                        <asp:AsyncPostBackTrigger ControlID="btnShiftSearch" />
                        <asp:AsyncPostBackTrigger ControlID="btnShiftSave" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
