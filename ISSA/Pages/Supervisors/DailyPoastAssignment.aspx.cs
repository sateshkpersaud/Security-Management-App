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
using System.Threading;

namespace ISSA.Pages.Supervisors
{
    public partial class DailyPoastAssignment : System.Web.UI.Page
    {
        BusinessTier.DailyPostAssignment DPA = new BusinessTier.DailyPostAssignment();
        BusinessTier.Misc MISC = new BusinessTier.Misc();
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

        public int postAssignmentID
        {
            get
            {
                return (int)ViewState["postAssignmentID"];
            }
            set
            {
                ViewState["postAssignmentID"] = value;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                userID = MISC.getUserID();
                if (userID != 0)
                {
                    string[] roleNames = MISC.getUserRoles();
                    if ((roleNames.Contains("Manager")) || (roleNames.Contains("Supervisor")))
                    {
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        clearForm();
                        tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbCallIns.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbCallOuts.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbCallsForAssistance.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbLateArrivals.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbNotDressedProperly.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbOfficersAbsent.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbOfficersPresent.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbOfficersAssigned.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbOfficersOnDayOff.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbSleeping.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                        tbCallIns.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbCallIns.ClientID + ")");
                        tbCallOuts.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbCallOuts.ClientID + ")");
                        tbCallsForAssistance.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbCallsForAssistance.ClientID + ")");
                        tbLateArrivals.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbLateArrivals.ClientID + ")");
                        tbNotDressedProperly.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbNotDressedProperly.ClientID + ")");
                        tbOfficersAbsent.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbOfficersAbsent.ClientID + ")");
                        tbOfficersPresent.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbOfficersPresent.ClientID + ")");
                        tbOfficersAssigned.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbOfficersAssigned.ClientID + ")");
                        tbOfficersOnDayOff.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbOfficersOnDayOff.ClientID + ")");
                        tbSleeping.Attributes.Add("onblur", "javascript:validateMaxNumber(" + tbOfficersOnDayOff.ClientID + ")");
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

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        private void loadDDLSearchShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlSearchShift.DataSource = dt;
            ddlSearchShift.DataTextField = "shift";
            ddlSearchShift.DataValueField = "ShiftID";
            ddlSearchShift.DataBind();
        }

        private void loadShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new post below";
            clearForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            ddlShift.Enabled = true;
        }

        private void clearForm()
        {
            loadDDLSearchShifts();
            loadShifts();
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            dateNow = dateNow.Substring(0, 10);
            tbDate.Text = dateNow;
            gvResults.Visible = false;
            ddlShift.SelectedIndex = 0;
            lblGVResultsHeader.Text = "";
            lblReportNumber.Text = "";
            postAssignmentID = 0;
            isLoaded = false;
            generateReportNumber();
            ddlShift.SelectedIndex = 0;
            tbLateArrivals.Text = "";
            tbCallsForAssistance.Text = "";
            tbOfficersAssigned.Text = "";
            tbOfficersPresent.Text = "";
            tbOfficersAbsent.Text = "";
            tbCallIns.Text = "";
            tbCallOuts.Text = "";
            tbOfficersOnDayOff.Text = "";
            tbNotDressedProperly.Text = "";
            gvRoster.Visible = false;
            tbShiftSummary.Text = "";
            tbShiftBriefingNotes.Text = "";
            tbPassedOnFrom.Text = "";
            tbPassedOnTo.Text = "";
            tbIncidentsSummary.Text = "";
            tbSleeping.Text = "";
            ddlShift.Enabled = true;
            ibDate.Enabled = true;
            lblAuditTrail.Text = "";
        }

        //generate the incident number
        private void generateReportNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_DailyPostAssignment
                           where a.Date.Value.Year == year
                           select new
                           {
                               a.PostAssignmentID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "DPA" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "DPA" + year.ToString() + currentSequence.ToString();
            }
        }

        
        private void populateTextBoxes()
        {
            int officersScheduled = DPA.sp_DailyPostStats_OfficersScheduled(tbDate.Text, ddlShift.SelectedValue);
            int officersPresent = DPA.sp_DailyPostStats_OfficersPresent(tbDate.Text, ddlShift.SelectedValue);
            int officersCallOutToWork = DPA.sp_DailyPostStats_OfficersCalledInOnDayOff(tbDate.Text, ddlShift.SelectedValue);
            int officersCallInForDayOff = DPA.sp_DailyPostStats_OfficersCalledInForDayOff(tbDate.Text, ddlShift.SelectedValue);
            //int officersAbsent = DPA.sp_DailyPostStats_OfficersAbsent(tbDate.Text, ddlShift.SelectedValue);
            int officersOnDayOff = DPA.sp_DailyPostStats_OfficersOnDayOff(tbDate.Text, ddlShift.SelectedValue);
           int officersAbsent = officersScheduled - officersPresent;
            tbOfficersAssigned.Text = officersScheduled.ToString();
            tbOfficersPresent.Text = officersPresent.ToString();
            tbOfficersAbsent.Text = officersAbsent.ToString();
            tbLateArrivals.Text = "0";
            tbCallIns.Text = officersCallInForDayOff.ToString();
            tbCallOuts.Text = officersCallOutToWork.ToString();
            tbCallsForAssistance.Text = "0";
            tbLateArrivals.Text = "0";
            tbNotDressedProperly.Text = "0";
            tbSleeping.Text = "0";
            tbOfficersOnDayOff.Text = officersOnDayOff.ToString();
        }

        private void loadMasterRoster()
        {
            DataTable dt = DPA.sp_DailyPostStats_Masterroster(tbDate.Text, ddlShift.SelectedValue);

            if (DPA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvRoster.DataSource = dt;
                    gvRoster.DataBind();
                    gvRoster.Visible = true;
                    lblRoster.Visible = true;
                }
                else
                {
                    gvRoster.Visible = false;
                    lblRoster.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and the daily master roster was not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvRoster.Visible = false;
                lblRoster.Visible = false;
            }
        }

        protected void tbDate_TextChanged(object sender, EventArgs e)
        {
            if ((tbDate.Text != "") && (ddlShift.SelectedIndex >0))
            {
                DateTime date = new DateTime();
                DateTime.TryParse(tbDate.Text, out date);
                date = date.Date;
                if (!duplicateCheck(date, Convert.ToByte(ddlShift.SelectedValue)))
                {
                    populateTextBoxes();
                    loadMasterRoster();
                }
                else
                {
                    ShowMessage("A Daily Post Assignment Roster already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                    clearForm();
                }
            }
        }

        protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tbDate.Text != "") && (ddlShift.SelectedIndex > 0))
            {
                DateTime date = new DateTime();
                DateTime.TryParse(tbDate.Text, out date);
                date = date.Date;
                if (!duplicateCheck(date, Convert.ToByte(ddlShift.SelectedValue)))
                {
                    populateTextBoxes();
                    loadMasterRoster();
                }
                else
                {
                    ShowMessage("A Daily Post Assignment Roster already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                    clearForm();
                }
            }
        }

        protected void gvRoster_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRoster.PageIndex = e.NewPageIndex;
            gvRoster.SelectedIndex = -1;
            loadMasterRoster();
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            clearForm();
            pnlSearch.Visible = true;
            lblGVResultsHeader.Text = "";
            if (gvResults.Rows.Count > 0)
            {
                loadSearchedResults();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearForm();
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchReportNumber.Text = "";
            tbSearchDate.Text = "";
            ddlSearchShift.SelectedIndex = 0;
            gvResults.Visible = false;
        }

        public bool duplicateCheck(DateTime date, byte shiftID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_DailyPostAssignment
                           where a.Date == date && a.ShiftID == shiftID
                           select new
                           {
                               a.PostAssignmentID
                           }).ToList();

            if (results.Count > 0)
            {
                foreach (var val in results)
                {
                    postAssignmentID = val.PostAssignmentID;
                }
                isDuplicate = true;
            }
            else
            {
                isDuplicate = false;
            }
            return isDuplicate;
        }

        private bool savePostAssignment()
        {
            bool success = false;

            if (isLoaded)
            {
                try
                {
                    //Update shift report
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        tbDate.BorderColor = System.Drawing.Color.LightGray;
                        date = date.Date;
                        DPA.update_dailyPostAssignment(postAssignmentID, date, Convert.ToByte(ddlShift.SelectedValue), Convert.ToInt16(tbLateArrivals.Text), Convert.ToInt16(tbCallsForAssistance.Text), Convert.ToInt16(tbOfficersPresent.Text), Convert.ToInt16(tbOfficersAbsent.Text), Convert.ToInt16(tbOfficersAssigned.Text), Convert.ToInt16(tbCallOuts.Text), Convert.ToInt16(tbCallIns.Text), Convert.ToInt16(tbOfficersOnDayOff.Text), Convert.ToInt16(tbNotDressedProperly.Text), tbShiftSummary.Text, tbShiftBriefingNotes.Text, tbPassedOnFrom.Text, tbPassedOnTo.Text, tbIncidentsSummary.Text, lblReportNumber.Text, userID, Convert.ToInt16(tbSleeping.Text));
                        if (DPA.successfulCommit)
                        {
                            success = true;
                        }
                        else
                        {
                            success = false;
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                        }
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Warning);
                        tbDate.BorderColor = System.Drawing.Color.Red;
                        tbDate.Focus();
                        success = false;
                    }
                }
                catch (System.Exception excec)
                {
                    success = false;
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    //return success = false;
                }
            }
            else
            {
                try
                {
                    //insert incident
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        tbDate.BorderColor = System.Drawing.Color.LightGray;
                        date = date.Date;
                        if (!duplicateCheck(date, Convert.ToByte(ddlShift.SelectedValue)))
                        {
                            //regenerate the number in case another record was added
                            generateReportNumber();

                            postAssignmentID = DPA.insert_DailyPostAssignment(date, Convert.ToByte(ddlShift.SelectedValue), Convert.ToInt16(tbLateArrivals.Text), Convert.ToInt16(tbCallsForAssistance.Text), Convert.ToInt16(tbOfficersPresent.Text), Convert.ToInt16(tbOfficersAbsent.Text), Convert.ToInt16(tbOfficersAssigned.Text), Convert.ToInt16(tbCallOuts.Text), Convert.ToInt16(tbCallIns.Text), Convert.ToInt16(tbOfficersOnDayOff.Text), Convert.ToInt16(tbNotDressedProperly.Text), tbShiftSummary.Text, tbShiftBriefingNotes.Text, tbPassedOnFrom.Text, tbPassedOnTo.Text, tbIncidentsSummary.Text, lblReportNumber.Text, userID, Convert.ToInt16(tbSleeping.Text));

                            if (postAssignmentID != 0)
                            {
                                success = true;
                                isLoaded = true;
                            }
                            else
                            {
                                success = false;
                                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                            }
                        }
                        else
                        {
                            ShowMessage("A Daily Post Assignment Roster already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                        }
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Warning);
                        tbDate.BorderColor = System.Drawing.Color.Red;
                        tbDate.Focus();
                        success = false;
                    }
                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    success = false;
                }
            }
            return success;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = savePostAssignment();
                if (success)
                {
                    ddlShift.Enabled = false;
                    ibDate.Enabled = false;
                    ShowMessage("Record saved", MessageType.Success);
                }
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = savePostAssignment();
                if (success)
                {
                    clearForm();
                    ShowMessage("Record saved", MessageType.Success);
                }
            }
        }

        private void loadSearchedResults()
        {
            DataTable dt = DPA.sp_DailyPostAssignment_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchShift.SelectedValue);
            if (DPA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvResults.DataSource = dt;
                    gvResults.DataBind();
                    gvResults.Visible = true;
                    lblGVResultsHeader.Text = "Daily Post Assignments found: " + dt.Rows.Count;
                }
                else
                {
                    gvResults.Visible = false;
                    lblGVResultsHeader.Text = "No Daily Post Assignments found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvResults.Visible = false;
                lblGVResultsHeader.Text = "No Daily Post Assignment found due to error.";
            }
        }

        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResults.PageIndex = e.NewPageIndex;
            gvResults.SelectedIndex = -1;
            loadSearchedResults();
        }

        protected void gvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvResults.SelectedIndex > -1)
            {
                clearForm();
                Label lblPostAssignmentID = (Label)gvResults.SelectedRow.FindControl("lblPostAssignmentID");
                Label lblShiftID = (Label)gvResults.SelectedRow.FindControl("lblShiftID");
                Label lblReportNumber2 = (Label)gvResults.SelectedRow.FindControl("lblReportNumber");
                Label lblDate = (Label)gvResults.SelectedRow.FindControl("lblDate");
                Label lblLateArrivals = (Label)gvResults.SelectedRow.FindControl("lblLateArrivals");
                Label lblCallsForAssistance = (Label)gvResults.SelectedRow.FindControl("lblCallsForAssistance");
                Label lblOfficersAssigned = (Label)gvResults.SelectedRow.FindControl("lblOfficersAssigned");
                Label lblOfficersPresent = (Label)gvResults.SelectedRow.FindControl("lblOfficersPresent");
                Label lblOfficersAbsent = (Label)gvResults.SelectedRow.FindControl("lblOfficersAbsent");
                Label lblOfficersCallInForDayOff = (Label)gvResults.SelectedRow.FindControl("lblOfficersCallInForDayOff");
                Label lblOfficersCalledOut = (Label)gvResults.SelectedRow.FindControl("lblOfficersCalledOut");
                Label lblOfficersOnDayOff = (Label)gvResults.SelectedRow.FindControl("lblOfficersOnDayOff");
                Label lblNotDressedProperly = (Label)gvResults.SelectedRow.FindControl("lblNotDressedProperly");
                Label lblShiftSummary = (Label)gvResults.SelectedRow.FindControl("lblShiftSummary");
                Label lblShiftBriefingNotes = (Label)gvResults.SelectedRow.FindControl("lblShiftBriefingNotes");
                Label lblPassedOnFrom = (Label)gvResults.SelectedRow.FindControl("lblPassedOnFrom");
                Label lblPassedOnTo = (Label)gvResults.SelectedRow.FindControl("lblPassedOnTo");
                Label lblSummaryOfIncidents = (Label)gvResults.SelectedRow.FindControl("lblSummaryOfIncidents");
                Label lblCreatedBy = (Label)gvResults.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvResults.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedOn");
                Label lblSleeping = (Label)gvResults.SelectedRow.FindControl("lblSleeping");
                
                ibDate.Enabled = false;
                ddlShift.Enabled = false;
                postAssignmentID = Convert.ToInt32(lblPostAssignmentID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                if (ddlShift.Items.FindByValue(lblShiftID.Text) != null)
                    ddlShift.SelectedValue = lblShiftID.Text;
                else
                    ddlShift.SelectedIndex = 0;
                tbDate.Text = lblDate.Text;
                tbLateArrivals.Text = lblLateArrivals.Text;
                tbCallsForAssistance.Text = lblCallsForAssistance.Text;
                tbOfficersAssigned.Text = lblOfficersAssigned.Text;
                tbOfficersPresent.Text = lblOfficersPresent.Text;
                tbOfficersAbsent.Text = lblOfficersAbsent.Text;
                tbCallIns.Text = lblOfficersCallInForDayOff.Text;
                tbCallOuts.Text = lblOfficersCalledOut.Text;
                tbOfficersOnDayOff.Text = lblOfficersOnDayOff.Text;
                tbNotDressedProperly.Text = lblNotDressedProperly.Text;
                tbSleeping.Text = lblSleeping.Text;
                tbShiftSummary.Text = lblShiftSummary.Text;
                tbShiftBriefingNotes.Text = lblShiftBriefingNotes.Text;
                tbPassedOnFrom.Text = lblPassedOnFrom.Text;
                tbPassedOnTo.Text = lblPassedOnTo.Text;
                tbIncidentsSummary.Text = lblSummaryOfIncidents.Text;
                lblHeader.Text = "Update the selected daily post assignment below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;
                
                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                loadMasterRoster();
                isLoaded = true;
            }
        }

        protected void btnSearchResults_Click(object sender, EventArgs e)
        {
            loadSearchedResults();
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}