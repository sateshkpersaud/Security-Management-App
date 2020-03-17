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
    public partial class ShiftReport : System.Web.UI.Page
    {
        BusinessTier.ShiftReport SHIFTREPORT = new BusinessTier.ShiftReport();
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

        public int shiftReportID
        {
            get
            {
                return (int)ViewState["shiftReportID"];
            }
            set
            {
                ViewState["shiftReportID"] = value;
            }
        }

        public int checkID
        {
            get
            {
                return (int)ViewState["checkID"];
            }
            set
            {
                ViewState["checkID"] = value;
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

        public bool isChecksLoaded
        {
            get
            {
                return (bool)ViewState["isChecksLoaded"];
            }
            set
            {
                ViewState["isChecksLoaded"] = value;
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
                userID = MISC.getUserID();
                if (userID != 0)
                {
                   roleNames = MISC.getUserRoles();
                    if ((roleNames.Contains("Manager")) || (roleNames.Contains("Supervisor")))
                    {
                        isLoaded = false;
                        isChecksLoaded = false;
                        checkID = 0;
                        shiftReportID = 0;
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        clearForm();
                        tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
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

        private void loadDDLSupervisor()
        {
            DataTable dt = MISC.sp_Employee_Sel4DDL_ByPositionID(4);
            ddlSupervisor.DataSource = dt;
            ddlSupervisor.DataTextField = "FullName";
            ddlSupervisor.DataValueField = "EmployeeID";
            ddlSupervisor.DataBind();
        }

        private void loadDDLSearchSupervisor()
        {
            DataTable dt = MISC.sp_Employee_Sel4DDL_ByPositionID(4);
            ddlSearchSupervisor.DataSource = dt;
            ddlSearchSupervisor.DataTextField = "FullName";
            ddlSearchSupervisor.DataValueField = "EmployeeID";
            ddlSearchSupervisor.DataBind();
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

        //private void loadDDLSearchLocations(object AreaID)
        //{
        //    DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
        //    ddlSearchLocation.DataSource = dt;
        //    ddlSearchLocation.DataTextField = "LocationName";
        //    ddlSearchLocation.DataValueField = "LocationID";
        //    ddlSearchLocation.DataBind();
        //}


        private void loadDDLSearchShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlSearchShift.DataSource = dt;
            ddlSearchShift.DataTextField = "shift";
            ddlSearchShift.DataValueField = "ShiftID";
            ddlSearchShift.DataBind();
        }

        //protected void ddlSearchArea_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlSearchArea.SelectedIndex > 0)
        //        loadDDLSearchLocations(Convert.ToByte(ddlSearchArea.SelectedValue));
        //    else
        //        loadDDLSearchLocations(null);
        //}

        protected void btnNew_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add new shift reports below";
            clearForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            btnBackToSearch.Visible = true;
            
        }

        private void clearForm()
        {
            loadDDLSearchArea();
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchShifts();
            loadDDLSearchSupervisor();
            loadDDLArea();
            loadShifts();
            loadDDLSupervisor();
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            //string timeNow = currentDatetime.TimeOfDay.ToString();
            dateNow = dateNow.Substring(0, 10);
            tbDate.Text = dateNow;
            ddlArea.SelectedIndex = 0;
            ddlShift.SelectedIndex = 0;
            ddlSupervisor.SelectedIndex = 0;
            gvResults.Visible = false;
            lblGVResultsHeader.Text = "";
            lblReportNumber.Text = "";
            gvChecks.Visible = false;
            shiftReportID = 0;
            isLoaded = false;
            isChecksLoaded = false;
            checkID = 0;
            generateShiftReportNumber();
            ddlArea.Enabled = true;
            ddlShift.Enabled = true;
            lblAuditTrail.Text = "";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearForm();
        }

        private bool saveShiftReport()
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
                        SHIFTREPORT.update_ShiftReport(shiftReportID, Convert.ToByte(ddlArea.SelectedValue), Convert.ToByte(ddlShift.SelectedValue), Convert.ToInt16(ddlSupervisor.SelectedValue), lblReportNumber.Text, userID);
                        if (SHIFTREPORT.successfulCommit)
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
                        if (!duplicateCheck_ShiftReport(date, Convert.ToByte(ddlArea.SelectedValue), Convert.ToByte(ddlShift.SelectedValue), Convert.ToInt16(ddlSupervisor.SelectedValue)))
                        {
                            //regenerate the number in case another record was added
                            generateShiftReportNumber();

                            shiftReportID = SHIFTREPORT.insert_ShiftReport(date, Convert.ToByte(ddlArea.SelectedValue), Convert.ToByte(ddlShift.SelectedValue), Convert.ToInt16(ddlSupervisor.SelectedValue), lblReportNumber.Text, userID);

                            if (shiftReportID != 0)
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
                            ShowMessage("A shift report already exists with the details selected. Kindly click on the Back To Search button and search for same.", MessageType.Info);
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

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        //generate the number
        private void generateShiftReportNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_ShiftReport
                           where a.Date.Value.Year == year
                           select new
                           {
                               a.ShiftReportID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "SR" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "SR" + year.ToString() + currentSequence.ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveShiftReport();
                if (success)
                {
                    ShowMessage("Record saved", MessageType.Success);
                }
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveShiftReport();
                if (success)
                {
                    ShowMessage("Record saved.", MessageType.Success);
                    clearForm();
                }
            }
        }

        public bool duplicateCheck_ShiftReport(DateTime date, byte areaID, byte shiftID, Int16 supervisorEmployeeID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_ShiftReport
                           where a.Date == date && a.AreaID == areaID && a.ShiftID == shiftID && a.SupervisorID == supervisorEmployeeID
                           select new
                           {
                               a.ShiftReportID
                           }).ToList();

            if (results.Count > 0)
            {
                foreach (var val in results)
                {
                    shiftReportID = val.ShiftReportID;
                }
                isDuplicate = true;
            }
            else
            {
                isDuplicate = false;
            }
            return isDuplicate;
        }

        private void clearChecksPopUp()
        {
            isChecksLoaded = false;
            checkID = 0;
            lblSequence.Text = "";
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
            tbGeneralObservations.Text = "";
            tbCorrectionsMade.Text = "";
            setSequence();
            SetInitialRow();
        }

        private void showChecksModal()
        {
            //mpeChecks.Show();
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#checksModal').modal('toggle');</script>", false);
        }

        private void hideChecksModal()
        {
            //mpeChecks.Hide();
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#checksModal').modal('hide');</script>", false);
        }

        protected void btnNewChecks_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = true;
                if (isLoaded == false)
                {
                    success = saveShiftReport();
                }

                if (success == true)
                {
                    clearChecksPopUp();
                    SetInitialRow();
                    showChecksModal();
                }
                else
                {
                    ShowMessage("Something went wrong and the Shift Report could not have been saved before opening the Checks PopUp. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSequence();
            SetInitialRow();
            showChecksModal();
        }

        private void setSequence()
        {
            Int16 locationID = Convert.ToInt16(ddlLocation.SelectedValue);
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_ShiftReportChecks
                           where a.ShiftReportID == shiftReportID && a.LocationID == locationID
                           select new
                           {
                               a.ChecksID
                           }).ToList();
            int count = 0;
            if (results.Count > 0)
            {
                foreach (var val in results)
                {
                    count++;
                }
                count = count + 1;
            }
            else
            {
                count = 1;
            }
            lblSequence.Text = count.ToString();
        }

        protected void btnSearchResults_Click(object sender, EventArgs e)
        {
            loadSearchedResults();
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchReportNumber.Text = "";
            tbSearchDate.Text = "";
            ddlSearchArea.SelectedIndex = 0;
            ddlSearchShift.SelectedIndex = 0;
            ddlSearchSupervisor.SelectedIndex = 0;
            gvResults.Visible = false;
            lblGVResultsHeader.Text = "";
        }

        private void loadSearchedResults()
        {
            DataTable dt = SHIFTREPORT.sp_ShiftReports_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchArea.SelectedValue, ddlSearchShift.SelectedValue, ddlSearchSupervisor.SelectedValue);
            if (SHIFTREPORT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvResults.DataSource = dt;
                    gvResults.DataBind();
                    gvResults.Visible = true;
                    lblGVResultsHeader.Text = "Shift Reports found: " + dt.Rows.Count;
                }
                else
                {
                    gvResults.Visible = false;
                    lblGVResultsHeader.Text = "No Shift Reports found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvResults.Visible = false;
                lblGVResultsHeader.Text = "No Shift Reports found due to error.";
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
                Label lblShiftReportID = (Label)gvResults.SelectedRow.FindControl("lblShiftReportID");
                Label lblAreaID = (Label)gvResults.SelectedRow.FindControl("lblAreaID");
                Label lblSupervisorID = (Label)gvResults.SelectedRow.FindControl("lblSupervisorID");
                Label lblShiftID = (Label)gvResults.SelectedRow.FindControl("lblShiftID");
                Label lblDate = (Label)gvResults.SelectedRow.FindControl("lblDate");
                Label lblCreatedBy = (Label)gvResults.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvResults.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvResults.SelectedRow.FindControl("lblLastModifiedOn");
                Label lblReportNumber2 = (Label)gvResults.SelectedRow.FindControl("lblReportNumber");

                ddlArea.Enabled = false;
                ddlShift.Enabled = false;
                shiftReportID = Convert.ToInt32(lblShiftReportID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;
                loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
                if (ddlSupervisor.Items.FindByValue(lblSupervisorID.Text) != null)
                    ddlSupervisor.SelectedValue = lblSupervisorID.Text;
                else
                    ddlSupervisor.SelectedIndex = 0;
                ddlSupervisor.SelectedValue = lblSupervisorID.Text;
                if (ddlShift.Items.FindByValue(lblShiftID.Text) != null)
                    ddlShift.SelectedValue = lblShiftID.Text;
                else
                    ddlShift.SelectedIndex = 0;
                tbDate.Text = lblDate.Text;
                lblHeader.Text = "Update the selected shift report below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;

                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                btnBackToSearch.Visible = true;
                isLoaded = true;
                loadChecks(shiftReportID);
                //load checks
            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            btnBackToSearch.Visible = false;
            clearForm();
            pnlSearch.Visible = true;
            lblGVResultsHeader.Text = "";
            if (gvResults.Rows.Count > 0)
                loadSearchedResults();
        }

        private void SetInitialRow()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("Column1", typeof(string)));
            dt.Columns.Add(new DataColumn("Column2", typeof(string)));
            dt.Columns.Add(new DataColumn("Column3", typeof(string)));
            dr = dt.NewRow();
            dr["Column1"] = string.Empty;
            dr["Column2"] = string.Empty;
            dr["Column3"] = string.Empty;
            dt.Rows.Add(dr);
            //dr = dt.NewRow();

            //Store the DataTable in ViewState
            ViewState["CurrentTable"] = dt;

            gvOfficers.DataSource = dt;
            gvOfficers.DataBind();

        }

        private void AddNewRowToGrid()
        {
            int rowIndex = 0;

            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        //extract the TextBox values
                        DropDownList ddl1 = (DropDownList)gvOfficers.Rows[rowIndex].Cells[1].FindControl("ddlOfficerName");
                        TextBox tb1 = (TextBox)gvOfficers.Rows[rowIndex].Cells[2].FindControl("tbTime");

                        drCurrentRow = dtCurrentTable.NewRow();
                        dtCurrentTable.Rows[i - 1]["Column1"] = ddl1.SelectedValue;
                        dtCurrentTable.Rows[i - 1]["Column2"] = tb1.Text;

                        rowIndex++;
                    }

                    dtCurrentTable.Rows.Add(drCurrentRow);
                    ViewState["CurrentTable"] = dtCurrentTable;

                    gvOfficers.DataSource = dtCurrentTable;
                    gvOfficers.DataBind();
                }
            }
            else
            {
                ShowMessage("Cannot load new officer row. ViewState is null", MessageType.Error);
            }

            //Set Previous Data on Postbacks
            SetPreviousData();
        }

        private void SetPreviousData()
        {
            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DropDownList ddl1 = (DropDownList)gvOfficers.Rows[rowIndex].Cells[1].FindControl("ddlOfficerName");
                        TextBox tb1 = (TextBox)gvOfficers.Rows[rowIndex].Cells[2].FindControl("tbTime");

                        ddl1.Text = dt.Rows[i]["Column1"].ToString();
                        tb1.Text = dt.Rows[i]["Column2"].ToString();

                        rowIndex++;
                    }
                }
            }
        }

        protected void btnNewOfficers_Click(object sender, EventArgs e)
        {
            AddNewRowToGrid();
            showChecksModal();
        }

        protected void gvOfficers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (gvOfficers.Rows.Count <= 1)
            {
                ShowMessage("You must have at least one Officer. You cannot delete all rows!", MessageType.Warning);
            }
            else if ((gvOfficers.Rows.Count > 1) && (ViewState["CurrentTable"] != null))
            {
                int rowIndex = 0;
                Label rowNumber = (Label)gvOfficers.Rows[Convert.ToInt32(e.RowIndex)].FindControl("lblNo");
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCurrentTable.Rows.Count; i++)
                    {
                        //extract the TextBox values
                        DropDownList ddl1 = (DropDownList)gvOfficers.Rows[rowIndex].Cells[1].FindControl("ddlOfficerName");
                        TextBox tb1 = (TextBox)gvOfficers.Rows[rowIndex].Cells[2].FindControl("tbTime");
                        dtCurrentTable.Rows[i]["Column1"] = ddl1.SelectedValue;
                        dtCurrentTable.Rows[i]["Column2"] = tb1.Text;
                        rowIndex++;
                    }
                }
                int rowCount = Convert.ToInt32(rowNumber.Text) - 1;
                dtCurrentTable.Rows.RemoveAt(rowCount);
                ViewState["CurrentTable"] = dtCurrentTable;
                gvOfficers.DataSource = dtCurrentTable;
                gvOfficers.DataBind();
                SetPreviousData();
            }
            else
            {
                ShowMessage("Cannot load new officer row. ViewState is null", MessageType.Error);
            }
            showChecksModal();
        }

        protected void gvOfficers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl1 = (DropDownList)e.Row.FindControl("ddlOfficerName");
                ddl1.DataSource = MISC.sp_Employee_Sel4DDL_ByPositionIDAndLocation(1, ddlLocation.SelectedValue, ddlArea.SelectedValue);
                ddl1.DataTextField = "FullName";
                ddl1.DataValueField = "EmployeeID";
                ddl1.DataBind();

                TextBox tbTime = (TextBox)e.Row.FindControl("tbTime");
                tbTime.Attributes.Add("onblur", "javascript:validateTime(" + tbTime.ClientID + ")");
            }
        }

        protected void btnCancelOfficer_Click(object sender, EventArgs e)
        {
            hideChecksModal();
        }

        protected void btnAddCloseOfficer_Click(object sender, EventArgs e)
        {
            if (saveChecks())
            {
                ShowMessage("Record saved.", MessageType.Success);
                hideChecksModal();
                loadChecks(shiftReportID);
            }
            else
                showChecksModal();
        }

        protected void btnAddNextOfficer_Click(object sender, EventArgs e)
        {
            if (saveChecks())
            {
                ShowMessage("Record saved.", MessageType.Success);
                clearChecksPopUp();
                loadChecks(shiftReportID);
                showChecksModal();
            }
            else
                showChecksModal();
        }

        private bool saveChecks()
        {
            bool success = false;

            if (officerValidFields())
            {
                if (isChecksLoaded)
                {
                    try
                    {
                        //Update check
                        SHIFTREPORT.update_Check(checkID, shiftReportID, Convert.ToInt16(ddlLocation.SelectedValue), Convert.ToByte(lblSequence.Text), tbGeneralObservations.Text, tbCorrectionsMade.Text, lblReportNumber.Text, gvOfficers, userID);
                        if (SHIFTREPORT.successfulCommit)
                        {
                            success = true;
                        }
                        else
                        {
                            success = false;
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
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
                        //insert check
                        SHIFTREPORT.insert_Check(shiftReportID, Convert.ToInt16(ddlLocation.SelectedValue), Convert.ToByte(lblSequence.Text), tbGeneralObservations.Text, tbCorrectionsMade.Text, lblReportNumber.Text, gvOfficers, userID);

                        if (SHIFTREPORT.successfulCommit)
                        {
                            success = true;
                        }
                        else
                        {
                            success = false;
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                        }

                    }
                    catch (System.Exception excec)
                    {
                        MISC.writetoAlertLog(excec.ToString());
                        ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                        success = false;
                    }
                }
            }
            return success;
        }

        private bool officerValidFields()
        {
            bool success = false;
            int officerAdded = 0;
            bool missingTime = false;
            bool missingOfficer = false;
            bool invalidtime = false;
            bool isduplicate = false;

            //check if any officer is added
            for (int i = 0; i < gvOfficers.Rows.Count; i++)
            {
                DropDownList ddlOfficerName = (DropDownList)gvOfficers.Rows[i].FindControl("ddlOfficerName");
                TextBox tbTime = (TextBox)gvOfficers.Rows[i].FindControl("tbTime");


                //Check for valid time entered
                if (tbTime.Text != "")
                {
                    TimeSpan time = new TimeSpan();
                    if (TimeSpan.TryParse(tbTime.Text, out time) == false)
                    {
                        invalidtime = true;
                        tbTime.BorderColor = System.Drawing.Color.Red;
                    }
                }
                else
                    tbTime.BorderColor = System.Drawing.Color.DarkGray;

                if ((ddlOfficerName.SelectedIndex > 0) && (tbTime.Text == ""))
                {
                    missingTime = true;
                    tbTime.BorderColor = System.Drawing.Color.Red;
                }
                else
                    tbTime.BorderColor = System.Drawing.Color.DarkGray;

                if ((ddlOfficerName.SelectedIndex == 0) && (tbTime.Text != "") && (!invalidtime))
                {
                    missingOfficer = true;
                    ddlOfficerName.BorderColor = System.Drawing.Color.Red;
                }
                else
                    ddlOfficerName.BorderColor = System.Drawing.Color.DarkGray;

                if ((ddlOfficerName.SelectedIndex > 0) && (tbTime.Text != "") && (!invalidtime))
                {
                    byte duplicatecount = 0;
                    string officerSelected = ddlOfficerName.SelectedItem.Text;
                    for (int j = 0; j < gvOfficers.Rows.Count; j++)
                    {
                        if (j != i)
                        {
                            DropDownList ddlOName = (DropDownList)gvOfficers.Rows[j].FindControl("ddlOfficerName");
                            if (officerSelected == ddlOName.SelectedItem.Text)
                            {
                                isduplicate = true;
                                duplicatecount++;
                            }
                        }
                    }
                    if (duplicatecount == 0)
                    {
                        ddlOfficerName.BorderColor = System.Drawing.Color.DarkGray;
                        officerAdded++;
                    }
                    else
                        ddlOfficerName.BorderColor = System.Drawing.Color.Red;
                }
            }

            if (invalidtime)
                ShowMessage("Incorrect time format.", MessageType.Warning);
            else if (missingTime)
                ShowMessage("Time field cannot be blank.", MessageType.Warning);
            else if (missingOfficer)
                ShowMessage("Officer field cannot be blank.", MessageType.Warning);
            else if (isduplicate)
                ShowMessage("Duplicate officer cannot be added. Kindly remove one and try saving again.", MessageType.Warning);
            else if (officerAdded == 0)
                ShowMessage("You must add at least one officer.", MessageType.Warning);


            if ((invalidtime) || (missingOfficer) || (missingTime) || (officerAdded == 0) || isduplicate == true)
                success = false;
            else
                success = true;

            showChecksModal();
            return success;
        }

        //Load the call logs
        private void loadChecks(int shiftReporttID)
        {
            DataTable dt = SHIFTREPORT.sp_shiftReport_Checks_Select(shiftReporttID);

            if ((dt.Rows.Count > 0) && (dt != null))
            {
                gvChecks.DataSource = dt;
                gvChecks.DataBind();
                gvChecks.Visible = true;
            }
            else
            {
                gvChecks.DataSource = dt;
                gvChecks.DataBind();
                gvChecks.Visible = false;
            }
        }

        protected void gvChecks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChecks.PageIndex = e.NewPageIndex;
            gvChecks.SelectedIndex = -1;
            loadChecks(shiftReportID);
        }

        protected void gvChecks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblShortGeneralObservations = (Label)e.Row.FindControl("lblShortGeneralObservations");
                if (lblShortGeneralObservations.Text.Length > 70)
                {
                    ViewState["OrigData"] = lblShortGeneralObservations.Text;
                    lblShortGeneralObservations.Text = lblShortGeneralObservations.Text.Substring(0, 70) + "...";
                    lblShortGeneralObservations.ToolTip = ViewState["OrigData"].ToString();
                }
                Label lblShortCorrectionsMage = (Label)e.Row.FindControl("lblShortCorrectionsMage");
                if (lblShortCorrectionsMage.Text.Length > 70)
                {
                    ViewState["OrigData2"] = lblShortCorrectionsMage.Text;
                    lblShortCorrectionsMage.Text = lblShortCorrectionsMage.Text.Substring(0, 70) + "...";
                    lblShortCorrectionsMage.ToolTip = ViewState["OrigData2"].ToString();
                }
            }
        }

        protected void gvChecks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvChecks.SelectedIndex > -1)
            {
                Label lblCreatedOn = (Label)gvChecks.SelectedRow.FindControl("lblCreatedOn");
                DateTime currentDateTime = DateTime.Now;
                DateTime createdOn = Convert.ToDateTime(lblCreatedOn.Text);

                TimeSpan timePassed = currentDateTime.Subtract(createdOn);
                double minutesElapsed = timePassed.Minutes;

                ////only allow editing before five minutes have passed since check saved or user has manager role
                //if ((minutesElapsed < 5) || (roleNames.Contains("Manager")))
                //{
                Label lblSequence2 = (Label)gvChecks.SelectedRow.FindControl("lblSequence");
                Label lblChecksID = (Label)gvChecks.SelectedRow.FindControl("lblChecksID");
                Label lblLocationID = (Label)gvChecks.SelectedRow.FindControl("lblLocationID");
                Label lblGeneralObservations = (Label)gvChecks.SelectedRow.FindControl("lblGeneralObservations");
                Label lblCorrectionsMadeSuggested = (Label)gvChecks.SelectedRow.FindControl("lblCorrectionsMadeSuggested");

                lblSequence.Text = lblSequence2.Text;
                loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
                ddlLocation.SelectedValue = lblLocationID.Text;
                checkID = Convert.ToInt32(lblChecksID.Text);
                tbGeneralObservations.Text = lblGeneralObservations.Text;
                tbCorrectionsMade.Text = lblCorrectionsMadeSuggested.Text;

                //load officers
                DataTable dt = SHIFTREPORT.sp_ShiftReport_Checks_GetOfficers(checkID);
                if ((dt.Rows.Count > 0) && (dt != null))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            //Set the first row and add the details
                            SetInitialRow();
                            DropDownList ddlOfficerName = (DropDownList)gvOfficers.Rows[i].Cells[1].FindControl("ddlOfficerName");
                            TextBox tbTime = (TextBox)gvOfficers.Rows[i].Cells[2].FindControl("tbTime");
                            ddlOfficerName.SelectedValue = dt.Rows[i]["EmployeeID"].ToString();
                            tbTime.Text = dt.Rows[i]["timeClocked"].ToString();
                        }
                        else
                        {
                            //Add a new row and add the details
                            AddNewRowToGrid();
                            DropDownList ddlOfficerName = (DropDownList)gvOfficers.Rows[i].Cells[1].FindControl("ddlOfficerName");
                            TextBox tbTime = (TextBox)gvOfficers.Rows[i].Cells[2].FindControl("tbTime");
                            ddlOfficerName.SelectedValue = dt.Rows[i]["EmployeeID"].ToString();
                            tbTime.Text = dt.Rows[i]["timeClocked"].ToString();
                        }
                    }
                }
                else
                {
                    ShowMessage("No Officers found.", MessageType.Info);
                }
                isChecksLoaded = true;
                showChecksModal();
                //}
                //else
                //    ShowMessage("Time period to edit has passed.", MessageType.Info);
            }
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}