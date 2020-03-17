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

namespace ISSA.Pages.Operations
{
    public partial class Incidents : System.Web.UI.Page
    {
        BusinessTier.Incident INCIDENT = new BusinessTier.Incident();
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

        public int incidentID
        {
            get
            {
                return (int)ViewState["incidentID"];
            }
            set
            {
                ViewState["incidentID"] = value;
            }
        }

        public int personAffectedID
        {
            get
            {
                return (int)ViewState["personAffectedID"];
            }
            set
            {
                ViewState["personAffectedID"] = value;
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
                    if ((roleNames.Contains("Manager")) || roleNames.Contains("Contract Compliance Officer"))
                    {
                        //Load for manager and compliance office
                        pnlIncidentSearch.Visible = true;
                        pnlIncidentDetails.Visible = false;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        pageLoad();
                        tbSearchDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbSearchDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbDate.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDate.ClientID + ",'" + DateTime.Now.Date + "')");
                        //Check if a query string is in the url
                        object incNumber = Request.QueryString["incNumber"];
                        if (incNumber != null)
                        {
                            tbSearchReportNumber.Text = incNumber.ToString();
                            loadSearchedIncidents();
                        }
                    }
                    else if (roleNames.Contains("Operator"))
                    {
                        //load for operator
                        pnlIncidentSearch.Visible = false;
                        pnlIncidentDetails.Visible = true;
                        btnBackToSearch.Visible = false;
                        btnBackToSearch2.Visible = false;
                        pageLoad();
                        //show gridview with persons affected
                        loadPersonsAffected(incidentID);
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
            tbTime.Attributes.Add("onblur", "javascript:validateTime(" + tbTime.ClientID + ")");
            resetForm();
            //set the validation for the other text boxes
            tbOther.Attributes.Add("onblur", "javascript:verifyIncidentOther(" + ddlIncidentType.ClientID + "," + tbOther.ClientID + ")");
            tbOtherPersonAffectedType.Attributes.Add("onblur", "javascript:verifyPersonAffectedOther(" + ddlPersonAffectedType.ClientID + "," + tbOtherPersonAffectedType.ClientID + ")");
            gvIncidents.Visible = false;
            lblGVIncidentsHeader.Text = "";
        }

        private void loadDDLShifts()
        {
            DataTable dt = MISC.sp_Shift_Sel4DDL_ByPositionID(1);
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "shift";
            ddlShift.DataValueField = "ShiftID";
            ddlShift.DataBind();
        }

        //populate the Area ddl
        private void loadDDLArea()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_Areas
                           where a.IsActive == true
                           select new
                           {
                               a.Seq,
                               a.Area,
                               a.AreaID
                           }).OrderBy(Seq => Seq).ToList();
            ddlArea.DataSource = results;
            ddlArea.DataTextField = "Area";
            ddlArea.DataValueField = "AreaID";
            ddlArea.DataBind();
        }

        //populate the Area ddl
        private void loadDDLSearchArea()
        {
            DataTable dt = MISC.sp_Area_Sel4DDL();
            ddlSearchArea.DataSource = dt;
            ddlSearchArea.DataTextField = "Area";
            ddlSearchArea.DataValueField = "AreaID";
            ddlSearchArea.DataBind();
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

        //populate the location ddl based on the area selected
        private void loadDDLSearchLocations(object AreaID)
        {
            DataTable dt = MISC.sp_Location_Sel4DDL_ByAreaID(AreaID);
            ddlSearchLocation.DataSource = dt;
            ddlSearchLocation.DataTextField = "LocationName";
            ddlSearchLocation.DataValueField = "LocationID";
            ddlSearchLocation.DataBind();
        }

        //populate the incident type
        private void loadDDLIncidentTypes()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_IncidentTypes
                           where a.IsActive == true
                           select new
                           {
                               a.Seq,
                               a.IncidentType,
                               a.IncidentTypeID
                           }).OrderBy(Seq => Seq).ToList();
            ddlIncidentType.DataSource = results;
            ddlIncidentType.DataTextField = "IncidentType";
            ddlIncidentType.DataValueField = "IncidentTypeID";
            ddlIncidentType.DataBind();
        }

        //populate the search incident type ddl
        private void loadDDLSearchIncidentTypes()
        {
            DataTable dt = INCIDENT.sp_IncidentType_Sel4DDL();
            ddlSearchIncidentType.DataSource = dt;
            ddlSearchIncidentType.DataTextField = "IncidentType";
            ddlSearchIncidentType.DataValueField = "IncidentTypeID";
            ddlSearchIncidentType.DataBind();
        }

        //populate the affected person type ddl
        private void loadDDLPersonAffectedTypes()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Ref_IncidentPersonAffectedType
                           where a.IsActive == true
                           select new
                           {
                               a.Seq,
                               a.TypeName,
                               a.AffectedTypeID
                           }).OrderBy(Seq => Seq).ToList();
            ddlPersonAffectedType.DataSource = results;
            ddlPersonAffectedType.DataTextField = "TypeName";
            ddlPersonAffectedType.DataValueField = "AffectedTypeID";
            ddlPersonAffectedType.DataBind();
        }

        //reload the location ddl on the area ddl index change
        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
        }

        protected void ddlSearchArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchArea.SelectedIndex > 0)
                loadDDLSearchLocations(Convert.ToByte(ddlSearchArea.SelectedValue));
            else
                loadDDLSearchLocations(null);
        }

        //show hide the other boc and label when the other incident type is selected
        protected void ddlIncidentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlIncidentType.SelectedItem.Text == "Other")
            {
                lblOtherIncidentType.Visible = true;
                tbOther.Visible = true;
                tbOther.Focus();
            }
            else
            {
                lblOtherIncidentType.Visible = false;
                tbOther.Visible = false;
                tbOther.Text = "";
            }
        }

        //Load the persons affected based on the incidentid
        private void loadPersonsAffected(int incidentID)
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_IncidentPersonsAffected
                           join b in model.tbl_Ref_IncidentPersonAffectedType on a.AffectedTypeID equals b.AffectedTypeID
                           where a.IncidentID == incidentID
                           select new
                           {
                               personAffectedID = a.PersonAffectedID,
                               fullName = a.FirstName.Length > 0 ? a.LastName + ", " + a.FirstName : a.LastName,
                               phoneNumber = a.PhoneNumber,
                               address = a.Address,
                               description = a.Description,
                               affectedType = b.TypeName == "Other" ? b.TypeName + " - " + a.OtherTypeDescription : b.TypeName,
                               firstName = a.FirstName,
                               lastName = a.LastName,
                               affectedTypeID = a.AffectedTypeID,
                               otherTypeDescription = a.OtherTypeDescription
                           }).OrderBy(name => name).ToList();

            if (results.Count > 0)
            {
                gvPersonsAffected.DataSource = results;
                gvPersonsAffected.DataBind();
                gvPersonsAffected.Visible = true;

            }
            else
            {
                gvPersonsAffected.Visible = false;
            }
        }

        //select the person
        protected void gvPersonsAffected_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if selected index has been changed
            if (gvPersonsAffected.SelectedIndex > -1)
            {
                resetPersonsAffected();
                Label lblPersonAffectedID = (Label)gvPersonsAffected.SelectedRow.FindControl("lblPersonAffectedID");
                Label lblFirstName = (Label)gvPersonsAffected.SelectedRow.FindControl("lblFirstName");
                Label lblLastName = (Label)gvPersonsAffected.SelectedRow.FindControl("lblLastName");
                Label lblPhoneNumber = (Label)gvPersonsAffected.SelectedRow.FindControl("lblPhoneNumber");
                Label lblAddress = (Label)gvPersonsAffected.SelectedRow.FindControl("lblAddress");
                Label lblDescription = (Label)gvPersonsAffected.SelectedRow.FindControl("lblDescription");
                Label lblAffectedTypeID = (Label)gvPersonsAffected.SelectedRow.FindControl("lblAffectedTypeID");
                Label lblOtherPersonAffectedTyp = (Label)gvPersonsAffected.SelectedRow.FindControl("lblOtherPersonAffectedType");

                tbFirstName.Text = lblFirstName.Text;
                tbLastName.Text = lblLastName.Text;
                ddlPersonAffectedType.SelectedValue = lblAffectedTypeID.Text;
                tbPhone.Text = lblPhoneNumber.Text;
                tbPersonDescription.Text = lblDescription.Text;
                tbOtherPersonAffectedType.Text = lblOtherPersonAffectedTyp.Text;
                tbAddress.Text = lblAddress.Text;
                personAffectedID = lblPersonAffectedID.Text != "" ? Convert.ToInt16(lblPersonAffectedID.Text) : 0;

                if (ddlPersonAffectedType.SelectedItem.Text == "Other")
                {
                    lblOtherPersonAffectedType.Visible = true;
                    tbOtherPersonAffectedType.Visible = true;
                }
                else
                {
                    lblOtherPersonAffectedType.Visible = false;
                    tbOtherPersonAffectedType.Visible = false;
                }
                showPersonAffectedModal();
            }
        }

        private void showPersonAffectedModal()
        {
            //mpePersonAffected.Show();
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#personAffectedModal').modal('toggle');</script>", false);
        }

        private void hidePersonAffectedModal()
        {
            //mpePersonAffected.Hide();
            ClientScript.RegisterStartupScript(GetType(), "Hide", "<script> jQuery.noConflict(); $('#personAffectedModal').modal('hide');</script>", false);
        }

        //show the person affected popup
        protected void btnAddNewPersonAffected_Click(object sender, EventArgs e)
        {
            resetPersonsAffected();
            showPersonAffectedModal();
        }

        //show hide the other fields on the other person affected type
        protected void ddlPersonAffectedType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPersonAffectedType.SelectedItem.Text == "Other")
            {
                lblOtherPersonAffectedType.Visible = true;
                tbOtherPersonAffectedType.Visible = true;
                tbOtherPersonAffectedType.Focus();
                showPersonAffectedModal();
            }
            else
            {
                lblOtherPersonAffectedType.Visible = false;
                tbOtherPersonAffectedType.Visible = false;
                tbOtherPersonAffectedType.Text = "";
                showPersonAffectedModal();
            }
        }

        //clear the person affected fields
        private void resetPersonsAffected()
        {
            tbFirstName.Text = "";
            tbLastName.Text = "";
            ddlPersonAffectedType.SelectedIndex = 0;
            tbAddress.Text = "";
            tbPersonDescription.Text = "";
            tbOtherPersonAffectedType.Text = "";
            tbOtherPersonAffectedType.Visible = false;
            lblOtherPersonAffectedType.Visible = false;
            tbPhone.Text = "";
            personAffectedID = 0;
        }

        //clear the entire form
        private void resetForm()
        {
            loadDDLSearchArea();
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            loadDDLSearchIncidentTypes();
            loadDDLArea();
            loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
            loadDDLIncidentTypes();
            loadDDLPersonAffectedTypes();
            loadDDLShifts();

            //Set the current date time in the fields
            DateTime currentDatetime = DateTime.Now;
            string dateNow = currentDatetime.Date.ToString("MM'/'dd'/'yyyy");
            string timeNow = currentDatetime.TimeOfDay.ToString();
            dateNow = dateNow.Substring(0, 10);
            tbDate.Text = dateNow;
            tbTime.Text = timeNow.Substring(0, 5);
            ddlIncidentType.SelectedIndex = 0;
            tbDescription.Text = "";
            tbActionTaken.Text = "";
            gvPersonsAffected.Visible = false;
            isLoaded = false;
            resetPersonsAffected();
            incidentID = 0;
            tbOther.Text = "";
            tbOther.Visible = false;
            lblOtherIncidentType.Visible = false;
            generateIncidentNumber();
            lblAuditTrail.Text = "";
            lblHeader.Text = "Add a new incident report below";
            ddlShift.SelectedIndex = 0;
           
            //Datetime grant
            DateTime dtNow = DateTime.Now;
            if ((MISC.checkGrantStatus(dtNow, dtNow)) || (roleNames.Contains("Manager")))
            {
                ibCalendar.Enabled = true;
                tbTime.Enabled = true;
                tbDate.Enabled = true;
            }
            else
            {
                ibCalendar.Enabled = false;
                tbTime.Enabled = false;
                tbDate.Enabled = false;
            }
        }

        //generate the incident number
        private void generateIncidentNumber()
        {
            DateTime now = DateTime.Now;
            Int32 year = now.Year;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_Incidents
                           where a.DateOccured.Value.Year == year
                           select new
                           {
                               a.IncidentID
                           }).ToList();
            int currentSequence = Convert.ToInt32(results.Count) + 1;
            if (currentSequence < 10)
            {
                lblReportNumber.Text = "INC" + year.ToString() + "0" + currentSequence.ToString();
            }
            else
            {
                lblReportNumber.Text = "INC" + year.ToString() + currentSequence.ToString();
            }
        }

        //clear the form 
        protected void btnClear_Click(object sender, EventArgs e)
        {
            resetForm();
        }

        //function to save the incident, either a new insert or an update
        private bool saveIncident()
        {
            bool success = false;

            if (isLoaded)
            {
                try
                {
                    //Update incident
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        TimeSpan time = new TimeSpan();
                        if (TimeSpan.TryParse(tbTime.Text, out time))
                        {
                            tbTime.BorderColor = System.Drawing.Color.LightGray;
                            tbDate.BorderColor = System.Drawing.Color.LightGray;
                            date = date.Date;

                            INCIDENT.update_Incident(incidentID, Convert.ToByte(ddlArea.SelectedValue), Convert.ToInt16(ddlLocation.SelectedValue), date, time, Convert.ToByte(ddlIncidentType.SelectedValue), tbOther.Text, tbDescription.Text, lblReportNumber.Text, userID, Convert.ToByte(ddlShift.SelectedValue), tbActionTaken.Text);
                            if (INCIDENT.successfulCommit)
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
                            success = false;
                            ShowMessage("Incorrect time format.", MessageType.Warning);
                            tbTime.BorderColor = System.Drawing.Color.Red;
                            tbTime.Focus();
                        }
                        return success;
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Warning);
                        tbDate.BorderColor = System.Drawing.Color.Red;
                        tbDate.Focus();
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
                    //insert incident
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(tbDate.Text, out date))
                    {
                        TimeSpan time = new TimeSpan();
                        if (TimeSpan.TryParse(tbTime.Text, out time))
                        {
                            tbTime.BorderColor = System.Drawing.Color.LightGray;
                            tbDate.BorderColor = System.Drawing.Color.LightGray;
                            date = date.Date;

                            //regenerate the number in case another record was added
                            generateIncidentNumber();

                            incidentID = INCIDENT.insert_Incident(Convert.ToByte(ddlArea.SelectedValue), Convert.ToInt16(ddlLocation.SelectedValue), date, time, Convert.ToByte(ddlIncidentType.SelectedValue), tbOther.Text, tbDescription.Text, lblReportNumber.Text, userID, Convert.ToByte(ddlShift.SelectedValue), tbActionTaken.Text);
                            if (incidentID != 0)
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
                            success = false;
                            ShowMessage("Incorrect time format.", MessageType.Warning);
                            tbTime.BorderColor = System.Drawing.Color.Red;
                            tbTime.Focus();
                        }
                        return success;
                    }
                    else
                    {
                        ShowMessage("Incorrect date format.", MessageType.Warning);
                        tbDate.BorderColor = System.Drawing.Color.Red;
                        tbDate.Focus();
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveIncident();
                if (success)
                {
                    ShowMessage("Record saved.", MessageType.Success);
                }
            }
        }

        protected void btnSaveAndNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = saveIncident();
                if (success)
                {
                    ShowMessage("Record saved.", MessageType.Success);
                    resetForm();
                }
            }
        }

        protected void btnSaveNextPersonAffected_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //update
                savePersonAffected();
                showPersonAffectedModal();
            }
            else
                showPersonAffectedModal();
        }

        protected void btnSaveClosePersonAffected_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //update
                savePersonAffected();
                hidePersonAffectedModal();
            }
            else
                showPersonAffectedModal();
        }

        private void savePersonAffected()
        {
            if (personAffectedID != 0)
            {
                try
                {
                    INCIDENT.update_PersonAffected(personAffectedID, Convert.ToByte(ddlPersonAffectedType.SelectedValue), tbFirstName.Text, tbLastName.Text, tbPersonDescription.Text, tbAddress.Text, tbOtherPersonAffectedType.Text, tbPhone.Text, userID, lblReportNumber.Text, incidentID);
                    if (INCIDENT.successfulCommit)
                    {
                        loadPersonsAffected(incidentID);
                        resetPersonsAffected();
                        showPersonAffectedModal();
                    }
                    else
                    {
                        ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            //insert
            else
            {
                try
                {
                    bool success = false;
                    if (!isLoaded)
                    {
                        success = saveIncident();
                        if (success && incidentID != 0)
                        {
                            INCIDENT.insert_IncidentPersonAffected(Convert.ToByte(ddlPersonAffectedType.SelectedValue), tbFirstName.Text, tbLastName.Text, tbPersonDescription.Text, tbAddress.Text, tbOtherPersonAffectedType.Text, incidentID, tbPhone.Text, userID);
                            if (INCIDENT.successfulCommit)
                            {
                                loadPersonsAffected(incidentID);
                                resetPersonsAffected();
                                showPersonAffectedModal();
                            }
                            else
                            {
                                INCIDENT.delete_Incident(incidentID);
                                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                            }
                        }
                    }
                    else
                    {
                        bool isDuplicate = INCIDENT.duplicateCheck_personsAffected(tbFirstName.Text, tbLastName.Text, incidentID);
                        if (!isDuplicate)
                        {
                            INCIDENT.insert_IncidentPersonAffected(Convert.ToByte(ddlPersonAffectedType.SelectedValue), tbFirstName.Text, tbLastName.Text, tbPersonDescription.Text, tbAddress.Text, tbOtherPersonAffectedType.Text, incidentID, tbPhone.Text, userID);
                            if (INCIDENT.successfulCommit)
                            {
                                loadPersonsAffected(incidentID);
                                resetPersonsAffected();
                                showPersonAffectedModal();
                            }
                            else
                            {
                                ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                            }
                        }
                        else
                        {
                            ShowMessage("This person cannot be added as he/she has already been added!! Kindly check the Persons Affected list to verify.", MessageType.Error);
                        }
                    }
                }
                catch (System.Exception excec)
                {
                    MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        protected void lbClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchReportNumber.Text = "";
            tbSearchDate.Text = "";
            ddlSearchArea.SelectedIndex = 0;
            ddlSearchIncidentType.SelectedIndex = 0;
            object AreaID = null;
            if (ddlSearchArea.SelectedIndex > 0)
                AreaID = Convert.ToByte(ddlSearchArea.SelectedValue);
            loadDDLSearchLocations(AreaID);
            gvIncidents.Visible = false;
            lblGVIncidentsHeader.Text = "";
        }

        //Load the persons affected based on the incidentid
        private void loadIncidents(string reportNumber, DateTime date, byte areaID, byte locationID)
        {
            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_IncidentPersonsAffected
                           join b in model.tbl_Ref_IncidentPersonAffectedType on a.AffectedTypeID equals b.AffectedTypeID
                           where a.IncidentID == incidentID
                           select new
                           {
                               personAffectedID = a.PersonAffectedID,
                               fullName = a.FirstName.Length > 0 ? a.LastName + ", " + a.FirstName : a.LastName,
                               phoneNumber = a.PhoneNumber,
                               address = a.Address,
                               description = a.Description,
                               affectedType = b.TypeName == "Other" ? b.TypeName + " - " + a.OtherTypeDescription : b.TypeName,
                               firstName = a.FirstName,
                               lastName = a.LastName,
                               affectedTypeID = a.AffectedTypeID,
                               otherTypeDescription = a.OtherTypeDescription
                           }).OrderBy(name => name).ToList();

            if (results.Count > 0)
            {
                gvPersonsAffected.DataSource = results;
                gvPersonsAffected.DataBind();
                gvPersonsAffected.Visible = true;

            }
            else
            {
                gvPersonsAffected.Visible = false;
            }
        }


        protected void btnSearchIncident_Click(object sender, EventArgs e)
        {
            loadSearchedIncidents();
        }

        private void loadSearchedIncidents()
        {
            DataTable dt = INCIDENT.sp_Incidents_Search(tbSearchReportNumber.Text == "" ? null : tbSearchReportNumber.Text, tbSearchDate.Text == "" ? null : tbSearchDate.Text, ddlSearchArea.SelectedValue, ddlSearchLocation.SelectedValue, ddlSearchIncidentType.SelectedValue);
            if (INCIDENT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvIncidents.DataSource = dt;
                    gvIncidents.DataBind();
                    gvIncidents.Visible = true;
                    lblGVIncidentsHeader.Text = "Incident Reports found: " + dt.Rows.Count;
                }
                else
                {
                    gvIncidents.Visible = false;
                    lblGVIncidentsHeader.Text = "No incident reports found.";
                }
            }
            else
            {
                ShowMessage("Something went wrong and the search was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvIncidents.Visible = false;
                lblGVIncidentsHeader.Text = "No reports found due to error.";
            }
        }

        protected void btnNewIncident_Click(object sender, EventArgs e)
        {
            lblHeader.Text = "Add a new incident report below";
            resetForm();
            pnlIncidentSearch.Visible = false;
            pnlIncidentDetails.Visible = true;
            btnBackToSearch.Visible = true;
            btnBackToSearch2.Visible = true;
        }

        protected void gvIncidents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblShortDescription = (Label)e.Row.FindControl("lblShortDescription");
                if (lblShortDescription.Text.Length > 70)
                {
                    ViewState["OrigData"] = lblShortDescription.Text;
                    lblShortDescription.Text = lblShortDescription.Text.Substring(0, 70) + "...";
                    lblShortDescription.ToolTip = ViewState["OrigData"].ToString();
                }

                Label lblShortActionTaken = (Label)e.Row.FindControl("lblShortActionTaken");
                if (lblShortActionTaken.Text.Length > 70)
                {
                    ViewState["OrigData"] = lblShortActionTaken.Text;
                    lblShortActionTaken.Text = lblShortActionTaken.Text.Substring(0, 70) + "...";
                    lblShortActionTaken.ToolTip = ViewState["OrigData"].ToString();
                }
            }
        }

        protected void gvIncidents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //load next set of rows on next page
            gvIncidents.PageIndex = e.NewPageIndex;
            gvIncidents.SelectedIndex = -1;
            loadSearchedIncidents();
        }

        protected void gvPersonsAffected_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPersonsAffected.PageIndex = e.NewPageIndex;
            gvPersonsAffected.SelectedIndex = -1;
            loadPersonsAffected(incidentID);
        }

        protected void gvIncidents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvIncidents.SelectedIndex > -1)
            {
                resetForm();
                Label lblIncidentID = (Label)gvIncidents.SelectedRow.FindControl("lblIncidentID");
                Label lblReportNumber2 = (Label)gvIncidents.SelectedRow.FindControl("lblReportNumber");
                Label lblAreaID = (Label)gvIncidents.SelectedRow.FindControl("lblAreaID");
                Label lblLocationID = (Label)gvIncidents.SelectedRow.FindControl("lblLocationID");
                Label lblIncidentTypeID = (Label)gvIncidents.SelectedRow.FindControl("lblIncidentTypeID");
                Label lblDescription = (Label)gvIncidents.SelectedRow.FindControl("lblDescription");
                Label lblOtherTypeDescription = (Label)gvIncidents.SelectedRow.FindControl("lblOtherTypeDescription");
                Label lblDateOccured = (Label)gvIncidents.SelectedRow.FindControl("lblDateOccured");
                Label lblTimeOccured = (Label)gvIncidents.SelectedRow.FindControl("lblTimeOccured");
                Label lblShiftID = (Label)gvIncidents.SelectedRow.FindControl("lblShiftID");
                Label lblCreatedBy = (Label)gvIncidents.SelectedRow.FindControl("lblCreatedBy");
                Label lblCreatedOn = (Label)gvIncidents.SelectedRow.FindControl("lblCreatedOn");
                Label lblLastModifiedBy = (Label)gvIncidents.SelectedRow.FindControl("lblLastModifiedBy");
                Label lblLastModifiedOn = (Label)gvIncidents.SelectedRow.FindControl("lblLastModifiedOn");
                Label lblActionTaken = (Label)gvIncidents.SelectedRow.FindControl("lblActionTaken");

                incidentID = Convert.ToInt32(lblIncidentID.Text);
                lblReportNumber.Text = lblReportNumber2.Text;
                if (ddlArea.Items.FindByValue(lblAreaID.Text) != null)
                    ddlArea.SelectedValue = lblAreaID.Text;
                else
                    ddlArea.SelectedIndex = 0;
                loadDDLLocations(Convert.ToByte(ddlArea.SelectedValue));
                if (ddlLocation.Items.FindByValue(lblLocationID.Text) != null)
                    ddlLocation.SelectedValue = lblLocationID.Text;
                else
                    ddlLocation.SelectedIndex = 0;
                ddlIncidentType.SelectedValue = lblIncidentTypeID.Text;
                tbDescription.Text = lblDescription.Text;
                tbActionTaken.Text = lblActionTaken.Text;
                tbDate.Text = lblDateOccured.Text;
                tbTime.Text = lblTimeOccured.Text;
                if (ddlShift.Items.FindByValue(lblShiftID.Text) != null)
                    ddlShift.SelectedValue = lblShiftID.Text;
                else
                    ddlShift.SelectedIndex = 0;
                lblHeader.Text = "Update the selected incident report below";
                lblAuditTrail.Text = "Created By: " + lblCreatedBy.Text + " Created On: " + lblCreatedOn.Text + " Last Modified By: " + lblLastModifiedBy.Text + " Last Modified On: " + lblLastModifiedOn.Text;

                if (ddlIncidentType.SelectedItem.Text == "Other")
                {
                    tbOther.Text = lblOtherTypeDescription.Text;
                    lblOtherIncidentType.Visible = true;
                    tbOther.Visible = true;
                }
                else
                {
                    lblOtherIncidentType.Visible = false;
                    tbOther.Visible = false;
                }
                loadPersonsAffected(incidentID);
                isLoaded = true;
                pnlIncidentSearch.Visible = false;
                pnlIncidentDetails.Visible = true;
                btnBackToSearch.Visible = true;
                btnBackToSearch2.Visible = true;
            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            pnlIncidentDetails.Visible = false;
            btnBackToSearch.Visible = false;
            btnBackToSearch2.Visible = false;
            resetForm();
            pnlIncidentSearch.Visible = true;
            lblGVIncidentsHeader.Text = "";
            if (gvIncidents.Rows.Count > 0)
                loadSearchedIncidents();
        }

        //Function to show message
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void btnCancelPersonAffected_Click(object sender, EventArgs e)
        {
            hidePersonAffectedModal();
        }

        protected void gvPersonsAffected_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}