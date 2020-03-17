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
    public partial class WeeklySecurityReport : System.Web.UI.Page
    {
        BusinessTier.WeeklySecurityReport WSR = new BusinessTier.WeeklySecurityReport();
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

        public int securityReportID
        {
            get
            {
                return (int)ViewState["securityReportID"];
            }
            set
            {
                ViewState["securityReportID"] = value;
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

        public DataTable dtFinished
        {
            get
            {
                return (DataTable)ViewState["dtFinished"];
            }
            set
            {
                ViewState["dtFinished"] = value;
            }
        }

        public DataTable dtComparisonIDs
        {
            get
            {
                return (DataTable)ViewState["dtComparisonIDs"];
            }
            set
            {
                ViewState["dtComparisonIDs"] = value;
            }
        }

        public Dictionary<string, string> Shifts
        {
            get
            {
                return (Dictionary<string, string>)ViewState["Shifts"];
            }
            set
            {
                ViewState["Shifts"] = value;
            }
        }

        //public List<string> lShiftName
        //{
        //    get
        //    {
        //        return (List<string>)ViewState["lShiftName"];
        //    }
        //    set
        //    {
        //        ViewState["lShiftName"] = value;
        //    }
        //}

        public int shiftCount
        {
            get
            {
                return (int)ViewState["shiftCount"];
            }
            set
            {
                ViewState["shiftCount"] = value;
            }
        }

        public bool allshiftsLoaded
        {
            get
            {
                return (bool)ViewState["allshiftsLoaded"];
            }
            set
            {
                ViewState["allshiftsLoaded"] = value;
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
                    if (roleNames.Contains("Manager"))
                    {
                        isLoaded = false;
                        securityReportID = 0;
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        clearForm();
                        tbWeekEnd.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbWeekEnd.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbWeekStart.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbWeekStart.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbSearchWeekEnd.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchWeekEnd.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbSearchWeekStart.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchWeekStart.ClientID + ",'" + DateTime.Now.Date + "')");
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

        private void clearForm()
        {
            Shifts = new Dictionary<string, string>();
            loadDDLSearchArea();
            loadDDLArea();
            loadDDLShift();
            ddlArea.SelectedIndex = 0;
            gvResults.Visible = false;
            pnlReportDetails.Visible = false;
            lblGVResultsHeader.Text = "";
            lblReportNumber.Text = "";
            securityReportID = 0;
            isLoaded = false;
            generateShiftReportNumber();
            dtFinished = new DataTable();
            dtComparisonIDs = new DataTable();
            shiftCount = 0;
            allshiftsLoaded = false;
            tbWeekEnd.Text = "";
            tbWeekStart.Text = "";
            gvShiftReport.DataSource = null;
            gvShiftReport.DataBind();
        }

        //generate the number
        private void generateShiftReportNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Mgr_WeeklySecurityReport
                           where a.CreatedOn.Value.Year == year
                           select new
                           {
                               a.SecurityReportID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "WSR" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "WSR" + year.ToString() + currentSequence.ToString();
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

        private void loadDDLSearchArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlSearchArea.DataSource = dt;
            ddlSearchArea.DataTextField = "Area";
            ddlSearchArea.DataValueField = "AreaID";
            ddlSearchArea.DataBind();
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void gvShiftReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string shiftName = e.Row.Cells[0].Text;
                Label lblType = (Label)e.Row.FindControl("lblType");
                //Label lblCashInGT = (Label)e.Row.FindControl("lblCashInGT");
                //Label lblCashOutGT = (Label)e.Row.FindControl("lblCashOutGT");
                //Label lblOther = (Label)e.Row.FindControl("lblOther");
                Label lblMonday = (Label)e.Row.FindControl("lblMonday");
                Label lblTuesday = (Label)e.Row.FindControl("lblTuesday");
                Label lblWednesday = (Label)e.Row.FindControl("lblWednesday");
                Label lblThursday = (Label)e.Row.FindControl("lblThursday");
                Label lblFriday = (Label)e.Row.FindControl("lblFriday");
                Label lblSaturday = (Label)e.Row.FindControl("lblSaturday");
                Label lblSunday = (Label)e.Row.FindControl("lblSunday");
                Label lblPresent = (Label)e.Row.FindControl("lblPresent");
                Label lblAbsent = (Label)e.Row.FindControl("lblAbsent");
                Label lblIncidentsComplaints = (Label)e.Row.FindControl("lblIncidentsComplaints");
                Label lblSwitches = (Label)e.Row.FindControl("lblSwitches");
                Label lblShiftID = (Label)e.Row.FindControl("lblShiftID");
                TextBox tbMonday = (TextBox)e.Row.FindControl("tbMonday");
                TextBox tbTuesday = (TextBox)e.Row.FindControl("tbTuesday");
                TextBox tbWednesday = (TextBox)e.Row.FindControl("tbWednesday");
                TextBox tbThursday = (TextBox)e.Row.FindControl("tbThursday");
                TextBox tbFriday = (TextBox)e.Row.FindControl("tbFriday");
                TextBox tbSaturday = (TextBox)e.Row.FindControl("tbSaturday");
                TextBox tbSunday = (TextBox)e.Row.FindControl("tbSunday");
                TextBox tbPresent = (TextBox)e.Row.FindControl("tbPresent");
                TextBox tbAbsent = (TextBox)e.Row.FindControl("tbAbsent");
                TextBox tbIncidentsComplaints = (TextBox)e.Row.FindControl("tbIncidentsComplaints");
                TextBox tbSwitches = (TextBox)e.Row.FindControl("tbSwitches");

                if (lblType.Text == "Scheduled")
                {
                    //lblCashInGT.Visible = true;
                    //lblCashOutGT.Visible = true;
                    //lblOther.Visible = true;
                    lblMonday.Visible = true;
                    lblTuesday.Visible = true;
                    lblWednesday.Visible = true;
                    lblThursday.Visible = true;
                    lblFriday.Visible = true;
                    lblSaturday.Visible = true;
                    lblSunday.Visible = true;
                    lblPresent.Visible = true;
                    lblAbsent.Visible = true;
                    lblIncidentsComplaints.Visible = true;
                    lblSwitches.Visible = true;
                    tbMonday.Visible = false;
                    tbTuesday.Visible = false;
                    tbWednesday.Visible = false;
                    tbThursday.Visible = false;
                    tbFriday.Visible = false;
                    tbSaturday.Visible = false;
                    tbSunday.Visible = false;
                    tbPresent.Visible = false;
                    tbAbsent.Visible = false;
                    tbIncidentsComplaints.Visible = false;
                    tbSwitches.Visible = false;
                }
                else if (lblType.Text == "Actual")
                {
                    //lblCashInGT.Visible = false;
                    //lblCashOutGT.Visible = false;
                    //lblOther.Visible = false;
                    lblMonday.Visible = false;
                    lblTuesday.Visible = false;
                    lblWednesday.Visible = false;
                    lblThursday.Visible = false;
                    lblFriday.Visible = false;
                    lblSaturday.Visible = false;
                    lblSunday.Visible = false;
                    lblPresent.Visible = false;
                    lblAbsent.Visible = false;
                    lblIncidentsComplaints.Visible = false;
                    lblSwitches.Visible = false;
                    tbMonday.Visible = true;
                    tbTuesday.Visible = true;
                    tbWednesday.Visible = true;
                    tbThursday.Visible = true;
                    tbFriday.Visible = true;
                    tbSaturday.Visible = true;
                    tbSunday.Visible = true;
                    tbPresent.Visible = true;
                    tbAbsent.Visible = true;
                    tbIncidentsComplaints.Visible = true;
                    tbSwitches.Visible = true;

                    tbMonday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbTuesday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbWednesday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbThursday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbFriday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbSaturday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbSunday.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbPresent.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbAbsent.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbIncidentsComplaints.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                    tbSwitches.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
                }

                if (!Shifts.ContainsKey(lblShiftID.Text))
                    Shifts.Add(lblShiftID.Text, shiftName);
            }
        }

        protected void gvShiftReport_PreRender(object sender, EventArgs e)
        {
            MergeGridViewRows(gvShiftReport, 0, 17);
        }

        private void loadDDLShift()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            foreach (var vals in Shifts)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["ShiftID"].ToString() == vals.Key)
                    {
                        dr.Delete();
                    }
                }
                    dt.AcceptChanges();
            }
            if (dt.Rows.Count == 1)
                allshiftsLoaded = true;
            else if (dt.Rows.Count > 1)
                allshiftsLoaded = false;

            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new weekly security report below";
            clearForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
        }

        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((ddlArea.SelectedIndex > 0) && (tbWeekStart.Text != "") && (tbWeekEnd.Text != ""))
            {
                if (gvShiftReport.Rows.Count > 0)
                {
                    dtFinished = new DataTable();
                }
                pnlReportDetails.Visible = true;
            }
            else
            {
                pnlReportDetails.Visible = false;
            }
        }

        protected void ibWeekStart_Click(object sender, ImageClickEventArgs e)
        {
            if ((ddlArea.SelectedIndex > 0) && (tbWeekStart.Text != "") && (tbWeekEnd.Text != ""))
            {
                DateTime weekStart = Convert.ToDateTime(tbWeekStart.Text);
                DateTime weekEnd = Convert.ToDateTime(tbWeekEnd.Text);
                if (!duplicateCheck(weekStart, weekEnd, Convert.ToByte(ddlArea.SelectedValue)))
                {
                    if (gvShiftReport.Rows.Count > 0)
                    {
                        dtFinished = new DataTable();
                        loadSelectedShiftDetails(Shifts);
                    }
                    pnlReportDetails.Visible = true;
                }
                else
                {
                    ShowMessage("A Weekly Security Report already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                    clearForm();
                }
            }
            else
            {
                pnlReportDetails.Visible = false;
            }
        }

        protected void tbWeekStart_TextChanged(object sender, EventArgs e)
        {
            if ((ddlArea.SelectedIndex > 0) && (tbWeekStart.Text != "") && (tbWeekEnd.Text != ""))
            {
                DateTime weekStart = Convert.ToDateTime(tbWeekStart.Text);
                DateTime weekEnd = Convert.ToDateTime(tbWeekEnd.Text);
                if (!duplicateCheck(weekStart,weekEnd, Convert.ToByte(ddlArea.SelectedValue)))
                {
                    if (gvShiftReport.Rows.Count > 0)
                    {
                        dtFinished = new DataTable();
                        loadSelectedShiftDetails(Shifts);
                    }
                    pnlReportDetails.Visible = true;
                }
                else
                {
                    ShowMessage("A Weekly Security Report already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                    clearForm();
                }
            }
            else
            {
                pnlReportDetails.Visible = false;
            }
        }

        protected void tbWeekEnd_TextChanged(object sender, EventArgs e)
        {
            if ((ddlArea.SelectedIndex > 0) && (tbWeekStart.Text != "") && (tbWeekEnd.Text != ""))
            {
                DateTime weekStart = Convert.ToDateTime(tbWeekStart.Text);
                DateTime weekEnd = Convert.ToDateTime(tbWeekEnd.Text);
                if (!duplicateCheck(weekStart, weekEnd, Convert.ToByte(ddlArea.SelectedValue)))
                {
                    if (gvShiftReport.Rows.Count > 0)
                    {
                        dtFinished = new DataTable();
                        loadSelectedShiftDetails(Shifts);
                    }
                    pnlReportDetails.Visible = true;
                }
                else
                {
                    ShowMessage("A Weekly Security Report already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                    clearForm();
                }
            }
            else
            {
                pnlReportDetails.Visible = false;
            }
        }

        private void loadSelectedShiftDetails(Dictionary<string, string> Shiftsss)
        {
            try
            {
                if (Shiftsss.Count > 0)
                {
                    foreach (var vals in Shiftsss)
                    {
                        string ShiftID = vals.Key;
                        string ShiftName = vals.Value;

                        DataTable dtSO = new DataTable();
                        DataTable dtOfficersPerShift = new DataTable();
                        DataTable dtComputation = new DataTable();

                        dtSO = WSR.sp_WeeklySecurityReport_SpecialOperations_SelectFor(tbWeekStart.Text, tbWeekEnd.Text, ddlArea.SelectedValue, ShiftID);
                        dtOfficersPerShift = WSR.sp_WeeklySecurityReport_OfficersPerShift_SelectFor(tbWeekStart.Text, tbWeekEnd.Text, ddlArea.SelectedValue, ShiftID);
                        dtComputation = WSR.sp_WeeklySecurityReport_Computation_SelectFor(tbWeekStart.Text, tbWeekEnd.Text, ddlArea.SelectedValue, ShiftID);

                        if ((dtFinished == null) || (dtFinished.Columns.Count == 0))
                        {
                            dtFinished = new DataTable();
                            dtFinished.Columns.Add(new DataColumn("cashInGT", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("cashOutGT", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("other", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Monday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Tuesday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Wednesday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Thursday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Friday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Saturday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Sunday", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Present", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Absentt", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Switches", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("IncidentsComplaints", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("ShiftID", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Shift", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("Type", typeof(string)));
                            dtFinished.Columns.Add(new DataColumn("ComparisonID", typeof(string)));
                        }

                        DataRow drr1 = dtFinished.NewRow();
                        DataRow drr2 = dtFinished.NewRow();

                        if ((dtSO != null) && (dtSO.Rows.Count > 0))
                        {
                            if (dtSO.Rows.Count == 1)
                            {
                                drr1["cashInGT"] = dtSO.Rows[0]["cashInGT"].ToString();
                                drr1["cashOutGT"] = dtSO.Rows[0]["cashOutGT"].ToString();
                                drr1["other"] = dtSO.Rows[0]["other"].ToString();
                                dtFinished.Rows.Add(drr1);

                                drr2["cashInGT"] = "0";
                                drr2["cashOutGT"] = "0";
                                drr2["other"] = "0";
                                dtFinished.Rows.Add(drr2);
                            }
                            else
                            {
                                drr1["cashInGT"] = dtSO.Rows[0]["cashInGT"].ToString();
                                drr1["cashOutGT"] = dtSO.Rows[0]["cashOutGT"].ToString();
                                drr1["other"] = dtSO.Rows[0]["other"].ToString();
                                dtFinished.Rows.Add(drr1);

                                drr2["cashInGT"] = dtSO.Rows[1]["cashInGT"].ToString();
                                drr2["cashOutGT"] = dtSO.Rows[1]["cashOutGT"].ToString();
                                drr2["other"] = dtSO.Rows[1]["other"].ToString();
                                dtFinished.Rows.Add(drr2);
                            }
                        }
                        else
                        {
                            drr1["cashInGT"] = "-";
                            drr1["cashOutGT"] = "-";
                            drr1["other"] = "-";
                            dtFinished.Rows.Add(drr1);

                            drr2["cashInGT"] = "0";
                            drr2["cashOutGT"] = "0";
                            drr2["other"] = "0";
                            dtFinished.Rows.Add(drr2);
                        }

                        int rowOneCount = dtFinished.Rows.Count - 2;
                        int rowTwoCount = dtFinished.Rows.Count - 1;

                        if ((dtOfficersPerShift != null) && (dtOfficersPerShift.Rows.Count > 0))
                        {
                            //If only the scheduled data is returned
                            if (dtOfficersPerShift.Rows.Count == 1)
                            {
                                dtFinished.Rows[rowOneCount]["Monday"] = dtOfficersPerShift.Rows[0]["Monday"].ToString();
                                dtFinished.Rows[rowOneCount]["Tuesday"] = dtOfficersPerShift.Rows[0]["Tuesday"].ToString();
                                dtFinished.Rows[rowOneCount]["Wednesday"] = dtOfficersPerShift.Rows[0]["Wednesday"].ToString();
                                dtFinished.Rows[rowOneCount]["Thursday"] = dtOfficersPerShift.Rows[0]["Thursday"].ToString();
                                dtFinished.Rows[rowOneCount]["Friday"] = dtOfficersPerShift.Rows[0]["Friday"].ToString();
                                dtFinished.Rows[rowOneCount]["Saturday"] = dtOfficersPerShift.Rows[0]["Saturday"].ToString();
                                dtFinished.Rows[rowOneCount]["Sunday"] = dtOfficersPerShift.Rows[0]["Sunday"].ToString();

                                dtFinished.Rows[rowTwoCount]["Monday"] = "0";
                                dtFinished.Rows[rowTwoCount]["Tuesday"] = "0";
                                dtFinished.Rows[rowTwoCount]["Wednesday"] = "0";
                                dtFinished.Rows[rowTwoCount]["Thursday"] = "0";
                                dtFinished.Rows[rowTwoCount]["Friday"] = "0";
                                dtFinished.Rows[rowTwoCount]["Saturday"] = "0";
                                dtFinished.Rows[rowTwoCount]["Sunday"] = "0";
                            }
                            else
                            {
                                dtFinished.Rows[rowOneCount]["Monday"] = dtOfficersPerShift.Rows[0]["Monday"].ToString();
                                dtFinished.Rows[rowOneCount]["Tuesday"] = dtOfficersPerShift.Rows[0]["Tuesday"].ToString();
                                dtFinished.Rows[rowOneCount]["Wednesday"] = dtOfficersPerShift.Rows[0]["Wednesday"].ToString();
                                dtFinished.Rows[rowOneCount]["Thursday"] = dtOfficersPerShift.Rows[0]["Thursday"].ToString();
                                dtFinished.Rows[rowOneCount]["Friday"] = dtOfficersPerShift.Rows[0]["Friday"].ToString();
                                dtFinished.Rows[rowOneCount]["Saturday"] = dtOfficersPerShift.Rows[0]["Saturday"].ToString();
                                dtFinished.Rows[rowOneCount]["Sunday"] = dtOfficersPerShift.Rows[0]["Sunday"].ToString();

                                dtFinished.Rows[rowTwoCount]["Monday"] = dtOfficersPerShift.Rows[1]["Monday"].ToString();
                                dtFinished.Rows[rowTwoCount]["Tuesday"] = dtOfficersPerShift.Rows[1]["Tuesday"].ToString();
                                dtFinished.Rows[rowTwoCount]["Wednesday"] = dtOfficersPerShift.Rows[1]["Wednesday"].ToString();
                                dtFinished.Rows[rowTwoCount]["Thursday"] = dtOfficersPerShift.Rows[1]["Thursday"].ToString();
                                dtFinished.Rows[rowTwoCount]["Friday"] = dtOfficersPerShift.Rows[1]["Friday"].ToString();
                                dtFinished.Rows[rowTwoCount]["Saturday"] = dtOfficersPerShift.Rows[1]["Saturday"].ToString();
                                dtFinished.Rows[rowTwoCount]["Sunday"] = dtOfficersPerShift.Rows[1]["Sunday"].ToString();
                            }
                        }
                        else
                        {
                            dtFinished.Rows[rowOneCount]["Monday"] = "0";
                            dtFinished.Rows[rowOneCount]["Tuesday"] = "0";
                            dtFinished.Rows[rowOneCount]["Wednesday"] = "0";
                            dtFinished.Rows[rowOneCount]["Thursday"] = "0";
                            dtFinished.Rows[rowOneCount]["Friday"] = "0";
                            dtFinished.Rows[rowOneCount]["Saturday"] = "0";
                            dtFinished.Rows[rowOneCount]["Sunday"] = "0";

                            dtFinished.Rows[rowTwoCount]["Monday"] = "0";
                            dtFinished.Rows[rowTwoCount]["Tuesday"] = "0";
                            dtFinished.Rows[rowTwoCount]["Wednesday"] = "0";
                            dtFinished.Rows[rowTwoCount]["Thursday"] = "0";
                            dtFinished.Rows[rowTwoCount]["Friday"] = "0";
                            dtFinished.Rows[rowTwoCount]["Saturday"] = "0";
                            dtFinished.Rows[rowTwoCount]["Sunday"] = "0";
                        }

                        if ((dtComputation != null) && (dtComputation.Rows.Count > 0))
                        {
                            //If only the scheduled data is returned
                            if (dtComputation.Rows.Count == 1)
                            {
                                dtFinished.Rows[rowOneCount]["Present"] = dtComputation.Rows[0]["Present"].ToString();
                                dtFinished.Rows[rowOneCount]["Absentt"] = dtComputation.Rows[0]["Absentt"].ToString();
                                dtFinished.Rows[rowOneCount]["Switches"] = dtComputation.Rows[0]["Switches"].ToString();
                                dtFinished.Rows[rowOneCount]["IncidentsComplaints"] = dtComputation.Rows[0]["IncidentsComplaints"].ToString();

                                dtFinished.Rows[rowTwoCount]["Present"] = "0";
                                dtFinished.Rows[rowTwoCount]["Absentt"] = "0";
                                dtFinished.Rows[rowTwoCount]["Switches"] = "0";
                                dtFinished.Rows[rowTwoCount]["IncidentsComplaints"] = "0";

                            }
                            else
                            {
                                dtFinished.Rows[rowOneCount]["Present"] = dtComputation.Rows[0]["Present"].ToString();
                                dtFinished.Rows[rowOneCount]["Absentt"] = dtComputation.Rows[0]["Absentt"].ToString();
                                dtFinished.Rows[rowOneCount]["Switches"] = dtComputation.Rows[0]["Switches"].ToString();
                                dtFinished.Rows[rowOneCount]["IncidentsComplaints"] = dtComputation.Rows[0]["IncidentsComplaints"].ToString();

                                dtFinished.Rows[rowTwoCount]["Present"] = dtComputation.Rows[1]["Present"].ToString();
                                dtFinished.Rows[rowTwoCount]["Absentt"] = dtComputation.Rows[1]["Absentt"].ToString();
                                dtFinished.Rows[rowTwoCount]["Switches"] = dtComputation.Rows[1]["Switches"].ToString();
                                dtFinished.Rows[rowTwoCount]["IncidentsComplaints"] = dtComputation.Rows[1]["IncidentsComplaints"].ToString();
                            }
                        }
                        else
                        {
                            dtFinished.Rows[rowOneCount]["Present"] = "0";
                            dtFinished.Rows[rowOneCount]["Absentt"] = "0";
                            dtFinished.Rows[rowOneCount]["Switches"] = "0";
                            dtFinished.Rows[rowOneCount]["IncidentsComplaints"] = "0";

                            dtFinished.Rows[rowTwoCount]["Present"] = "0";
                            dtFinished.Rows[rowTwoCount]["Absentt"] = "0";
                            dtFinished.Rows[rowTwoCount]["Switches"] = "0";
                            dtFinished.Rows[rowTwoCount]["IncidentsComplaints"] = "0";
                        }

                        dtFinished.Rows[rowOneCount]["ShiftID"] = ShiftID;
                        dtFinished.Rows[rowOneCount]["Shift"] = ShiftName;
                        dtFinished.Rows[rowOneCount]["Type"] = "Scheduled";
                        dtFinished.Rows[rowTwoCount]["ShiftID"] = ShiftID;
                        dtFinished.Rows[rowTwoCount]["Shift"] = ShiftName;
                        dtFinished.Rows[rowTwoCount]["Type"] = "Actual";
                        dtFinished.Rows[rowOneCount]["ComparisonID"] = "0";
                        dtFinished.Rows[rowTwoCount]["ComparisonID"] = "0";

                        if (isLoaded)
                        {
                            //Append the comparison to the shift ids so that they can be updated and not reinserted
                            if ((dtComparisonIDs.Rows.Count > 0) && (dtComparisonIDs != null))
                            {
                                foreach (DataRow drs in dtComparisonIDs.Rows)
                                {
                                    string compShiftID = drs["ShiftID"].ToString();
                                    string comType = drs["Type"].ToString();
                                    if ((ShiftID == compShiftID) && (comType == "Scheduled"))
                                    {
                                        dtFinished.Rows[rowOneCount]["ComparisonID"] = drs["ComparisonID"];
                                    }
                                    else if ((ShiftID == compShiftID) && (comType == "Actual"))
                                    {
                                        dtFinished.Rows[rowTwoCount]["ComparisonID"] = drs["ComparisonID"];
                                    }
                                }
                            }
                        }
                    }
                    if ((dtFinished.Rows.Count > 0) && (dtFinished != null))
                    {
                        //populate the gridview
                        gvShiftReport.DataSource = dtFinished;
                        gvShiftReport.DataBind();
                    }
                }
                else
                    ShowMessage("No shift available to load data.", MessageType.Error);
            }
            catch (System.Exception excec)
            {
                MISC.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the Shift could not be appended. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }
        }

        protected void btnNewSecurityShiftReport_Click(object sender, EventArgs e)
        {
            loadDDLShift();
            if (allshiftsLoaded == false)
            {
                showNextShiftModal();
            }
            else
                ShowMessage("All shifts have been loaded into the security report. No other shifts are available!", MessageType.Info);
        }

        private void showNextShiftModal()
        {
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#newShiftModal').modal('toggle');</script>", false);
        }

        private void hideNextShiftModal()
        {
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#newShiftModal').modal('hide');</script>", false);
        }

        protected void btnAppendClose_Click(object sender, EventArgs e)
        {
            if (ddlShift.SelectedIndex > 0)
            {
                Dictionary<string, string> tmpShiftID = new Dictionary<string, string>();
                tmpShiftID.Add(ddlShift.SelectedValue, ddlShift.SelectedItem.Text);
                loadSelectedShiftDetails(tmpShiftID);
                hideNextShiftModal();
            }
            else
            {
                ShowMessage("You must select a shift.", MessageType.Warning);
                showNextShiftModal();
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

        protected void btnAppendNext_Click(object sender, EventArgs e)
        {
            if (ddlShift.SelectedIndex > 0)
            {
                Dictionary<string, string> tmpShiftID = new Dictionary<string, string>();
                tmpShiftID.Add(ddlShift.SelectedValue, ddlShift.SelectedItem.Text);
                loadSelectedShiftDetails(tmpShiftID);
                loadDDLShift();
                if (allshiftsLoaded == false)
                {
                    showNextShiftModal();
                }
                else
                    ShowMessage("All shifts have been loaded into the security report. No other shifts are available!", MessageType.Info);
            }
            else
            {
                ShowMessage("You must select a shift.", MessageType.Warning);
                showNextShiftModal();
            }
        }

        private void showcomparisonModal()
        {
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#comparisonModal').modal('toggle');</script>", false);
        }

        private void hideComparisonModal()
        {
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#comparisonModal').modal('hide');</script>", false);
        }

        protected void gvShiftReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Monday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblMonday = (Label)row.FindControl("lblMonday");
                if (lblMonday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Monday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Monday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
            else if (e.CommandName == "Tuesday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblTuesday = (Label)row.FindControl("lblTuesday");
                if (lblTuesday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Tuesday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Tuesday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
            else if (e.CommandName == "Wednesday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblWednesday = (Label)row.FindControl("lblWednesday");
                if (lblWednesday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Wednesday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Wednesday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
            else if (e.CommandName == "Thursday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblThursday = (Label)row.FindControl("lblThursday");
                if (lblThursday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Thursday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Thursday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
            else if (e.CommandName == "Friday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblFriday = (Label)row.FindControl("lblFriday");
                if (lblFriday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Friday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Friday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
            else if (e.CommandName == "Saturday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblSaturday = (Label)row.FindControl("lblSaturday");
                if (lblSaturday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Saturday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Saturday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
            else if (e.CommandName == "Sunday")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShiftReport.Rows[index];
                Label lblSunday = (Label)row.FindControl("lblSunday");
                if (lblSunday.Text != "0")
                {
                    int index2 = index + 1;
                    GridViewRow row2 = gvShiftReport.Rows[index2];
                    Label lblShiftID = (Label)row2.FindControl("lblShiftID");
                    string ShiftName = row2.Cells[0].Text;
                    if (ShiftName != "")
                    {
                        lblEditHeader.Text = "Sunday (" + ShiftName + ") -- Comparison";
                        loadComparisonDetails(lblShiftID.Text, "Sunday");
                        showcomparisonModal();
                    }
                    else
                        ShowMessage("Something went wrong and the comparison could not be loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

                }
            }
        }

        private void loadComparisonDetails(string shiftID, string dayOfWeek)
        {
            DataTable dt = new DataTable();
            dt = WSR.sp_WeeklySecurityReport_Comparison_Select(tbWeekStart.Text, tbWeekEnd.Text, ddlArea.SelectedValue, shiftID, dayOfWeek);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                dt.Rows.Cast<DataRow>().Where(r => r.ItemArray[1].ToString() == "" && r.ItemArray[2].ToString() == "").ToList().ForEach(r => r.Delete());
                dt.AcceptChanges();
                 gvComparison.DataSource = dt;
                 gvComparison.DataBind();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            hideComparisonModal();
        }

        protected void gvComparison_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
           bool success =  saveWSR();
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            bool success = saveWSR();
            if (success)
                clearForm();
        }

        private bool saveWSR()
        {
            bool success = false;
            if (gvShiftReport.Rows.Count > 0)
            {
                if (isLoaded)
                {
                    try
                    {
                        DateTime weekStart = new DateTime();
                        DateTime weekEnd = new DateTime();
                        if ((DateTime.TryParse(tbWeekStart.Text, out weekStart)) && (DateTime.TryParse(tbWeekEnd.Text, out weekEnd)))
                        {
                            byte areaID = Convert.ToByte(ddlArea.SelectedValue);
                            
                            WSR.update_WeeklySecurityReport(securityReportID, lblReportNumber.Text, weekStart, weekEnd, areaID, tbComments.Text, gvShiftReport, userID);
                            if (WSR.successfulCommit)
                            {
                                ShowMessage("Record Saved.", MessageType.Success);
                                success = true;
                            }
                            else
                            {
                                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                                success = false;
                            }
                        }
                        else
                        {
                            ShowMessage("Incorrect date format. Either the Week Start or End dates are invalid", MessageType.Warning);
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
                        DateTime weekStart = new DateTime();
                        DateTime weekEnd = new DateTime();
                        if ((DateTime.TryParse(tbWeekStart.Text, out weekStart)) && (DateTime.TryParse(tbWeekEnd.Text, out weekEnd)))
                        {
                            byte areaID = Convert.ToByte(ddlArea.SelectedValue);
                            generateShiftReportNumber();
                            if (!duplicateCheck(weekStart, weekEnd, areaID))
                            {
                                securityReportID = WSR.insert_WeeklySecurityReport(lblReportNumber.Text, weekStart, weekEnd, areaID, tbComments.Text, gvShiftReport, userID);
                                if (WSR.successfulCommit)
                                {
                                    ShowMessage("Record Saved.", MessageType.Success);
                                    isLoaded = true;
                                    success = true;
                                }
                                else
                                {
                                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                                    success = false;
                                }
                            }
                            else
                            {
                                ShowMessage("A Daily Post Assignment Roster already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
                                success = false;
                            }
                        }
                        else
                        {
                            ShowMessage("Incorrect date format. Either the Week Start or End dates are invalid", MessageType.Warning);
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
            }
            else
                ShowMessage("No shifts are available to be saved.", MessageType.Error);

            return success;
        }

        public bool duplicateCheck(DateTime weekStart, DateTime weekEnd, byte areaID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Mgr_WeeklySecurityReport
                           where a.WeekStart == weekStart && a.WeekEnd == weekEnd && a.AreaID == areaID
                           select new
                           {
                               a.SecurityReportID
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearForm();
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

        private void loadSearchedResults()
        {
            DataTable dt = WSR.sp_WeeklySecurityReport_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchWeekStart.Text == "" ? null : tbSearchWeekStart.Text, tbSearchWeekEnd.Text == "" ? null : tbSearchWeekEnd.Text, ddlSearchArea.SelectedValue);
            if (WSR.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvResults.DataSource = dt;
                    gvResults.DataBind();
                    gvResults.Visible = true;
                    lblGVResultsHeader.Text = "Weekly Security Reports found: " + dt.Rows.Count;
                }
                else
                {
                    gvResults.Visible = false;
                    lblGVResultsHeader.Text = "No Weekly Security Report found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvResults.Visible = false;
                lblGVResultsHeader.Text = "No Daily Post Assignment found due to error.";
            }
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchWeekStart.Text = "";
            tbSearchWeekEnd.Text = "";
            ddlArea.SelectedIndex = 0;
            lblGVResultsHeader.Text = "";
            gvResults.Visible = false;
            tbSearchReportNumber.Text = "";
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

        protected void gvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvResults.SelectedIndex > -1)
            {
                clearForm();
                Label lblSecurityReportID = (Label)gvResults.SelectedRow.FindControl("lblSecurityReportID");
                Label lblAreaID = (Label)gvResults.SelectedRow.FindControl("lblAreaID");
                Label lblReportNumber2 = (Label)gvResults.SelectedRow.FindControl("lblReportNumber");
                Label lblWeekStart = (Label)gvResults.SelectedRow.FindControl("lblWeekStart");
                Label lblWeekEnd = (Label)gvResults.SelectedRow.FindControl("lblWeekEnd");
                Label lblComments = (Label)gvResults.SelectedRow.FindControl("lblComments");
                Label lblCreatedBy = (Label)gvResults.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvResults.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedOn");

                securityReportID = Convert.ToInt32(lblSecurityReportID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;
                tbWeekStart.Text = lblWeekStart.Text;
                tbWeekEnd.Text = lblWeekEnd.Text;
                tbComments.Text = lblComments.Text;
                lblHeader.Text = "Update the selected security report below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;
               
                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                isLoaded = true;
                loadSavedShifts();
            }
        }

        private void loadSavedShifts()
        {
            dtFinished = new DataTable();
            dtFinished = WSR.sp_WeeklySecurityReport_ComparisonSelect(securityReportID);
            if (WSR.successfulCommit)
            {
                if ((dtFinished != null) && (dtFinished.Rows.Count > 0))
                {
                    dtComparisonIDs = new DataTable();
                    dtComparisonIDs.Columns.Add(new DataColumn("ShiftID", typeof(string)));
                    dtComparisonIDs.Columns.Add(new DataColumn("Type", typeof(string)));
                    dtComparisonIDs.Columns.Add(new DataColumn("ComparisonID", typeof(string)));

                    foreach (DataRow dr in dtFinished.Rows)
                    {
                        //Add the shifts to the shifts dictionary
                        string shiftID = dr["ShiftID"].ToString();
                        string shiftName = dr["Shift"].ToString();
                        if (!Shifts.ContainsKey(shiftID))
                        {
                            Shifts.Add(shiftID, shiftName);

                        }

                        //Add the comparionIDs
                        DataRow drr1 = dtComparisonIDs.NewRow();
                        drr1["ShiftID"] = shiftID;
                        drr1["Type"] = dr["Type"].ToString(); ;
                        drr1["ComparisonID"] = dr["ComparisonID"].ToString();
                        dtComparisonIDs.Rows.Add(drr1);
                    }
                    gvShiftReport.DataSource = dtFinished;
                    gvShiftReport.DataBind();
                    pnlReportDetails.Visible = true;
                }
                else
                {
                    ShowMessage("No shifts found.", MessageType.Info);
                }
            }
            else
            {
                ShowMessage("Something went wrong and the saved shifts were not loaded. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                lblGVResultsHeader.Text = "No Daily Post Assignment found due to error.";
            }
        }

        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResults.PageIndex = e.NewPageIndex;
            gvResults.SelectedIndex = -1;
            loadSearchedResults();
        }

        //protected void gvShiftReport_RowCreated(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.Header)
        //    {
        //            GridViewRow HeaderRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
        //            TableCell HeaderCell2 = new TableCell();

        //            //FIRST COLUMN
        //            HeaderCell2 = new TableCell();
        //            HeaderCell2.Text = "Shift";
        //            HeaderCell2.ColumnSpan = 1;
        //            HeaderCell2.RowSpan = 2;
        //            HeaderRow.Cells.Add(HeaderCell2);

        //            HeaderCell2 = new TableCell();
        //            HeaderCell2.Text = "Special Operations";
        //            HeaderCell2.ColumnSpan = 3;
        //            HeaderCell2.RowSpan = 1;
        //            HeaderRow.Cells.Add(HeaderCell2);

        //            HeaderCell2 = new TableCell();
        //            HeaderCell2.Text = "Officers Per Shift";
        //            HeaderCell2.ColumnSpan = 7;
        //            HeaderCell2.RowSpan = 1;
        //            HeaderRow.Cells.Add(HeaderCell2);

        //            HeaderCell2 = new TableCell();
        //            HeaderCell2.Text = "Computation";
        //            HeaderCell2.ColumnSpan = 4;
        //            HeaderCell2.RowSpan = 1;
        //            HeaderRow.Cells.Add(HeaderCell2);

        //            //ADDS THE HEADER TO THE GRID
        //            gvShiftReport.Controls[0].Controls.AddAt(0, HeaderRow);

        //            //THIS IS THE SUB HEADER
        //            GridViewRow HeaderRow1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
        //            TableCell HeaderCell = new TableCell();

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "CIT in GT";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "CIT out GT";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Other";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Avg";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Mon";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Tue";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Wed";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Thu";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Fri";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Sat";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Sun";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Present";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Absent";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Inc. & Com.";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            HeaderCell = new TableCell();
        //            HeaderCell.Text = "Switches";
        //            HeaderRow1.Cells.Add(HeaderCell);

        //            //ADDS THE SUB-HEADER TO THE GRID
        //            gvShiftReport.Controls[0].Controls.AddAt(1, HeaderRow1);
        //    }
        //}

    }
}