using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;
using System.Collections;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Transactions;
using System.Text.RegularExpressions;

namespace ISSA.Pages.Operations
{
    public partial class CallInAbsent : System.Web.UI.Page
    {
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        BusinessTier.CallInAbsent CALLINABSENT = new BusinessTier.CallInAbsent();
        public enum MessageType { Success, Error, Info, Warning };

        public byte userID
        {
            get
            {
                return (byte)ViewState["userID"];
            }
            set
            {
                ViewState["userID"] = value;
            }
        }

        public int absenceID
        {
            get
            {
                return (int)ViewState["absenceID"];
            }
            set
            {
                ViewState["absenceID"] = value;
            }
        }

        public bool isLoaded
        {
            get
            {
                return (bool)ViewState["isLoaded"];
            }
            set
            {
                ViewState["isLoaded"] = value;
            }
        }

        public string[] roleNames
        {
            get
            {
                return (string[])ViewState["roleNames"];
            }
            set
            {
                ViewState["roleNames"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Get the loggedinuser ID
                userID = MISC.getUserID();
                if (userID != 0)
                {
                    //Get the logged in user role
                    roleNames = MISC.getUserRoles();
                    if (roleNames.Contains("Manager")) 
                    {
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        pageLoad();
                    }
                    else if (roleNames.Contains("Operator"))
                    {
                        //load for operator
                        pnlSearch.Visible = false;
                        pnlDetails.Visible = true;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        pageLoad();
                    }
                    else
                    {
                        ShowMessage("No ID returned for logged in user", MessageType.Error);
                        Response.Redirect("../../Pages/Account/Login.aspx");
                    }
                }
                else
                {
                    ShowMessage("No ID returned for logged in user", MessageType.Error);
                    Response.Redirect("../../Pages/Account/Login.aspx");
                }
            }
        }

        private void pageLoad()
        {
            //set the validation for the other text boxes
            ddlSickLeaveFor.Attributes.Add("onblur", "javascript:verifySickLeaveFor(" + ddlAbsenceReason.ClientID + "," + ddlSickLeaveFor.ClientID + ")");
            tbOtherAbsenceReason.Attributes.Add("onblur", "javascript:verifyAbsenceReasonOther(" + ddlAbsenceReason.ClientID + "," + tbOtherAbsenceReason.ClientID + ")");
            tbOtherSickLeaveFor.Attributes.Add("onblur", "javascript:verifySickLeaveForOther(" + ddlSickLeaveFor.ClientID + "," + tbOtherSickLeaveFor.ClientID + ")");
            tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
            tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
            tbStartDate.Attributes.Add("onblur", "javascript:validateDateFormat2(" + tbStartDate.ClientID + ",'" + DateTime.Now.Date + "')");
            tbEndDate.Attributes.Add("onblur", "javascript:validateDateFormat2(" + tbEndDate.ClientID + ",'" + DateTime.Now.Date + "')");
            tbTime.Attributes.Add("onblur", "javascript:validateTime(" + tbTime.ClientID + ")");
            resetForm();
            gvAbsence.Visible = false;
            lblGVAbsenceHeader.Text = "";
            //loadDDLShifts();
        }

        //private void loadDDLShifts()
        //{
        //    DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
        //    ddlShift.DataSource = dt;
        //    ddlShift.DataTextField = "shift";
        //    ddlShift.DataValueField = "ShiftID";
        //    ddlShift.DataBind();
        //}

        //populate the Area ddl
        private void loadDDLSearchCallTakenBy()
        {
            DataTable dt = CALLINABSENT.sp_CallTakenBy_Sel4DDL();
            ddlSearchCallTakenBy.DataSource = dt;
            ddlSearchCallTakenBy.DataTextField = "fullName";
            ddlSearchCallTakenBy.DataValueField = "EmployeeID";
            ddlSearchCallTakenBy.DataBind();
        }

        //populate the Area ddl
        private void loadDDLSearchCallForwardedTo()
        {
            DataTable dt = CALLINABSENT.sp_CallForwardedTo_Sel4DDL();
            ddlSearchCallForwardedTo.DataSource = dt;
            ddlSearchCallForwardedTo.DataTextField = "fullName";
            ddlSearchCallForwardedTo.DataValueField = "EmployeeID";
            ddlSearchCallForwardedTo.DataBind();
        }

        //populate the Area ddl
        private void loadDDLCallTakenBy()
        {
            DataTable dt = CALLINABSENT.sp_CallTakenBy_Sel4DDL();
            ddlCallTakenBy.DataSource = dt;
            ddlCallTakenBy.DataTextField = "fullName";
            ddlCallTakenBy.DataValueField = "EmployeeID";
            ddlCallTakenBy.DataBind();
        }

        //populate the Area ddl
        private void loadDDLCallForwardedTo()
        {
            DataTable dt = CALLINABSENT.sp_CallForwardedTo_Sel4DDL();
            ddlCallForwardedTo.DataSource = dt;
            ddlCallForwardedTo.DataTextField = "fullName";
            ddlCallForwardedTo.DataValueField = "EmployeeID";
            ddlCallForwardedTo.DataBind();
        }
        //populate the employee search ddl
        private void loadDDLSearchEmployee()
        {
            DataTable dt = MISC.sp_Employee_Sel4DDL();
           ddlSearchEmployee.DataSource = dt;
           ddlSearchEmployee.DataTextField = "FullName";
           ddlSearchEmployee.DataValueField = "EmployeeID";
           ddlSearchEmployee.DataBind();
        }

        //populate the location ddl based on the area selected
        private void loadDDLSickLeaveFor()
        {
            DataTable dt = CALLINABSENT.sp_SickLeaveFor_Sel4DDL();
            ddlSickLeaveFor.DataSource = dt;
            ddlSickLeaveFor.DataTextField = "LeaveFor";
            ddlSickLeaveFor.DataValueField = "SickLeaveForID";
            ddlSickLeaveFor.DataBind();
        }

        //populate the complaint mode
        private void loadDDLAbsenceReason()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_AbsentType
                           where a.IsActive == true
                           select new
                           {
                               a.Seq,
                               a.AbsentTypeID,
                               a.TypeName
                           }).OrderBy(Seq => Seq).ToList();
           ddlAbsenceReason.DataSource = results;
           ddlAbsenceReason.DataTextField = "TypeName";
           ddlAbsenceReason.DataValueField = "AbsentTypeID";
           ddlAbsenceReason.DataBind();
        }

        //populate the complaint mode
        private void loadDDLEmployees()
        {
            DataTable dt = MISC.sp_Employee_Sel4DDL();
            ddlEmployee.DataSource = dt;
            ddlEmployee.DataTextField = "FullName";
            ddlEmployee.DataValueField = "EmployeeID";
            ddlEmployee.DataBind();
        }

        //clear the entire form
        private void resetForm()
        {
            loadDDLAbsenceReason();
            loadDDLSickLeaveFor();
            loadDDLCallTakenBy();
            loadDDLCallForwardedTo();
            loadDDLEmployees();
            loadDDLSearchEmployee();
            loadDDLSearchCallTakenBy();
            loadDDLSearchCallForwardedTo();
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            string timeNow = currentDatetime.TimeOfDay.ToString();
            dateNow = dateNow.Substring(0, 10);
            tbDate.Text = dateNow;
            tbStartDate.Text = dateNow;
            tbEndDate.Text = dateNow;
            tbTime.Text = timeNow.Substring(0, 5);
            rbYes.Checked = true;
            rbNo.Checked = false;
            isLoaded = false;
            tbContactNumber.Text = "";
            tbWorkSchedule.Text = "";
            ddlAbsenceReason.SelectedIndex = 0;
            ddlSickLeaveFor.SelectedIndex = 0;
            tbComments.Text = "";
            absenceID = 0;
            generateAbsenceNumber();
            lblAuditTrail.Text = "";
            tbOtherAbsenceReason.Visible = false;
            lblOtherAbsenceReason.Visible = false;
            tbOtherSickLeaveFor.Visible = false;
            lblOthersickLeaveFor.Visible = false;
            ddlSickLeaveFor.Visible = false;
            lblSickLeaveFor.Visible = false;
            lblHeader.Text = "Add a new absent report below";
            DateTime dtNow = DateTime.Now;
            if ((MISC.checkGrantStatus(dtNow, dtNow)) || (roleNames.Contains("Manager")))
            {
                ibCalendar.Enabled = true;
                tbTime.Enabled = true;
                tbDate.Enabled = true;
            }
            else
            {
                ibCalendar.Enabled = false;
                tbTime.Enabled = false;
                tbDate.Enabled = false;
                tbStartDate.Enabled = false;
                tbEndDate.Enabled = false; 
            }
            btnSave.Visible = true;
            btnSave2.Visible = true;
            btnSaveAndNext.Visible = true;
            btnSaveAndNext2.Visible = true;
        }

        //generate the incident number
        private void generateAbsenceNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_CallIn_Absent
                           where a.Date.Value.Year == year
                           select new
                           {
                               a.CallInOffID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "CIA" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "CIA" + year.ToString() + currentSequence.ToString();
            }
        }

        protected void btnNewAbsence_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new absent report below";
            resetForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            btnBackToSearch.Visible = true;
            btnBackToSearch2.Visible = true;
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchReportNumber.Text = "";
            tbSearchDate.Text = "";
            ddlSearchCallTakenBy.SelectedIndex = 0;
            ddlSearchCallForwardedTo.SelectedIndex = 0;
            ddlSearchEmployee.SelectedIndex = 0;
            //ddlSearchArea.SelectedIndex = 0;
            //object AreaID = null;
            //if (ddlSearchArea.SelectedIndex > 0)
            //    AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            //loadDDLSearchLocations(AreaID);
            gvAbsence.Visible = false;
            lblGVAbsenceHeader.Text = "";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            resetForm();
        }
        
        protected void ddlAbsenceReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAbsenceReason.SelectedItem.Text == "Other")
            {
                lblOtherAbsenceReason.Visible = true;
                tbOtherAbsenceReason.Visible = true;
                tbOtherAbsenceReason.Focus();
            }
            else
            {
                lblOtherAbsenceReason.Visible = false;
                tbOtherAbsenceReason.Visible = false;
                tbOtherAbsenceReason.Text = "";
            }

            if (ddlAbsenceReason.SelectedItem.Text == "Illness")
            {
                lblSickLeaveFor.Visible = true;
                ddlSickLeaveFor.Visible = true;
                ddlSickLeaveFor.Focus();
            }
            else
            {
                lblSickLeaveFor.Visible = false;
                ddlSickLeaveFor.Visible = false;
                ddlSickLeaveFor.SelectedIndex = 0;
                lblOthersickLeaveFor.Visible = false;
                tbOtherSickLeaveFor.Visible = false;
                tbOtherSickLeaveFor.Text = "";
            }
        }

        protected void ddlSickLeaveFor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSickLeaveFor.SelectedItem.Text == "Other")
            {
                lblOthersickLeaveFor.Visible = true;
                tbOtherSickLeaveFor.Visible = true;
                tbOtherSickLeaveFor.Focus();
            }
            else
            {
                lblOthersickLeaveFor.Visible = false;
                tbOtherSickLeaveFor.Visible = false;
                tbOtherSickLeaveFor.Text = "";
            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            btnBackToSearch.Visible = false;
            resetForm();
            pnlSearch.Visible = true;
            lblGVAbsenceHeader.Text = "";
            if (gvAbsence.Rows.Count > 0)
                loadSearchedAbsence();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveAbsence();
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveAbsence();
                if (success)
                    resetForm();
            }
        }

        //function to save the absence, either a new insert or an update
        private bool saveAbsence()
        {
            bool success = false;

            if (isLoaded)
            {
                try
                {
                    //Update absence
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                            TimeSpan time = new TimeSpan();
                            if (TimeSpan.TryParse(tbTime.Text, out time))
                            {
                                tbTime.BorderColor = System.Drawing.Color.LightGray;
                                tbDate.BorderColor = System.Drawing.Color.LightGray;
                                date = date.Date;
                                DateTime startDate = new DateTime();
                                DateTime endDate = new DateTime();
                                startDate = Convert.ToDateTime(tbStartDate.Text);
                                endDate = Convert.ToDateTime(tbEndDate.Text);

                                Int16 callTakenBy = new Int16();
                                if (ddlCallTakenBy.SelectedIndex > 0)
                                    callTakenBy = Convert.ToInt16(ddlCallTakenBy.SelectedValue);

                                Int16 ForwardToID = new Int16();
                                if (ddlCallForwardedTo.SelectedIndex > 0)
                                    ForwardToID = Convert.ToInt16(ddlCallForwardedTo.SelectedValue);

                                byte sickLeaveForID = new byte();
                                if (ddlSickLeaveFor.SelectedIndex > 0)
                                    sickLeaveForID = Convert.ToByte(ddlSickLeaveFor.SelectedValue);

                                bool isCallIn = rbYes.Checked ? true : false;

                                CALLINABSENT.update_Absence(absenceID, isCallIn, Convert.ToInt16(ddlEmployee.SelectedValue), date, time, callTakenBy, ForwardToID, tbWorkSchedule.Text, tbContactNumber.Text, Convert.ToByte(ddlAbsenceReason.SelectedValue), tbOtherAbsenceReason.Text, sickLeaveForID, tbOtherSickLeaveFor.Text, tbComments.Text, userID, lblReportNumber.Text, startDate, endDate);

                                if (CALLINABSENT.successfulCommit)
                                {
                                    success = true;
                                    ShowMessage("Record saved.", MessageType.Success);
                                }
                                else
                                {
                                    success = false;
                                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                                }
                            }
                            else
                            {
                                success = false;
                                ShowMessage("Incorrect time format.", MessageType.Warning);
                                tbTime.BorderColor = System.Drawing.Color.Red;
                                tbTime.Focus();
                            }
                            return success;
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Warning);
                        tbDate.BorderColor = System.Drawing.Color.Red;
                        tbDate.Focus();
                        return success = false;
                    }
                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    return success = false;
                }
            }
            else
            {
                try
                {
                    //insert absence
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        DateTime startDate = new DateTime();
                        DateTime endDate = new DateTime();
                        startDate = Convert.ToDateTime(tbStartDate.Text);
                        endDate = Convert.ToDateTime(tbEndDate.Text);

                        if (!duplicateCheck_CallIn(Convert.ToInt16(ddlEmployee.SelectedValue), Convert.ToByte(ddlAbsenceReason.SelectedValue), date, startDate, endDate))
                        {
                            TimeSpan time = new TimeSpan();
                            if (TimeSpan.TryParse(tbTime.Text, out time))
                            {
                                tbTime.BorderColor = System.Drawing.Color.LightGray;
                                tbDate.BorderColor = System.Drawing.Color.LightGray;
                                date = date.Date;
                               

                                Int16 callTakenBy = new Int16();
                                if (ddlCallTakenBy.SelectedIndex > 0)
                                    callTakenBy = Convert.ToInt16(ddlCallTakenBy.SelectedValue);

                                Int16 ForwardToID = new Int16();
                                if (ddlCallForwardedTo.SelectedIndex > 0)
                                    ForwardToID = Convert.ToInt16(ddlCallForwardedTo.SelectedValue);

                                byte sickLeaveForID = new byte();
                                if (ddlSickLeaveFor.SelectedIndex > 0)
                                    sickLeaveForID = Convert.ToByte(ddlSickLeaveFor.SelectedValue);

                                bool isCallIn = rbYes.Checked ? true : false;

                                //regenerate the number in case another record was added
                                generateAbsenceNumber();


                                absenceID = CALLINABSENT.insert_Absence(lblReportNumber.Text, isCallIn, Convert.ToInt16(ddlEmployee.SelectedValue), date, time, callTakenBy, ForwardToID, tbWorkSchedule.Text, tbContactNumber.Text, Convert.ToByte(ddlAbsenceReason.SelectedValue), tbOtherAbsenceReason.Text, sickLeaveForID, tbOtherSickLeaveFor.Text, tbComments.Text, userID, startDate, endDate);

                                if (absenceID != 0)
                                {
                                    success = true;
                                    isLoaded = true;
                                    ShowMessage("Record saved.", MessageType.Success);
                                }
                                else
                                {
                                    success = false;
                                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                                }
                            }
                            else
                            {
                                success = false;
                                ShowMessage("Incorrect time format.", MessageType.Warning);
                                tbTime.BorderColor = System.Drawing.Color.Red;
                                tbTime.Focus();
                            }
                            return success;
                        }
                        else
                        {
                            ShowMessage(ddlEmployee.SelectedItem.Text + " has already been added as being " + ddlAbsenceReason.SelectedItem.Text + " on " + tbDate.Text + " or for the period " + tbStartDate.Text + " to " + tbEndDate.Text + ".", MessageType.Info);
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Warning);
                        tbDate.BorderColor = System.Drawing.Color.Red;
                        tbDate.Focus();
                        return success = false;
                    }
                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    return success = false;
                }
            }
        }

        protected void btnSearchAbsence_Click(object sender, EventArgs e)
        {
            loadSearchedAbsence();
        }

        private void loadSearchedAbsence()
        {
            DataTable dt = CALLINABSENT.sp_CallInOff_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchCallTakenBy.SelectedValue, ddlSearchCallForwardedTo.SelectedValue, ddlSearchEmployee.SelectedValue);
            if (CALLINABSENT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvAbsence.DataSource = dt;
                    gvAbsence.DataBind();
                    gvAbsence.Visible = true;
                    lblGVAbsenceHeader.Text = "Absence Reports found: " + dt.Rows.Count;
                }
                else
                {
                    gvAbsence.Visible = false;
                    lblGVAbsenceHeader.Text = "No absence reports found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvAbsence.Visible = false;
                lblGVAbsenceHeader.Text = "No absence reports found due to error.";
            }
        }

        protected void gvAbsence_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblShortComments = (Label)e.Row.FindControl("lblShortComments");
                if (lblShortComments.Text.Length > 70)
                {
                    ViewState["OrigData"] = lblShortComments.Text;
                    lblShortComments.Text = lblShortComments.Text.Substring(0, 70) + "...";
                    lblShortComments.ToolTip = ViewState["OrigData"].ToString();
                }
            }
        }

        protected void gvAbsence_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //load next set of rows on next page
            gvAbsence.PageIndex = e.NewPageIndex;
            gvAbsence.SelectedIndex = -1;
            loadSearchedAbsence();
        }

        protected void gvAbsence_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvAbsence.SelectedIndex > -1)
            {
                resetForm();
                Label lblCallInOffID = (Label)gvAbsence.SelectedRow.FindControl("lblCallInOffID");
                Label lblReportNumber2 = (Label)gvAbsence.SelectedRow.FindControl("lblReportNumber");
                Label lblIsCallIn = (Label)gvAbsence.SelectedRow.FindControl("lblIsCallIn");
                Label lblDate = (Label)gvAbsence.SelectedRow.FindControl("lblDate");
                Label lblTime = (Label)gvAbsence.SelectedRow.FindControl("lblTime");
                Label lblEmployeeID = (Label)gvAbsence.SelectedRow.FindControl("lblEmployeeID");
                Label lblCallTakenByID = (Label)gvAbsence.SelectedRow.FindControl("lblCallTakenByID");
                Label lblForwardedToID = (Label)gvAbsence.SelectedRow.FindControl("lblForwardedToID");
                Label lblWorkSchedule = (Label)gvAbsence.SelectedRow.FindControl("lblWorkSchedule");
                Label lblPhoneNumber = (Label)gvAbsence.SelectedRow.FindControl("lblPhoneNumber");
                Label lblAbsentTypeID = (Label)gvAbsence.SelectedRow.FindControl("lblAbsentTypeID");
                Label lblSickLeaveForID = (Label)gvAbsence.SelectedRow.FindControl("lblSickLeaveForID");
                Label lblComments = (Label)gvAbsence.SelectedRow.FindControl("lblComments");
                Label lblOtherTypeDescription = (Label)gvAbsence.SelectedRow.FindControl("lblOtherTypeDescription");
                Label lblOtherSickLeaveForTypeDescription = (Label)gvAbsence.SelectedRow.FindControl("lblOtherSickLeaveForTypeDescription");
                Label lblShiftID = (Label)gvAbsence.SelectedRow.FindControl("lblShiftID");
                Label lblCreatedBy = (Label)gvAbsence.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvAbsence.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvAbsence.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvAbsence.SelectedRow.FindControl("lblLastModifiedOn");
                Label lblEmployee = (Label)gvAbsence.SelectedRow.FindControl("lblEmployee");
                Label lblCallTakenBy = (Label)gvAbsence.SelectedRow.FindControl("lblCallTakenBy");
                Label lblForwardedTo = (Label)gvAbsence.SelectedRow.FindControl("lblForwardedTo");
                Label lblStartDate = (Label)gvAbsence.SelectedRow.FindControl("lblStartDate");
                Label lblEndDate = (Label)gvAbsence.SelectedRow.FindControl("lblEndDate");

                absenceID = Convert.ToInt32(lblCallInOffID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                if (lblIsCallIn.Text == "YES")
                {
                    rbYes.Checked = true;
                    rbNo.Checked = false;
                }
                else
                {
                    rbNo.Checked = true;
                    rbYes.Checked = false;
                }
                tbDate.Text = lblDate.Text;
                tbTime.Text = lblTime.Text;
                tbStartDate.Text = lblStartDate.Text == "" ? lblDate.Text : lblStartDate.Text;
                tbEndDate.Text = lblEndDate.Text == "" ? lblDate.Text : lblEndDate.Text;
                if (ddlEmployee.Items.FindByValue(lblEmployeeID.Text) != null)
                {
                    ddlEmployee.SelectedValue = lblEmployeeID.Text;
                    btnSave.Visible = true;
                    btnSave2.Visible = true;
                    btnSaveAndNext.Visible = true;
                    btnSaveAndNext2.Visible = true;
                }
                else
                {
                    ddlEmployee.SelectedIndex = 0;
                    btnSave.Visible = false;
                    btnSave2.Visible = false;
                    btnSaveAndNext.Visible = false;
                    btnSaveAndNext2.Visible = false;
                }

                tbContactNumber.Text = lblPhoneNumber.Text;
                if (ddlCallTakenBy.Items.FindByValue(lblCallTakenByID.Text) != null)
                {
                    ddlCallTakenBy.SelectedValue = lblCallTakenByID.Text;
                }
                else
                    ddlCallTakenBy.SelectedIndex = 0;
                if (ddlCallForwardedTo.Items.FindByValue(lblForwardedToID.Text) != null)
                {
                    ddlCallForwardedTo.SelectedValue = lblForwardedToID.Text;
                }
                else
                {
                    ddlCallForwardedTo.SelectedIndex = 0;
                }
                tbWorkSchedule.Text = lblWorkSchedule.Text;
                ddlAbsenceReason.SelectedValue = lblAbsentTypeID.Text;
                tbOtherAbsenceReason.Text = lblOtherTypeDescription.Text;
                ddlSickLeaveFor.SelectedValue = lblSickLeaveForID.Text;
                tbOtherSickLeaveFor.Text = lblOtherSickLeaveForTypeDescription.Text;
                tbComments.Text = lblComments.Text;
                //ddlShift.SelectedValue = lblShiftID.Text;
                lblHeader.Text = "Update the selected call in report below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;
                isLoaded = true;
                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                btnBackToSearch.Visible = true;
                btnBackToSearch2.Visible = true;

                if (ddlAbsenceReason.SelectedItem.Text == "Other")
                {
                    lblOtherAbsenceReason.Visible = true;
                    tbOtherAbsenceReason.Visible = true;
                    tbOtherAbsenceReason.Focus();
                }
                else
                {
                    lblOtherAbsenceReason.Visible = false;
                    tbOtherAbsenceReason.Visible = false;
                    tbOtherAbsenceReason.Text = "";
                }

                if (ddlAbsenceReason.SelectedItem.Text == "Illness")
                {
                    lblSickLeaveFor.Visible = true;
                    ddlSickLeaveFor.Visible = true;
                    ddlSickLeaveFor.Focus();
                }
                else
                {
                    lblSickLeaveFor.Visible = false;
                    ddlSickLeaveFor.Visible = false;
                    ddlSickLeaveFor.SelectedIndex = 0;
                    lblOthersickLeaveFor.Visible = false;
                    tbOtherSickLeaveFor.Visible = false;
                    tbOtherSickLeaveFor.Text = "";
                }

                if (ddlSickLeaveFor.SelectedItem.Text == "Other")
                {
                    lblOthersickLeaveFor.Visible = true;
                    tbOtherSickLeaveFor.Visible = true;
                    tbOtherSickLeaveFor.Focus();
                }
                else
                {
                    lblOthersickLeaveFor.Visible = false;
                    tbOtherSickLeaveFor.Visible = false;
                    tbOtherSickLeaveFor.Text = "";
                }
            }
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        public bool duplicateCheck_CallIn(Int16 empID, short absentType, DateTime date, DateTime startDate, DateTime endDate)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_CallIn_Absent
                           where a.Date == date && a.EmployeeID == empID && a.AbsentTypeID == absentType && startDate  >= a.StartDate && endDate <= a.EndDate
                           select new
                           {
                               a.EmployeeID
                           }).ToList();

            if (results.Count > 0)
            {
                isDuplicate = true;
            }
            else
            {
                isDuplicate = false;
            }
            return isDuplicate;
        }

    }
}