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

namespace ISSA.Pages.Supervisors
{
    public partial class Employees : System.Web.UI.Page
    {
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        BusinessTier.Employee EMPLOYEE = new BusinessTier.Employee();
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

        public short employeeID
        {
            get
            {
                return (short)ViewState["employeeID"];
            }
            set
            {
                ViewState["employeeID"] = value;
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
                //Get the loggedinuser ID
                userID = MISC.getUserID();
                if (userID != 0)
                {
                    string[] roleNames = MISC.getUserRoles();
                    if (roleNames.Contains("Manager"))
                    {
                        pnlSearch.Visible = true;
                        pnlDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        pageLoad();
                        tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
                    }
                    else
                    {
                        ShowMessage("No ID returned for logged in user", MessageType.Error);
                        Response.Redirect("../../Pages/Account/Login.aspx");
                    }
                    //}
                    //else
                    //{
                    //    //load for operator
                    //    ibCalendar.Enabled = false; //This will have to be changed when the table to allow explicit changing is set up
                    //    pnlSearch.Visible = false;
                    //    pnlDetails.Visible = true;
                    //    btnBackToSearch.Visible = false;
                    //    pageLoad();
                    //}
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
            gvEmployees.Visible = false;
            lblGVEmployeesHeader.Text = "";
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

        //populate the location ddl based on the area selected
        private void loadDDLSearchDepartment()
        {
            DataTable dt = EMPLOYEE.sp_Department_Sel4DDL();
            ddlSearchDepartment.DataSource = dt;
            ddlSearchDepartment.DataTextField = "Department";
            ddlSearchDepartment.DataValueField = "DepartmentID";
            ddlSearchDepartment.DataBind();
        }

        private void loadDDLDepartment()
        {
            DataTable dt = EMPLOYEE.sp_Department_Sel4DDL();
            ddlDepartment.DataSource = dt;
            ddlDepartment.DataTextField = "Department";
            ddlDepartment.DataValueField = "DepartmentID";
            ddlDepartment.DataBind();
        }

        //populate the location ddl based on the area selected
        private void loadDDLSearchPosition()
        {
            DataTable dt = EMPLOYEE.sp_Position_Sel4DDL();
            ddlSearchPosition.DataSource = dt;
            ddlSearchPosition.DataTextField = "PositionName";
            ddlSearchPosition.DataValueField = "PositionID";
            ddlSearchPosition.DataBind();
        }

        private void loadDDLPosition()
        {
            DataTable dt = EMPLOYEE.sp_Position_Sel4DDL();
            ddlPosition.DataSource = dt;
            ddlPosition.DataTextField = "PositionName";
            ddlPosition.DataValueField = "PositionID";
            ddlPosition.DataBind();
        }

        private void loadDDLShift(object positionID)
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(positionID);
            ddlShiftAssigned.DataSource = dt;
            ddlShiftAssigned.DataTextField = "shift";
            ddlShiftAssigned.DataValueField = "ShiftID";
            ddlShiftAssigned.DataBind();
        }

        private void loadDDLSearchShift(object positionID)
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(positionID);
            ddlSearchShift.DataSource = dt;
            ddlSearchShift.DataTextField = "shift";
            ddlSearchShift.DataValueField = "ShiftID";
            ddlSearchShift.DataBind();
        }

        protected void ddlSearchArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchArea.SelectedIndex > 0)
                loadDDLSearchLocations(Convert.ToByte(ddlSearchArea.SelectedValue));
            else
                loadDDLSearchLocations(null);
        }

        protected void ddlPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPosition.SelectedIndex > 0)
                loadDDLShift(Convert.ToByte(ddlPosition.SelectedValue));
            else
                loadDDLShift(null);
        }

        //reload the location ddl on the area ddl index change
        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
        }

        protected void btnNewAbsence_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new employee record below";
            resetForm();
            pnlSearch.Visible = false;
            pnlDetails.Visible = true;
            btnBackToSearch.Visible = true;
        }

        //clear the entire form
        private void resetForm()
        {
            loadDDLSearchArea();
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            loadDDLSearchDepartment();
            loadDDLSearchPosition();
            object PositionID = null;
            if (ddlSearchPosition.SelectedIndex > 0)
                PositionID = Convert.ToByte(ddlSearchPosition.SelectedValue);
            loadDDLSearchShift(PositionID);
            loadDDLArea();
            if (ddlArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlArea.SelectedValue);
            loadDDLLocations(AreaID);
            loadDDLDepartment();
            loadDDLPosition();
            if (ddlPosition.SelectedIndex > 0)
                PositionID = Convert.ToByte(ddlPosition.SelectedValue);
            loadDDLShift(PositionID);
            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            dateNow = dateNow.Substring(0, 10);
            tbDateHired.Text = dateNow;
            isLoaded = false;
            tbFirstName.Text = "";
            tbLastName.Text = "";
            tbOtherName.Text = "";
            rbStandByYes.Checked = false;
            rbStandByNo.Checked = true;
            rbActiveYes.Checked = true;
            rbActiveNo.Checked = false;
            employeeID = 0;
            generateEmployeeNumber();
            lblAuditTrail.Text = "";
            lblHeader.Text = "Add a new employee below";
            btnDeleteEmployee.Visible = false;
        }

        //generate the incident number
        private void generateEmployeeNumber()
        {
            DateTime dateHired = new DateTime();
             if (DateTime.TryParse(tbDateHired.Text, out dateHired))
             {
                 tbDateHired.BorderColor = System.Drawing.Color.LightGray;
                 string month = "";
                 string day = "";
                 if (dateHired.Month < 10)
                     month = "0" + dateHired.Month.ToString();
                 else
                     month = dateHired.Month.ToString();

                 if (dateHired.Day < 10)
                     day = "0" + dateHired.Day.ToString();
                 else
                     day = dateHired.Day.ToString();

                 var model = new EDM.DataSource();
                 var results = (from a in model.tbl_Sup_Employees
                                where a.DateHired == dateHired.Date
                                select new
                                {
                                    a.EmployeeID
                                }).ToList();
                 int currentSequence = Convert.ToInt32(results.Count) + 1;
                 if (currentSequence < 10)
                 {
                     lblEmployeeNumber.Text = dateHired.ToString("yy") + month + day + "0" + currentSequence.ToString();
                 }
                 else
                 {
                     lblEmployeeNumber.Text = dateHired.ToString("yy") + month + day + currentSequence.ToString();
                 }
             }
             else
             {
                 ShowMessage("Incorrect date format.", MessageType.Error);
                 tbDateHired.BorderColor = System.Drawing.Color.Red;
                 tbDateHired.Focus();
             }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            resetForm();
        }

        protected void tbDateHired_TextChanged(object sender, EventArgs e)
        {
            generateEmployeeNumber();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveEmployee();
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveEmployee();
                if (success)
                    resetForm();
            }
        }

        //function to save the employee, either a new insert or an update
        private bool saveEmployee()
        {
            bool success = false;

            if (isLoaded)
            {
                try
                {
                    //Update 
                    DateTime dateHired = new DateTime();
                    if (DateTime.TryParse(tbDateHired.Text, out dateHired))
                    {
                        tbDateHired.BorderColor = System.Drawing.Color.LightGray;
                        dateHired = dateHired.Date;

                        byte areaID = new byte();
                        if (ddlArea.SelectedIndex > 0)
                            areaID = Convert.ToByte(ddlArea.SelectedValue);

                        Int16 locationID = new byte();
                        if (ddlLocation.SelectedIndex > 0)
                            locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                        byte departmentID = new byte();
                        if (ddlDepartment.SelectedIndex > 0)
                            departmentID = Convert.ToByte(ddlDepartment.SelectedValue);

                        byte positionID = new byte();
                        if (ddlPosition.SelectedIndex > 0)
                            positionID = Convert.ToByte(ddlPosition.SelectedValue);

                        byte shiftID = new byte();
                        if (ddlShiftAssigned.SelectedIndex > 0)
                            shiftID = Convert.ToByte(ddlShiftAssigned.SelectedValue);

                        bool isStandByEmployee = rbStandByYes.Checked ? true : false;
                        bool isActive = rbActiveYes.Checked ? true : false;

                        if (lblEmployeeNumber.Text == "")
                            generateEmployeeNumber();

                        EMPLOYEE.update_Employee(employeeID, tbFirstName.Text, tbLastName.Text, tbOtherName.Text, dateHired, areaID, locationID, departmentID, positionID, shiftID, isStandByEmployee, isActive, lblEmployeeNumber.Text, userID);

                        if (EMPLOYEE.successfulCommit)
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
                        tbDateHired.BorderColor = System.Drawing.Color.Red;
                        tbDateHired.Focus();
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
                    //insert 
                    DateTime dateHired = new DateTime();
                    if (DateTime.TryParse(tbDateHired.Text, out dateHired))
                    {
                        tbDateHired.BorderColor = System.Drawing.Color.LightGray;
                        dateHired = dateHired.Date;
                        if (!isDuplicateCheck(dateHired))
                        {
                            byte areaID = new byte();
                            if (ddlArea.SelectedIndex > 0)
                                areaID = Convert.ToByte(ddlArea.SelectedValue);

                            Int16 locationID = new byte();
                            if (ddlLocation.SelectedIndex > 0)
                                locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                            byte departmentID = new byte();
                            if (ddlDepartment.SelectedIndex > 0)
                                departmentID = Convert.ToByte(ddlDepartment.SelectedValue);

                            byte positionID = new byte();
                            if (ddlPosition.SelectedIndex > 0)
                                positionID = Convert.ToByte(ddlPosition.SelectedValue);

                            byte shiftID = new byte();
                            if (ddlShiftAssigned.SelectedIndex > 0)
                                shiftID = Convert.ToByte(ddlShiftAssigned.SelectedValue);

                            bool isStandByEmployee = rbStandByYes.Checked ? true : false;
                            bool isActive = rbActiveYes.Checked ? true : false;

                            generateEmployeeNumber();

                            employeeID = EMPLOYEE.insert_Employee(tbFirstName.Text, tbLastName.Text, tbOtherName.Text, dateHired, areaID, locationID, departmentID, positionID, shiftID, isStandByEmployee, isActive, lblEmployeeNumber.Text, userID);

                            if (employeeID != 0)
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
                            ShowMessage("This employee has already been added to the system. Kindly verify entry.", MessageType.Info);
                        }
                        return success;
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Success);
                        tbDateHired.BorderColor = System.Drawing.Color.Red;
                        tbDateHired.Focus();
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

        protected void btnSearchEmployee_Click(object sender, EventArgs e)
        {
            loadSearchedEmployees();
        }

        private void loadSearchedEmployees()
        {
            DataTable dt = EMPLOYEE.sp_Employee_Search(tbSearchName.Text == "" ? null : tbSearchName.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchDepartment.SelectedValue, ddlSearchPosition.SelectedValue, ddlSearchArea.SelectedValue, ddlSearchLocation.SelectedValue, ddlSearchShift.SelectedValue, ddlSearchStandByStaff.SelectedValue, ddlSearchIsActive.SelectedValue);
            if (EMPLOYEE.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvEmployees.DataSource = dt;
                    gvEmployees.DataBind();
                    gvEmployees.Visible = true;
                    lblGVEmployeesHeader.Text = "Employees found: " + dt.Rows.Count;
                }
                else
                {
                    gvEmployees.Visible = false;
                    lblGVEmployeesHeader.Text = "No employees found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvEmployees.Visible = false;
                lblGVEmployeesHeader.Text = "No reports found due to error.";
            }
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //load next set of rows on next page
            gvEmployees.PageIndex = e.NewPageIndex;
            gvEmployees.SelectedIndex = -1;
            loadSearchedEmployees();
        }

        protected void gvEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvEmployees.SelectedIndex > -1)
            {
                resetForm();
                Label lblEmployeeID = (Label)gvEmployees.SelectedRow.FindControl("lblEmployeeID");
                Label lblEmployeeNumber2 = (Label)gvEmployees.SelectedRow.FindControl("lblEmployeeNumber");
                Label lblDateHired = (Label)gvEmployees.SelectedRow.FindControl("lblDateHired");
                Label lblIsStandByStaff = (Label)gvEmployees.SelectedRow.FindControl("lblIsStandByStaff");
                Label lblIsActive = (Label)gvEmployees.SelectedRow.FindControl("lblIsActive");
                Label lblFirstName = (Label)gvEmployees.SelectedRow.FindControl("lblFirstName");
                Label lblLastName = (Label)gvEmployees.SelectedRow.FindControl("lblLastName");
                Label lblOtherName = (Label)gvEmployees.SelectedRow.FindControl("lblOtherName");
                Label lblAreaID = (Label)gvEmployees.SelectedRow.FindControl("lblAreaID");
                Label lblLocationID = (Label)gvEmployees.SelectedRow.FindControl("lblLocationID");
                Label lblDepartmentID = (Label)gvEmployees.SelectedRow.FindControl("lblDepartmentID");
                Label lblPositionID = (Label)gvEmployees.SelectedRow.FindControl("lblPositionID");
                Label lblShiftID = (Label)gvEmployees.SelectedRow.FindControl("lblShiftID");
                Label lblCreatedBy = (Label)gvEmployees.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvEmployees.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvEmployees.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvEmployees.SelectedRow.FindControl("lblLastModifiedOn");

                employeeID = Convert.ToInt16(lblEmployeeID.Text);
                lblEmployeeNumber.Text = lblEmployeeNumber2.Text;
                if (lblIsStandByStaff.Text == "YES")
                {
                    rbStandByYes.Checked = true;
                    rbStandByNo.Checked = false;
                }
                else
                {
                    rbStandByNo.Checked = true;
                    rbStandByYes.Checked = false;
                }

                if (lblIsActive.Text == "YES")
                {
                    rbActiveYes.Checked = true;
                    rbActiveNo.Checked = false;
                }
                else
                {
                    rbActiveNo.Checked = true;
                    rbActiveYes.Checked = false;
                }
                tbLastName.Text = lblLastName.Text;
                tbFirstName.Text = lblFirstName.Text;
                tbOtherName.Text = lblOtherName.Text;
                tbDateHired.Text = lblDateHired.Text;

                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;

                if (lblAreaID.Text != "")
                {
                    loadDDLLocations(lblAreaID.Text);
                    if (ddlLocation.Items.FindByValue(lblLocationID.Text) != null)
                        ddlLocation.SelectedValue = lblLocationID.Text;
                    else
                        ddlLocation.SelectedIndex = 0;
                }
                else
                    ddlLocation.SelectedIndex = 0;
                if (ddlDepartment.Items.FindByValue(lblDepartmentID.Text) != null)
                    ddlDepartment.SelectedValue = lblDepartmentID.Text;
                else
                    ddlDepartment.SelectedIndex = 0;
                if (ddlPosition.Items.FindByValue(lblPositionID.Text) != null)
                    ddlPosition.SelectedValue = lblPositionID.Text;
                else
                    ddlPosition.SelectedIndex = 0;
                if (lblPositionID.Text != "")
                {
                    loadDDLShift(lblPositionID.Text);
                    if (ddlShiftAssigned.Items.FindByValue(lblShiftID.Text) != null)
                        ddlShiftAssigned.SelectedValue = lblShiftID.Text;
                    else
                        ddlShiftAssigned.SelectedIndex = 0;
                }
                else
                    ddlShiftAssigned.SelectedIndex = 0;
                lblHeader.Text = "Update the selected employee record below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;
                isLoaded = true;
                pnlSearch.Visible = false;
                pnlDetails.Visible = true;
                btnBackToSearch.Visible = true;
                btnDeleteEmployee.Visible = true;
            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            btnBackToSearch.Visible = false;
            resetForm();
            pnlSearch.Visible = true;
            lblGVEmployeesHeader.Text = "";
            if (gvEmployees.Rows.Count > 0)
                loadSearchedEmployees();
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchName.Text = "";
            tbSearchDate.Text = "";
            ddlSearchDepartment.SelectedIndex = 0;
            ddlSearchIsActive.SelectedIndex = 0;
            ddlSearchStandByStaff.SelectedIndex = 0;
            ddlSearchPosition.SelectedIndex = 0;
            ddlSearchArea.SelectedIndex = 0;
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            object PositionID = null;
            if (ddlSearchPosition.SelectedIndex > 0)
                PositionID = Convert.ToByte(ddlSearchPosition.SelectedValue);
            loadDDLSearchShift(PositionID);
            loadDDLSearchLocations(AreaID);
            gvEmployees.Visible = false;
            lblGVEmployeesHeader.Text = "";
        }

        protected void ddlSearchPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchPosition.SelectedIndex > 0)
                loadDDLSearchShift(Convert.ToByte(ddlSearchPosition.SelectedValue));
            else
                loadDDLSearchShift(null);
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void gvEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        public bool isDuplicateCheck(DateTime dateHired)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Sup_Employees
                           where a.FirstName == tbFirstName.Text && a.LastName == tbLastName.Text && a.DateHired == dateHired
                           select new
                           {
                               a.EmployeeID
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

        private void showConfirmDeleteModal()
        {
            mpeConfirmDelete.Show();
        }

        private void hideConfirmDeleteModal()
        {
            mpeConfirmDelete.Hide();
        }

        protected void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            showConfirmDeleteModal();
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                EMPLOYEE.employee_Delete(employeeID);
                if (EMPLOYEE.successfulCommit)
                {
                    ShowMessage("Employee deleted", MessageType.Success);
                    resetForm();
                }
                else
                    ShowMessage("Employee cannot be deleted! He/she has records attached to their name.", MessageType.Warning);
            }
        }

        protected void btnNo_Click(object sender, EventArgs e)
        {
            hideConfirmDeleteModal();
        }

    }
}