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

namespace ISSA.Pages.Operations
{
    public partial class DailyLog : System.Web.UI.Page
    {
        BusinessTier.DailyLog DAILYLOG = new BusinessTier.DailyLog();
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

        public int dailyLogID
        {
            get
            {
                return (int)ViewState["dailyLogID"];
            }
            set
            {
                ViewState["dailyLogID"] = value;
            }
        }

        public int workScheduleID
        {
            get
            {
                return (int)ViewState["workScheduleID"];
            }
            set
            {
                ViewState["workScheduleID"] = value;
            }
        }

        public int newCallID
        {
            get
            {
                return (int)ViewState["newCallID"];
            }
            set
            {
                ViewState["newCallID"] = value;
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

        public bool isCallLogLoaded
        {
            get
            {
                return (bool)ViewState["isCallLogLoaded"];
            }
            set
            {
                ViewState["isCallLogLoaded"] = value;
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
                   // if ((roleNames.Contains("Manager")) || (roleNames.Contains("Supervisor")))
                    if ((roleNames.Contains("Manager")) || (roleNames.Contains("Contract Compliance Officer")) || (roleNames.Contains("Supervisor")))
                    {
                        //Load for manager
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        clearForm();
                        loadDDLSearchArea();
                        object AreaID = null;
                        if (ddlSearchArea.SelectedIndex > 0)
                            AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
                        loadDDLSearchLocations(AreaID);
                        loadDDLSearchShifts();
                        tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
                    }
                    else if (roleNames.Contains("Operator"))
                    {
                        pnlSearch.Visible = false;
                        pnlDetails.Visible = true;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        clearForm();
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

        private void loadDDLArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlArea.DataSource = dt;
            ddlArea.DataTextField = "Area";
            ddlArea.DataValueField = "AreaID";
            ddlArea.DataBind();
        }

        private void loadShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
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

        private void loadDDLSearchArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlSearchArea.DataSource = dt;
            ddlSearchArea.DataTextField = "Area";
            ddlSearchArea.DataValueField = "AreaID";
            ddlSearchArea.DataBind();
        }

        private void loadDDLSearchLocations(object AreaID)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddlSearchLocation.DataSource = dt;
            ddlSearchLocation.DataTextField = "LocationName";
            ddlSearchLocation.DataValueField = "LocationID";
            ddlSearchLocation.DataBind();
        }


        private void loadDDLSearchShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlSearchShift.DataSource = dt;
            ddlSearchShift.DataTextField = "shift";
            ddlSearchShift.DataValueField = "ShiftID";
            ddlSearchShift.DataBind();
        }

        //reload the location ddl on the area ddl index change
        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
            clearForNewSelect();
        }

        protected void ddlSearchArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchArea.SelectedIndex > 0)
                loadDDLSearchLocations(Convert.ToByte(ddlSearchArea.SelectedValue));
            else
                loadDDLSearchLocations(null);
        }

        private void configureLogData()
        {
            if (ddlShift.SelectedIndex > 0)
            {
                if ((!roleNames.Contains("Manager")) && (!roleNames.Contains("Supervisor")))
                {
                    setDate();
                    byte shiftID = Convert.ToByte(ddlShift.SelectedValue);
                    TimeSpan fromTime = new TimeSpan();
                    TimeSpan toTime = new TimeSpan();
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;

                    var model = new EDM.DataSource();
                    var results = (from a in model.tbl_Ref_Shifts
                                   where a.ShiftID == shiftID
                                   select new
                                   {
                                       a.FromTime,
                                       a.ToTime
                                   }).ToList();

                    if (results.Count > 0)
                    {
                        foreach (var val in results)
                        {
                            fromTime = TimeSpan.Parse(val.FromTime.ToString());
                            toTime = TimeSpan.Parse(val.ToTime.ToString());
                        }
                    }

                    if (fromTime <= toTime)
                    {
                        if ((currentTime >= fromTime) && (currentTime <= toTime) && (tbDate.Text != "") && (ddlArea.SelectedIndex > 0))
                        {
                            clearForDataGetSet();
                            loadPersonsWorking();
                            loadSwitches();
                            setGetData();
                        }
                        else
                        {
                            ShowMessage("Incorrect shift selected. Kindly select a shift for the current time.", MessageType.Info);
                            clearForDataGetSet();
                            pnlDLDetails.Visible = false;
                        }
                    }
                    else
                    {
                        if (((currentTime >= fromTime) || (currentTime <= toTime)) && (tbDate.Text != "") && (ddlArea.SelectedIndex > 0))
                        {
                            clearForDataGetSet();
                            loadPersonsWorking();
                            loadSwitches();
                            setGetData();
                        }
                        else
                        {
                            ShowMessage("Incorrect shift selected. Kindly select a shift for the current time.", MessageType.Info);
                            clearForDataGetSet();
                            pnlDLDetails.Visible = false;
                        }
                    }
                }
                else if ((tbDate.Text != "") && (ddlArea.SelectedIndex > 0))
                {
                    clearForDataGetSet();
                    loadPersonsWorking();
                    loadSwitches();
                    setGetData();
                }
            }
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            configureLogData();
        }

        protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
        {
           configureLogData();
        }

        protected void tbDate_TextChanged(object sender, EventArgs e)
        {
            configureLogData();
        }

        private void setGetData()
        {
            if (isLoaded == false)
            {
                try
                {
                    if (duplicateCheck_DailyLog(Convert.ToDateTime(tbDate.Text), Convert.ToByte(ddlArea.SelectedValue), Convert.ToInt16(ddlLocation.SelectedValue), Convert.ToByte(ddlShift.SelectedValue)))
                    {
                        loadPersonsWorking();
                        loadSwitches();
                        loadCallLogs(dailyLogID);
                        loadDoubles();
                        getSetLastCheckLocationTimeForArea();
                        setDailyLogAuditLog(dailyLogID);
                        btnNewCallLog.Visible = true;
                        isLoaded = true;
                        pnlDLDetails.Visible = true;
                    }
                    else
                    {
                        //check if a schedule has been created for the parameters selected
                        DateTime date = Convert.ToDateTime(tbDate.Text);
                        date = date.Date;
                        string year = date.Year.ToString();
                        string month = date.Month.ToString();
                        byte areaID = Convert.ToByte(ddlArea.SelectedValue);
                        byte shiftID = Convert.ToByte(ddlShift.SelectedValue);

                        if (workScheduleCreated(year, month, areaID, shiftID) == false)
                        {
                            ShowMessage("A work schedule has not been created for the parameters selected. Kindly check with a Manager to verify the schedule has been created.", MessageType.Info);
                            pnlDLDetails.Visible = false;
                        }
                        else
                        {
                            Int16 locationID = Convert.ToInt16(ddlLocation.SelectedValue);
                            generateLogNumber();
                            dailyLogID = DAILYLOG.insert_dailyLog(date, areaID, locationID, shiftID, gvPersonWorking, ddlArea.SelectedItem.Text, ddlLocation.SelectedItem.Text, ddlShift.SelectedItem.Text, lblReportNumber.Text, userID, workScheduleID);
                            if (DAILYLOG.successfulCommit)
                            {
                                ShowMessage("Log created.", MessageType.Success);
                                loadPersonsWorking();
                                loadSwitches();
                                loadCallLogs(dailyLogID);
                                loadDoubles();
                                btnNewCallLog.Visible = true;
                                getSetLastCheckLocationTimeForArea();
                                isLoaded = true;
                                pnlDLDetails.Visible = true;
                            }
                            else
                            {
                                ShowMessage("Something went wrong and the log was not created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                                pnlDLDetails.Visible = false;
                            }
                        }
                    }
                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    pnlDLDetails.Visible = false;
                }
            }
        }

        private void setDate()
        {
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            string nowTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("HH:mm:ss");
            TimeSpan timeNow = TimeSpan.Parse(nowTime);
            string dateNow = "";
            if ((timeNow >= TimeSpan.Parse("00:00:00")) && (timeNow <= TimeSpan.Parse("06:59:59")))
                dateNow = currentDatetime.Date.AddDays(-1).ToString("MM'/'dd'/'yyyy");
            else
                dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            //string timeNow = currentDatetime.TimeOfDay.ToString();
            dateNow = dateNow.Substring(0, 10);
            tbDate.Text = dateNow;
        }

        private void clearForm()
        {
            setDate();
            loadDDLArea();
            loadShifts();
            byte areaID = 0;
            if (ddlArea.SelectedIndex > 0)
                areaID = Convert.ToByte(ddlArea.SelectedValue);
            loadDDLLocations(areaID);
            gvResults.Visible = false;
            lblGVResultsHeader.Text = "";
            lblLastCheckLocation.Text = "";
            lblLastAreaCheckTime.Text = "";
            clearForNewSelect();

            DateTime dtNow = DateTime.Now;
            if ((MISC.checkGrantStatus(dtNow, dtNow)) || (roleNames.Contains("Manager")) || (roleNames.Contains("Supervisor")))
            {
                ibDate.Enabled = true;
            }
            else
            {
                ibDate.Enabled = false;
            }
        }

        private void clearForNewSelect()
        {
            lblReportNumber.Text = "";
            ddlShift.SelectedIndex = 0;
            gvPersonWorking.DataSource = null;
            gvPersonWorking.DataBind();
            gvSwitches.DataSource = null;
            gvSwitches.DataBind();
            gvCallLog.DataSource = null;
            gvCallLog.DataBind();
            gvDoubles.DataSource = null;
            gvDoubles.DataBind();
            gvPersonWorking.Visible = false;
            gvSwitches.Visible = false;
            gvCallLog.Visible = false;
            gvDoubles.Visible = false;
            dailyLogID = 0;
            workScheduleID = 0;
            isLoaded = false;
            isCallLogLoaded = false;
            newCallID = 0;
            btnNewCallLog.Visible = false;
            clearCallLogControls();
            pnlDLDetails.Visible = false;
        }

        private void clearForDataGetSet()
        {
            lblLastCheckLocation.Text = "";
            lblLastAreaCheckTime.Text = "";
            gvPersonWorking.Visible = false;
            gvSwitches.Visible = false;
            gvCallLog.Visible = false;
            gvDoubles.Visible = false;
            dailyLogID = 0;
            workScheduleID = 0;
            isLoaded = false;
            isCallLogLoaded = false;
            newCallID = 0;
            lblReportNumber.Text = "";
        }

        //generate the incident number
        private void generateLogNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;
            
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_DailyLogs
                           where a.CreatedOn.Value.Year == year
                           select new
                           {
                               a.DailyLogID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "DL" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "DL" + year.ToString() + currentSequence.ToString();
            }
        }

        private void loadPersonsWorking()
        {
            //Get person on the log
            DataTable dtPersonsWorking = DAILYLOG.sp_DailyLog_ActualPersonWorking_Select(dailyLogID);
            if (DAILYLOG.successfulCommit)
            {
                if ((dtPersonsWorking.Rows.Count > 0) && (dtPersonsWorking != null))
                {
                    //Get persons scheduled but not working
                    DataTable dt = DAILYLOG.sp_DailyLog_PersonWorking_Select(tbDate.Text, ddlArea.SelectedValue, ddlLocation.SelectedValue, ddlShift.SelectedValue);
                    if (dt.Rows.Count > 0)
                    {
                       dtPersonsWorking.Merge(dt);
                    }
                    gvPersonWorking.DataSource = dtPersonsWorking;
                    gvPersonWorking.DataBind();
                    gvPersonWorking.Visible = true;
                }
                else
                {
                    //Get persons scheduled to work
                    DataTable dt = DAILYLOG.sp_DailyLog_PersonWorking_Select(tbDate.Text, ddlArea.SelectedValue, ddlLocation.SelectedValue, ddlShift.SelectedValue);
                    if (DAILYLOG.successfulCommit)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            gvPersonWorking.DataSource = dt;
                            gvPersonWorking.DataBind();
                            gvPersonWorking.Visible = true;
                        }
                        else
                        {
                            gvPersonWorking.DataSource = dt;
                            gvPersonWorking.DataBind();
                            gvPersonWorking.Visible = false;
                        }
                    }
                }
            }
            if (DAILYLOG.successfulCommit == false)
                ShowMessage("Something went wrong and Persons Working were not loaded. Kindly try again and if the error persists, check the application log for details.", MessageType.Error);

        }

        private void loadSwitches()
        {
            //Get switches saved
            DataTable dtSwitchesSaved = new DataTable();
            dtSwitchesSaved.Columns.Add("EmployeeID", typeof(int));
            dtSwitchesSaved.Columns.Add("FullName", typeof(string));
            dtSwitchesSaved.Columns.Add("isStandBy", typeof(bool));
            dtSwitchesSaved.Columns.Add("IsDayOff", typeof(bool));
            dtSwitchesSaved.Columns.Add("SwitchID", typeof(Int64));
            dtSwitchesSaved.Columns.Add("IsChecked", typeof(bool));

            dtSwitchesSaved = DAILYLOG.sp_DailyLog_SwitchesWorking_Select(dailyLogID);
            if (DAILYLOG.successfulCommit)
            {
                if ((dtSwitchesSaved.Rows.Count > 0) && (dtSwitchesSaved != null))
                {
                    //Get all standby employees and other employees scheduled for this shift
                    DataTable dtAllAvailableSwitched = new DataTable();
                    dtAllAvailableSwitched.Columns.Add("EmployeeID", typeof(int));
                    dtAllAvailableSwitched.Columns.Add("FullName", typeof(string));
                    dtAllAvailableSwitched.Columns.Add("isStandBy", typeof(bool));
                    dtAllAvailableSwitched.Columns.Add("IsDayOff", typeof(bool));
                    dtAllAvailableSwitched.Columns.Add("SwitchID", typeof(Int64));
                    dtAllAvailableSwitched.Columns.Add("IsChecked", typeof(bool));

                    dtAllAvailableSwitched = DAILYLOG.sp_DailyLog_Switches_Select(tbDate.Text, ddlArea.SelectedValue, ddlLocation.SelectedValue, ddlShift.SelectedValue);
                    if (DAILYLOG.successfulCommit)
                    {
                        if ((dtAllAvailableSwitched.Rows.Count > 0) && (dtAllAvailableSwitched != null))
                        {
                            dtAllAvailableSwitched.Merge(dtSwitchesSaved);
                            ////merge the two dt to get the common rows
                            //var commonRows = from dt1 in dtAllAvailableSwitched.AsEnumerable()
                            //                 join dt2 in dtSwitchesSaved.AsEnumerable() on dt1.Field<Int16>("EmployeeID") equals dt2.Field<Int16>("EmployeeID")
                            //                 select new { dt1, SwitchID = dt2.Field<Int64>("SwitchID"), IsChecked = dt2.Field<bool>("IsChecked") };
                            //if (commonRows != null)
                            //{
                            //    //for the rows that are common, update the dtAllAvailableSwitches table
                            //    foreach (var commonRowInfo in commonRows)
                            //    {
                            //        commonRowInfo.dt1.SetField("SwitchID", commonRowInfo.SwitchID);
                            //        commonRowInfo.dt1.SetField("IsChecked", commonRowInfo.IsChecked);
                            //    }
                            //}
                            dtAllAvailableSwitched.DefaultView.Sort = "IsChecked desc";
                        }
                        gvSwitches.DataSource = dtAllAvailableSwitched;
                        gvSwitches.DataBind();
                        gvSwitches.Visible = true;
                    }
                }
                else
                {
                    DataTable dt = DAILYLOG.sp_DailyLog_Switches_Select(tbDate.Text, ddlArea.SelectedValue, ddlLocation.SelectedValue, ddlShift.SelectedValue);
                    if (DAILYLOG.successfulCommit)
                    {
                        if ((dt.Rows.Count > 0) && (dt != null))
                        {
                            gvSwitches.DataSource = dt;
                            gvSwitches.DataBind();
                            gvSwitches.Visible = true;
                        }
                        else
                        {
                            gvSwitches.Visible = false;
                        }
                    }
                }
            }

            if (DAILYLOG.successfulCommit == false)
                ShowMessage("Something went wrong and Switches were not loaded. Kindly try again and if the error persists, check the application log for details.", MessageType.Error);

        }

        private void loadDoubles()
        {
            //Get person on the log
            DataTable dt = DAILYLOG.sp_DailyLog_DoublesWorking_Select(dailyLogID);
            if (DAILYLOG.successfulCommit)
            {
                if ((dt.Rows.Count > 0) && (dt != null))
                {
                    gvDoubles.DataSource = dt;
                    gvDoubles.DataBind();
                    gvDoubles.Visible = true;
                }
                else
                {
                    gvDoubles.DataSource = dt;
                    gvDoubles.DataBind();
                    gvDoubles.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and Doubles were not loaded. Kindly try again and if the error persists, check the application log for details.", MessageType.Error);
            }
        }

        protected void gvDoubles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox cbPresent = (CheckBox)e.Row.FindControl("cbPresent");
                cbPresent.Checked = true;
            }
        }

        protected void gvPersonWorking_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblisStandBy = (Label)e.Row.FindControl("lblisStandBy");
                Label lblIsDayOff = (Label)e.Row.FindControl("lblIsDayOff");
                Label lblFullName = (Label)e.Row.FindControl("lblFullName");
                Label lblIsPresent = (Label)e.Row.FindControl("lblIsPresent");
                CheckBox cbPresent = (CheckBox)e.Row.FindControl("cbPresent");

                if (((lblisStandBy.Text == "0") || (lblisStandBy.Text == "False")) && ((lblIsDayOff.Text == "1") || (lblIsDayOff.Text == "True")))
                {
                    lblFullName.ForeColor = System.Drawing.Color.Maroon;
                }
                else if ((lblisStandBy.Text == "1") || (lblisStandBy.Text == "True"))
                {
                    lblFullName.ForeColor = System.Drawing.Color.Green;
                }

                if ((lblIsPresent.Text == "1") || (lblIsPresent.Text == "True"))
                {
                    cbPresent.Checked = true;
                }
            }
        }

        protected void gvSwitches_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblisStandBy = (Label)e.Row.FindControl("lblisStandBy");
                Label lblIsDayOff = (Label)e.Row.FindControl("lblIsDayOff");
                Label lblFullName = (Label)e.Row.FindControl("lblFullName");
                Label lblIsChecked = (Label)e.Row.FindControl("lblIsChecked");
                CheckBox cbPresent = (CheckBox)e.Row.FindControl("cbPresent");

                if ((lblisStandBy.Text == "1") || (lblisStandBy.Text == "True"))
                {
                    lblFullName.ForeColor = System.Drawing.Color.Green;
                }

                if ((lblIsChecked.Text == "1") || (lblIsChecked.Text == "True"))
                {
                    cbPresent.Checked = true;
                }
            }
        }


        public bool workScheduleCreated(string year, string month, byte areaID, byte shiftID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Mgr_WorkSchedule
                           where a.Year == year && a.Month == month && a.AreaID == areaID && a.ShiftID == shiftID
                           select new
                           {
                               a.WorkScheduleID
                           }).ToList();

            if (results.Count > 0)
            {
                foreach (var val in results)
                    workScheduleID = val.WorkScheduleID;
                isDuplicate = true;
            }
            else
            {
                isDuplicate = false;
            }
            return isDuplicate;
        }

        public bool duplicateCheck_DailyLog(DateTime date, byte areaID, Int16 LocationID, byte shiftID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_DailyLogs
                           where a.Date == date && a.LocationID == LocationID && a.AreaID == areaID && a.ShiftID == shiftID
                           select new
                           {
                               a.DailyLogID,
                               a.LogNumber
                           }).ToList();

            if (results.Count > 0)
            {
                foreach (var val in results)
                {
                    dailyLogID = val.DailyLogID;
                    lblReportNumber.Text = val.LogNumber;
                }
                isDuplicate = true;
                isLoaded = true;
            }
            else
            {
                isDuplicate = false;
                isLoaded = false;
            }
            return isDuplicate;
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearForm();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                updateDate();
            }
        }


        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                updateDate();
                clearForm();
            }
        }

        private void updateDate()
        {
            DateTime date = Convert.ToDateTime(tbDate.Text);
            date = date.Date;
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            byte areaID = Convert.ToByte(ddlArea.SelectedValue);
            byte shiftID = Convert.ToByte(ddlShift.SelectedValue);
            Int16 locationID = Convert.ToInt16(ddlLocation.SelectedValue);

            DAILYLOG.update_DailyLog(dailyLogID, date, areaID, locationID, shiftID, gvPersonWorking, ddlArea.SelectedItem.Text, ddlLocation.SelectedItem.Text, ddlShift.SelectedItem.Text, lblReportNumber.Text, userID, workScheduleID, gvSwitches, gvDoubles);
            if (DAILYLOG.successfulCommit)
            {
                ShowMessage("Record saved.", MessageType.Success);
                loadPersonsWorking();
                loadSwitches();
                loadDoubles();
                isLoaded = true;
            }
            else
            {
                ShowMessage("Something went wrong and the log was not created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }
        }

        protected void btnNewCallLog_Click(object sender, EventArgs e)
        {
            clearCallLogControls();
            showCallLogModal();
        }

        private void showCallLogModal()
        {
            // mpeCallLog.Show();
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#callLogModal').modal('toggle');</script>", false);
        }

        private void hideCallLogModal()
        {
            // mpeCallLog.Hide();
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#callLogModal').modal('hide');</script>", false);
        }

        private void clearCallLogControls()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            string s = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("HH:mm:ss");
            lblTime.Text = s;
            rbYes.Checked = false;
            rbNo.Checked = true;
            tbReports.Text = "";
            isCallLogLoaded = false;
            getSetLastCheckTimeForSelectedLocation(dailyLogID);
            isCallLogLoaded = false;
            lblSelectedCallLogID.Text = "";
        }

        protected void btnCancelCallLog_Click(object sender, EventArgs e)
        {
            clearCallLogControls();
            hideCallLogModal();
        }

        protected void btnSaveCloseCallLog_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveUpdateCallLog();
                if (success)
                {
                    ShowMessage("Record saved.", MessageType.Success);
                    loadCallLogs(dailyLogID);
                    hideCallLogModal();
                }
                else
                {
                    ShowMessage("Something went wrong and the call was not created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    showCallLogModal();
                }
            }
        }

        protected void btnSaveNextCallLog_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveUpdateCallLog();
                if (success)
                {
                    ShowMessage("Record saved", MessageType.Success);
                    clearCallLogControls();
                    loadCallLogs(dailyLogID);
                    showCallLogModal();
                }
                else
                {
                    ShowMessage("Something went wrong and the call was not created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    showCallLogModal();
                }
            }
        }

        private bool saveUpdateCallLog()
        {
            bool success = false;
            try
            {
                if (isCallLogLoaded)
                {
                    TimeSpan time = new TimeSpan();
                    if (TimeSpan.TryParse(lblTime.Text, out time))
                    {
                        bool supCheckLocation = rbYes.Checked ? true : false;
                        DateTime date = Convert.ToDateTime(tbDate.Text);
                        Int64 callLogID = 0;
                        if (lblSelectedCallLogID.Text != "")
                            callLogID = Convert.ToInt64(lblSelectedCallLogID.Text);
                        if (callLogID != 0)
                        {
                            DAILYLOG.update_CallLog(callLogID, dailyLogID, time, tbReports.Text, supCheckLocation, date.Date, ddlArea.SelectedItem.Text, ddlLocation.SelectedItem.Text, ddlShift.SelectedItem.Text, userID, lblReportNumber.Text);
                            success = DAILYLOG.successfulCommit;
                            getSetLastCheckLocationTimeForArea();
                            setDailyLogAuditLog(dailyLogID);
                        }
                        else
                        {
                            success = false;
                            ShowMessage("Call log ID not found", MessageType.Warning);
                        }
                    }
                    else
                    {
                        success = false;
                        ShowMessage("Invalid time format", MessageType.Warning);
                    }
                }
                else
                {
                    TimeSpan time = new TimeSpan();
                    if (TimeSpan.TryParse(lblTime.Text, out time))
                    {
                        bool supCheckLocation = rbYes.Checked ? true : false;
                        DateTime date = Convert.ToDateTime(tbDate.Text);
                        DAILYLOG.insert_CallLog(dailyLogID, time, tbReports.Text, supCheckLocation, date.Date, ddlArea.SelectedItem.Text, ddlLocation.SelectedItem.Text, ddlShift.SelectedItem.Text, userID, lblReportNumber.Text);
                        success = DAILYLOG.successfulCommit;
                        getSetLastCheckLocationTimeForArea();
                        setDailyLogAuditLog(dailyLogID);
                    }
                    else
                    {
                        success = false;
                        ShowMessage("Invalid time format", MessageType.Warning);
                    }
                }
            }
            catch (System.Exception excec)
            {
                MISC.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                success = false;
            }
            return success;
        }

        //Load the call logs
        private void loadCallLogs(int dailyLogID)
        {
             DataTable dtCallLogs = DAILYLOG.sp_DailyLog_CallLog_Select(dailyLogID);

             if ((dtCallLogs.Rows.Count > 0) && (dtCallLogs != null))
             {
                 gvCallLog.DataSource = dtCallLogs;
                 gvCallLog.DataBind();
                 gvCallLog.Visible = true;
             }
             else
             {
                 gvCallLog.Visible = false;
             }
        }

        private void getSetLastCheckLocationTimeForArea()
        {
            DataTable dt = DAILYLOG.sp_DailyLog_LastCheckLocationAndTime(Convert.ToDateTime(tbDate.Text), Convert.ToByte(ddlArea.SelectedValue), Convert.ToByte(ddlShift.SelectedValue));
            if ((dt.Rows.Count > 0) && (dt != null))
            {
                lblLastCheckLocation.Text = dt.Rows[0]["LocationName"].ToString();
                lblLastAreaCheckTime.Text = dt.Rows[0]["TimeOfCall"].ToString();
            }
            else
            {
                lblLastCheckLocation.Text = "";
                lblLastAreaCheckTime.Text = "";
            }
        }

        private void getSetLastCheckTimeForSelectedLocation(int dailyLodIG)
        {
            DataTable dt = DAILYLOG.sp_DailyLog_LastCheckTime(dailyLodIG);
            if ((dt.Rows.Count > 0) && (dt != null))
                lblLastCheckTimeAtThisLocation.Text = dt.Rows[0]["TimeOfCall"].ToString();
            else
                lblLastCheckTimeAtThisLocation.Text = "";
        }

        protected void gvCallLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCallLog.PageIndex = e.NewPageIndex;
            gvCallLog.SelectedIndex = -1;
            loadCallLogs(dailyLogID);
        }

        private void setDailyLogAuditLog(int dlID)
        {
            DataTable dt = DAILYLOG.sp_DailyLog_Audit_Select(dlID);
            if ((dt.Rows.Count > 0) && (dt != null))
            {
                lblAuditTrail.Text = "Created By: " + dt.Rows[0]["CreatedBy"].ToString() + " Created On: " + dt.Rows[0]["CreatedOn"].ToString() + " Last Modified By: " + dt.Rows[0]["LastModifiedBy"].ToString() + " Last Modified On: " + dt.Rows[0]["LastModifiedOn"].ToString();
            }
            else
            {
                lblAuditTrail.Text = "";
            }
        }

        protected void gvCallLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvCallLog.SelectedIndex > -1)
            {
                Label lblCreatedOn = (Label)gvCallLog.SelectedRow.FindControl("lblCreatedOn");
                DateTime currentDateTime = DateTime.Now;
                DateTime createdOn = Convert.ToDateTime(lblCreatedOn.Text);

                TimeSpan timePassed = currentDateTime.Subtract(createdOn);
                double minutesElapsed = timePassed.Minutes;

             //only allow editing before five minte has passed since call saved or user has manager role
                if ((minutesElapsed < 5) || (roleNames.Contains("Manager")) || (roleNames.Contains("Supervisor")))
                {
                    Label lblTime2 = (Label)gvCallLog.SelectedRow.FindControl("lblTime");
                    Label lblReports = (Label)gvCallLog.SelectedRow.FindControl("lblReports");
                    Label lblSupCheck = (Label)gvCallLog.SelectedRow.FindControl("lblSupCheck");
                    Label lblCallLogID = (Label)gvCallLog.SelectedRow.FindControl("lblCallLogID");

                    lblTime.Text = lblTime2.Text;
                    if (lblSupCheck.Text == "YES")
                    {
                        rbYes.Checked = true;
                        rbNo.Checked = false;
                    }
                    else
                    {
                        rbYes.Checked = false;
                        rbNo.Checked = true;
                    }
                    tbReports.Text = lblReports.Text;
                    getSetLastCheckTimeForSelectedLocation(dailyLogID);
                    lblSelectedCallLogID.Text = lblCallLogID.Text;
                    isCallLogLoaded = true;
                    showCallLogModal();
                }
                else
                    ShowMessage("Time period to edit has passed.", MessageType.Info);
            }
        }

        private void showDoublesModal()
        {
            //mpeDoubles.Show();
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#doublesModal').modal('toggle');</script>", false);
        }

        private void hideDoublesModal()
        {
            //mpeDoubles.Hide();
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#doublesModal').modal('hide');</script>", false);
        }

        protected void btnNewDouble_Click(object sender, EventArgs e)
        {
            if (isLoaded == true)
            {
                clearDoublesPopUp();
                showDoublesModal();
            }
        }
        private void clearDoublesPopUp()
        {
            tbName.Text = "";
            gvDoublesFound.Visible = false;
            rbAnyEmployee.Checked = false;
            rbPreviousShifts.Checked = true;
        }

        protected void btnCancelDouble_Click(object sender, EventArgs e)
        {
            hideDoublesModal();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (rbPreviousShifts.Checked)
            {
                //Get the current shift start and end times
                string fromTime = "";
                string totime = "";
                byte shiftID = Convert.ToByte(ddlShift.SelectedValue);
                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Ref_Shifts
                               where a.IsActive == true && a.ShiftID == shiftID
                               select new
                               {
                                   a.FromTime,
                                   a.ToTime
                               }).ToList();

                foreach (var val in results)
                {
                    fromTime = val.FromTime.ToString();
                    totime = val.ToTime.ToString();
                }

                DataTable dt = new DataTable();
                bool validShift = false;
                if (fromTime == "19:00:00")
                {
                    dt = DAILYLOG.sp_DailyLog_Doubles_Select(tbDate.Text, ddlArea.SelectedValue, fromTime, totime, tbName.Text);
                    validShift = true;
                }
                else if (fromTime == "07:00:00")
                {
                    DateTime logDate = Convert.ToDateTime(tbDate.Text);
                    DateTime previousDayDate = logDate.AddDays(-1);

                    dt = DAILYLOG.sp_DailyLog_Doubles_Select(previousDayDate, ddlArea.SelectedValue, fromTime, totime, tbName.Text);
                    validShift = true;
                }
                if (validShift == true)
                {
                    if (DAILYLOG.successfulCommit)
                    {
                        if ((dt.Rows.Count > 0) && (dt != null))
                        {
                            gvDoublesFound.DataSource = dt;
                            gvDoublesFound.Visible = true;
                            gvDoublesFound.DataBind();
                        }
                        else
                            gvDoublesFound.Visible = false;
                    }
                    else
                    {
                        ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                        gvDoublesFound.Visible = false;
                    }
                }
                else
                {
                    ShowMessage("Previous shift only works for the 12 hours shifts (07:00 - 19:00 and 19:00 - 07:00). Kindly use the Any Employee search feature to find the desired employee for this shift.", MessageType.Info);
                    gvDoublesFound.Visible = false;
                }
            }
            else
            {
                DataTable dt = DAILYLOG.sp_DailyLog_Doubles_SelectAllEmployees(tbName.Text);
                if (DAILYLOG.successfulCommit)
                {
                    if ((dt.Rows.Count > 0) && (dt != null))
                    {
                        gvDoublesFound.DataSource = dt;
                        gvDoublesFound.Visible = true;
                        gvDoublesFound.DataBind();
                    }
                    else
                        gvDoublesFound.Visible = false;
                }
                else
                {
                    ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    gvDoublesFound.Visible = false;
                }
            }
            showDoublesModal();
        }

        private bool addDoubles()
        {
            bool success = false;
            try
            {
                DateTime date = Convert.ToDateTime(tbDate.Text);

                for (int i = 0; i < gvDoublesFound.Rows.Count; i++)
                {
                    Label lblEmployeeID = (Label)gvDoublesFound.Rows[i].FindControl("lblEmployeeID");
                    CheckBox cbSelect = (CheckBox)gvDoublesFound.Rows[i].FindControl("cbSelect");
                    if (cbSelect.Checked)
                    {
                        Int16 empID = 0;
                        empID = Convert.ToInt16(lblEmployeeID.Text);
                        DAILYLOG.insert_double(dailyLogID, empID, date, ddlArea.SelectedItem.Text, ddlLocation.SelectedItem.Text, ddlShift.SelectedItem.Text, userID, lblReportNumber.Text);
                        success = DAILYLOG.successfulCommit;
                    }
                }
            }
            catch (System.Exception excec)
            {
                MISC.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
               success = false;
            }
            return success;
        }

        protected void btnAddCloseDouble_Click(object sender, EventArgs e)
        {
            bool success = addDoubles();
            if (success)
            {
                ShowMessage("Record saved", MessageType.Success);
                loadDoubles();
                hideDoublesModal();
            }
            else
            {
                ShowMessage("Something went wrong and the double was not added. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }
        }

        protected void btnAddNextDouble_Click(object sender, EventArgs e)
        {
            bool success = addDoubles();
            if (success)
            {
                ShowMessage("Record saved", MessageType.Success);
                loadDoubles();
                clearDoublesPopUp();
                showDoublesModal();
            }
            else
            {
                ShowMessage("Something went wrong and the double was not added. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                showDoublesModal();
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new or modify existing daily log report below";
            clearForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            btnBackToSearch.Visible = true;
            btnBackToSearch2.Visible = true;
        }

        protected void btnSearchResults_Click(object sender, EventArgs e)
        {
            loadSearchedResults();
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchReportNumber.Text = "";
            tbSearchDate.Text = "";
            ddlSearchShift.SelectedIndex = 0;
            ddlSearchArea.SelectedIndex = 0;
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            gvResults.Visible = false;
            lblGVResultsHeader.Text = "";
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            btnBackToSearch.Visible = false;
            btnBackToSearch2.Visible = false;
            pnlSearch.Visible = true;
            lblGVResultsHeader.Text = "";
            if (gvResults.Rows.Count > 0)
                loadSearchedResults();
        }

        private void loadSearchedResults()
        {
            DataTable dt = DAILYLOG.sp_DailyLog_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchArea.SelectedValue, ddlSearchLocation.SelectedValue, ddlSearchShift.SelectedValue);
            if (DAILYLOG.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvResults.DataSource = dt;
                    gvResults.DataBind();
                    gvResults.Visible = true;
                    lblGVResultsHeader.Text = "Daily Logs found: " + dt.Rows.Count;
                }
                else
                {
                    gvResults.Visible = false;
                    lblGVResultsHeader.Text = "No Daily Logs found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvResults.Visible = false;
                lblGVResultsHeader.Text = "No Daily Logs found due to error.";
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
                Label lblDailyLogID = (Label)gvResults.SelectedRow.FindControl("lblDailyLogID");
                Label lblAreaID = (Label)gvResults.SelectedRow.FindControl("lblAreaID");
                Label lblLocationID = (Label)gvResults.SelectedRow.FindControl("lblLocationID");
                Label lblShiftID = (Label)gvResults.SelectedRow.FindControl("lblShiftID");
                Label lblDate = (Label)gvResults.SelectedRow.FindControl("lblDate");
                Label lblCreatedBy = (Label)gvResults.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvResults.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedOn");
                Label lblLogNumber = (Label)gvResults.SelectedRow.FindControl("lblLogNumber");

                dailyLogID = Convert.ToInt32(lblDailyLogID.Text);
                lblReportNumber.Text = lblLogNumber.Text;
                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;
                loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
                if (ddlLocation.Items.FindByValue(lblLocationID.Text) != null)
                    ddlLocation.SelectedValue = lblLocationID.Text;
                else
                    ddlLocation.SelectedIndex = 0;
                if (ddlShift.Items.FindByValue(lblShiftID.Text) != null)
                    ddlShift.SelectedValue = lblShiftID.Text;
                else
                    ddlShift.SelectedIndex = 0;
                tbDate.Text = lblDate.Text;
                lblHeader.Text = "Update the selected daily log below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;

                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                btnBackToSearch.Visible = true;
                btnBackToSearch2.Visible = true;
                setGetData();
                ibDate.Enabled = false;
            }
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvCallLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvDoublesFound_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void cbPresent_CheckedChanged(object sender, EventArgs e)
        {
            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
            //    if (chk.Value == chk.FalseValue || chk.Value == null)
            //    {
            //        chk.Value = chk.TrueValue;
            //    }
            //    else
            //    {
            //        chk.Value = chk.FalseValue;
            //    }

            //}
            //dataGridView1.EndEdit();
        }

       
    }
}