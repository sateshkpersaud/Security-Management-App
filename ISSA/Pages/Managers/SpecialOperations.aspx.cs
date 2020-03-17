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
    public partial class SpecialOperations : System.Web.UI.Page
    {
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        BusinessTier.SpecialOperations SPECIALOPERATIONS = new BusinessTier.SpecialOperations();
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

        public int soID
        {
            get
            {
                return (int)ViewState["soID"];
            }
            set
            {
                ViewState["soID"] = value;
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
                    if (roleNames.Contains("Manager"))
                    {
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        loadDDLSearchArea();
                        object AreaID = null;
                        if (ddlSearchArea.SelectedIndex > 0)
                            AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
                        loadDDLSearchLocations(AreaID);
                        pageLoad();
                        tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
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
            resetForm();
            tbAmount.Attributes.Add("onkeypress", "javascript:return positiveNumber(event)");
            gvSO.Visible = false;
            lblGVHeader.Text = "";
        }

        //populate the searchArea ddl
        private void loadDDLSearchArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlSearchArea.DataSource = dt;
            ddlSearchArea.DataTextField = "Area";
            ddlSearchArea.DataValueField = "AreaID";
            ddlSearchArea.DataBind();
        }

        //populate the Area ddl
        private void loadDDLArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlArea.DataSource = dt;
            ddlArea.DataTextField = "Area";
            ddlArea.DataValueField = "AreaID";
            ddlArea.DataBind();
        }

        //populate the searchlocation ddl based on the area selected
        private void loadDDLSearchLocations(object AreaID)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddlSearchLocation.DataSource = dt;
            ddlSearchLocation.DataTextField = "LocationName";
            ddlSearchLocation.DataValueField = "LocationID";
            ddlSearchLocation.DataBind();
        }

        //populate the location ddl based on the area selected
        private void loadDDLLocations(object AreaID)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "LocationName";
            ddlLocation.DataValueField = "LocationID";
            ddlLocation.DataBind();
        }

        protected void ddlSearchArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchArea.SelectedIndex > 0)
                loadDDLSearchLocations(Convert.ToByte(ddlSearchArea.SelectedValue));
            else
                loadDDLSearchLocations(null);
        }

        //reload the location ddl on the area ddl index change
        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
        }

        //clear the entire form
        private void resetForm()
        {
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            dateNow = dateNow.Substring(0, 10);
            tbDate.Text = dateNow;
            isLoaded = false;
            loadDDLArea();
            loadDDLShifts();
            object AreaID = null;
            if (ddlArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlArea.SelectedValue);
            loadDDLLocations(AreaID);
            rbESYes.Checked = false;
            rbESNo.Checked = true;
            rbCITGTYes.Checked = false;
            rbCITGTNo.Checked = true;
            rbCIToGTYes.Checked = false;
            rbCIToGTNo.Checked = true;
            rbEOYes.Checked = false;
            rbEONo.Checked = true;
            tbAmount.Text = "";
            tbComments.Text = "";
            lblAmount.Visible = false;
            tbAmount.Visible = false;
            soID = 0;
            generateSONumber();
            lblAuditTrail.Text = "";
            lblHeader.Text = "Add new special operations report below";
        }

        private void loadDDLShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        //generate the incident number
        private void generateSONumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Mgr_SpecialOperations
                           where a.Date.Value.Year == year
                           select new
                           {
                               a.SpecialOperationsID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "SO" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "SO" + year.ToString() + currentSequence.ToString();
            }
        }

        protected void btnNewSO_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add new special operations report below";
            resetForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            btnBackToSearch.Visible = true;
        }

        protected void rbEOYes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEOYes.Checked)
            {
                tbAmount.Text = "";
                lblAmount.Visible = true;
                tbAmount.Visible = true;
            }
        }

        protected void rbEONo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEONo.Checked)
            {
                tbAmount.Text = "";
                lblAmount.Visible = false;
                tbAmount.Visible = false;
            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            btnBackToSearch.Visible = false;
            resetForm();
            pnlSearch.Visible = true;
            lblGVHeader.Text = "";
            if (gvSO.Rows.Count > 0)
            {
                loadSearchedSOs();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            resetForm();
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            object AreaID = null;
            ddlSearchArea.SelectedIndex = 0;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            tbSearchDate.Text = "";
            ddlEscortService.SelectedIndex = 0;
            ddlCITinGT.SelectedIndex = 0;
            ddlExtraOfficers.SelectedIndex = 0;
            gvSO.Visible = false;
            lblGVHeader.Text = "";
        }

        protected void btnSearchSO_Click(object sender, EventArgs e)
        {
            loadSearchedSOs();
        }

        private void loadSearchedSOs()
        {
            DataTable dt = SPECIALOPERATIONS.sp_SpecialOperations_Search(tbSearchDate.Text, ddlSearchArea.SelectedValue, ddlSearchLocation.SelectedValue, ddlEscortService.SelectedValue, ddlCITinGT.SelectedValue, ddlExtraOfficers.SelectedValue);
            if (SPECIALOPERATIONS.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvSO.DataSource = dt;
                    gvSO.DataBind();
                    gvSO.Visible = true;
                    lblGVHeader.Text = "Special Operations found: " + dt.Rows.Count;
                }
                else
                {
                    gvSO.Visible = false;
                    lblGVHeader.Text = "No special operations found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvSO.Visible = false;
                lblGVHeader.Text = "No reports found due to error.";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveSO();
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveSO();
                if (success)
                    resetForm();
            }
        }

        private bool saveSO()
        {
            bool success = false;

            if (isLoaded)
            {
                try
                {
                    if (tbAmount.Text != "" && Convert.ToInt32(tbAmount.Text) > 254)
                    {
                        ShowMessage("Incorrect amount entered.", MessageType.Warning);
                        tbAmount.BorderColor = System.Drawing.Color.Red;
                        tbAmount.Focus();
                        return success = false;
                    }
                    else
                    {
                        //Update 
                        tbAmount.BorderColor = System.Drawing.Color.LightGray;
                        DateTime date = new DateTime();
                        if (DateTime.TryParse(tbDate.Text, out date))
                        {
                            tbDate.BorderColor = System.Drawing.Color.LightGray;
                            date = date.Date;

                            byte areaID = new byte();
                            if (ddlArea.SelectedIndex > 0)
                                areaID = Convert.ToByte(ddlArea.SelectedValue);

                            Int16 locationID = new byte();
                            if (ddlLocation.SelectedIndex > 0)
                                locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                            bool escortService = rbESYes.Checked ? true : false;
                            bool citInGT = rbCITGTYes.Checked ? true : false;
                            bool citOutGT = rbCIToGTYes.Checked ? true : false;
                            bool extraOfficers = rbEOYes.Checked ? true : false;

                            byte amtOfficers = new byte();
                            if (tbAmount.Text != "")
                                amtOfficers = Convert.ToByte(tbAmount.Text);

                            byte shiftID = new byte();
                            if (ddlShift.SelectedIndex > 0)
                                shiftID = Convert.ToByte(ddlShift.SelectedValue);

                            SPECIALOPERATIONS.update_SO(soID, areaID, locationID, date, escortService, citInGT, citOutGT, extraOfficers, amtOfficers, lblReportNumber.Text, tbComments.Text, userID, shiftID);

                            if (SPECIALOPERATIONS.successfulCommit)
                            {
                                success = true;
                                ShowMessage("Record saved.", MessageType.Success);
                            }
                            else
                            {
                                success = false;
                                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                            }
                            return success;
                        }
                        else
                        {
                            ShowMessage("Incorrect date format.", MessageType.Success);
                            tbDate.BorderColor = System.Drawing.Color.Red;
                            tbDate.Focus();
                            return success = false;
                        }
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
                    if (tbAmount.Text != "" && Convert.ToInt32(tbAmount.Text) > 254)
                    {
                        ShowMessage("Incorrect amount entered.", MessageType.Warning);
                        tbAmount.BorderColor = System.Drawing.Color.Red;
                        tbAmount.Focus();
                        return success = false;
                    }
                    else
                    {
                        //insert 
                        tbAmount.BorderColor = System.Drawing.Color.LightGray;
                        DateTime date = new DateTime();
                        if (DateTime.TryParse(tbDate.Text, out date))
                        {
                            tbDate.BorderColor = System.Drawing.Color.LightGray;
                            date = date.Date;

                            byte areaID = new byte();
                            if (ddlArea.SelectedIndex > 0)
                                areaID = Convert.ToByte(ddlArea.SelectedValue);

                            Int16 locationID = new byte();
                            if (ddlLocation.SelectedIndex > 0)
                                locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                            bool escortService = rbESYes.Checked ? true : false;
                            bool citInGT = rbCITGTYes.Checked ? true : false;
                            bool citOutGT = rbCIToGTYes.Checked ? true : false;
                            bool extraOfficers = rbEOYes.Checked ? true : false;

                            byte amtOfficers = new byte();
                            if (tbAmount.Text != "")
                                amtOfficers = Convert.ToByte(tbAmount.Text);

                            generateSONumber();

                            byte shiftID = new byte();
                            if (ddlShift.SelectedIndex > 0)
                                shiftID = Convert.ToByte(ddlShift.SelectedValue);

                            soID = SPECIALOPERATIONS.insert_SO(areaID, locationID, date, escortService, citInGT, citOutGT, extraOfficers, amtOfficers, lblReportNumber.Text, tbComments.Text, userID, shiftID);

                            if (soID != 0)
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
                            return success;
                        }
                        else
                        {
                            ShowMessage("Incorrect date format.", MessageType.Success);
                            tbDate.BorderColor = System.Drawing.Color.Red;
                            tbDate.Focus();
                            return success = false;
                        }
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

        protected void gvSO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblShortCommnets = (Label)e.Row.FindControl("lblShortCommnets");
                if (lblShortCommnets.Text.Length > 100)
                {
                    ViewState["OrigData"] = lblShortCommnets.Text;
                    lblShortCommnets.Text = lblShortCommnets.Text.Substring(0, 100) + "...";
                    lblShortCommnets.ToolTip = ViewState["OrigData"].ToString();
                }
            }
        }

        protected void gvSO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //load next set of rows on next page
            gvSO.PageIndex = e.NewPageIndex;
            gvSO.SelectedIndex = -1;
            loadSearchedSOs();
        }

        protected void gvSO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvSO.SelectedIndex > -1)
            {
                resetForm();
                Label lblSpecialOperationsID = (Label)gvSO.SelectedRow.FindControl("lblSpecialOperationsID");
                Label lblReportNumber2 = (Label)gvSO.SelectedRow.FindControl("lblReportNumber");
                Label lblDate = (Label)gvSO.SelectedRow.FindControl("lblDate");
                Label lblEscortService = (Label)gvSO.SelectedRow.FindControl("lblEscortService");
                Label lblCashInGT = (Label)gvSO.SelectedRow.FindControl("lblCashInGT");
                Label lblCashOutGT = (Label)gvSO.SelectedRow.FindControl("lblCashOutGT");
                Label lblExtraOfficers = (Label)gvSO.SelectedRow.FindControl("lblExtraOfficers");
                Label lblAmountOfficers = (Label)gvSO.SelectedRow.FindControl("lblAmountOfficers");
                Label lblAreaID = (Label)gvSO.SelectedRow.FindControl("lblAreaID");
                Label lblLocationID = (Label)gvSO.SelectedRow.FindControl("lblLocationID");
                Label lblComments = (Label)gvSO.SelectedRow.FindControl("lblComments");
                Label lblShiftID = (Label)gvSO.SelectedRow.FindControl("lblShiftID");
                Label lblCreatedBy = (Label)gvSO.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvSO.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvSO.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvSO.SelectedRow.FindControl("lblLastModifiedOn");

                soID = Convert.ToInt16(lblSpecialOperationsID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                tbDate.Text = lblDate.Text;
                if (lblEscortService.Text == "YES")
                {
                    rbESYes.Checked = true;
                    rbESNo.Checked = false;
                }
                else
                {
                    rbESYes.Checked = false;
                    rbESNo.Checked = true;
                }

                if (lblCashInGT.Text == "YES")
                {
                    rbCITGTYes.Checked = true;
                    rbCITGTNo.Checked = false;
                }
                else
                {
                    rbCITGTYes.Checked = false;
                    rbCITGTNo.Checked = true;
                }

                if (lblCashOutGT.Text == "YES")
                {
                    rbCIToGTYes.Checked = true;
                    rbCIToGTNo.Checked = false;
                }
                else
                {
                    rbCIToGTYes.Checked = false;
                    rbCIToGTNo.Checked = true;
                }

                if (lblExtraOfficers.Text == "True")
                {
                    rbEOYes.Checked = true;
                    rbEONo.Checked = false;
                    lblAmount.Visible = true;
                    tbAmount.Visible = true;
                }
                else
                {
                    rbEOYes.Checked = false;
                    rbEONo.Checked = true;
                    lblAmount.Visible = false;
                    tbAmount.Visible = false;
                }
                tbAmount.Text = lblAmountOfficers.Text;
                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;
                if (ddlShift.Items.FindByValue(lblShiftID.Text) != null)
                    ddlShift.SelectedValue = lblShiftID.Text;
                else
                    ddlShift.SelectedIndex = 0;
                if (lblAreaID.Text != "")
                {
                    loadDDLLocations(lblAreaID.Text);
                    if (ddlLocation.Items.FindByValue(lblLocationID.Text) != null)
                        ddlLocation.SelectedValue = lblLocationID.Text;
                    else
                        ddlLocation.SelectedIndex = 0;
                }
                tbComments.Text = lblComments.Text;
                lblHeader.Text = "Update the selected SO report below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;
                isLoaded = true;
                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                btnBackToSearch.Visible = true;
            }
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

    }
}