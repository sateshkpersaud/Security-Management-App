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

namespace ISSA.Pages.Managers
{
    public partial class WorkSchedule : System.Web.UI.Page
    {
        BusinessTier.WorkSchedule WORKSCHEDULE = new BusinessTier.WorkSchedule();
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

        public List<int> illegalIDs
        {
            get
            {
                return (List<int>)ViewState["illegalIDs"];
            }
            set
            {
                ViewState["illegalIDs"] = value;
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
                       rbCreate.Checked = true;
                       rbModify.Checked = false;
                       loadYear();
                       loadShifts();
                       loadDDLArea();
                       btnSave.Visible = false;
                       btnSave2.Visible = false;
                       btnUnscheduledEmps.Visible = false;
                       btnUnscheduledEmps2.Visible = false;
                       setCurrentMonth();
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

        private void setCurrentMonth()
        {
            int currentMonth = DateTime.Now.Month;
            ddlMonth.SelectedValue = currentMonth.ToString();
        }

        private void loadEmployees_ByShift(object areaID, object shiftID)
        {
            DataTable dt = new DataTable();
            if (rbCreate.Checked)
            {
                dt = WORKSCHEDULE.sp_Employee_SelectForWorkSchedule(areaID, shiftID);
            }
            else 
            {
                dt = WORKSCHEDULE.sp_Employee_SelectFromWorkSchedule(workScheduleID);
            }
           
            if (WORKSCHEDULE.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    //Get illegal employees
                    loadIllegalEmployeesDetails();
                    gvEmployeesSchedule.DataSource = dt;
                    gvEmployeesSchedule.DataBind();
                    gvEmployeesSchedule.Visible = true;
                    if (!isBeforeCurrentMonth(ddlMonth.SelectedValue, ddlYear.SelectedValue))
                    {
                        btnSave.Visible = true;
                        btnSave2.Visible = true;

                        if (rbModify.Checked)
                        {
                            loadUnscheduledEmployees(ddlYear.SelectedValue, ddlMonth.SelectedValue, ddlArea.SelectedValue, ddlShift.SelectedValue);
                        }
                    }
                    lblemployeesFound.Text = "Employees on schedule: " + dt.Rows.Count;
                    if (rbModify.Checked)
                    {
                        isLoaded = true;
                        setAuditTrail();
                    }
                }
                else
                {
                    ShowMessage("No employees found.", MessageType.Info);
                    gvEmployeesSchedule.Visible = false;
                    btnSave.Visible = false;
                    btnSave2.Visible = false;
                    btnUnscheduledEmps.Visible = false;
                    btnUnscheduledEmps2.Visible = false;
                    lblemployeesFound.Text = "No employees on schedule";
                    if (rbModify.Checked)
                    {
                        isLoaded = true;
                        setAuditTrail();
                    }
                }
            }
            else
            {
                ShowMessage("Something went wrong and the employees were not selected. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvEmployeesSchedule.Visible = false;
                isLoaded = false;
            }
        }

        private void loadEmployeesSaved_ByShift(object areaID, object shiftID)
        {
            DataTable dt = WORKSCHEDULE.sp_Employee_SelectForWorkSchedule(areaID, shiftID);
            if (WORKSCHEDULE.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvEmployeesSchedule.DataSource = dt;
                    gvEmployeesSchedule.DataBind();
                    gvEmployeesSchedule.Visible = true;
                    if (!isBeforeCurrentMonth(ddlMonth.SelectedValue, ddlYear.SelectedValue))
                    {
                        btnSave.Visible = true;
                        btnSave2.Visible = true;
                        if (rbModify.Checked)
                        {
                            loadUnscheduledEmployees(ddlYear.SelectedValue, ddlMonth.SelectedValue, ddlArea.SelectedValue, ddlShift.SelectedValue);
                        }
                    }
                    lblemployeesFound.Text = "Employees on schedule: " + dt.Rows.Count;
                }
                else
                {
                    ShowMessage("No employees found.", MessageType.Info);
                    gvEmployeesSchedule.Visible = false;
                    btnSave.Visible = false;
                    btnSave2.Visible = false;
                    btnUnscheduledEmps.Visible = false;
                    btnUnscheduledEmps2.Visible = false;
                    lblemployeesFound.Text = "No employees on schedule";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the employees were not selected. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvEmployeesSchedule.Visible = false;
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

        private void loadDDLTransferArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlTransferArea.DataSource = dt;
            ddlTransferArea.DataTextField = "Area";
            ddlTransferArea.DataValueField = "AreaID";
            ddlTransferArea.DataBind();
        }

        private void loadDDLLocations(object AreaID, DropDownList ddl)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddl.DataSource = dt;
            ddl.DataTextField = "LocationName";
            ddl.DataValueField = "LocationID";
            ddl.DataBind();
        }

        private void loadYear()
        {
            //If create, get current year only and next year if month is october
            DataTable dt = new DataTable();
            dt.Columns.Add("Year", typeof(string));

            if (rbCreate.Checked == true)
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                string sCurrentYear = currentYear.ToString();
                DataRow dr = dt.NewRow();
                dr["Year"] = sCurrentYear ;
                dt.Rows.Add(dr);
                //ddlYear.Items.Add(sCurrentYear);
                if (currentMonth > 10)
                {
                    int nextYear = currentYear + 1;
                    DataRow dr2 = dt.NewRow();
                    dr2["Year"] = nextYear.ToString();
                    dt.Rows.Add(dr2);
                    //ddlYear.Items.Add(nextYear.ToString());
                }
            }
            //if modify, get years from database
            else
            {
                var model = new EDM.DataSource();
                var results = (from a in model.tbl_Mgr_WorkSchedule
                               select new
                               {
                                   a.Year
                               }).Distinct().ToList();

                if (results.Count > 0)
                {
                    foreach (var val in results)
                    {
                       // ddlYear.Items.Add(val.Year);
                        DataRow dr = dt.NewRow();
                        dr["Year"] = val.Year;
                        dt.Rows.Add(dr);
                    }
                }
            }
            ddlYear.DataSource = dt;
            ddlYear.DataTextField = "Year";
            ddlYear.DataValueField = "Year";
            ddlYear.DataBind();
        }

        private void loadShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        private void loadDDLTransferShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlTransferShift.DataSource = dt;
            ddlTransferShift.DataTextField = "shift";
            ddlTransferShift.DataValueField = "ShiftID";
            ddlTransferShift.DataBind();
        }


        private void loadDDLStandBy(DropDownList ddl)
        {
            DataTable dt = WORKSCHEDULE.sp_StandByEmployee_Sel4DDL();
            ddl.DataSource = dt;
            ddl.DataTextField = "FullName";
            ddl.DataValueField = "EmployeeID";
            ddl.DataBind();
        }

        protected void gvEmployeesSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlLocation = (DropDownList)e.Row.FindControl("ddlLocation");
                DropDownList ddlStandByEmployee = (DropDownList)e.Row.FindControl("ddlStandByEmployee");
                Label lblLocationID = (Label)e.Row.FindControl("lblLocationID");
                Label lblStandyByEmployeeID = (Label)e.Row.FindControl("lblStandyByEmployeeID");

                TextBox tbDayOff1 = (TextBox)e.Row.FindControl("tbDayOff1");
                TextBox tbDayOff2 = (TextBox)e.Row.FindControl("tbDayOff2");
                TextBox tbDayOff3 = (TextBox)e.Row.FindControl("tbDayOff3");
                TextBox tbDayOff4 = (TextBox)e.Row.FindControl("tbDayOff4");

                //Restrict the datetime picker to only the selected month/year
                string thisDate = ddlMonth.SelectedValue + "/01/" + ddlYear.SelectedItem.Text;
                DateTime baseDate = Convert.ToDateTime(thisDate);
                var baseMonthStart = baseDate.AddDays(1 - baseDate.Day);
                var baseMonthEnd = baseMonthStart.AddMonths(1).AddSeconds(-1);
                
                string starDate = baseMonthStart.Date.ToString();
                string endDate = baseMonthEnd.Date.ToString();
                tbDayOff1.Attributes.Add("onblur", "javascript:validateDateRange(" + tbDayOff1.ClientID + ",'" + starDate + "','" + endDate + "','" + ddlMonth.SelectedItem.Text + "'," + ddlYear.ClientID + ")");
                tbDayOff2.Attributes.Add("onblur", "javascript:validateDateRange(" + tbDayOff2.ClientID + ",'" + starDate + "','" + endDate + "','" + ddlMonth.SelectedItem.Text + "'," + ddlYear.ClientID + ")");
                tbDayOff3.Attributes.Add("onblur", "javascript:validateDateRange(" + tbDayOff3.ClientID + ",'" + starDate + "','" + endDate + "','" + ddlMonth.SelectedItem.Text + "'," + ddlYear.ClientID + ")");
                tbDayOff4.Attributes.Add("onblur", "javascript:validateDateRange(" + tbDayOff4.ClientID + ",'" + starDate + "','" + endDate + "','" + ddlMonth.SelectedItem.Text + "'," + ddlYear.ClientID + ")");
                
                loadDDLLocations(ddlArea.SelectedValue, ddlLocation);
                loadDDLStandBy(ddlStandByEmployee);
                //set the location
                ddlLocation.SelectedValue = lblLocationID.Text;
                //set standby
                ddlStandByEmployee.SelectedValue = lblStandyByEmployeeID.Text;

                //Set the checks
                Label lblIsComplete = (Label)e.Row.FindControl("lblIsComplete");
                Label lblExclude = (Label)e.Row.FindControl("lblExclude");
                CheckBox cbComplete = (CheckBox)e.Row.FindControl("cbComplete");
                CheckBox cbExclude = (CheckBox)e.Row.FindControl("cbExclude");

                cbComplete.Checked = lblIsComplete.Text == "False" ? false : true;
                cbExclude.Checked = lblExclude.Text == "False" ? false : true;

                //set the day offs
                Label lblWorkScheduleEmployeeID = (Label)e.Row.FindControl("lblWorkScheduleEmployeeID");
                if (lblWorkScheduleEmployeeID.Text != "0")
                {
                    Int64 wseID = Convert.ToInt64(lblWorkScheduleEmployeeID.Text);
                    DataTable dt = WORKSCHEDULE.sp_Employee_SelectDayOffs(wseID);
                    if ((dt.Rows.Count > 0) && (dt != null))
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i == 0)
                            {
                                tbDayOff1.Text = dt.Rows[i]["Date"].ToString();
                            }
                            else if (i == 1)
                            {
                                tbDayOff2.Text = dt.Rows[i]["Date"].ToString();
                            }
                            else if (i == 2)
                            {
                                tbDayOff3.Text = dt.Rows[i]["Date"].ToString();
                            }
                            else
                            {
                                tbDayOff4.Text = dt.Rows[i]["Date"].ToString();
                            }
                        }
                    }
                }
                
                Label lblEmployeeID = (Label)e.Row.FindControl("lblEmployeeID");
                int empID = Convert.ToInt32(lblEmployeeID.Text);
                if (illegalIDs.Contains(empID))
                {
                    Label lblFullName = (Label)e.Row.FindControl("lblFullName");
                    lblFullName.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void lbGenerate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (rbCreate.Checked)
                {
                    if (duplicateCheck_WorkSchedule(ddlYear.SelectedValue, ddlMonth.SelectedValue, Convert.ToByte(ddlArea.SelectedValue), Convert.ToByte(ddlShift.SelectedValue)))
                    {
                        lblMessage.InnerText = "A work schedule has already been created for " + ddlMonth.SelectedItem.Text + ", " + ddlYear.SelectedItem.Text + " for " + ddlArea.SelectedItem.Text + " (Shift -"  + ddlShift.SelectedItem.Text + "). Kindly click on an option below to proceed: ";
                        mpeDupWorkSchedule.Show();
                    }
                    else
                        loadEmployees_ByShift(ddlArea.SelectedValue, ddlShift.SelectedValue);
                }
                else
                {
                    if (duplicateCheck_WorkSchedule(ddlYear.SelectedValue, ddlMonth.SelectedValue, Convert.ToByte(ddlArea.SelectedValue), Convert.ToByte(ddlShift.SelectedValue)))
                    {
                        loadEmployees_ByShift(ddlArea.SelectedValue, ddlShift.SelectedValue);
                    }
                    else
                        ShowMessage("A schedule has not been created for the selected parameters", MessageType.Info);
                }
            }
        }

        public bool duplicateCheck_WorkSchedule(string year, string month, byte areaID, byte shiftID)
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


        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void clearFields()
        {
            loadYear();
            //ddlYear.SelectedIndex = 0;
            ddlArea.SelectedIndex = 0;
            ddlShift.SelectedIndex = 0;
            workScheduleID = 0;
            isLoaded = false;
            gvEmployeesSchedule.Visible = false;
            lblemployeesFound.Text = "";
            setCurrentMonth();
            btnSave.Visible = false;
            btnSave2.Visible = false;
           btnUnscheduledEmps.Visible = false;
           btnUnscheduledEmps2.Visible = false;
           lblAuditTrail.Visible = false;
            illegalIDs = new List<int>();
        }

        protected void rbCreate_CheckedChanged(object sender, EventArgs e)
        {
            clearFields();
        }

        protected void rbModify_CheckedChanged(object sender, EventArgs e)
        {
            clearFields();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                byte areaID = new byte();
                if (ddlArea.SelectedIndex > 0)
                    areaID = Convert.ToByte(ddlArea.SelectedValue);

                byte shiftID = new byte();
                if (ddlShift.SelectedIndex > 0)
                    shiftID = Convert.ToByte(ddlShift.SelectedValue);

                if (isLoaded)
                {
                    WORKSCHEDULE.update_WorkSchedule(workScheduleID, ddlYear.SelectedValue, ddlMonth.SelectedValue, gvEmployeesSchedule, ddlArea.SelectedItem.Text, ddlShift.SelectedItem.Text, userID);
                    if (WORKSCHEDULE.successfulCommit)
                    {
                        ShowMessage("Record saved.", MessageType.Success);
                    }
                    else
                    {
                        ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                }
                else
                {
                    workScheduleID = WORKSCHEDULE.insert_WorkSchedule(ddlYear.SelectedValue, ddlMonth.SelectedValue, areaID, shiftID, gvEmployeesSchedule, ddlArea.SelectedItem.Text, ddlShift.SelectedItem.Text, userID);
                    if (WORKSCHEDULE.successfulCommit)
                    {
                        ShowMessage("Record saved.", MessageType.Success);
                        rbCreate.Checked = false;
                        rbModify.Checked = true;
                        loadEmployees_ByShift(ddlArea.SelectedValue, ddlShift.SelectedValue);
                        isLoaded = true;
                    }
                    else
                    {
                        ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                        isLoaded = false;
                    }
                }
            }
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
            gvEmployeesSchedule.Visible = false;
            lblemployeesFound.Text = "";
            btnSave.Visible = false;
            btnSave2.Visible = false;
            btnUnscheduledEmps.Visible = false;
            btnUnscheduledEmps2.Visible = false;
            isLoaded = false;
            lblAuditTrail.Visible = false;
        }

        protected void btnCreateNewSchedule_Click(object sender, EventArgs e)
        {
            mpeDupWorkSchedule.Hide();
            clearFields();
        }

        protected void btnviewThisSchedule_Click(object sender, EventArgs e)
        {
            mpeDupWorkSchedule.Hide();
            rbCreate.Checked = false;
            rbModify.Checked = true;
            loadEmployees_ByShift(ddlArea.SelectedValue, ddlShift.SelectedValue);
            isLoaded = true;
        }

        protected void setAuditTrail()
        {
            DataTable dt = WORKSCHEDULE.sp_WorkSchedule_AuditSelect(workScheduleID);
            if (dt.Rows.Count > 0)
            {
                lblAuditTrail.Text = "Created By: " + dt.Rows[0]["CreatedBy"].ToString() + " Created On: " + dt.Rows[0]["CreatedOn"].ToString() + " Last Modified By: " + dt.Rows[0]["LastModifiedBy"].ToString() + " Last Modified On: " + dt.Rows[0]["LastModifiedOn"].ToString();
                lblAuditTrail.Visible = true;
            }
        }

        protected void btnUnscheduledEmps_Click(object sender, EventArgs e)
        {
            mpeUnscheduledEmps.Show();
        }

        private void loadUnscheduledEmployees(object year, object month, object areaID, object shiftID)
        {
            DataTable dt = WORKSCHEDULE.sp_Employee_UnscheduledSelectForWorkSchedule(workScheduleID, areaID, shiftID);
            if (WORKSCHEDULE.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvUnscheduledEmps.DataSource = dt;
                    gvUnscheduledEmps.DataBind();
                    gvUnscheduledEmps.Visible = true;
                    btnUnscheduledEmps.Visible = true;
                    btnUnscheduledEmps2.Visible = true;
                    btnUnscheduledEmps.Text = "Unscheduled Employees: " + dt.Rows.Count;
                    btnUnscheduledEmps2.Text = "Unscheduled Employees: " + dt.Rows.Count;
                }
                else
                {
                    gvUnscheduledEmps.Visible = false;
                    btnUnscheduledEmps.Visible = false;
                    btnUnscheduledEmps2.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and unscheduled employees were not selected. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvUnscheduledEmps.Visible = false;
                btnUnscheduledEmps.Visible = false;
                btnUnscheduledEmps2.Visible = false;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            mpeUnscheduledEmps.Hide();
        }

        protected void btnAppend_Click(object sender, EventArgs e)
        {
            int count = 0;
            byte areaID = new byte();
            if (ddlArea.SelectedIndex > 0)
                areaID = Convert.ToByte(ddlArea.SelectedValue);

            byte shiftID = new byte();
            if (ddlShift.SelectedIndex > 0)
                shiftID = Convert.ToByte(ddlShift.SelectedValue);

            if (isLoaded)
            {
                count = WORKSCHEDULE.insert_UnscheduledWorkSchedule(workScheduleID, ddlYear.SelectedValue, ddlMonth.SelectedValue, areaID, shiftID, gvUnscheduledEmps, ddlArea.SelectedItem.Text, ddlShift.SelectedItem.Text, userID);
                if ((WORKSCHEDULE.successfulCommit) && (count > 0))
                {
                    loadEmployees_ByShift(ddlArea.SelectedValue, ddlShift.SelectedValue);
                    ShowMessage("Employees appended.", MessageType.Success);
                    mpeUnscheduledEmps.Hide();
                }
                else if (WORKSCHEDULE.successfulCommit == false)
                {
                    ShowMessage("Something went wrong and the employees were not appended. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    mpeUnscheduledEmps.Show();
                }
                else if ((WORKSCHEDULE.successfulCommit) && (count == 0))
                {
                    ShowMessage("No employees are selected. You have to select an employee before they can be appended.", MessageType.Info);
                    mpeUnscheduledEmps.Show();
                }
            }
        }

        protected void gvEmployeesSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((gvEmployeesSchedule.SelectedIndex > -1) && (!isBeforeCurrentMonth(ddlMonth.SelectedValue, ddlYear.SelectedValue)))
            {
                Label lblEmployeeID = (Label)gvEmployeesSchedule.SelectedRow.FindControl("lblEmployeeID");
                Label lblFullName = (Label)gvEmployeesSchedule.SelectedRow.FindControl("lblFullName");
                lblEmpName.InnerText = lblFullName.Text;
                lblEmpID.InnerText = lblEmployeeID.Text;
                ddlTransferFor.SelectedIndex = 0;
                loadDDLTransferArea();
                loadDDLTransferShifts();
                trLocation.Visible = false;
                mpeEmployeeTransfer.Show();
            }
        }

        protected void btnCancel2_Click(object sender, EventArgs e)
        {
            mpeEmployeeTransfer.Hide();
        }

        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            if ((ddlTransferArea.SelectedIndex > 0) || (ddlTransferShift.SelectedIndex > 0))
            {
                    byte areaID = 0;
                    Int16 locationID = 0;
                    if (ddlTransferArea.SelectedIndex > 0)
                    {
                        areaID = Convert.ToByte(ddlTransferArea.SelectedValue);
                        locationID = Convert.ToInt16(ddlTransferLocation.SelectedValue);
                    }
                    else
                        areaID = Convert.ToByte(ddlArea.SelectedValue);

                    byte shiftID = 0;
                    if (ddlTransferShift.SelectedIndex > 0)
                        shiftID = Convert.ToByte(ddlTransferShift.SelectedValue);
                    else
                        shiftID = Convert.ToByte(ddlShift.SelectedValue);

                    var model = new EDM.DataSource();
                    var WS = (from a in model.tbl_Mgr_WorkSchedule
                              where a.AreaID == areaID && a.ShiftID == shiftID && a.Year == ddlYear.SelectedValue && a.Month == ddlMonth.SelectedValue
                              select new
                              {
                                  a.WorkScheduleID
                              }).ToList();

                    if (WS.Count > 0)
                    {
                        int newWSID = 0;
                        foreach (var val in WS)
                            newWSID = val.WorkScheduleID;

                        WORKSCHEDULE.transferEmployee(workScheduleID, newWSID, areaID, shiftID, ddlYear.SelectedValue.ToString(), ddlMonth.SelectedValue.ToString(), ddlTransferFor.SelectedValue.ToString(), Convert.ToInt16(lblEmpID.InnerText), locationID, ddlArea.SelectedItem.Text, ddlShift.SelectedItem.Text, ddlTransferArea.SelectedItem.Text, ddlTransferShift.SelectedItem.Text, lblEmpName.InnerText, tbTransferComments.Text, userID);
                        if (WORKSCHEDULE.successfulCommit)
                        {
                            ShowMessage("Employee transferred.", MessageType.Success);
                            loadEmployees_ByShift(ddlArea.SelectedValue, ddlShift.SelectedValue);
                            mpeEmployeeTransfer.Hide();
                        }
                        else
                        {
                            ShowMessage("Something went wrong and the employee was not transferred. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                            mpeEmployeeTransfer.Show();
                        }
                    }
                    else
                    {
                        ShowMessage("The work schedule Area or Shift you are trying to transfer the employee to has not been created. Kindly create the work schedule first and try the transfer again.", MessageType.Warning);
                        mpeEmployeeTransfer.Show();
                    }
            }
            else
            {
                ShowMessage("Either the area or shift has to be selected before the employee is transferred", MessageType.Warning);
                mpeEmployeeTransfer.Show();
            }
        }

        //populate the location ddl based on the area selected
        private void loadDDLTransferLocations(byte AreaID)
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Locations
                           where a.IsActive == true && a.AreaID == AreaID
                           select new
                           {
                               a.LocationName,
                               a.LocationID
                           }).OrderBy(LocationName => LocationName).ToList();
            ddlTransferLocation.DataSource = results;
            ddlTransferLocation.DataTextField = "LocationName";
            ddlTransferLocation.DataValueField = "LocationID";
            ddlTransferLocation.DataBind();
        }

        protected void ddlTransferArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTransferArea.SelectedIndex > 0)
            {
                trLocation.Visible = true;
                loadDDLTransferLocations(Convert.ToByte(ddlTransferArea.SelectedValue));
                ddlTransferLocation.Attributes.Add("onblur", "javascript:verifyLocation(" + ddlTransferArea.ClientID + "," + ddlTransferLocation.ClientID + ")");
                mpeEmployeeTransfer.Show();
            }
            else
            {
                trLocation.Visible = false;
                mpeEmployeeTransfer.Show();
            }
        }

        protected void gvUnscheduledEmps_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlLocation = (DropDownList)e.Row.FindControl("ddlLocation");
                Label lblLocationID = (Label)e.Row.FindControl("lblLocationID");
               
                loadDDLLocations(ddlArea.SelectedValue, ddlLocation);
                //set the location
                ddlLocation.SelectedValue = lblLocationID.Text;
            }
        }

        private bool isBeforeCurrentMonth(string month, string year)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            if ((Convert.ToInt32(month) < currentMonth) && (Convert.ToInt32(year) == currentYear))
               return true;
            else if ((Convert.ToInt32(month) < currentMonth) && (Convert.ToInt32(year) > currentYear))
                return false;
            else
                return false;
        }

        private void loadIllegalEmployeesDetails()
        {
            DateTime FromTime = DateTime.Now;
            DateTime dtStart = FromTime.AddDays(-6);
            illegalIDs = WORKSCHEDULE.sp_Alerts_Attendance_IllegalEmployees(dtStart, FromTime, ddlArea.SelectedValue, ddlShift.SelectedValue, 6, FromTime);
        }

        protected void lbView_Click(object sender, EventArgs e)
        {
            if (ddlArea.SelectedIndex > 0)
            {
                this.rvWorkSchedule.ProcessingMode = ProcessingMode.Local;

                ISSA.Pages.Reports.DataSet1 DS_Report = new ISSA.Pages.Reports.DataSet1();
                DS_Report.EnforceConstraints = false;

                ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WorkScheduleTableAdapter Rpt_TableAdapter = new ISSA.Pages.Reports.DataSet1TableAdapters.TA_RPT_WorkScheduleTableAdapter();
                Rpt_TableAdapter.Fill(DS_Report.TA_RPT_WorkSchedule, ddlYear.SelectedValue, ddlMonth.SelectedValue, Convert.ToByte(ddlArea.SelectedValue), ddlShift.SelectedValue);

                if (DS_Report.TA_RPT_WorkSchedule.Rows.Count > 0)
                {
                    ISSA.Pages.Reports.DataSet1.dtParametersDataTable dtParam = new ISSA.Pages.Reports.DataSet1.dtParametersDataTable();

                    DataRow dr = dtParam.NewRow();
                    dr["ParamOne"] = ddlMonth.SelectedItem.Text + ", " + ddlYear.SelectedValue;
                    dr["ParamTwo"] = ddlArea.SelectedItem.Text;
                    if (ddlShift.SelectedIndex == 0)
                        dr["ParamThree"] = "All";
                    else
                        dr["ParamThree"] = ddlShift.SelectedItem.Text;
                    dr["User"] = MISC.getUserName().ToUpper(); ;
                    dr["Date"] = DateTime.Now;
                    dtParam.Rows.Add(dr);

                    Microsoft.Reporting.WebForms.ReportDataSource RpDs1 = new Microsoft.Reporting.WebForms.ReportDataSource();
                    Microsoft.Reporting.WebForms.ReportDataSource RpDsParameters = new Microsoft.Reporting.WebForms.ReportDataSource();

                    RpDs1.Name = "TA_RPT_WorkSchedule";
                    RpDs1.Value = DS_Report.TA_RPT_WorkSchedule;
                    RpDsParameters.Name = "dtParameters";
                    RpDsParameters.Value = dtParam;

                    rvWorkSchedule.LocalReport.ReportPath = "Pages/Reports/rpt_WorkSchedule.rdlc";
                    rvWorkSchedule.LocalReport.DataSources.Clear();
                    rvWorkSchedule.LocalReport.DataSources.Add(RpDs1);
                    rvWorkSchedule.LocalReport.DataSources.Add(RpDsParameters);
                    rvWorkSchedule.LocalReport.Refresh();
                    rvWorkSchedule.Visible = true;
                    rvWorkSchedule.ShowReportBody = true;
                    mpeViewWorkSchedule.Show();
                }
                else
                {
                    ShowMessage("No record found for selected parameters. Schedule may not have been created.", MessageType.Info);
                    mpeViewWorkSchedule.Hide();
                }
            }
            else
            {
                ShowMessage("Area is required", MessageType.Warning);
                //mpeViewWorkSchedule.Hide();
            }
        }

        protected void btnCancelWS_Click(object sender, EventArgs e)
        {
            mpeViewWorkSchedule.Hide();
        }
    }
}