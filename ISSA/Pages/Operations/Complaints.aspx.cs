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
    public partial class Complaints : System.Web.UI.Page
    {
        BusinessTier.Complaint COMPLAINT = new BusinessTier.Complaint();
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

        public int complaintID
        {
            get
            {
                return (int)ViewState["complaintID"];
            }
            set
            {
                ViewState["complaintID"] = value;
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
                    if ((roleNames.Contains("Manager")) || roleNames.Contains("Contract Compliance Officer"))
                    {
                        //Load for manager and compliance office
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        pageLoad();
                        
                        //Check if a query string is in the url
                        object comNumber = Request.QueryString["comNumber"];
                        if (comNumber != null)
                        {
                            tbSearchReportNumber.Text = comNumber.ToString();
                            loadSearchedComplaints();
                        }

                        ibCalendar.Enabled = true;
                        tbTime.Enabled = true;
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
            tbTime.Attributes.Add("onblur", "javascript:validateTime(" + tbTime.ClientID + ")");
            tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
            tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
            tbDateClientContacted.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDateClientContacted.ClientID + ",'" + DateTime.Now.Date + "')");
            tbDateReceived.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDateReceived.ClientID + ",'" + DateTime.Now.Date + "')");
            resetForm();
            gvComplaints.Visible = false;
            lblGVComplaintsHeader.Text = "";
        }

        private void loadDDLShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        //populate the Received by DDL
        private void loadDDLReceivedBy()
        {
            DataTable dt = COMPLAINT.sp_ComplaintReceivedBy_Sel4DDL();
            ddlReceivedBy.DataSource = dt;
            ddlReceivedBy.DataTextField = "fullName";
            ddlReceivedBy.DataValueField = "EmployeeID";
            ddlReceivedBy.DataBind();
        }

        //populate the complaint mode
        private void loadDDLComplaintMode()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_ComplaintChannel
                           where a.IsActive == true && a.IsComplaintsMade == true
                           select new
                           {
                               a.ComplaintChannelID,
                               a.Channel
                           }).OrderBy(LastName => LastName).ToList();
            ddlComplaintMode.DataSource = results;
            ddlComplaintMode.DataTextField = "Channel";
            ddlComplaintMode.DataValueField = "ComplaintChannelID";
            ddlComplaintMode.DataBind();
        }

        //populate the acknowledgement ddl
        private void loadDDLAcknowledgement()
        {
            DataTable dt = COMPLAINT.sp_Acknowledgement_Sel4DDL();
            ddlAcknowledgement.DataSource = dt;
            ddlAcknowledgement.DataTextField = "Channel";
            ddlAcknowledgement.DataValueField = "ComplaintChannelID";
            ddlAcknowledgement.DataBind();
        }

        //populate the Area ddl
        private void loadDDLArea()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Areas
                           where a.IsActive == true
                           select new
                           {
                               a.Seq,
                               a.Area,
                               a.AreaID
                           }).OrderBy(Seq => Seq).ToList();
            ddlArea.DataSource = results;
            ddlArea.DataTextField = "Area";
            ddlArea.DataValueField = "AreaID";
            ddlArea.DataBind();
        }

        //populate the Area ddl
        private void loadDDLSearchArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlSearchArea.DataSource = dt;
            ddlSearchArea.DataTextField = "Area";
            ddlSearchArea.DataValueField = "AreaID";
            ddlSearchArea.DataBind();
        }

        //populate the Mode ddl
        private void loadDDLSearchMode()
        {
            DataTable dt = COMPLAINT.sp_ComplaintMode_Sel4DDL();
            ddlSearchMode.DataSource = dt;
            ddlSearchMode.DataTextField = "Channel";
            ddlSearchMode.DataValueField = "ComplaintChannelID";
            ddlSearchMode.DataBind();
        }

        //populate the location ddl based on the area selected
        private void loadDDLLocations(byte AreaID)
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Locations
                           where a.IsActive == true && a.AreaID == AreaID
                           select new
                           {
                               a.LocationName,
                               a.LocationID
                           }).OrderBy(LocationName => LocationName).ToList();
            ddlLocation.DataSource = results;
            ddlLocation.DataTextField = "LocationName";
            ddlLocation.DataValueField = "LocationID";
            ddlLocation.DataBind();
        }

        //populate the location ddl based on the area selected
        private void loadDDLSearchLocations(object AreaID)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddlSearchLocation.DataSource = dt;
            ddlSearchLocation.DataTextField = "LocationName";
            ddlSearchLocation.DataValueField = "LocationID";
            ddlSearchLocation.DataBind();
        }

        //reload the location ddl on the area ddl index change
        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
        }

        protected void ddlSearchArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchArea.SelectedIndex > 0)
                loadDDLSearchLocations(Convert.ToByte(ddlSearchArea.SelectedValue));
            else
                loadDDLSearchLocations(null);
        }

        //clear the entire form
        private void resetForm()
        {
            loadDDLSearchMode();
            loadDDLSearchArea();
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            loadDDLArea();
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
            loadDDLReceivedBy();
            loadDDLComplaintMode();
            loadDDLAcknowledgement();
            loadDDLShifts();

            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            string timeNow = currentDatetime.TimeOfDay.ToString();
            dateNow = dateNow.Substring(0, 10);
            //tbDate.Text = String.Format("{0:dd/MM/yyyy}", dateNow);
            tbDate.Text = dateNow;
            tbTime.Text = timeNow.Substring(0, 5);
            tbDateReceived.Text = dateNow;
            rbYes.Checked = true;
            rbNo.Checked = false;
            tbClientcomplaint.Text = "";
            isLoaded = false;
            tbPersonMakingReport.Text = "";
            tbContactNumber.Text = "";
            tbDateClientContacted.Text = "";
            tbActionTaken.Text = "";
            complaintID = 0;
            generateComplaintNumber();
            lblAuditTrail.Text = "";
            lblHeader.Text = "Add new complaint report below";
            ddlShift.SelectedIndex = 0;
            DateTime dtNow = DateTime.Now;
            if ((MISC.checkGrantStatus(dtNow, dtNow)) || (roleNames.Contains("Manager")))
            {
                //ibCalendar.Enabled = true;
                ibCalendarDateReceived.Enabled = true;
                //tbTime.Enabled = true;
                tbDateReceived.Enabled = true;
            }
            else
            {
                //ibCalendar.Enabled = false;
                //tbTime.Enabled = false;
                ibCalendarDateReceived.Enabled = false;
                tbDateReceived.Enabled = false;
            }
        }

        //generate the incident number
        private void generateComplaintNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_Complaints
                           where a.DateOfIncident.Value.Year == year
                           select new
                           {
                               a.ComplaintID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "COM" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "COM" + year.ToString() + currentSequence.ToString();
            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            btnBackToSearch.Visible = false;
            btnBackToSearch2.Visible = false;
            resetForm();
            pnlSearch.Visible = true;
            lblGVComplaintsHeader.Text = "";
            if (gvComplaints.Rows.Count > 0)
                loadSearchedComplaints();
        }

        protected void btnNewComplaint_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new complaint report below";
            resetForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            btnBackToSearch.Visible = true;
            btnBackToSearch2.Visible = true;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            resetForm();
            //Message.InnerHtml += "<a href=\"#\" class=\"close\" data-dismiss=\"alert\">&times;</a>" +
            //                         "<ul>SUCCESS</ul>";
            //Message.Attributes["class"] = "alert alert-success";
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchReportNumber.Text = "";
            tbSearchDate.Text = "";
            ddlSearchMode.SelectedIndex = 0;
            ddlSearchArea.SelectedIndex = 0;
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            gvComplaints.Visible = false;
            lblGVComplaintsHeader.Text = "";
        }

        protected void btnSearchComplaint_Click(object sender, EventArgs e)
        {
            loadSearchedComplaints();
        }

        private void loadSearchedComplaints()
        {
            DataTable dt = COMPLAINT.sp_Complaints_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchArea.SelectedValue, ddlSearchLocation.SelectedValue, ddlSearchMode.SelectedValue);
            if (COMPLAINT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvComplaints.DataSource = dt;
                    gvComplaints.DataBind();
                    gvComplaints.Visible = true;
                    lblGVComplaintsHeader.Text = "Complaint Reports found: " + dt.Rows.Count;
                }
                else
                {
                    gvComplaints.Visible = false;
                    lblGVComplaintsHeader.Text = "No complaint reports found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvComplaints.Visible = false;
                lblGVComplaintsHeader.Text = "No reports found due to error.";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveComplaint();
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveComplaint();
                if (success)
                    resetForm();
            }
        }

        //function to save the complaint, either a new insert or an update
        private bool saveComplaint()
        {
            bool success = false;

            if (isLoaded)
            {
                try
                {
                    //Update incident
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        TimeSpan time = new TimeSpan();
                        if (TimeSpan.TryParse(tbTime.Text, out time))
                        {
                            tbTime.BorderColor = System.Drawing.Color.LightGray;
                            tbDate.BorderColor = System.Drawing.Color.LightGray;
                            date = date.Date;

                            DateTime dateReceived = new DateTime();
                            dateReceived = DateTime.Parse(tbDateReceived.Text);
                            dateReceived = dateReceived.Date;

                            DateTime dateofFeedback = new DateTime();
                            if (tbDateClientContacted.Text.Length > 0)
                            {
                                dateofFeedback = DateTime.Parse(tbDateClientContacted.Text);
                                dateofFeedback = dateofFeedback.Date;
                            }

                            byte AckID = new byte();
                            if (ddlAcknowledgement.SelectedIndex > 0)
                                AckID = Convert.ToByte(ddlAcknowledgement.SelectedValue);

                            COMPLAINT.update_Complaint(complaintID, Convert.ToByte(ddlComplaintMode.SelectedValue), tbPersonMakingReport.Text, tbContactNumber.Text, Convert.ToByte(ddlArea.SelectedValue), Convert.ToInt16(ddlLocation.SelectedValue), date, time, Convert.ToInt16(ddlReceivedBy.SelectedValue), dateReceived, AckID, tbClientcomplaint.Text, rbYes.Checked ? true : false, dateofFeedback, tbActionTaken.Text, userID, lblReportNumber.Text,Convert.ToByte(ddlShift.SelectedValue));
                            if (COMPLAINT.successfulCommit)
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
                    //insert complaint
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        TimeSpan time = new TimeSpan();
                        if (TimeSpan.TryParse(tbTime.Text, out time))
                        {
                            tbTime.BorderColor = System.Drawing.Color.LightGray;
                            tbDate.BorderColor = System.Drawing.Color.LightGray;
                            date = date.Date;

                            //converting DateReceived
                            DateTime dateReceived = new DateTime();
                            dateReceived = DateTime.Parse(tbDateReceived.Text);
                            dateReceived = dateReceived.Date;

                            //converting Datecontacted
                            DateTime dateofFeedback = new DateTime();
                            if (tbDateClientContacted.Text.Length > 0)
                            {
                                dateofFeedback = DateTime.Parse(tbDateClientContacted.Text);
                                dateofFeedback = dateofFeedback.Date;
                            }

                            //regenerate the number in case another record was added
                            generateComplaintNumber();

                            byte AckID = new byte();
                            if (ddlAcknowledgement.SelectedIndex > 0)
                                AckID = Convert.ToByte(ddlAcknowledgement.SelectedValue);
                            complaintID = COMPLAINT.insert_Complaint(Convert.ToByte(ddlComplaintMode.SelectedValue), tbPersonMakingReport.Text, tbContactNumber.Text, Convert.ToByte(ddlArea.SelectedValue), Convert.ToInt16(ddlLocation.SelectedValue), date, time, Convert.ToInt16(ddlReceivedBy.SelectedValue), dateReceived, AckID, tbClientcomplaint.Text, rbYes.Checked ? true : false, dateofFeedback, tbActionTaken.Text, lblReportNumber.Text, userID, Convert.ToByte(ddlShift.SelectedValue));
                            if (complaintID != 0)
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

        protected void gvComplaints_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblShortClientComplaint = (Label)e.Row.FindControl("lblShortClientComplaint");
                if (lblShortClientComplaint.Text.Length > 70)
                {
                    ViewState["OrigData"] = lblShortClientComplaint.Text;
                    lblShortClientComplaint.Text = lblShortClientComplaint.Text.Substring(0, 70) + "...";
                    lblShortClientComplaint.ToolTip = ViewState["OrigData"].ToString();
                }
            }
        }

        protected void gvComplaints_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //load next set of rows on next page
            gvComplaints.PageIndex = e.NewPageIndex;
            gvComplaints.SelectedIndex = -1;
            loadSearchedComplaints();
        }

        protected void gvComplaints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvComplaints.SelectedIndex > -1)
            {
                resetForm();
                Label lblComplaintID = (Label)gvComplaints.SelectedRow.FindControl("lblComplaintID");
                Label lblReportNumber2 = (Label)gvComplaints.SelectedRow.FindControl("lblReportNumber");
                Label lblAreaID = (Label)gvComplaints.SelectedRow.FindControl("lblAreaID");
                Label lblLocationID = (Label)gvComplaints.SelectedRow.FindControl("lblLocationID");
                Label lblDateofComplaint = (Label)gvComplaints.SelectedRow.FindControl("lblDateofComplaint");
                Label lblTime = (Label)gvComplaints.SelectedRow.FindControl("lblTime");
                Label lblPersonMakingReport = (Label)gvComplaints.SelectedRow.FindControl("lblPersonMakingReport");
                Label lblContactNumber = (Label)gvComplaints.SelectedRow.FindControl("lblContactNumber");
                Label lblDateReceived = (Label)gvComplaints.SelectedRow.FindControl("lblDateReceived");
                Label lblFeedbackProvided = (Label)gvComplaints.SelectedRow.FindControl("lblFeedbackProvided");
                Label lblComplaintChannelID = (Label)gvComplaints.SelectedRow.FindControl("lblComplaintChannelID");
                Label lblClientComplaint = (Label)gvComplaints.SelectedRow.FindControl("lblClientComplaint");
                Label lblReceivedByID = (Label)gvComplaints.SelectedRow.FindControl("lblReceivedByID");
                Label lblAcknowledgementID = (Label)gvComplaints.SelectedRow.FindControl("lblAcknowledgementID");
                Label lblDateOfFeedback = (Label)gvComplaints.SelectedRow.FindControl("lblDateOfFeedback");
                Label lblActionTaken = (Label)gvComplaints.SelectedRow.FindControl("lblActionTaken");
                Label lblShiftID = (Label)gvComplaints.SelectedRow.FindControl("lblShiftID");
                Label lblCreatedBy = (Label)gvComplaints.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvComplaints.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvComplaints.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvComplaints.SelectedRow.FindControl("lblLastModifiedOn");

                complaintID = Convert.ToInt32(lblComplaintID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                ddlComplaintMode.SelectedValue = lblComplaintChannelID.Text;
                tbDate.Text = lblDateofComplaint.Text;
                tbTime.Text = lblTime.Text;

                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;

                loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
                if (ddlLocation.Items.FindByValue(lblLocationID.Text) != null)
                    ddlLocation.SelectedValue = lblLocationID.Text;
                else
                    ddlLocation.SelectedIndex = 0;
                tbPersonMakingReport.Text = lblPersonMakingReport.Text;
                tbContactNumber.Text = lblContactNumber.Text;
                if (ddlReceivedBy.Items.FindByValue(lblReceivedByID.Text) != null)
                    ddlReceivedBy.SelectedValue = lblReceivedByID.Text;
                else
                    ddlReceivedBy.SelectedIndex = 0;
                tbDateReceived.Text = lblDateReceived.Text;
                ddlAcknowledgement.SelectedValue = lblAcknowledgementID.Text;
                tbClientcomplaint.Text = lblClientComplaint.Text;
                if (lblFeedbackProvided.Text == "YES")
                {
                    rbYes.Checked = true;
                    rbNo.Checked = false;
                }
                else
                {
                    rbNo.Checked = true;
                    rbYes.Checked = false;
                }
                tbDateClientContacted.Text = lblDateOfFeedback.Text;
                tbActionTaken.Text = lblActionTaken.Text;
                if (ddlShift.Items.FindByValue(lblShiftID.Text) != null)
                    ddlShift.SelectedValue = lblShiftID.Text;
                else
                    ddlShift.SelectedIndex = 0;
                lblHeader.Text = "Update the selected complaint report below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;

                isLoaded = true;
                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                btnBackToSearch.Visible = true;
                btnBackToSearch2.Visible = true;
            }
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

    }
}