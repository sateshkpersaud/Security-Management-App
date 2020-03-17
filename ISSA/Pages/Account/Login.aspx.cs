using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ISSA.Account
{
    public partial class Login : Page
    {
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        public enum MessageType { Success, Error, Info, Warning };
        public int? errCount
        {
            get { return ViewState["count"] as int?; }
            set { ViewState["count"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //display the error 
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        public void loginError()
        {
            if (errCount == null)
                errCount = MISC.getFailCount(LoginControl.UserName);
            else 
                errCount++;
        }

        /*Events*/
        //after user has been logged into the website and authenticated
        protected void Unnamed_LoggedIn(object sender, EventArgs e)
        {
            if (MISC.isResetRequired(LoginControl.UserName) == true)
            {
                Response.Redirect("~/Pages/Account/ChangePassword.aspx");
            }
            else
            {
                if (Roles.IsUserInRole(LoginControl.UserName, "Manager"))
                {
                    Response.Redirect("~/Pages/Managers/Alerts.aspx");
                }
                else if (Roles.IsUserInRole(LoginControl.UserName, "Supervisor"))
                {
                    Response.Redirect("~/Pages/Supervisors/ShiftReport.aspx");
                }
                else if (Roles.IsUserInRole(LoginControl.UserName, "Operator"))
                {
                    Response.Redirect("~/Pages/Operations/DailyLog.aspx");
                }
                else if (Roles.IsUserInRole(LoginControl.UserName, "Contract Compliance Officer"))
                {
                    Response.Redirect("~/Pages/Operations/Incidents.aspx");
                }
                else if (Roles.IsUserInRole(LoginControl.UserName, "Administrator"))
                {
                    Response.Redirect("~/Pages/Administrator/ManageUsers.aspx");
                }
            }
        }

        //handle user being active
        protected void LoginControl_LoggingIn(object sender, LoginCancelEventArgs e)
        {
            if (MISC.isUserActive(LoginControl.UserName) == false)
            {
                e.Cancel = true;
                ShowMessage("The account you are trying to login with does not exist or is inactive. Kindly check with the Administrator.", MessageType.Error);
            }
        }

        //handle errors and display messages to the user 
        protected void LoginControl_LoginError(object sender, EventArgs e)
        {
            loginError();
            if (errCount != null && errCount <= 4)
            {
                if (errCount == 4)
                {
                    ShowMessage("Your login attempt was not successful, please try again.", MessageType.Warning);
                    LoginControl.FailureText = (5 - errCount) + " attempt remaining.";
                }
                else
                {
                    ShowMessage("Your login attempt was not successful, please try again.", MessageType.Warning);
                    LoginControl.FailureText = (5 - errCount) + " attempts remaining.";
                }
            }
            else if (errCount != null && errCount == 5)
            {
                ShowMessage("Your login attempt was not successful, you have been locked out of your account! Please Contact your System Administrator to resolve this issue.", MessageType.Error);
                LoginControl.FailureText = "Too many login attempts. Your user account is now disabled. Contact the Admin to reactivate.";
            }
            else if (errCount != null && errCount > 5)
            {
                LoginControl.FailureText = "User account is disabled, you cannot Sign In.";
            }
        }


    }
}