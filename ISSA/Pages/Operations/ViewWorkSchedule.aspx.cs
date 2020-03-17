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
using Microsoft.Reporting.WebForms;

namespace ISSA.Pages.Operations
{
    public partial class PrintWorkSchedule : System.Web.UI.Page
    {
        //BusinessTier.WorkSchedule WORKSCHEDULE = new BusinessTier.WorkSchedule();
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
                    if ((roleNames.Contains("Manager")) || (roleNames.Contains("Operator")) || (roleNames.Contains("Supervisor")))
                    {
                        loggedInUser = MISC.getUserName().ToUpper();
                        loadDDLArea();
                        loadYear();
                        loadShifts();
                        clearFields();
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

        private void setCurrentMonth()
        {
            int currentMonth = DateTime.Now.Month;
            ddlMonth.SelectedValue = currentMonth.ToString();
        }

        private void loadYear()
        {
           var model = new EDM.DataSource();
            var results = (from a in model.tbl_Mgr_WorkSchedule
                           select new
                           {
                               a.Year
                           }).Distinct().ToList();

            if (results.Count > 0)
            {
                ddlYear.DataSource = results;
                ddlYear.DataTextField = "Year";
                ddlYear.DataValueField = "Year";
                ddlYear.DataBind();
            }
        }

        private void loadShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearFields2();
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearFields2();
        }

        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearFields2();
        }

        protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearFields2();
        }

        private void clearFields2()
        {
            this.rvWorkSchedule.LocalReport.DataSources.Clear();
        }

        protected void generateWorkSchedule()
        {
            this.rvWorkSchedule.ProcessingMode = ProcessingMode.Local;

            ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
            DS_Report.EnforceConstraints = false;

            ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WorkScheduleTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WorkScheduleTableAdapter();
            Rpt_TableAdapter.Fill(DS_Report.TA_RPT_WorkSchedule, ddlYear.SelectedValue, ddlMonth.SelectedValue, Convert.ToByte(ddlArea.SelectedValue), ddlShift.SelectedValue);

            ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

            DataRow dr = dtParam.NewRow();
            dr["ParamOne"] = ddlMonth.SelectedItem.Text + ", " + ddlYear.SelectedValue;
            dr["ParamTwo"] = ddlArea.SelectedItem.Text;
            if (ddlShift.SelectedIndex == 0)
                dr["ParamThree"] = "All";
            else
                dr["ParamThree"] = ddlShift.SelectedItem.Text;
            dr["User"] = loggedInUser;
            dr["Date"] = DateTime.Now;
            dtParam.Rows.Add(dr);

            Microsoft.Reporting.WebForms.ReportDataSource RpDs1 = new Microsoft.Reporting.WebForms.ReportDataSource();
            Microsoft.Reporting.WebForms.ReportDataSource RpDsParameters = new Microsoft.Reporting.WebForms.ReportDataSource();

            RpDs1.Name = "TA_RPT_WorkSchedule";
            RpDs1.Value = DS_Report.TA_RPT_WorkSchedule;
            RpDsParameters.Name = "dtParameters";
            RpDsParameters.Value = dtParam;

            this.rvWorkSchedule.LocalReport.ReportPath = "Pages/Reports/rpt_WorkSchedule.rdlc";
            this.rvWorkSchedule.LocalReport.DataSources.Clear();
            this.rvWorkSchedule.LocalReport.DataSources.Add(RpDs1);
            this.rvWorkSchedule.LocalReport.DataSources.Add(RpDsParameters);
            this.rvWorkSchedule.LocalReport.Refresh();
            this.rvWorkSchedule.Visible = true;
            this.rvWorkSchedule.ShowReportBody = true;

        }

        protected void lbGenerate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                generateWorkSchedule();
            }
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void clearFields()
        {
            setCurrentMonth();
            ddlShift.SelectedIndex = 0;
            ddlArea.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
        }
    }
}