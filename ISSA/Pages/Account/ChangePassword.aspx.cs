using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISSA.BusinessTier;

namespace ISSA.Pages.Account
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        /*Class Objects*/
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        public enum MessageType { Success, Error, Info, Warning };

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void ChangePasswordPushButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if ((NewPassword.Text.Length >= 6) && (ConfirmNewPassword.Text.Length >= 6))
                {
                    //handle changing the password 
                    string newpassword = NewPassword.Text;
                    byte loggedInUser = MISC.getUserID();
                    MembershipUser user = Membership.GetUser();
                    var model = new EDM.DataSource();
                    List<string> roles = new List<string>(Roles.GetRolesForUser());

                    try
                    {
                        bool changedPassword = user.ChangePassword(user.ResetPassword(), newpassword);
                        using (TransactionScope ts = new TransactionScope())
                        {
                            if (changedPassword)
                            {
                                var query = (from a in model.tbl_Sup_Employees
                                             where (a.UID == loggedInUser)
                                             select a).FirstOrDefault();

                                if (query != null)
                                {
                                    query.ResetNeeded = false;
                                    query.LastModifiedBy = loggedInUser;
                                    query.LastModifiedOn = DateTime.Now;
                                    model.SaveChanges();
                                    ts.Complete();
                                    ShowMessage("Password Changed, Please wait to be redirected...", MessageType.Success);

                                    //redirect role based

                                    if (roles.Where(x => x.Contains("Manager")).FirstOrDefault() == "Manager")
                                    {
                                        Response.AddHeader("REFRESH", "5;URL=../../Pages/Managers/Alerts.aspx");
                                    }
                                    else if (roles.Where(x => x.Contains("Supervisor")).FirstOrDefault() == "Supervisor")
                                    {
                                        Response.AddHeader("REFRESH", "5;URL=../../Pages/Supervisors/ShiftReport.aspx");
                                    }
                                    else if (roles.Where(x => x.Contains("Operator")).FirstOrDefault() == "Operator")
                                    {
                                        Response.AddHeader("REFRESH", "5;URL=../../Pages/Operations/DailyLog.aspx");
                                    }
                                    else if (roles.Where(x => x.Contains("Contract Compliance Officer")).FirstOrDefault() == "Contract Compliance Officer")
                                    {
                                        Response.AddHeader("REFRESH", "5;URL=../../Pages/Operations/Incidents.aspx");
                                    }
                                    else if (roles.Where(x => x.Contains("Administrator")).FirstOrDefault() == "Administrator")
                                    {
                                        Response.AddHeader("REFRESH", "5;URL=../../Pages/Administrator/ManageUsers.aspx");
                                    }
                                }
                            }
                            else
                            {
                                ts.Dispose();
                            }
                        }
                    }
                    catch (Exception exec)
                    {
                        MISC.writetoAlertLog(exec.ToString());
                        ShowMessage("Something went wrong and the password could not be changed. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    }
                }
                else
                    ShowMessage("Password length must be more than or equal to six characters.", MessageType.Warning);
            }
        }
    }
}