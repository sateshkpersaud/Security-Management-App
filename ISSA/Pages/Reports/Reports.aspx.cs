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
using Microsoft.Reporting.WebForms;

namespace ISSA.Pages.Reports
{
    public partial class Reports : System.Web.UI.Page
    {
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        public enum MessageType { Success, Error, Info, Warning };

        public string loggedInUser
        {
            get
            {
                return (string)ViewState["loggedInUser"];
            }
            set
            {
                ViewState["loggedInUser"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                byte userID = MISC.getUserID();
                if (userID != 0)
                {
                    string[] roleNames = MISC.getUserRoles();
                    if (!roleNames.Contains("Supervisor"))
                    {
                        loggedInUser = MISC.getUserName().ToUpper();
                        loadDDLReports();
                        clearForm();
                        dtpStartDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + dtpStartDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        dtpEndDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + dtpEndDate.ClientID + ",'" + DateTime.Now.Date + "')");
                    }
                    else
                    {
                         byte positionID = MISC.getPositionID(MISC.getUserName());
                         if (roleNames.Contains("Supervisor") && (positionID == 10))
                         {
                             loggedInUser = MISC.getUserName().ToUpper();
                             loadDDLReports();
                             clearForm();
                             dtpStartDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + dtpStartDate.ClientID + ",'" + DateTime.Now.Date + "')");
                             dtpEndDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + dtpEndDate.ClientID + ",'" + DateTime.Now.Date + "')");
                         }
                         else
                         {
                             ShowMessage("No ID returned for logged in user", MessageType.Error);
                             Response.Redirect("../../Pages/Account/Login.aspx");
                         }
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

        private void loadDDLReports()
        {
            string[] roleNames = MISC.getUserRoles();

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Reports
                           join b in model.tbl_ReportsRole on a.ReportID equals b.ReportID
                           join c in model.Roles on b.RoleID equals c.RoleId
                           where roleNames.Contains(c.RoleName)
                           select new
                           {
                               a.ReportName,
                               a.ReportID
                           }).Distinct().OrderBy(ReportName => ReportName).ToList();

            if (results.Count > 0)
            {
                ddlReports.DataSource = results;
                ddlReports.DataTextField = "ReportName";
                ddlReports.DataValueField = "ReportID";
                ddlReports.DataBind();
            }
        }

        protected void rbToday_CheckedChanged(object sender, EventArgs e)
        {
            string todayDate = DateTime.Now.ToString("MM'/'dd'/'yyyy");
            if (rbToday.Checked == true)
            {
                dtpStartDate.Text = todayDate;
                dtpEndDate.Text = todayDate;
                loadSubParam();
            }

        }

        protected void rbThisWeek_CheckedChanged(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            GetDatesThisWeek(ref startDate, ref endDate);
            if (rbThisWeek.Checked == true)
            {
                dtpStartDate.Text = startDate.ToString("MM'/'dd'/'yyyy");
                dtpEndDate.Text = endDate.ToString("MM'/'dd'/'yyyy");
                loadSubParam();
            }
        }

        protected void rbThisMonth_CheckedChanged(object sender, EventArgs e)
        {
            int daysinMonth = new int();
            int currentYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            int currentMonth = Convert.ToInt32(DateTime.Now.Month.ToString());
            daysinMonth = System.DateTime.DaysInMonth(currentYear, currentMonth);

            if (rbThisMonth.Checked == true)
            {
                dtpStartDate.Text = currentMonth + "/01/" + currentYear.ToString();
                dtpEndDate.Text = currentMonth + "/" + daysinMonth + "/" + currentYear;
                loadSubParam();
            }

        }

        protected void rbThisQuarter_CheckedChanged(object sender, EventArgs e)
        {
            int daysinMonth = new int();
            int currentYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            int currentMonth = Convert.ToInt32(DateTime.Now.Month.ToString());
            int thisQuarterStartMonth = 0;
            int thisQuarterEndMonth = 0;

            if (rbThisQuarter.Checked == true)
            {
                if ((currentMonth == 1) || (currentMonth == 2) || (currentMonth == 3))
                {
                    thisQuarterStartMonth = 1;
                    thisQuarterEndMonth = 3;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, thisQuarterEndMonth);
                    dtpStartDate.Text = "0" + thisQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" + thisQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth == 4) || (currentMonth == 5) || (currentMonth == 6))
                {
                    thisQuarterStartMonth = 4;
                    thisQuarterEndMonth = 6;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, thisQuarterEndMonth);
                    dtpStartDate.Text = "0" + thisQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" + thisQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth == 7) || (currentMonth == 8) || (currentMonth == 9))
                {
                    thisQuarterStartMonth = 7;
                    thisQuarterEndMonth = 9;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, thisQuarterEndMonth);
                    dtpStartDate.Text = "0" + thisQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" + thisQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth == 10) || (currentMonth == 11) || (currentMonth == 12))
                {
                    thisQuarterStartMonth = 10;
                    thisQuarterEndMonth = 12;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, thisQuarterEndMonth);
                    dtpStartDate.Text = thisQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = thisQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                loadSubParam();
            }
        }

        protected void rbThisYear_CheckedChanged(object sender, EventArgs e)
        {
            string currentYear = DateTime.Now.Year.ToString();
            if (rbThisYear.Checked == true)
            {
                dtpStartDate.Text = "01/01/" + currentYear;
                dtpEndDate.Text = "12/31/" + currentYear;
                loadSubParam();
            }

        }

        protected void rbYesterday_CheckedChanged(object sender, EventArgs e)
        {
            string yesterday = DateTime.Today.AddDays(-1).ToString("MM'/'dd'/'yyyy");
            if (rbYesterday.Checked == true)
            {
                dtpStartDate.Text = yesterday;
                dtpEndDate.Text = yesterday;
                loadSubParam();
            }
        }

        protected void rbLastWeek_CheckedChanged(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            GetDatesLastWeek(ref startDate, ref endDate);
            if (rbLastWeek.Checked == true)
            {
                dtpStartDate.Text = startDate.ToString("MM'/'dd'/'yyyy");
                dtpEndDate.Text = endDate.ToString("MM'/'dd'/'yyyy");
                loadSubParam();
            }
        }

        protected void rbLastMonth_CheckedChanged(object sender, EventArgs e)
        {
            int daysinMonth = new int();
            int currentYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            int currentMonth = Convert.ToInt32(DateTime.Now.Month.ToString());

            if (rbLastMonth.Checked == true)
            {
                if (currentMonth == 1)
                {
                    currentYear = currentYear - 1;
                    currentMonth = 12;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, currentMonth);
                    dtpStartDate.Text = currentMonth + "/01/" + currentYear;
                    dtpEndDate.Text = currentMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth > 1) && (currentMonth <= 10))
                {
                    currentMonth = currentMonth - 1;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, currentMonth);
                    dtpStartDate.Text = "0" + currentMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" +  currentMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else
                {
                    currentMonth = currentMonth - 1;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, currentMonth);
                    dtpStartDate.Text = currentMonth + "/01/" + currentYear;
                    dtpEndDate.Text = currentMonth + "/" + daysinMonth + "/" + currentYear;
                }
                loadSubParam();
            }
        }

        protected void rbLastQuarter_CheckedChanged(object sender, EventArgs e)
        {
            int daysinMonth = new int();
            int currentYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            int currentMonth = Convert.ToInt32(DateTime.Now.Month.ToString());
            int lastQuarterStartMonth = 0;
            int lastQuarterEndMonth = 0;

            if (rbLastQuarter.Checked == true)
            {
                if ((currentMonth == 1) || (currentMonth == 2) || (currentMonth == 3))
                {
                    lastQuarterStartMonth = 10;
                    lastQuarterEndMonth = 12;
                    currentYear = currentYear - 1;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, lastQuarterEndMonth);
                    dtpStartDate.Text = lastQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = lastQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth == 4) || (currentMonth == 5) || (currentMonth == 6))
                {
                    lastQuarterStartMonth = 1;
                    lastQuarterEndMonth = 3;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, lastQuarterEndMonth);
                    dtpStartDate.Text = "0" + lastQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" + lastQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth == 7) || (currentMonth == 8) || (currentMonth == 9))
                {
                    lastQuarterStartMonth = 4;
                    lastQuarterEndMonth = 6;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, lastQuarterEndMonth);
                    dtpStartDate.Text = "0" + lastQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" + lastQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                else if ((currentMonth == 10) || (currentMonth == 11) || (currentMonth == 12))
                {
                    lastQuarterStartMonth = 7;
                    lastQuarterEndMonth = 9;
                    daysinMonth = System.DateTime.DaysInMonth(currentYear, lastQuarterEndMonth);
                    dtpStartDate.Text = "0" + lastQuarterStartMonth + "/01/" + currentYear;
                    dtpEndDate.Text = "0" + lastQuarterEndMonth + "/" + daysinMonth + "/" + currentYear;
                }
                loadSubParam();
            }
        }

        protected void rbLastYear_CheckedChanged(object sender, EventArgs e)
        {
            int currentYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            if (rbLastYear.Checked == true)
            {
                currentYear -= 1;
                dtpStartDate.Text = "01/01/" + currentYear.ToString();
                dtpEndDate.Text = "12/31/" + currentYear.ToString();
                loadSubParam();
            }
        }

        private void GetDatesThisWeek(ref System.DateTime stDate, ref System.DateTime endDate)
        {
            double offset = 0;
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    offset = -1;
                    break;
                case DayOfWeek.Monday:
                    offset = -2;
                    break;
                case DayOfWeek.Tuesday:
                    offset = -3;
                    break;
                case DayOfWeek.Wednesday:
                    offset = -4;
                    break;
                case DayOfWeek.Thursday:
                    offset = -5;
                    break;
                case DayOfWeek.Friday:
                    offset = -6;
                    break;
                case DayOfWeek.Saturday:
                    offset = -7;
                    break;
            }
            stDate = DateTime.Now.AddDays(1 + offset);
            endDate = DateTime.Now.AddDays(7 + offset);
        }

        private void GetDatesLastWeek(ref System.DateTime stDate, ref System.DateTime endDate)
        {
            double offset = 0;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    offset = -1;
                    break;
                case DayOfWeek.Monday:
                    offset = -2;
                    break;
                case DayOfWeek.Tuesday:
                    offset = -3;
                    break;
                case DayOfWeek.Wednesday:
                    offset = -4;
                    break;
                case DayOfWeek.Thursday:
                    offset = -5;
                    break;
                case DayOfWeek.Friday:
                    offset = -6;
                    break;
                case DayOfWeek.Saturday:
                    offset = -7;
                    break;
            }
            stDate = DateTime.Now.AddDays(-6 + offset);
            endDate = DateTime.Now.AddDays(offset);
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            clearForm();
        }

        private void clearForm()
        {
            ddlReports.SelectedIndex = 0;
            dtpStartDate.Text = "";
            dtpEndDate.Text = "";
            rbToday.Checked = false;
            rbThisWeek.Checked = false;
            rbThisMonth.Checked = false;
            rbThisQuarter.Checked = false;
            rbThisYear.Checked = false;
            rbYesterday.Checked = false;
            rbLastWeek.Checked = false;
            rbLastMonth.Checked = false;
            rbLastQuarter.Checked = false;
            rbLastYear.Checked = false;
            rvReports.LocalReport.DataSources.Clear();
        }

        private string isEmployeeStandby(Int16 employeeID)
        {
            string isEmpStandby = "";

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_Employees
                           where a.EmployeeID == employeeID
                           select new
                           {
                               a.IsStandbyStaff
                           }).FirstOrDefault();
            if (results != null)
            {
                isEmpStandby = results.IsStandbyStaff.ToString();
            }
            return isEmpStandby;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                    DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);
                    //ATTENDANCE REPORT
                    if (ddlReports.SelectedValue == "1")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        Int16 employeeID = 0;
                        employeeID = Convert.ToInt16(ddlSubParam.SelectedValue);

                        double halves = 0.0;
                        double firstHalfDays = 0;
                        double secondHalfDays = 0;
                        double days = (endDate - startDate).TotalDays;
                        days = days + 1; //Add one as the subtraction does not include the first day
                        halves = days / 2; //Halve the days 
                        if (days >= 30.0) //If more than or equal to 30 days
                        {
                            if (halves % 1 == 0) //If there is no decimal
                            {
                                firstHalfDays = halves;
                                secondHalfDays = days - firstHalfDays;
                            }
                            else //If there is a decimal
                            {
                                firstHalfDays = halves - 0.5;
                                secondHalfDays = (days - firstHalfDays);
                            }
                        }
                        else if (days > 15.0) //Is more than 15 but less than 30 days
                        {
                            firstHalfDays = 15.0;
                            secondHalfDays = days - firstHalfDays;
                        }
                        else //Less than 15 days
                        {
                            firstHalfDays = days;
                            secondHalfDays = 0.0;
                        }

                        DateTime firstStart = new DateTime();
                        DateTime firstEnd = new DateTime();
                        DateTime secondStart = new DateTime();
                        DateTime secondEnd = new DateTime();

                        firstStart = startDate;
                        firstEnd = startDate.AddDays(firstHalfDays);
                        firstEnd = firstEnd.AddDays(-1);
                        secondStart = firstEnd.AddDays(1);
                        secondEnd = firstEnd.AddDays(secondHalfDays);

                        string isEmpStandBy = isEmployeeStandby(employeeID);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAttendance_HeaderTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAttendance_HeaderTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_EmployeeAttendance_Header, employeeID);

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAttendance_FirstHalfTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAttendance_FirstHalfTableAdapter();
                        Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_EmployeeAttendance_FirstHalf, employeeID, firstStart.Date, firstEnd.Date, isEmpStandBy);

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAttendance_SecondHalfTableAdapter Rpt_TableAdapter3 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAttendance_SecondHalfTableAdapter();
                        Rpt_TableAdapter3.Fill(DS_Report.TA_RPT_EmployeeAttendance_SecondHalf, employeeID, secondStart.Date, secondEnd.Date, isEmpStandBy);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (firstStart.Month == secondEnd.Month)
                            dr["ParamOne"] = "For the month of: " + firstStart.ToString("MMMM");
                        else
                            dr["ParamOne"] = "For the months of: " + firstStart.ToString("MMMM") + " to " + secondEnd.ToString("MMMM");
                        dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["ParamThree"] = firstStart.Year;
                        string currDate = DateTime.Now.ToString("MM'/'dd'/'yyyy");
                        currDate = currDate.Substring(0, 10);
                        dr["ParamFour"] = currDate;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDs2 = new ReportDataSource();
                        ReportDataSource RpDs3 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_EmployeeAttendance_Header";
                        RpDs1.Value = DS_Report.TA_RPT_EmployeeAttendance_Header;
                        RpDs2.Name = "TA_RPT_EmployeeAttendance_FirstHalf";
                        RpDs2.Value = DS_Report.TA_RPT_EmployeeAttendance_FirstHalf;
                        RpDs3.Name = "TA_RPT_EmployeeAttendance_SecondHalf";
                        RpDs3.Value = DS_Report.TA_RPT_EmployeeAttendance_SecondHalf;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_EmployeeAttendance.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDs2);
                        rvReports.LocalReport.DataSources.Add(RpDs3);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }
                    //ABSENCE REPORT
                    else if (ddlReports.SelectedValue == "2")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        byte shiftID = 0;
                        if (ddlSubParam.SelectedIndex > 0)
                            shiftID = Convert.ToByte(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAbsentTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAbsentTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_EmployeeAbsent, startDate.Date, endDate.Date, shiftID);

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAbsent_OperatorTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_EmployeeAbsent_OperatorTableAdapter();
                        Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_EmployeeAbsent_Operator, startDate.Date, endDate.Date, shiftID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (startDate < endDate)
                            dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                        else if (startDate == endDate)
                            dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDs2 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_EmployeeAbsent";
                        RpDs1.Value = DS_Report.TA_RPT_EmployeeAbsent;
                        RpDs2.Name = "TA_RPT_EmployeeAbsent_Operator";
                        RpDs2.Value = DS_Report.TA_RPT_EmployeeAbsent_Operator;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_EmployeeAbsent.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDs2);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }
                    //DAILY POST ASSIGNMENTS
                    else if (ddlReports.SelectedValue == "3")
                    {
                        if (ddlSubParam.SelectedIndex > -1)
                        {
                            rvReports.ProcessingMode = ProcessingMode.Local;
                            int postAssID = 0;
                            postAssID = Convert.ToInt32(ddlSubParam.SelectedValue);

                            ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                            DS_Report.EnforceConstraints = false;

                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyPostAssignmentTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyPostAssignmentTableAdapter();
                            Rpt_TableAdapter.Fill(DS_Report.TA_RPT_DailyPostAssignment, postAssID);
                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyPostAssignment_MasterRosterTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyPostAssignment_MasterRosterTableAdapter();
                            //Get the date and shift for the post assignment ID
                            Dictionary<string, string> vals = getShiftIDAndDate(postAssID);
                            byte ShiftID = new byte();

                            DateTime dt = new DateTime();

                            foreach (var val in vals)
                            {
                                ShiftID = Convert.ToByte(val.Key);
                                dt = Convert.ToDateTime(val.Value);
                            }
                            string date = dt.Date.ToString("MM'/'dd'/'yyyy");
                            Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_DailyPostAssignment_MasterRoster, date, ShiftID);

                            ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                            DataRow dr = dtParam.NewRow();
                            if (startDate < endDate)
                                dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                            else if (startDate == endDate)
                                dr["ParamOne"] = dtpStartDate.Text;
                            if (ddlSubParam.SelectedIndex == 0)
                                dr["ParamTwo"] = "All";
                            else
                                dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                            dr["User"] = loggedInUser;
                            dr["Date"] = DateTime.Now;
                            dtParam.Rows.Add(dr);

                            ReportDataSource RpDs1 = new ReportDataSource();
                            ReportDataSource RpDs2 = new ReportDataSource();
                            ReportDataSource RpDsParameters = new ReportDataSource();

                            RpDs1.Name = "TA_RPT_DailyPostAssignment";
                            RpDs1.Value = DS_Report.TA_RPT_DailyPostAssignment;
                            RpDs2.Name = "TA_RPT_DailyPostAssignment_MasterRoster";
                            RpDs2.Value = DS_Report.TA_RPT_DailyPostAssignment_MasterRoster;
                            RpDsParameters.Name = "dtParameters";
                            RpDsParameters.Value = dtParam;

                            rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_DailyPostAssignment.rdlc";
                            rvReports.LocalReport.DataSources.Clear();
                            rvReports.LocalReport.DataSources.Add(RpDs1);
                            rvReports.LocalReport.DataSources.Add(RpDs2);
                            rvReports.LocalReport.DataSources.Add(RpDsParameters);
                            rvReports.LocalReport.Refresh();
                            rvReports.Visible = true;
                            rvReports.ShowReportBody = true;
                        }
                        else
                            ShowMessage("This report cannot be generated as no Daily Post Assignment has been created.", MessageType.Info);
                    }

                    //WEEKLY SECURITY REPORT
                    else if (ddlReports.SelectedValue == "4")
                    {
                        if (ddlSubParam.SelectedIndex > -1)
                        {
                            rvReports.ProcessingMode = ProcessingMode.Local;
                            int securityID = 0;
                            securityID = Convert.ToInt32(ddlSubParam.SelectedValue);

                            ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                            DS_Report.EnforceConstraints = false;

                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WeeklySecurityReportTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WeeklySecurityReportTableAdapter();
                            Rpt_TableAdapter.Fill(DS_Report.TA_RPT_WeeklySecurityReport, securityID);

                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WeeklySecurityReport_ComparisonTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WeeklySecurityReport_ComparisonTableAdapter();
                            Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_WeeklySecurityReport_Comparison, securityID);

                            ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                            DataRow dr = dtParam.NewRow();
                            if (startDate < endDate)
                                dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                            else if (startDate == endDate)
                                dr["ParamOne"] = dtpStartDate.Text;
                            if (ddlSubParam.SelectedIndex == 0)
                                dr["ParamTwo"] = "All";
                            else
                                dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                            dr["User"] = loggedInUser;
                            dr["Date"] = DateTime.Now;
                            dtParam.Rows.Add(dr);

                            ReportDataSource RpDs1 = new ReportDataSource();
                            ReportDataSource RpDs2 = new ReportDataSource();
                            ReportDataSource RpDsParameters = new ReportDataSource();

                            RpDs1.Name = "TA_RPT_WeeklySecurityReport";
                            RpDs1.Value = DS_Report.TA_RPT_WeeklySecurityReport;
                            RpDs2.Name = "TA_RPT_WeeklySecurityReport_Comparison";
                            RpDs2.Value = DS_Report.TA_RPT_WeeklySecurityReport_Comparison;
                            RpDsParameters.Name = "dtParameters";
                            RpDsParameters.Value = dtParam;

                            rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_WSR.rdlc";
                            rvReports.LocalReport.DataSources.Clear();
                            rvReports.LocalReport.DataSources.Add(RpDs1);
                            rvReports.LocalReport.DataSources.Add(RpDs2);
                            rvReports.LocalReport.DataSources.Add(RpDsParameters);
                            rvReports.LocalReport.Refresh();
                            rvReports.Visible = true;
                            rvReports.ShowReportBody = true;
                        }
                        else
                            ShowMessage("This report cannot be generated as no Work Schedule has been created.", MessageType.Info);
                           
                    }
                    //DAILY OPERATIONS REPORT
                    else if (ddlReports.SelectedValue == "5")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        byte shiftID = 0;
                        if (ddlSubParam.SelectedIndex > 0)
                            shiftID = Convert.ToByte(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyOperationsTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyOperationsTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_DailyOperations, startDate.Date, shiftID);

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyOperations_IncidentTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyOperations_IncidentTableAdapter();
                        Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_DailyOperations_Incident, startDate.Date, shiftID);

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyOperations_SupervisorsTableAdapter Rpt_TableAdapter3 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_DailyOperations_SupervisorsTableAdapter();
                        Rpt_TableAdapter3.Fill(DS_Report.TA_RPT_DailyOperations_Supervisors, startDate.Date, shiftID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDs2 = new ReportDataSource();
                        ReportDataSource RpDs3 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_DailyOperations";
                        RpDs1.Value = DS_Report.TA_RPT_DailyOperations;
                        RpDs2.Name = "TA_RPT_DailyOperations_Incident";
                        RpDs2.Value = DS_Report.TA_RPT_DailyOperations_Incident;
                        RpDs3.Name = "TA_RPT_DailyOperations_Supervisors";
                        RpDs3.Value = DS_Report.TA_RPT_DailyOperations_Supervisors;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_DailyOperations.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDs2);
                        rvReports.LocalReport.DataSources.Add(RpDs3);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }
                    //COMPLAINT REPORT
                    else if (ddlReports.SelectedValue == "6")
                    {
                        if (ddlSubParam.SelectedIndex > -1)
                        {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        int complaintID = 0;
                        complaintID = Convert.ToInt32(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ComplaintTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ComplaintTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_Complaint, complaintID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (startDate < endDate)
                            dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                        else if (startDate == endDate)
                            dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_Complaint";
                        RpDs1.Value = DS_Report.TA_RPT_Complaint;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_Complaints.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                        }
                        else
                            ShowMessage("This report cannot be generated as no Complaint Report has been created.", MessageType.Info);
                    }
                    //INCIDENT REPORT
                    else if (ddlReports.SelectedValue == "7")
                    {
                        if (ddlSubParam.SelectedIndex > -1)
                        {
                            rvReports.ProcessingMode = ProcessingMode.Local;
                            int incidentID = 0;
                            incidentID = Convert.ToInt32(ddlSubParam.SelectedValue);

                            ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                            DS_Report.EnforceConstraints = false;

                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_IncidentTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_IncidentTableAdapter();
                            Rpt_TableAdapter.Fill(DS_Report.TA_RPT_Incident, incidentID);
                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_Incident_PersonsAffectedTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_Incident_PersonsAffectedTableAdapter();
                            Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_Incident_PersonsAffected, incidentID);

                            ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                            DataRow dr = dtParam.NewRow();
                            if (startDate < endDate)
                                dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                            else if (startDate == endDate)
                                dr["ParamOne"] = dtpStartDate.Text;
                            if (ddlSubParam.SelectedIndex == 0)
                                dr["ParamTwo"] = "All";
                            else
                                dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                            dr["User"] = loggedInUser;
                            dr["Date"] = DateTime.Now;
                            dtParam.Rows.Add(dr);

                            ReportDataSource RpDs1 = new ReportDataSource();
                            ReportDataSource RpDs2 = new ReportDataSource();
                            ReportDataSource RpDsParameters = new ReportDataSource();

                            RpDs1.Name = "TA_RPT_Incident";
                            RpDs1.Value = DS_Report.TA_RPT_Incident;
                            RpDs2.Name = "TA_RPT_Incident_PersonsAffected";
                            RpDs2.Value = DS_Report.TA_RPT_Incident_PersonsAffected;
                            RpDsParameters.Name = "dtParameters";
                            RpDsParameters.Value = dtParam;

                            rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_Incidents.rdlc";
                            rvReports.LocalReport.DataSources.Clear();
                            rvReports.LocalReport.DataSources.Add(RpDs1);
                            rvReports.LocalReport.DataSources.Add(RpDs2);
                            rvReports.LocalReport.DataSources.Add(RpDsParameters);
                            rvReports.LocalReport.Refresh();
                            rvReports.Visible = true;
                            rvReports.ShowReportBody = true;
                        }
                        else
                            ShowMessage("This report cannot be generated as no Incident Report has been created.", MessageType.Info);
                    }
                    //PAYROLL PROCESSING REPORT
                    else if (ddlReports.SelectedValue == "8")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        byte shiftID = 0;
                        if (ddlSubParam.SelectedIndex > 0)
                            shiftID = Convert.ToByte(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_PayrollProcessingTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_PayrollProcessingTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_PayrollProcessing, startDate.Date, endDate.Date, shiftID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (startDate < endDate)
                            dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                        else if (startDate == endDate)
                            dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_PayrollProcessing";
                        RpDs1.Value = DS_Report.TA_RPT_PayrollProcessing;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_PayrollProcessing.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }
                    //MANPOWER ALLOCATION
                    else if (ddlReports.SelectedValue == "9")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        Int16 locationID = 0;
                        locationID = Convert.ToInt16(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ManpowerAllocationTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ManpowerAllocationTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_ManpowerAllocation, startDate.Date, endDate.Date, locationID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (startDate < endDate)
                            dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                        else if (startDate == endDate)
                            dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_ManpowerAllocation";
                        RpDs1.Value = DS_Report.TA_RPT_ManpowerAllocation;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_ManpowerAllocation.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }
                    //SHORATGE REPORT - SUMMARY
                    else if (ddlReports.SelectedValue == "10")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        byte areaID = 0;
                        if (ddlSubParam.SelectedIndex > 0)
                            areaID = Convert.ToByte(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ShortageTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ShortageTableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_Shortage, startDate.Date, endDate.Date, areaID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (startDate < endDate)
                            dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                        else if (startDate == endDate)
                            dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_Shortage";
                        RpDs1.Value = DS_Report.TA_RPT_Shortage;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_Shortage.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }
                    //SHIFT REPORT
                    else if (ddlReports.SelectedValue == "11")
                    {
                        if (ddlSubParam.SelectedIndex > -1)
                        {
                            rvReports.ProcessingMode = ProcessingMode.Local;
                            int shiftReportID = 0;
                            shiftReportID = Convert.ToInt32(ddlSubParam.SelectedValue);

                            ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                            DS_Report.EnforceConstraints = false;

                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ShiftReportsTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ShiftReportsTableAdapter();
                            Rpt_TableAdapter.Fill(DS_Report.TA_RPT_ShiftReports, shiftReportID);
                            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ShiftReport_ChecksTableAdapter Rpt_TableAdapter2 = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_ShiftReport_ChecksTableAdapter();
                            Rpt_TableAdapter2.Fill(DS_Report.TA_RPT_ShiftReport_Checks, shiftReportID);

                            ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                            DataRow dr = dtParam.NewRow();
                            if (startDate < endDate)
                                dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                            else if (startDate == endDate)
                                dr["ParamOne"] = dtpStartDate.Text;
                            if (ddlSubParam.SelectedIndex == 0)
                                dr["ParamTwo"] = "All";
                            else
                                dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                            dr["User"] = loggedInUser;
                            dr["Date"] = DateTime.Now;
                            dtParam.Rows.Add(dr);

                            ReportDataSource RpDs1 = new ReportDataSource();
                            ReportDataSource RpDs2 = new ReportDataSource();
                            ReportDataSource RpDsParameters = new ReportDataSource();

                            RpDs1.Name = "TA_RPT_ShiftReports";
                            RpDs1.Value = DS_Report.TA_RPT_ShiftReports;
                            RpDs2.Name = "TA_RPT_ShiftReport_Checks";
                            RpDs2.Value = DS_Report.TA_RPT_ShiftReport_Checks;
                            RpDsParameters.Name = "dtParameters";
                            RpDsParameters.Value = dtParam;

                            rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_ShiftReport.rdlc";
                            rvReports.LocalReport.DataSources.Clear();
                            rvReports.LocalReport.DataSources.Add(RpDs1);
                            rvReports.LocalReport.DataSources.Add(RpDs2);
                            rvReports.LocalReport.DataSources.Add(RpDsParameters);
                            rvReports.LocalReport.Refresh();
                            rvReports.Visible = true;
                            rvReports.ShowReportBody = true;
                        }
                        else
                            ShowMessage("This report cannot be generated as no Shift Report has been created.", MessageType.Info);
                    }
                    //SHORATGE REPORT - DAILY
                    else if (ddlReports.SelectedValue == "12")
                    {
                        rvReports.ProcessingMode = ProcessingMode.Local;
                        byte areaID = 0;
                        if (ddlSubParam.SelectedIndex > 0)
                            areaID = Convert.ToByte(ddlSubParam.SelectedValue);

                        ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                        DS_Report.EnforceConstraints = false;

                        ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_Shortage3TableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_Shortage3TableAdapter();
                        Rpt_TableAdapter.Fill(DS_Report.TA_RPT_Shortage3, startDate.Date, endDate.Date, areaID);

                        ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                        DataRow dr = dtParam.NewRow();
                        if (startDate < endDate)
                            dr["ParamOne"] = dtpStartDate.Text + " to " + dtpEndDate.Text;
                        else if (startDate == endDate)
                            dr["ParamOne"] = dtpStartDate.Text;
                        if (ddlSubParam.SelectedIndex == 0)
                            dr["ParamTwo"] = "All";
                        else
                            dr["ParamTwo"] = ddlSubParam.SelectedItem.Text;
                        dr["User"] = loggedInUser;
                        dr["Date"] = DateTime.Now;
                        dtParam.Rows.Add(dr);

                        ReportDataSource RpDs1 = new ReportDataSource();
                        ReportDataSource RpDsParameters = new ReportDataSource();

                        RpDs1.Name = "TA_RPT_Shortage3";
                        RpDs1.Value = DS_Report.TA_RPT_Shortage3;
                        RpDsParameters.Name = "dtParameters";
                        RpDsParameters.Value = dtParam;

                        rvReports.LocalReport.ReportPath = "Pages/Reports/rpt_ShortageDaily.rdlc";
                        rvReports.LocalReport.DataSources.Clear();
                        rvReports.LocalReport.DataSources.Add(RpDs1);
                        rvReports.LocalReport.DataSources.Add(RpDsParameters);
                        rvReports.LocalReport.Refresh();
                        rvReports.Visible = true;
                        rvReports.ShowReportBody = true;
                    }

                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the report could not be generated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        protected void ddlReports_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadSubParam();
        }

        private void loadShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
           ddlSubParam.DataSource = dt;
           ddlSubParam.DataTextField = "shift";
           ddlSubParam.DataValueField = "ShiftID";
           ddlSubParam.DataBind();
        }

        private void loadArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlSubParam.DataSource = dt;
            ddlSubParam.DataTextField = "Area";
            ddlSubParam.DataValueField = "AreaID";
            ddlSubParam.DataBind();
        }

        private void loadComplaints()
        {
            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_Complaints
                           where a.DateOfIncident >= startDate.Date && a.DateOfIncident <= endDate.Date
                           select new
                           {
                               a.ComplaintID,
                               a.ComplaintNumber
                           }).OrderBy(ComplaintNumber => ComplaintNumber).ToList();
            ddlSubParam.DataSource = results;
            ddlSubParam.DataTextField = "ComplaintNumber";
            ddlSubParam.DataValueField = "ComplaintID";
            ddlSubParam.DataBind();
            }
        }

        private void loadIncidents()
        {
            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);

                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Op_Incidents
                               where a.DateOccured >= startDate.Date && a.DateOccured <= endDate.Date
                               select new
                               {
                                   a.IncidentID,
                                   a.IncidentNumber
                               }).OrderBy(IncidentNumber => IncidentNumber).ToList();
                ddlSubParam.DataSource = results;
                ddlSubParam.DataTextField = "IncidentNumber";
                ddlSubParam.DataValueField = "IncidentID";
                ddlSubParam.DataBind();
            }
        }

        private void loadLocations()
        {
            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);

                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Ref_Locations
                               select new
                               {
                                   a.LocationName,
                                   a.LocationID
                               }).OrderBy(LocationName => LocationName).ToList();
                ddlSubParam.DataSource = results;
                ddlSubParam.DataTextField = "LocationName";
                ddlSubParam.DataValueField = "LocationID";
                ddlSubParam.DataBind();
            }
        }

        private void loadEmployees()
        {
            //if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            //{
            //    var model = new EDM.DataSource();
            //    var results = (from a in model.tbl_Sup_Employees
            //                   where a.IsActive == true && a.IsEmployee == true
            //                   select new
            //                   {
            //                       FullName = a.FirstName == null ? a.LastName : a.LastName + ", " + a.FirstName,
            //                       a.EmployeeID
            //                   }).OrderBy(FullName => FullName).ToList();
            //    ddlSubParam.DataSource = results;
            //    ddlSubParam.DataTextField = "FullName";
            //    ddlSubParam.DataValueField = "EmployeeID";
            //    ddlSubParam.DataBind();
            //}

            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DataTable dt = MISC.sp_Employee_Sel4DDL_Report(dtpStartDate.Text, dtpEndDate.Text);
                ddlSubParam.DataSource = dt;
                ddlSubParam.DataTextField = "FullName";
                ddlSubParam.DataValueField = "EmployeeID";
                ddlSubParam.DataBind();
            }
        }

        private void loadWeeklySecurityReports()
        {
            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);

                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Mgr_WeeklySecurityReport
                               where a.WeekStart >= startDate.Date && a.WeekEnd <= endDate.Date
                               select new
                               {
                                   a.SecurityReportID,
                                   a.ReportNumber
                               }).OrderBy(ReportNumber => ReportNumber).ToList();
                ddlSubParam.DataSource = results;
                ddlSubParam.DataTextField = "ReportNumber";
                ddlSubParam.DataValueField = "SecurityReportID";
                ddlSubParam.DataBind();
            }
        }

        private void loadDailyPostAssignments()
        {
            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);

                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Sup_DailyPostAssignment
                               where a.Date >= startDate.Date && a.Date <= endDate.Date
                               select new
                               {
                                   a.PostAssignmentID,
                                   a.ReportNumber
                               }).OrderBy(ReportNumber => ReportNumber).ToList();
                ddlSubParam.DataSource = results;
                ddlSubParam.DataTextField = "ReportNumber";
                ddlSubParam.DataValueField = "PostAssignmentID";
                ddlSubParam.DataBind();
            }
        }

        private Dictionary<string, string> getShiftIDAndDate(int postAssignmentID)
        {
            Dictionary<string, string> vals = new Dictionary<string, string>();

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_DailyPostAssignment
                           where a.PostAssignmentID == postAssignmentID
                           select new
                           {
                               a.ShiftID,
                               a.Date
                           }).FirstOrDefault();
            vals.Add(results.ShiftID.ToString(), results.Date.ToString());
            return vals;
        }

        private void loadShiftReports()
        {
            if ((dtpStartDate.Text != "") && (dtpEndDate.Text != ""))
            {
                DateTime startDate = Convert.ToDateTime(dtpStartDate.Text);
                DateTime endDate = Convert.ToDateTime(dtpEndDate.Text);

                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Sup_ShiftReport
                               where a.Date >= startDate.Date && a.Date <= endDate.Date
                               select new
                               {
                                   a.ShiftReportID,
                                   a.ReportNumber
                               }).OrderBy(ReportNumber => ReportNumber).ToList();
                ddlSubParam.DataSource = results;
                ddlSubParam.DataTextField = "ReportNumber";
                ddlSubParam.DataValueField = "ShiftReportID";
                ddlSubParam.DataBind();
            }
        }

        protected void dtpStartDate_TextChanged(object sender, EventArgs e)
        {
            loadSubParam();
        }

        protected void dtpEndDate_TextChanged(object sender, EventArgs e)
        {
            loadSubParam();
        }

        private void loadSubParam()
        {
            if (ddlReports.SelectedValue == "1")
            {
                loadEmployees();
            }
            else if ((ddlReports.SelectedValue == "2") || (ddlReports.SelectedValue == "8") || (ddlReports.SelectedValue == "5"))
            {
                loadShifts();
            }
            else if (ddlReports.SelectedValue == "3")
            {
                loadDailyPostAssignments();
            }
            else if (ddlReports.SelectedValue == "4")
            {
                loadWeeklySecurityReports();
            }
            else if (ddlReports.SelectedValue == "6")
            {
                loadComplaints();
            }
            else if (ddlReports.SelectedValue == "7")
            {
                loadIncidents();
            }
            else if (ddlReports.SelectedValue == "9")
            {
                loadLocations();
            }
            else if (ddlReports.SelectedValue == "10")
            {
                loadArea();
            }
            else if (ddlReports.SelectedValue == "11")
            {
                loadShiftReports();
            }
            else if (ddlReports.SelectedValue == "12")
            {
                loadArea();
            }
            else
            {
                ddlSubParam.DataSource = null;
                ddlSubParam.DataBind();
            }
        }
    }
}