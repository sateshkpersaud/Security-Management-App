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

namespace ISSA.Pages.Managers
{
    public partial class Alerts : System.Web.UI.Page
    {
        BusinessTier.Alert ALERT = new BusinessTier.Alert();
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

        public byte illegalDays
        {
            get
            {
                return (byte)ViewState["illegalDays"];
            }
            set
            {
                ViewState["illegalDays"] = value;
            }
        }

        public bool dorLoaded
        {
            get
            {
                return (bool)ViewState["dorLoaded"];
            }
            set
            {
                ViewState["dorLoaded"] = value;
            }
        }

        public byte counter
        {
            get
            {
                return (byte)ViewState["counter"];
            }
            set
            {
                ViewState["counter"] = value;
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
                     string[] roleNames = MISC.getUserRoles();
                     if (roleNames.Contains("Manager"))
                     {
                         illegalDays = 6;
                         loadDDLSearchArea();
                         object AreaID = null;
                         if (ddlLogArea.SelectedIndex > 0)
                             AreaID = Convert.ToByte(ddlLogArea.SelectedValue);
                         loadDDLSearchLocations(AreaID);
                         clearForm();
                         loadCallLogs();
                         loadIncidentsComplaints();
                         tbLogDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbLogDate.ClientID + ",'" + DateTime.Now.Date + "')");
                         tbAttendanceDateFrom.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbAttendanceDateFrom.ClientID + ",'" + DateTime.Now.Date + "')");
                         tbAttendanceDateTo.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbAttendanceDateTo.ClientID + ",'" + DateTime.Now.Date + "')");
                         tbDORDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDORDate.ClientID + ",'" + DateTime.Now.Date + "')");
                         Timer1.Interval = 1000;
                         Timer1.Enabled = true;
                         counter = 60;
                         lblCounter.Text = counter.ToString() + " sec...";
                         loadShifts();
                         loadDDLAttendanceArea();
                         loadAttendance();
                         loadDoRShifts();
                     }
                     else
                     {
                         ShowMessage("No ID returned for logged in user", MessageType.Error);
                         Response.Redirect("../../Pages/Account/Login.aspx");
                     }
                }
            }
        }

        private void loadShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlAttendanceShift.DataSource = dt;
            ddlAttendanceShift.DataTextField = "shift";
            ddlAttendanceShift.DataValueField = "ShiftID";
            ddlAttendanceShift.DataBind();
        }

        private void loadDoRShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlDORShift.DataSource = dt;
            ddlDORShift.DataTextField = "shift";
            ddlDORShift.DataValueField = "ShiftID";
            ddlDORShift.DataBind();
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        private void clearForm()
        {
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            dateNow = dateNow.Substring(0, 10); //This substring is here just in case more that ten charactera are returned
            tbLogDate.Text = dateNow;
            tbAttendanceDateFrom.Text = dateNow;
            tbAttendanceDateTo.Text = dateNow;
            DateTime yesterdayDatee = currentDatetime.AddDays(-1);
            string yesterdayDate = yesterdayDatee.Date.ToString("MM'/'dd'/'yyyy");
            yesterdayDate = yesterdayDate.Substring(0, 10);
            tbDORDate.Text = yesterdayDate;
            lblDayOfWeek.Text = yesterdayDatee.DayOfWeek.ToString();
            pnlDORDetails.Visible = false;
            dorLoaded = false;
        }

        //populate the Area ddl
        private void loadDDLSearchArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlLogArea.DataSource = dt;
            ddlLogArea.DataTextField = "Area";
            ddlLogArea.DataValueField = "AreaID";
            ddlLogArea.DataBind();
        }

        private void loadDDLAttendanceArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlAttendanceArea.DataSource = dt;
            ddlAttendanceArea.DataTextField = "Area";
            ddlAttendanceArea.DataValueField = "AreaID";
            ddlAttendanceArea.DataBind();
        }

        //populate the location ddl based on the area selected
        private void loadDDLSearchLocations(object AreaID)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddlLogLocation.DataSource = dt;
            ddlLogLocation.DataTextField = "LocationName";
            ddlLogLocation.DataValueField = "LocationID";
            ddlLogLocation.DataBind();
        }

        protected void ddlLogArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLogArea.SelectedIndex > 0)
                loadDDLSearchLocations(Convert.ToByte(ddlLogArea.SelectedValue));
            else
                loadDDLSearchLocations(null);
        }

        private void loadCallLogs()
        {
            DataTable dt = ALERT.sp_Alerts_CallLog_Select(tbLogDate.Text, ddlLogArea.SelectedValue, ddlLogLocation.SelectedValue);
            if (ALERT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvCallLog.DataSource = dt;
                    gvCallLog.DataBind();
                    gvCallLog.Visible = true;
                }
                else
                {
                    gvCallLog.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and the call logs were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvCallLog.Visible = false;
            }
        }

        private void loadIncidentsComplaints()
        {
            DataTable dt = ALERT.sp_Alerts_IncidentsComplaints_Select(tbLogDate.Text, ddlLogArea.SelectedValue, ddlLogLocation.SelectedValue);
            if (ALERT.successfulCommit)
            {
                //for all rows, check if both columns 2 and 3 equal 0 and remove
                dt.Rows.Cast<DataRow>().Where(r => r.ItemArray[2].ToString() == "0" && r.ItemArray[3].ToString() == "0").ToList().ForEach(r => r.Delete());
                dt.AcceptChanges();
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvIncidentscomplaintsSummary.DataSource = dt;
                    gvIncidentscomplaintsSummary.DataBind();
                    gvIncidentscomplaintsSummary.Visible = true;
                }
                else
                {
                    gvIncidentscomplaintsSummary.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and the incidents were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvIncidentscomplaintsSummary.Visible = false;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                loadCallLogs();
                loadIncidentsComplaints();
            }
        }

        protected void gvCallLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCallLog.PageIndex = e.NewPageIndex;
            gvCallLog.SelectedIndex = -1;
            loadCallLogs();
        }

        protected void gvCallLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvIncidentscomplaintsSummary_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvIncidentscomplaintsSummary.PageIndex = e.NewPageIndex;
            gvIncidentscomplaintsSummary.SelectedIndex = -1;
            loadIncidentsComplaints();
        }

        protected void gvIncidentscomplaintsSummary_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //If the user clicks on an incident number
            if (e.CommandName == "IncidentView")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvIncidentscomplaintsSummary.Rows[index];
                Label lblIncidentCount = (Label)row.FindControl("lblIncidentCount");
                if (lblIncidentCount.Text != "0")
                {
                    Label lblLocationID = (Label)row.FindControl("lblLocationID");
                    if (lblLocationID.Text != "")
                    {
                        Label lblLocationName = (Label)row.FindControl("lblLocationName");
                        lblIncidentsHeader.Text = lblLocationName.Text + " -- Incidents Summary";
                        loadIncidentsDetails(lblLocationID.Text);
                        showIncidentModal();
                    }
                    else
                        ShowMessage("Something went wrong and the incidents were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
                //If the user clicks on a complaint number
            else if (e.CommandName == "ComplaintView")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvIncidentscomplaintsSummary.Rows[index];
                Label lblComplaintCount = (Label)row.FindControl("lblComplaintCount");
                if (lblComplaintCount.Text != "0")
                {
                    Label lblLocationID = (Label)row.FindControl("lblLocationID");
                    if (lblLocationID.Text != "")
                    {
                        Label lblLocationName = (Label)row.FindControl("lblLocationName");
                        lblComplaintsHeader.Text = lblLocationName.Text + " -- Complaints Summary";
                        loadComplaintsDetails(lblLocationID.Text);
                        showComplaintsModal();
                    }
                    else
                        ShowMessage("Something went wrong and the incidents were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        private void showIncidentModal()
        {
            Timer1.Enabled = false;
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#incidentsModal').modal('toggle');</script>", false);
        }

        private void hideIncidentModal()
        {
            Timer1.Enabled = true;
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#incidentsModal').modal('hide');</script>", false);
        }

        private void showAbsenceModal()
        {
            Timer1.Enabled = false;
            //ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#absenceModal').modal('toggle');</script>", false);
            mpeAbsence.Show();
        }

        private void hideAbsenceModal()
        {
            Timer1.Enabled = true;
            //ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#absenceModal').modal('hide');</script>", false);
            mpeAbsence.Hide();
        }

        private void showComplaintsModal()
        {
            Timer1.Enabled = false;
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#complaintsModal').modal('toggle');</script>", false);
        }

        private void hideComplaintsModal()
        {
            Timer1.Enabled = true;
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#complaintsModal').modal('hide');</script>", false);
        }

        protected void btnCancelIncidentsDetails_Click(object sender, EventArgs e)
        {
            hideIncidentModal();
        }

        private void loadIncidentsDetails(object locationID)
        {
            DataTable dt = ALERT.sp_Alerts_IncidentsSelect(tbLogDate.Text, locationID);
            if (ALERT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvIncidentsDetails.DataSource = dt;
                    gvIncidentsDetails.DataBind();
                    gvIncidentsDetails.Visible = true;
                }
                else
                {
                    gvIncidentsDetails.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and the incidents were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvIncidentsDetails.Visible = false;
            }
        }

        private void loadComplaintsDetails(object locationID)
        {
            DataTable dt = ALERT.sp_Alerts_ComplaintsSelect(tbLogDate.Text, locationID);
            if (ALERT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvComplaintsDetails.DataSource = dt;
                    gvComplaintsDetails.DataBind();
                    gvComplaintsDetails.Visible = true;
                }
                else
                {
                    gvComplaintsDetails.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and the complaints were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvComplaintsDetails.Visible = false;
            }
        }

        protected void btnCloseComplaintsDetails_Click(object sender, EventArgs e)
        {
            hideComplaintsModal();
        }

        protected void gvIncidentsDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvIncidentsDetails.SelectedIndex > -1)
            {
                Label lblIncidentNumber = (Label)gvIncidentsDetails.SelectedRow.FindControl("lblIncidentNumber");
                Response.Redirect("../../Pages/Operations/Incidents.aspx?incNumber=" + lblIncidentNumber.Text);
            }
        }

        protected void gvIncidentsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvComplaintsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvIncidentscomplaintsSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvCallLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvComplaintsDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvComplaintsDetails.SelectedIndex > -1)
            {
                Label lblComplaintNumber = (Label)gvComplaintsDetails.SelectedRow.FindControl("lblComplaintNumber");
                Response.Redirect("../../Pages/Operations/Complaints.aspx?comNumber=" + lblComplaintNumber.Text);
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (counter == 0)
            {
                lblCounter.Text = counter.ToString() + " sec...";
                loadCallLogs();
                loadIncidentsComplaints();
                upLogs.Update();
                loadAttendance();
                upAttendance.Update();
                counter = 60;
            }
            else if (counter == 1)
            {
                lblCounter.Text = counter.ToString() + " sec...";
                counter--;
            }
            else
            {
                lblCounter.Text = counter.ToString() + " secs...";
                counter--;
            }
        }

        protected void btnStopStartTimer_Click(object sender, EventArgs e)
        {
            if (Timer1.Enabled == true)
            {
                Timer1.Enabled = false;
                btnStopStartTimer.Text = "Start Timer";
                btnStopStartTimer.CssClass = "btn btn-success";
                btnStopStartTimer2.Text = "Start Timer";
                btnStopStartTimer2.CssClass = "btn btn-success";
                //btnStopStartTimer3.Text = "Start Timer";
                //btnStopStartTimer3.CssClass = "btn btn-success";
            }
            else
            {
                Timer1.Enabled = true;
                btnStopStartTimer.Text = "Stop Timer";
                btnStopStartTimer.CssClass = "btn btn-danger";
                btnStopStartTimer2.Text = "Stop Timer";
                btnStopStartTimer2.CssClass = "btn btn-danger";
                //btnStopStartTimer3.Text = "Stop Timer";
                //btnStopStartTimer3.CssClass = "btn btn-danger";
            }
        }

        private void loadAttendance()
        {
            //Declare the final result datatable
            DataTable dtFinalResult = new DataTable();
            dtFinalResult.Columns.Add("AreaID", typeof(string));
            dtFinalResult.Columns.Add("Area", typeof(string));
            dtFinalResult.Columns.Add("StandByUsed", typeof(string));
            dtFinalResult.Columns.Add("StandByUnused", typeof(string));
            dtFinalResult.Columns.Add("SentHome", typeof(string));
            dtFinalResult.Columns.Add("DayOff", typeof(string));
            dtFinalResult.Columns.Add("Absent", typeof(string));
            dtFinalResult.Columns.Add("Shortage", typeof(string));
            dtFinalResult.Columns.Add("IllegalEmployees", typeof(string));
            dtFinalResult.Columns.Add("Double", typeof(string));
            //dtFinalResult.Columns.Add("Sleeping", typeof(string));

            //Get the areas
            DataTable dtAreas = new DataTable();
            dtAreas.Columns.Add("AreaID", typeof(string));
            dtAreas.Columns.Add("Area", typeof(string));
            if (ddlAttendanceArea.SelectedIndex == 0)
                dtAreas = MISC.sp_Area_Sel4DDL();
            else
            {
                DataRow dr = dtAreas.NewRow();
                dr["AreaID"] = ddlAttendanceArea.SelectedValue;
                dr["Area"] = ddlAttendanceArea.SelectedItem.Text;
                dtAreas.Rows.Add(dr);
            }


            if ((dtAreas.Rows.Count != 0) && (dtAreas != null))
            {
                //Remove the first row that has the "...Select..."
                if (dtAreas.Rows[0]["Area"].ToString() == "...Select...")
                {
                    dtAreas.Rows[0].Delete();
                    dtAreas.AcceptChanges();
                }

                //iterate through the areas and get the information
                for (int i = 0; i < dtAreas.Rows.Count; i++)
                {
                    //Build the datatable for each area

                    string AreaID = dtAreas.Rows[i]["AreaID"].ToString();
                    string Area = dtAreas.Rows[i]["Area"].ToString();
                    byte areaID = Convert.ToByte(AreaID);

                    var model = new EDM.DataSource();

                    //Get standby employees count
                    int totalStandBys = (from a in model.tbl_Sup_Employees
                                            where a.AreaID == areaID && a.IsEmployee == true && a.IsActive == true && a.IsStandbyStaff == true
                                            select new
                                            {
                                                a.EmployeeID
                                            }).ToList().Count;

                    //Get standby employees working
                    int standByUsed = ALERT.sp_Alerts_Attendance_StandByUsed(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);

                    //Calculate standby unused
                    int standByUnUsed = totalStandBys - standByUsed;

                    //Get emps sent home
                    int empsSentHome = ALERT.sp_Alerts_Attendance_SentHome(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);

                    //Get dayoffs
                    int dayOffs = ALERT.sp_Alerts_Attendance_DayOffs(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);

                    //Get absents
                    int absents = ALERT.sp_Alerts_Attendance_Absent(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);

                    int totalEmployeesScheduled = ALERT.sp_Alerts_Attendance_TotalScheduled(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
                    int totalPersonsWorking = ALERT.sp_Alerts_Attendance_PersonsWorking(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
                    //int shortage = totalEmployeesScheduled - totalPersonsWorking;
                    int shortage = getShortageCount(areaID.ToString());
                    //Get illegals
                    //First, get 6 days date before From date
                    DateTime FromTime = Convert.ToDateTime(tbAttendanceDateFrom.Text);
                    DateTime dtStart = FromTime.AddDays(-illegalDays);
                    int illegals = ALERT.sp_Alerts_Attendance_IllegalEmployees(dtStart, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue, illegalDays, tbAttendanceDateFrom.Text);

                    //Get double 
                    int doubles = ALERT.sp_Alerts_Attendance_Doubles(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);

                    //Add all values to the datatable
                    DataRow dr = dtFinalResult.NewRow();
                    dr["AreaID"] = AreaID;
                    dr["Area"] = Area;
                    dr["StandByUsed"] = standByUsed.ToString();
                    dr["StandByUnused"] = standByUnUsed.ToString();
                    dr["SentHome"] = empsSentHome.ToString();
                    dr["DayOff"] = dayOffs.ToString();
                    dr["Absent"] = absents.ToString();
                    dr["Shortage"] = shortage.ToString();
                    dr["IllegalEmployees"] = illegals;
                    dr["Double"] = doubles.ToString();
                    //dr["Sleeping"] = "-";
                    dtFinalResult.Rows.Add(dr);
                }

                gvAttendance.DataSource = dtFinalResult;
                gvAttendance.DataBind();
            }
        }

        protected void gvAttendance_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Absent")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblAbsent = (Label)row.FindControl("lblAbsent");
                if (lblAbsent.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblAbsentDetails.Text = lblArea.Text + " -- Absence Details";
                        loadAbsenceDetails(lblAreaID.Text);
                        lblID.Text = lblAreaID.Text;
                        showAbsenceModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "DayOff")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblDayOff = (Label)row.FindControl("lblDayOff");
                if (lblDayOff.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                       lblDayOffHeader.Text = lblArea.Text + " -- Day Off Details";
                        loadDayOffDetails(lblAreaID.Text);
                        lblDayOffAreaID.Text = lblAreaID.Text;
                        showdayOffsModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "Doubles")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblDouble = (Label)row.FindControl("lblDouble");
                if (lblDouble.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblDoubleHeader.Text = lblArea.Text + " -- Double Details";
                        loadDoubleDetails(lblAreaID.Text);
                        lblDoublesAreaID.Text = lblAreaID.Text;
                        showDoublesModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "StandByUsed")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblStandByUsed = (Label)row.FindControl("lblStandByUsed");
                if (lblStandByUsed.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblStandbyUsedHeader.Text = lblArea.Text + " -- Stand-by Used Details";
                        loadStandbyUsedDetails(lblAreaID.Text);
                        lblStandByAreaID.Text = lblAreaID.Text;
                        showStandByUsedModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "StandByUnused")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblStandByUnused = (Label)row.FindControl("lblStandByUnused");
                if (lblStandByUnused.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblStandbyUnusedHeader.Text = lblArea.Text + " -- Stand-by Unused Details";
                        loadStandbyUnUsedDetails(lblAreaID.Text);
                        lblStandbyUnusedAreaID.Text = lblAreaID.Text;
                        showStandByUnUsedModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "SentHome")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblSentHome = (Label)row.FindControl("lblSentHome");
                if (lblSentHome.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblSentHomeHeader.Text = lblArea.Text + " -- Sent Home Details";
                        loadSentHomeDetails(lblAreaID.Text);
                        lblSentHomeAreaID.Text = lblAreaID.Text;
                        showSentHomeModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "IllegalEmployees")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblIllegalEmployees = (Label)row.FindControl("lblIllegalEmployees");
                if (lblIllegalEmployees.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblIllegalEmployeesHeader.Text = lblArea.Text + " -- Illegal Employees Details";
                        loadIllegalEmployeesDetails(lblAreaID.Text);
                        lblIllegalEmployeesAreaID.Text = lblAreaID.Text;
                        showIllegalEmployeesModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else if (e.CommandName == "Shortage")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvAttendance.Rows[index];
                Label lblShortage = (Label)row.FindControl("lblShortage");
                if (lblShortage.Text != "0")
                {
                    Label lblAreaID = (Label)row.FindControl("lblAreaID");
                    if (lblAreaID.Text != "")
                    {
                        Label lblArea = (Label)row.FindControl("lblArea");
                        lblShortageHeader.Text = lblArea.Text + " -- Shortage Details";
                        loadShortageDetails(lblAreaID.Text);
                        lblShortageAreaID.Text = lblAreaID.Text;
                        showShortageModal();
                    }
                    else
                        ShowMessage("Something went wrong and the details were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        protected void btnSearchAttendance_Click(object sender, EventArgs e)
        {
            loadAttendance();
        }

        private void loadAbsenceDetails(string areaID)
        {
            DataTable dt = ALERT.sp_Alerts_Attendance_AbsentSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            gvAbsenceDetails.DataSource = dt;
            gvAbsenceDetails.DataBind();
        }

        protected void btnCloseAttendanceDetails_Click(object sender, EventArgs e)
        {
            hideAbsenceModal();
        }

        private void loadDayOffDetails(string areaID)
        {
            DataTable dt = ALERT.sp_Alerts_Attendance_DayOffsSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            gvDayOffsDetails.DataSource = dt;
            gvDayOffsDetails.DataBind();
        }

        private void showdayOffsModal()
        {
            Timer1.Enabled = false;
            mpeDayOffs.Show();
        }

        private void hideDayOffsModal()
        {
            Timer1.Enabled = true;
            mpeDayOffs.Hide();
        }

        protected void btnCloseDayOffs_Click(object sender, EventArgs e)
        {
            hideDayOffsModal();
        }

        private void loadDoubleDetails(string areaID)
        {
            DataTable dt = ALERT.sp_Alerts_Attendance_DoublesSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            gvDoublesDetails.DataSource = dt;
            gvDoublesDetails.DataBind();
        }

        private void showDoublesModal()
        {
            Timer1.Enabled = false;
            mpeDoubles.Show();
        }

        private void hideDoublesModal()
        {
            Timer1.Enabled = true;
            mpeDoubles.Hide();
        }

        protected void btncloseDoubles_Click(object sender, EventArgs e)
        {
            hideDoublesModal();
        }

        private void loadStandbyUsedDetails(string areaID)
        {
            DataTable dt = ALERT.sp_Alerts_Attendance_StandByUsedSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
             gvStandbyUsed.DataSource = dt;
             gvStandbyUsed.DataBind();
        }

        private void showStandByUsedModal()
        {
            Timer1.Enabled = false;
            mpeStandbyUsed.Show();
        }

        private void hideStandByUsedModal()
        {
            Timer1.Enabled = true;
           mpeStandbyUsed.Hide();
        }

        protected void btnCloseStandbyUsed_Click(object sender, EventArgs e)
        {
            hideStandByUsedModal();
        }

        private void loadStandbyUnUsedDetails(string areaID)
        {
            DataTable dt = ALERT.sp_Alerts_Attendance_StandByUnusedSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            gvStandbyUnused.DataSource = dt;
            gvStandbyUnused.DataBind();        
        }

        private void showStandByUnUsedModal()
        {
            Timer1.Enabled = false;
            mpeStandbyUnused.Show();
        }

        private void hideStandByUnUsedModal()
        {
            Timer1.Enabled = true;
            mpeStandbyUnused.Hide();
        }

        protected void btnCloseStandbyUnused_Click(object sender, EventArgs e)
        {
            hideStandByUnUsedModal();
        }

        private void loadSentHomeDetails(string areaID)
        {
            DataTable dt = ALERT.sp_Alerts_Attendance_SentHomeSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            gvSentHome.DataSource = dt;
            gvSentHome.DataBind();
        }

        private void showSentHomeModal()
        {
            Timer1.Enabled = false;
            mpeSentHome.Show();
        }

        private void hideSentHomeModal()
        {
            Timer1.Enabled = true;
            mpeSentHome.Hide();
        }

        protected void btnCloseSentHome_Click(object sender, EventArgs e)
        {
            hideSentHomeModal();
        }

        private void loadIllegalEmployeesDetails(string areaID)
        {
            DateTime FromTime = Convert.ToDateTime(tbAttendanceDateFrom.Text);
            DateTime dtStart = FromTime.AddDays(-illegalDays);
            DataTable dt = ALERT.sp_Alerts_Attendance_IllegalEmployeesSelect(dtStart, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue, illegalDays, tbAttendanceDateFrom.Text);
            gvIllegalEmployees.DataSource = dt;
            gvIllegalEmployees.DataBind();
        }

        private void showIllegalEmployeesModal()
        {
            Timer1.Enabled = false;
            mpeIllegalEmployees.Show();
        }

        private void hideIllegalEmployeesModal()
        {
            Timer1.Enabled = true;
            mpeIllegalEmployees.Hide();
        }

        protected void btnCloseIllegalEmployees_Click(object sender, EventArgs e)
        {
            hideIllegalEmployeesModal();
        }

        private void loadShortageDetails(string areaID)
        {
            DataTable dt = new DataTable();
            dt = ALERT.sp_Alerts_Attendance_ShortageSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                int overallScheduled = 0;
                int overallPresent = 0;
                int overallAbsent = 0;
                int overallShort = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow drr = dt.Rows[i];
                    int totalScheduled = Convert.ToInt32(drr["TotalScheduled"]);
                    int totalPresent = Convert.ToInt32(drr["totalPresent"]);
                    int totalAbsent = Convert.ToInt32(drr["totalAbsent"]);
                    int totalShort = Convert.ToInt32(drr["totalShort"]);
                    //int totalShort = totalScheduled - totalPresent;
                    //drr["totalShort"] = totalShort;
                    overallScheduled += totalScheduled;
                    overallPresent += totalPresent;
                    overallAbsent += totalAbsent;
                    overallShort += totalShort;
                }
                DataRow dr = dt.NewRow();
                dr["LocationName"] = "TOTAL";
                dr["TotalScheduled"] = overallScheduled;
                dr["totalPresent"] = overallPresent;
                dr["totalAbsent"] = overallAbsent;
                dr["totalShort"] = overallShort;
                dt.Rows.Add(dr);
                gvShortageDetails.DataSource = dt;
                gvShortageDetails.DataBind();
            }
        }

        private int getShortageCount(string areaID)
        {
            int overallShort = 0;
            DataTable dt = new DataTable();
            dt = ALERT.sp_Alerts_Attendance_ShortageSelect(tbAttendanceDateFrom.Text, tbAttendanceDateTo.Text, areaID, ddlAttendanceShift.SelectedValue);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow drr = dt.Rows[i];
                    int totalShort = Convert.ToInt32(drr["totalShort"]);
                    overallShort += totalShort;
                }
            }
            return overallShort;
        }

        private void showShortageModal()
        {
            Timer1.Enabled = false;
            mpeShortage.Show();
        }

        private void hideShortageModal()
        {
            Timer1.Enabled = true;
            mpeShortage.Hide();
        }

        protected void btnCloseShortageDetails_Click(object sender, EventArgs e)
        {
            hideShortageModal();
        }

        protected void tbDORDate_TextChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = Convert.ToDateTime(tbDORDate.Text);
            lblDayOfWeek.Text = selectedDate.DayOfWeek.ToString();

            if ((tbDORDate.Text != "") && (ddlDORShift.SelectedIndex > 0))
            {
                loadDOR();
            }
        }

        //protected void btnPrint_Click(object sender, EventArgs e)
        //{
        //    dorLoaded = true;

        //    if (dorLoaded == true)
        //    {
        //        StringWriter sw = new StringWriter();
        //        HtmlTextWriter hw = new HtmlTextWriter(sw);
        //        //gvShiftSupervisors.RenderControl(hw);
        //        string gridHTML = sw.ToString().Replace("\"", "'")
        //            .Replace(System.Environment.NewLine, "");
        //        string header = "<p style='text-align:center'><strong><img alt='' src='../../Images/logo.png' style='float:left' align='left' /></strong></p>" +
        //                         "<p style='text-align:center'>&nbsp;</p><h2 style='text-align:center;line-height:100%'><strong>Daily Operations Report - </strong>" + tbDORDate.Text + "</h2>";
        //                          //"<h3 style='text-align:center;line-height:100%'><strong>Date From   </strong> " + tbDateFrom.Text + " <strong>  To  </strong> " + tbDateTo.Text + "</h3> <hr />";
        //        gridHTML = header + gridHTML;
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("<script type = 'text/javascript'>");
        //        sb.Append("window.onload = new function(){");
        //        sb.Append("var printWin = window.open('', '', 'left=0");
        //        sb.Append(",top=0,width=1000,height=600,status=0');");
        //        sb.Append("printWin.document.write(\"");
        //        sb.Append(gridHTML);
        //        sb.Append("\");");
        //        sb.Append("printWin.document.close();");
        //        sb.Append("printWin.focus();");
        //        sb.Append("printWin.print();");
        //        sb.Append("printWin.close();};");
        //        sb.Append("</script>");
        //        ClientScript.RegisterStartupScript(GetType(), "GridPrint", sb.ToString());
        //    }
        //    else
        //    {
        //        ShowMessage("There is nothing here to print.", MessageType.Error);
        //    }
        //}

        private void loadDOR()
        {
            DataTable dt = ALERT.sp_Alerts_DailyOperationsReport(tbDORDate.Text, ddlDORShift.SelectedValue);
            if ((dt.Rows.Count > 0) && (dt != null))
            {
                lblHoursInShift.Text = dt.Rows[0]["HoursInShift"].ToString();
                lblCallsForAssistance.Text = dt.Rows[0]["CallsForAssistance"].ToString();
                lblLateArrivals.Text = dt.Rows[0]["LateArrivals"].ToString();
                lblOnDuty.Text = dt.Rows[0]["OnDuty"].ToString();
                lblSleeping.Text = dt.Rows[0]["Sleeping"].ToString();
                lblNotDressedProperly.Text = dt.Rows[0]["NotDressedProperly"].ToString();
                lblIncidents.Text = dt.Rows[0]["IncidentsCount"].ToString();
                lblComplaints.Text = dt.Rows[0]["ComplaintsCount"].ToString();
                lblSickCalls.Text = dt.Rows[0]["sickCallsCount"].ToString();
                lblShiftSummary.Text = dt.Rows[0]["ShiftSummary"].ToString();
                lblShiftBriefingNotes.Text = dt.Rows[0]["ShiftBriefingNotes"].ToString();

                lblAuditTrail.Text = "Created By: " + dt.Rows[0]["CreatedBy"].ToString() + " Created On: " + dt.Rows[0]["CreatedOn"].ToString() + " Last Modified By: " + dt.Rows[0]["LastModifiedBy"].ToString() + " Last Modified On: " + dt.Rows[0]["LastModifiedOn"].ToString();

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                string s = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("HH:mm");
                lblTime.Text = s;

                //Get supervisors from shift report
                DataTable dtSups = ALERT.sp_Alerts_DailyOperationsReport_Supervisors(tbDORDate.Text, ddlDORShift.SelectedValue);
                if ((dtSups.Rows.Count > 0) && (dtSups != null))
                {
                    gvShiftSupervisors.DataSource = dtSups;
                    gvShiftSupervisors.DataBind();
                    gvShiftSupervisors.Visible = true;
                }
                else
                    gvShiftSupervisors.Visible = false;
                pnlDORDetails.Visible = true;
            }
            else
            {
                pnlDORDetails.Visible = false;
            }
        }

        protected void ddlDORShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tbDORDate.Text != "") && (ddlDORShift.SelectedIndex > 0))
            {
                loadDOR();
            }
        }

        //function to merge similar boundfield cells of gridview
        public void MergeGridViewRows(GridView gridView, int minCellCount, int maxCellcount)
        {
            for (int rowIndex = gridView.Rows.Count - 2; rowIndex >= 0; rowIndex--)
            {
                GridViewRow row = gridView.Rows[rowIndex];
                GridViewRow previousRow = gridView.Rows[rowIndex + 1];

                //Exclude the first and last cells. That is, the line number and the employee ID
                for (int i = minCellCount; i < row.Cells.Count - maxCellcount; i++)
                {
                    if (row.Cells[i].Text == previousRow.Cells[i].Text)
                    {
                        row.Cells[i].RowSpan = previousRow.Cells[i].RowSpan < 2 ? 2 :
                                               previousRow.Cells[i].RowSpan + 1;
                        previousRow.Cells[i].Visible = false;
                    }
                }
            }
        }

        protected void gvDayOffsDetails_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvDayOffsDetails, 1, 1);
        }

        protected void gvAbsenceDetails_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvAbsenceDetails, 1, 1);
        }

        protected void gvDoublesDetails_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvDoublesDetails, 1, 1);
        }

        protected void gvStandbyUsed_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvStandbyUsed, 1, 1);
        }

        protected void gvSentHome_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvSentHome, 1, 1);
        }

        protected void gvIllegalEmployees_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvIllegalEmployees, 1, 1);
        }

        protected void gvShortageDetails_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvShortageDetails, 1, 5);
        }

        protected void gvShortageDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string locationName = e.Row.Cells[1].Text.ToString();
                if ((locationName == "TOTAL"))
                {
                    Label lblNo = (Label)e.Row.FindControl("lblNo");
                    lblNo.Text = "";
                    e.Row.Font.Bold = true;
                    e.Row.BackColor = System.Drawing.Color.DarkGray;
                }
            }
        }

        protected void gvIllegalEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvSentHome_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvStandbyUnused_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvStandbyUsed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvDoublesDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvDayOffsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvAbsenceDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvShiftSupervisors_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}