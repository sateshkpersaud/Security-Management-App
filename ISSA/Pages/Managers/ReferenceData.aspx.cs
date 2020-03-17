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

namespace ISSA.Pages.Managers
{
    public partial class ReferenceData : System.Web.UI.Page
    {
        BusinessTier.ReferenceData REFERENCEDATA = new BusinessTier.ReferenceData();
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                userID = MISC.getUserID();
                if (userID != 0)
                {
                    string[] roleNames = MISC.getUserRoles();
                    if (roleNames.Contains("Manager"))
                    {
                        loadDDLArea();
                        loadDDLPosition();
                        rbAreaYes.Checked = true;
                        rbAreaNo.Checked = false;
                        rbDepartmentYes.Checked = true;
                        rbDepartmentNo.Checked = false;
                        rbLocationYes.Checked = true;
                        rbLocationNo.Checked = false;
                        rbPositionYes.Checked = true;
                        rbPositionNo.Checked = false;
                        rbShiftYes.Checked = true;
                        rbShiftNo.Checked = false;
                        tbFromTime.Attributes.Add("onblur", "javascript:validateTime(" + tbFromTime.ClientID + ")");
                        tbToTime.Attributes.Add("onblur", "javascript:validateTime(" + tbToTime.ClientID + ")");
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

        private void loadSearchedArea()
        {
            DataTable dt = REFERENCEDATA.sp_Ref_Area_Search(tbArea.Text == "" ? null : tbArea.Text, rbAreaYes.Checked ? true : false);
            if (REFERENCEDATA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvAreas.DataSource = dt;
                    gvAreas.DataBind();
                    gvAreas.Visible = true;
                    lblGVAreaHeader.Text = "Areas found: " + dt.Rows.Count;
                }
                else
                {
                    gvAreas.Visible = false;
                    lblGVAreaHeader.Text = "No results found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvAreas.Visible = false;
                lblGVAreaHeader.Text = "No results found due to error.";
            }
        }

        protected void btnAreaSearch_Click(object sender, EventArgs e)
        {
            loadSearchedArea();
        }

        protected void gvAreas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvAreas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAreas.PageIndex = e.NewPageIndex;
            gvAreas.SelectedIndex = -1;
            loadSearchedArea();
        }

        protected void gvAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvAreas.SelectedIndex > -1)
            {
                clearArea();
                Label lblArea = (Label)gvAreas.SelectedRow.FindControl("lblArea");
                Label lblAreaID = (Label)gvAreas.SelectedRow.FindControl("lblAreaID");
                Label lblIsActive = (Label)gvAreas.SelectedRow.FindControl("lblIsActive");
                if (lblIsActive.Text == "YES")
                {
                    rbAreaYes.Checked = true;
                    rbAreaNo.Checked = false;
                }
                else
                {
                    rbAreaYes.Checked = false;
                    rbAreaNo.Checked = true;
                }
                lblAreaLoaded.Text = "True";
                lblLoadedAreaID.Text = lblAreaID.Text;
                tbArea.Text = lblArea.Text;
            }
        }

        protected void btnAreaSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //UPDATE
                if (lblAreaLoaded.Text == "True")
                {
                    byte areaID = Convert.ToByte(lblLoadedAreaID.Text);
                    bool isActive = rbAreaYes.Checked ? true : false;
                    REFERENCEDATA.update_Area(areaID, tbArea.Text, isActive, userID);
                    if (REFERENCEDATA.successfulCommit)
                    {
                        ShowMessage("Record updated.", MessageType.Success);
                        clearArea();
                        loadSearchedArea();
                    }
                    else
                        ShowMessage("Something went wrong and the record was not updated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
                //INSERT
                else
                {
                    if (!areaDuplicateCheck(tbArea.Text))
                    {
                        bool isActive = rbAreaYes.Checked ? true : false;
                        REFERENCEDATA.insert_Area(tbArea.Text,isActive, userID);
                        if (REFERENCEDATA.successfulCommit)
                        {
                            ShowMessage("Record saved.", MessageType.Success);
                            clearArea();
                            loadSearchedArea();
                        }
                        else
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                    else
                    {
                        ShowMessage("An Area already exist with the name " + tbArea.Text + ".", MessageType.Warning);
                        clearArea();
                    }
                }
            }
        }

        public bool areaDuplicateCheck(string area)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Areas
                           where a.Area == area
                           select new
                           {
                               a.AreaID
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

        private void clearArea()
        {
            tbArea.Text = "";
            lblAreaLoaded.Text = "";
            lblLoadedAreaID.Text = "";
        }

        private void loadSearchedDepartment()
        {
            DataTable dt = REFERENCEDATA.sp_Ref_Department_Search(tbDepartment.Text == "" ? null : tbDepartment.Text, rbDepartmentYes.Checked ? true : false);
            if (REFERENCEDATA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvDepartment.DataSource = dt;
                    gvDepartment.DataBind();
                    gvDepartment.Visible = true;
                    lblGVDepartmentHeader.Text = "Departments found: " + dt.Rows.Count;
                }
                else
                {
                    gvDepartment.Visible = false;
                    lblGVDepartmentHeader.Text = "No results found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvDepartment.Visible = false;
                lblGVDepartmentHeader.Text = "No results found due to error.";
            }
        }

        protected void btnDepartmentSearch_Click(object sender, EventArgs e)
        {
            loadSearchedDepartment();
        }

        protected void gvDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvDepartment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDepartment.PageIndex = e.NewPageIndex;
            gvDepartment.SelectedIndex = -1;
            loadSearchedDepartment();
        }

        protected void gvDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvDepartment.SelectedIndex > -1)
            {
                clearDepartment();
                Label lblDepartment = (Label)gvDepartment.SelectedRow.FindControl("lblDepartment");
                Label lblDepartmentID = (Label)gvDepartment.SelectedRow.FindControl("lblDepartmentID");
                Label lblIsActive = (Label)gvDepartment.SelectedRow.FindControl("lblIsActive");
                if (lblIsActive.Text == "YES")
                {
                    rbDepartmentYes.Checked = true;
                    rbDepartmentNo.Checked = false;
                }
                else
                {
                    rbDepartmentYes.Checked = false;
                    rbDepartmentNo.Checked = true;
                }
                lblDepartmentLoaded.Text = "True";
                lblLoadedDepartmentID.Text = lblDepartmentID.Text;
                tbDepartment.Text = lblDepartment.Text;
            }
        }

        protected void btnDepartmentSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //UPDATE
                if (lblDepartmentLoaded.Text == "True")
                {
                    byte departmentID = Convert.ToByte(lblLoadedDepartmentID.Text);
                    bool isActive = rbDepartmentYes.Checked ? true : false;
                    REFERENCEDATA.update_Department(departmentID, tbDepartment.Text, isActive, userID);
                    if (REFERENCEDATA.successfulCommit)
                    {
                        ShowMessage("Record updated.", MessageType.Success);
                        clearDepartment();
                        loadSearchedDepartment();
                    }
                    else
                        ShowMessage("Something went wrong and the record was not updated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
                //INSERT
                else
                {
                    if (!departmentDuplicateCheck(tbDepartment.Text))
                    {
                        bool isActive = rbDepartmentYes.Checked ? true : false;
                        REFERENCEDATA.insert_Department(tbDepartment.Text,isActive, userID);
                        if (REFERENCEDATA.successfulCommit)
                        {
                            ShowMessage("Record saved.", MessageType.Success);
                            clearDepartment();
                            loadSearchedDepartment();
                        }
                        else
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                    else
                    {
                        ShowMessage("A department already exist with the name " + tbDepartment.Text + ".", MessageType.Warning);
                        clearDepartment();
                    }
                }
            }
        }

        public bool departmentDuplicateCheck(string search)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Departments
                           where a.Department == search
                           select new
                           {
                               a.DepartmentID
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

        private void clearDepartment()
        {
            tbDepartment.Text = "";
            lblDepartmentLoaded.Text = "";
            lblLoadedDepartmentID.Text = "";
        }

        private void loadDDLArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlArea.DataSource = dt;
            ddlArea.DataTextField = "Area";
            ddlArea.DataValueField = "AreaID";
            ddlArea.DataBind();
        }

        private void loadDDLPosition()
        {
            DataTable dt = MISC.sp_Position_Sel4DDL();
            ddlPosition.DataSource = dt;
            ddlPosition.DataTextField = "PositionName";
            ddlPosition.DataValueField = "PositionID";
            ddlPosition.DataBind();
        }

        private void loadSearchedLocation()
        {
            DataTable dt = REFERENCEDATA.sp_Ref_Location_Search(tbLocation.Text == "" ? null : tbLocation.Text, ddlArea.SelectedValue,  rbLocationYes.Checked ? true : false);
            if (REFERENCEDATA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvLocation.DataSource = dt;
                    gvLocation.DataBind();
                    gvLocation.Visible = true;
                    lblGVLocationHeader.Text = "Locations found: " + dt.Rows.Count;
                }
                else
                {
                    gvLocation.Visible = false;
                    lblGVLocationHeader.Text = "No results found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvLocation.Visible = false;
                lblGVLocationHeader.Text = "No results found due to error.";
            }
        }

        protected void btnLocationSearch_Click(object sender, EventArgs e)
        {
            loadSearchedLocation();
        }

        protected void gvLocation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvLocation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLocation.PageIndex = e.NewPageIndex;
            gvLocation.SelectedIndex = -1;
            loadSearchedLocation();
        }

        protected void gvLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvLocation.SelectedIndex > -1)
            {
                clearLocation();
                Label lblLocation = (Label)gvLocation.SelectedRow.FindControl("lblLocation");
                Label lblLocationID = (Label)gvLocation.SelectedRow.FindControl("lblLocationID");
                Label lblAreaID = (Label)gvLocation.SelectedRow.FindControl("lblAreaID");
                Label lblIsActive = (Label)gvLocation.SelectedRow.FindControl("lblIsActive");
                if (lblIsActive.Text == "YES")
                {
                    rbLocationYes.Checked = true;
                    rbLocationNo.Checked = false;
                }
                else
                {
                    rbLocationYes.Checked = false;
                    rbLocationNo.Checked = true;
                }
                ddlArea.SelectedValue = lblAreaID.Text;
                lblLocationLoaded.Text = "True";
                lblLoadedLocationID.Text = lblLocationID.Text;
                tbLocation.Text = lblLocation.Text;
            }
        }

        protected void btnLocationSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //UPDATE
                if (lblLocationLoaded.Text == "True")
                {
                    Int16 locationID = Convert.ToInt16(lblLoadedLocationID.Text);
                    Byte areaID = Convert.ToByte(ddlArea.SelectedValue);
                    bool isActive = rbLocationYes.Checked ? true : false;
                    REFERENCEDATA.update_Location(areaID, locationID, tbLocation.Text,isActive, userID);
                    if (REFERENCEDATA.successfulCommit)
                    {
                        ShowMessage("Record updated.", MessageType.Success);
                        clearLocation();
                        loadSearchedLocation();
                    }
                    else
                        ShowMessage("Something went wrong and the record was not updated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
                //INSERT
                else
                {
                    Byte areaID = Convert.ToByte(ddlArea.SelectedValue);
                    if (!LocationDuplicateCheck(tbLocation.Text, areaID))
                    {
                        bool isActive = rbLocationYes.Checked ? true : false;
                        REFERENCEDATA.insert_Location(areaID, tbLocation.Text,isActive, userID);
                        if (REFERENCEDATA.successfulCommit)
                        {
                            ShowMessage("Record saved.", MessageType.Success);
                            clearLocation();
                            loadSearchedLocation();
                        }
                        else
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                    else
                    {
                        ShowMessage("A Location already exist with the name " + tbDepartment.Text + ".", MessageType.Warning);
                        clearLocation();
                    }
                }
            }
        }

        public bool LocationDuplicateCheck(string search, byte areaID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Locations
                           where a.LocationName == search && a.AreaID == areaID
                           select new
                           {
                               a.LocationID
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

        private void clearLocation()
        {
            tbLocation.Text = "";
            lblLocationLoaded.Text = "";
            lblLoadedLocationID.Text = "";
            ddlArea.SelectedIndex = 0;
        }

        private void loadSearchedPosition()
        {
            DataTable dt = REFERENCEDATA.sp_Ref_Position_Search(tbPosition.Text == "" ? null : tbPosition.Text, rbPositionYes.Checked ? true : false);
            if (REFERENCEDATA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvPosition.DataSource = dt;
                    gvPosition.DataBind();
                    gvPosition.Visible = true;
                    lblGVPositionHeader.Text = "Positions found: " + dt.Rows.Count;
                }
                else
                {
                    gvPosition.Visible = false;
                    lblGVPositionHeader.Text = "No results found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvPosition.Visible = false;
                lblGVPositionHeader.Text = "No results found due to error.";
            }
        }

        protected void btnPositionSearch_Click(object sender, EventArgs e)
        {
            loadSearchedPosition();
        }

        protected void gvPosition_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvPosition_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPosition.PageIndex = e.NewPageIndex;
            gvPosition.SelectedIndex = -1;
            loadSearchedPosition();
        }

        protected void gvPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvPosition.SelectedIndex > -1)
            {
                clearPosition();
                Label lblPosition = (Label)gvPosition.SelectedRow.FindControl("lblPosition");
                Label lblPositionID = (Label)gvPosition.SelectedRow.FindControl("lblPositionID");
                Label lblIsActive = (Label)gvPosition.SelectedRow.FindControl("lblIsActive");
                if (lblIsActive.Text == "YES")
                {
                    rbPositionYes.Checked = true;
                    rbPositionNo.Checked = false;
                }
                else
                {
                    rbPositionYes.Checked = false;
                    rbPositionNo.Checked = true;
                }
                lblPositionLoaded.Text = "True";
                lblLoadedPositionID.Text = lblPositionID.Text;
                tbPosition.Text = lblPosition.Text;
            }
        }

        protected void btnPositionSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //UPDATE
                if (lblPositionLoaded.Text == "True")
                {
                    byte positionID = Convert.ToByte(lblLoadedPositionID.Text);
                    bool isActive = rbPositionYes.Checked ? true : false;
                    REFERENCEDATA.update_Position(positionID, tbPosition.Text, isActive, userID);
                    if (REFERENCEDATA.successfulCommit)
                    {
                        ShowMessage("Record updated.", MessageType.Success);
                        clearPosition();
                        loadSearchedPosition();
                    }
                    else
                        ShowMessage("Something went wrong and the record was not updated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
                //INSERT
                else
                {
                    if (!positionDuplicateCheck(tbPosition.Text))
                    {
                        bool isActive = rbPositionYes.Checked ? true : false;
                        REFERENCEDATA.insert_Position(tbPosition.Text, isActive, userID);
                        if (REFERENCEDATA.successfulCommit)
                        {
                            ShowMessage("Record saved.", MessageType.Success);
                            clearPosition();
                            loadSearchedPosition();
                        }
                        else
                            ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                    else
                    {
                        ShowMessage("A Position already exist with the name " + tbPosition.Text + ".", MessageType.Warning);
                        clearPosition();
                    }
                }
            }
        }

        public bool positionDuplicateCheck(string search)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Positions
                           where a.PositionName == search
                           select new
                           {
                               a.PositionID
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

        private void clearPosition()
        {
            tbPosition.Text = "";
            lblPositionLoaded.Text = "";
            lblLoadedPositionID.Text = "";
        }

        private void loadSearchedShift()
        {
            DataTable dt = REFERENCEDATA.sp_Ref_Shift_Search(tbFromTime.Text == "" ? null : tbFromTime.Text, tbToTime.Text == "" ? null : tbToTime.Text, ddlPosition.SelectedValue, rbShiftYes.Checked ? true : false);
            if (REFERENCEDATA.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvShift.DataSource = dt;
                    gvShift.DataBind();
                    gvShift.Visible = true;
                    lblGVShiftHeader.Text = "Shifts found: " + dt.Rows.Count;
                }
                else
                {
                    gvShift.Visible = false;
                    lblGVShiftHeader.Text = "No results found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvShift.Visible = false;
                lblGVShiftHeader.Text = "No results found due to error.";
            }
        }

        protected void btnShiftSearch_Click(object sender, EventArgs e)
        {
            loadSearchedShift();
        }

        protected void gvShift_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvShift_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvShift.PageIndex = e.NewPageIndex;
            gvShift.SelectedIndex = -1;
            loadSearchedShift();
        }

        protected void gvShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvShift.SelectedIndex > -1)
            {
                clearShift();
                Label lblFromTime = (Label)gvShift.SelectedRow.FindControl("lblFromTime");
                Label lblToTime = (Label)gvShift.SelectedRow.FindControl("lblToTime");
                Label lblShiftID = (Label)gvShift.SelectedRow.FindControl("lblShiftID");
                Label lblPositionID = (Label)gvShift.SelectedRow.FindControl("lblPositionID");
                Label lblIsActive = (Label)gvShift.SelectedRow.FindControl("lblIsActive");
                Label lblShift = (Label)gvShift.SelectedRow.FindControl("lblShift");
                Label lblHoursInShift = (Label)gvShift.SelectedRow.FindControl("lblHoursInShift");
                if (lblIsActive.Text == "YES")
                {
                    rbShiftYes.Checked = true;
                    rbShiftNo.Checked = false;
                }
                else
                {
                    rbShiftYes.Checked = false;
                    rbShiftNo.Checked = true;
                }
                ddlPosition.SelectedValue = lblPositionID.Text;
                lblShiftLoaded.Text = "True";
                lblLoadedShiftID.Text = lblShiftID.Text;
                tbFromTime.Text = lblFromTime.Text;
                tbToTime.Text = lblToTime.Text;
                lblShiftName.Text = lblShift.Text;
                tbHoursInShift.Text = lblHoursInShift.Text;
            }
        }

        protected void btnShiftSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                TimeSpan fTime = new TimeSpan();
                TimeSpan tTime = new TimeSpan();
                if ((TimeSpan.TryParse(tbFromTime.Text, out fTime)) && (TimeSpan.TryParse(tbToTime.Text, out tTime)))
                {
                    //UPDATE
                    if (lblShiftLoaded.Text == "True")
                    {
                        byte shiftID = Convert.ToByte(lblLoadedShiftID.Text);
                        Byte positionID = Convert.ToByte(ddlPosition.SelectedValue);
                        bool isActive = rbShiftYes.Checked ? true : false;
                        REFERENCEDATA.update_Shift(shiftID, positionID, fTime, tTime, lblShiftName.Text, tbHoursInShift.Text, isActive, userID);
                        if (REFERENCEDATA.successfulCommit)
                        {
                            ShowMessage("Record updated.", MessageType.Success);
                            clearShift();
                            loadSearchedShift();
                        }
                        else
                            ShowMessage("Something went wrong and the record was not updated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                    //INSERT
                    else
                    {
                        Byte positionID = Convert.ToByte(ddlPosition.SelectedValue);
                        if (!shiftDuplicateCheck(tbFromTime.Text, tbToTime.Text, positionID))
                        {
                            bool isActive = rbShiftYes.Checked ? true : false;
                            REFERENCEDATA.insert_Shift(positionID, fTime, tTime, lblShiftName.Text, tbHoursInShift.Text, isActive, userID);
                            if (REFERENCEDATA.successfulCommit)
                            {
                                ShowMessage("Record saved.", MessageType.Success);
                                clearShift();
                                loadSearchedShift();
                            }
                            else
                                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                        }
                        else
                        {
                            ShowMessage("A Shift already exist with the name " + tbDepartment.Text + ".", MessageType.Warning);
                            clearShift();
                        }
                    }
                }
                else
                    ShowMessage("Invalid time entered." + tbDepartment.Text + ".", MessageType.Warning);
            }
        }

        public bool shiftDuplicateCheck(string fromTime, string toTime, byte positionID)
        {
            bool isDuplicate = false;
            TimeSpan fTime = new TimeSpan();
            TimeSpan tTime =new TimeSpan();
            if ((TimeSpan.TryParse(fromTime, out fTime)) && (TimeSpan.TryParse(toTime, out tTime)))
            {

                tTime = TimeSpan.Parse(toTime);
                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Ref_Shifts
                               where a.FromTime == fTime && a.ToTime == tTime && a.PositionID == positionID
                               select new
                               {
                                   a.ShiftID
                               }).ToList();

                if (results.Count > 0)
                {
                    isDuplicate = true;
                }
                else
                {
                    isDuplicate = false;
                }
            }
            else
            {
                ShowMessage("Incorrect Time entered. Kindly verify.", MessageType.Warning);
            }
            return isDuplicate;
        }

        private void clearShift()
        {
            tbFromTime.Text = "";
            tbToTime.Text = "";
            lblShiftLoaded.Text = "";
            lblLoadedShiftID.Text = "";
            tbHoursInShift.Text = "";
            lblShiftName.Text = "";
            ddlPosition.SelectedIndex = 0;
        }

        //protected void tbFromTime_TextChanged(object sender, EventArgs e)
        //{
        //    if ((tbFromTime.Text != "") && (tbToTime.Text != ""))
        //    {
        //        TimeSpan dt = Convert.ToDateTime(tbToTime.Text) - Convert.ToDateTime(tbFromTime.Text);
        //        int hours = (int)dt.TotalHours;
        //        tbHoursInShift.Text = hours.ToString();
        //    }
        //    UpdatePanel4.Update();
        //}

        //protected void tbToTime_TextChanged(object sender, EventArgs e)
        //{
        //    if ((tbFromTime.Text != "") && (tbToTime.Text != ""))
        //    {
        //        TimeSpan dt = Convert.ToDateTime(tbToTime.Text) - Convert.ToDateTime(tbFromTime.Text);
        //        int hours = (int)dt.TotalHours;
        //        tbHoursInShift.Text = hours.ToString();
        //    }
        //    UpdatePanel4.Update();
        //}

    }
}