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
    public partial class DateTimeGrant : System.Web.UI.Page
    {
        BusinessTier.DateTimeGrant DTGRANT = new BusinessTier.DateTimeGrant();
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
                    if (roleNames.Contains("Administrator"))
                    {
                        loadDateTimeGrants();
                        tbDateStart.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDateStart.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbDateEnd.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDateEnd.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbTimeStart.Attributes.Add("onblur", "javascript:validateTime(" + tbTimeStart.ClientID + ")");
                        tbTimeEnd.Attributes.Add("onblur", "javascript:validateTime(" + tbTimeEnd.ClientID + ")");
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DateTime dtDateFrom = Convert.ToDateTime(tbDateStart.Text + " " + tbTimeStart.Text);
                DateTime dtDateTo = Convert.ToDateTime(tbDateEnd.Text + " " + tbTimeEnd.Text );

                if (!MISC.checkGrantStatus(dtDateFrom, dtDateTo))
                {
                    DTGRANT.insert_Grant(dtDateFrom, dtDateTo, tbComments.Text, userID);
                    if (DTGRANT.successfulCommit)
                    {
                        ShowMessage("Record saved.", MessageType.Success);
                        clearFields();
                        loadDateTimeGrants();
                    }
                    else
                        ShowMessage("Something went wrong and the record was not saved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
                else
                {
                    ShowMessage("An Grant already exist for the period specified.", MessageType.Warning);
                    clearFields();
                }
            }
        }

        private void clearFields()
        {
            tbComments.Text = "";
            tbDateEnd.Text = "";
            tbDateStart.Text = "";
            tbTimeEnd.Text = "";
            tbTimeStart.Text = "";
        }

        private void loadDateTimeGrants()
        {
            DataTable dt = DTGRANT.sp_DateTimeGrant_Select();
            if (DTGRANT.successfulCommit)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    gvGrants.DataSource = dt;
                    gvGrants.DataBind();
                    gvGrants.Visible = true;
                }
                else
                {
                    gvGrants.Visible = false;
                }
            }
            else
            {
                ShowMessage("Something went wrong and the Date Time Grants load was not successful. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                gvGrants.Visible = false;
            }
        }

        protected void gvGrants_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGrants.PageIndex = e.NewPageIndex;
            gvGrants.SelectedIndex = -1;
            loadDateTimeGrants();
        }
    }
}