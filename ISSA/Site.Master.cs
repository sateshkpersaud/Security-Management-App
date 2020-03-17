using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISSA.BusinessTier;

namespace ISSA
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        BusinessTier.Misc MISC = new BusinessTier.Misc();


        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Conditional Show/Hide 
            if (!IsPostBack)
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    if (MISC.isResetRequired(Membership.GetUser().UserName) != true)
                    {
                        bool Manager = Context.User.IsInRole("Manager");
                        bool Admin = Context.User.IsInRole("Administrator");
                        bool Operator = Context.User.IsInRole("Operator");
                        bool Supervisor = Context.User.IsInRole("Supervisor");
                        bool CCO = Context.User.IsInRole("Contract Compliance Officer");

                        pnlWorkSchedule.Visible = false;
                        pnlReports.Visible = false;

                        if (Manager)
                        {
                            pnlManagers.Visible = true;
                            pnlDefault.Visible = false;
                            pnlReports.Visible = true;
                        }
                        else
                        {
                            if (Operator)
                            {
                                pnlOperators.Visible = true;
                                pnlDefault.Visible = false;
                                pnlWorkSchedule.Visible = true;
                            }
                            else if (CCO)
                            {
                                pnlComplianceOff.Visible = true;
                                pnlReports.Visible = true;
                                pnlDefault.Visible = false;
                            }
                            else
                            {
                                pnlDefault.Visible = true;
                            }
                            if (Supervisor)
                            {
                                byte positionID = MISC.getPositionID(Membership.GetUser().UserName);
                                if (positionID == 10)
                                    pnlReports.Visible = true;
                                pnlSupervisors.Visible = true;
                                pnlDefault.Visible = false;
                                pnlWorkSchedule.Visible = true;
                            }
                        }
                       
                        //unique for admin
                        if (Admin)
                        {
                            pnlAdministrators.Visible = true;
                            pnlDefault.Visible = false;
                        }

                        pnlDefault.Visible = false;
                        pnlUserManual.Visible = true;
                    }
                    else
                    {
                        pnlDefault.Visible = false;
                    }
                }
            }
        }

       
        /*Methods*/

        //Function to show message
        //public void ShowMessage(string Message, ISSA.BusinessTier.MessageTypes.MessageType type)
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        //}
    }
}