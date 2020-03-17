using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Membership.OpenAuth;
using System.Transactions;

namespace ISSA.Pages.Administrator
{
    public partial class ManageUsers : System.Web.UI.Page
    {
        /*Class Objects*/
        BusinessTier.Misc MISC = new BusinessTier.Misc();
        public enum MessageType { Success, Error, Info, Warning };

        /*Local Variables */
        public DataTable users
        {
            get { return ViewState["users"] as DataTable; }
            set { ViewState["users"] = value; }
        }

        public DataTable history
        {
            get { return ViewState["history"] as DataTable; }
            set { ViewState["history"] = value; }
        }

        public Byte? userID
        {
            get { return ViewState["ID"] as Byte?; }
            set { ViewState["ID"] = value; }
        }

        public short? employeeID
        {
            get { return ViewState["employeeID"] as short?; }
            set { ViewState["employeeID"] = value; }
        }

        //persist form data
        [Serializable]
        public class prevVals
        {
            public DataTable empDetails { get; set; }
            public List<string> oldRoles { get; set; }
            public string oldEmail { get; set; }
        }

        public prevVals persistValues
        {
            get { return ViewState["prevValues"] as prevVals; }
            set { ViewState["prevValues"] = value; }
        }

        /*Methods*/
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                byte userrIDD = MISC.getUserID();
                if (userrIDD != 0)
                {
                    string[] roleNames = MISC.getUserRoles();
                    if (roleNames.Contains("Administrator"))
                    {
                        bindRoleDDL();
                        tbDateFrom.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDateFrom.ClientID + ",'" + DateTime.Now.Date + "')");
                        tbDateTo.Attributes.Add("onblur", "javascript:validateDateFormat(" + tbDateTo.ClientID + ",'" + DateTime.Now.Date + "')");
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

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }

        ///queries DB for user roles and binds results to ddlRoles
        protected void bindRoleDDL()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.Roles
                           select new
                           {
                               a.RoleId,
                               a.RoleName
                           }).OrderBy(Seq => Seq).ToList();
            ddlRole.DataSource = results;
            ddlRole.DataTextField = "RoleName";
            ddlRole.DataValueField = "RoleName";
            ddlRole.DataBind();
            ddlRole.Items.Insert(0, new ListItem() { Text = "...Select...", Value = "" });
        }

        protected DataTable getEmployeeDetails(Int16 empId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeID", typeof(Int16));
            dt.Columns.Add("FirstName", typeof(String));
            dt.Columns.Add("LastName", typeof(String));
            dt.Columns.Add("IsActive", typeof(Boolean));
            dt.Columns.Add("ResetNeeded", typeof(Boolean));

            var model = new EDM.DataSource();

            try
            {
                var query = (from a in model.tbl_Sup_Employees
                             where a.EmployeeID == empId
                             select new
                             {
                                 a.EmployeeID,
                                 a.FirstName,
                                 a.LastName,
                                 a.IsActive,
                                 a.ResetNeeded
                             }).FirstOrDefault();

                if (query != null)
                {
                    dt.LoadDataRow(new object[] { query.EmployeeID, query.FirstName, query.LastName, query.IsActive, query.ResetNeeded }, false);
                }
                else
                {
                    dt = null;
                }

            }
            catch (System.Exception excec)
            {
                MISC.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the employee details could not be retrieved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }

            return dt;
        }

        ///binds the users grid with the search results 
        protected void bindUsers()
        {
            gvUsers.DataSource = users;
            gvUsers.DataBind();
        }

        ///bind history ddl 
        protected void bindHistory()
        {
            gvHistory.Visible = true;
            gvHistory.DataSource = history;
            gvHistory.DataBind();
        }

        //display the error 
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        //perform some cleanup for the new profile being loaded
        protected void resetEdit()
        {
            //reset history gridview
            gvHistory.Visible = false;
            history = null;

            //reset history search
            tbDateFrom.Text = "";
            tbDateTo.Text = "";
            editUser.clearForm();
        }

        //save audit log 
        protected void AuditLog(int sourceID, string tableName,string AuditProcess, byte loggedInUser)
        {
            //Insert AuditTrail
            using (EDM.DataSource model = new EDM.DataSource())
            {
                EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                {
                    z.SourceID = sourceID;
                    z.TableName = tableName;
                    z.AuditProcess = AuditProcess;
                    z.DateTime = DateTime.Now;
                    z.UserID = loggedInUser;
                };
                model.tbl_AuditLog.Add(z);
                try
                {
                    model.SaveChanges();
                    //success = true;
                }
                catch (Exception exec)
                {
                    MISC.writetoAlertLog(exec.ToString());
                    ShowMessage("Something went wrong and the auditlog could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        //create new user login
        protected bool saveUserLogin()
        {
            bool success = false;
            //variables 
            List<string> role = addUser.Roles;
            string firstName = addUser.FirstName;
            string lastName = addUser.LastName;
            string email = addUser.Email;
            bool status = Convert.ToBoolean(addUser.Status);
            string passsword = addUser.Password;
            string confpassword = addUser.confPassword;
            bool reset = addUser.Reset;
            short employeeId = addUser.Employee;
            string username = addUser.UserName;
            byte loggedInUser = MISC.getUserID();
            byte newuserId = 0;

            if (employeeId > 0)
            {
                //create employee linq along with login
                try
                {
                    Membership.CreateUser(username, passsword, email);
                    Roles.AddUserToRoles(username, role.ToArray());
                    newuserId = MISC.getUserID(username);
                    using (TransactionScope scope = new TransactionScope())
                    {
                        if (linkEmployee(employeeId, reset, status, newuserId, loggedInUser))
                        {

                            success = true;
                            AuditLog(employeeId, "tbl_Sup_Employees", "Created User Details For - " + lastName + ", " + firstName, loggedInUser);
                            // The Complete method commits the transaction. If an exception has been thrown,
                            // Complete is not  called and the transaction is rolled back.
                            scope.Complete();
                        }
                        else
                        {
                            scope.Dispose();
                            Membership.DeleteUser(username, true);
                        }
                    }
                }
                catch (MembershipCreateUserException cuex)
                {
                    MISC.writetoAlertLog(cuex.ToString());
                    ShowMessage("Something went wrong and the login could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            else
            {
                //save non employee along with login
                try
                {
                    Membership.CreateUser(username, passsword, email);
                    Roles.AddUserToRoles(username, role.ToArray());
                    newuserId = MISC.getUserID(username);
                    using (TransactionScope scope = new TransactionScope())
                    {
                        if (saveNonEmployee(firstName, lastName, reset, status, newuserId))
                        {
                            success = true;
                            // The Complete method commits the transaction. If an exception has been thrown,
                            // Complete is not  called and the transaction is rolled back.
                            scope.Complete();
                        }
                        else
                        {
                            scope.Dispose();
                            Membership.DeleteUser(username, true);
                        }
                    }
                }
                catch (Exception cuex)
                {
                    MISC.writetoAlertLog(cuex.ToString());
                    ShowMessage("Something went wrong and the login could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }

            return success;
        }

        //save user that is not an employee
        protected bool saveNonEmployee(string firstname, string lastname, bool resetneeded, bool status, byte UID)
        {
            bool success = false;

            using (EDM.DataSource model = new EDM.DataSource())
            {
                EDM.tbl_Sup_Employees a = new EDM.tbl_Sup_Employees()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    IsActive = status,
                    ResetNeeded = resetneeded,
                    UID = UID,
                    IsEmployee = false,
                    CreatedBy = MISC.getUserID(),
                    CreatedOn = DateTime.Now
                };
                model.tbl_Sup_Employees.Add(a);
                try
                {
                    model.SaveChanges();
                    success = true;
                }
                catch (Exception exec)
                {
                    MISC.writetoAlertLog(exec.ToString());
                    ShowMessage("Something went wrong and the user could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
            return success;
        }

        //create employee link 
        protected bool linkEmployee(short empID, bool resetneeded, bool status, byte UID, byte loggedIN)
        {
            bool success = false;

            var model = new EDM.DataSource();
            try
            {
                var query = (from a in model.tbl_Sup_Employees
                             where (a.EmployeeID == empID)
                             && (a.UID == null)
                             select a).FirstOrDefault();
                query.UID = UID;
                query.LastModifiedBy = loggedIN;
                query.LastModifiedOn = DateTime.Now;

                model.SaveChanges();
                success = true;
            }
            catch (Exception exec)
            {
                 MISC.writetoAlertLog(exec.ToString());
                // ShowMessage("Something went wrong and the employee link could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }
            return success;
        }

        //update user details
        protected bool updateUserLogin()
        {
            bool success = false;
            //variables
            List<string> role = editUser.Roles;
            string firstName = editUser.FirstName;
            string lastName = editUser.LastName;
            string email = editUser.Email;
            bool status = Convert.ToBoolean(editUser.Status);
            string passsword = editUser.Password;
            string confpassword = editUser.confPassword;
            bool reset = editUser.Reset;
            short employeeId = (Int16)employeeID;
            string username = editUser.UserName;
            byte loggedInUser = MISC.getUserID();
            byte updateUser = MISC.getUserID(username);

            //old values and membership variables
            prevVals prev = persistValues;
            MembershipUser user;
            user = Membership.GetUser(username);

            /*Update employee details*/

            if (updateEmployee(status, reset, firstName, lastName, employeeId, loggedInUser))
            {
                try
                {
                    /*Update Membership details*/
                    if (prev.oldEmail != email)
                        user.Email = email;

                    if (passsword != "")
                        user.ChangePassword(user.ResetPassword(), passsword);

                    //Roles Update
                    if (!role.SequenceEqual(prev.oldRoles))
                    {
                        //update roles 
                        foreach (string rolename in prev.oldRoles)
                        {
                            if (Roles.IsUserInRole(username, rolename))
                                Roles.RemoveUserFromRole(username, rolename);
                        }
                        foreach (string rolename2 in role)
                        {
                            if (!Roles.IsUserInRole(username, rolename2))
                                Roles.AddUserToRole(username, rolename2);
                        }
                    }
                    //update user details
                    Membership.UpdateUser(user);
                    success = true;
                    AuditLog(employeeId, "tbl_Sup_Employees", "Updated User Details For - " + lastName + ", " + firstName, loggedInUser);
                }
                catch (Exception exec)
                {
                    //rollback employee update? 
                    MISC.writetoAlertLog(exec.ToString());
                }
            }

            return success;
        }

        protected bool updateEmployee(bool status, bool reset, string fname, string lname, short empId, byte loggedIN)
        {
            bool success = false;

            var model = new EDM.DataSource();
            try
            {
                var query = (from a in model.tbl_Sup_Employees
                             where (a.EmployeeID == empId)
                             select a).FirstOrDefault();
                if (query != null)
                {
                    query.FirstName = fname;
                    query.LastName = lname;
                    query.IsActive = status;
                    query.ResetNeeded = reset;
                    query.LastModifiedBy = loggedIN;
                    query.LastModifiedOn = DateTime.Now;

                    model.SaveChanges();
                    success = true;
                }
            }
            catch (Exception exec)
            {
                 MISC.writetoAlertLog(exec.ToString());
            }
            return success;
        }

        /*Events*/
        //buton click that performs a search based on the Filter criteria
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserId", typeof(Byte));
            dt.Columns.Add("EmployeeId", typeof(Int16));
            dt.Columns.Add("Name", typeof(String));
            dt.Columns.Add("UserName", typeof(String));
            dt.Columns.Add("Status", typeof(String));
            dt.Columns.Add("RoleName", typeof(String));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            dt.Columns.Add("LastActivityDate", typeof(DateTime));

            string name = tbName.Text == "" ? null : tbName.Text;
            string role = ddlRole.SelectedValue == "" ? null : ddlRole.SelectedValue;
            bool? status = (ddlStatus.SelectedValue == "" ? (bool?)null : (Convert.ToBoolean(ddlStatus.SelectedValue) == true ? true : false));

            var model = new EDM.DataSource();

            try
            {
                var query = model.sp_FilterUsers(role, name, status);

                var result = query.Distinct().ToList();
                if (result.Count > 0)
                {
                    foreach (var row in result)
                    {
                        dt.LoadDataRow(new object[] { row.UID, row.EmployeeID, row.Name, row.UserName, (row.IsActive == true ? "Active" : "Inactive"), row.RoleName, row.CreateDate, row.LastActivityDate }, false);
                    }
                }

                //bind grid 
                if (dt.Rows.Count > 0)
                    users = dt;
                else
                    users = null;


                bindUsers();
                lblUsersFound.Text = "Users Found: " + gvUsers.Rows.Count;
            }
            catch (System.Exception excec)
            {
                 MISC.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the search could not be completed. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }
        }

        //opens modal to edit specified user details 
        protected void lblName_Click(object sender, EventArgs e)
        {
            LinkButton name = sender as LinkButton;
            GridViewRow row = name.NamingContainer as GridViewRow;
            Label Id = row.FindControl("lblUserID") as Label;
            Label empId = row.FindControl("lblEmpID") as Label;
            Label username = row.FindControl("lblUserName") as Label;

            lblEditHeader.Text = name.Text;
            userID = Convert.ToByte(Id.Text);

            resetEdit();

            if (userID != null)
                ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#editUserModal').modal('toggle');</script>", false);
            else
                ShowMessage("Something went wrong and record could not be opened. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);

            //setup modals  
            editUser.Visible = true;
            addUser.Visible = false;

            editUser.ddl_Employee.Visible = false;
            editUser.lbl_Employee.Visible = true;
            editUser.lbl_Employee.Text = name.Text;

            

            ///*populate with user data*/
            MembershipUser usr = Membership.GetUser(username.Text);

            Int16 employeeId = Convert.ToInt16(empId.Text);
            DataTable dt = getEmployeeDetails(employeeId);

            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    editUser.FirstName = r["FirstName"].ToString();
                    editUser.LastName = r["LastName"].ToString();
                    editUser.Status = r["IsActive"].ToString();
                    if (r["ResetNeeded"] != DBNull.Value)
                        editUser.Reset = true;
                }
            }

            //user roles
            editUser.Roles = new List<string>(Roles.GetRolesForUser(usr.UserName));

            //username
            editUser.UserName = usr.UserName;

            //user email
            editUser.Email = usr.Email;

            //password, disable validators
            editUser.pwdvalidator.Enabled = false;
            editUser.confpwdvalidator.Enabled = false;
            //editUser.Password = usr.GetPassword();


            //store previous values 
            prevVals prev = new prevVals();
            prev.empDetails = dt;
            prev.oldEmail = usr.Email;
            prev.oldRoles = new List<string>(Roles.GetRolesForUser(usr.UserName));
            persistValues = prev;


            //save employee id
            employeeID = Convert.ToInt16(empId.Text);

            //modal performance
            editUser.isNewUser = false;

            if (users != null)
                bindUsers(); //needed for modal performance
        }

        //Databound event for the user gridview
        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //forces asp.net to render the gridview header as a <thead> instead of within the <tbody>
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            //register link button as asyncpostback, needed so linkbutton works
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton name = (LinkButton)e.Row.FindControl("lblName");
                ScriptManager.GetCurrent(this).RegisterPostBackControl(name);

                if (name.Text == "System Admin")
                {
                    string username = Membership.GetUser().UserName;
                    if (username != "SysAdmin")
                        e.Row.Enabled = false;
                }
            }
        }

        //handles the page index changing event, to correctly display the gridview
        protected void gvUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUsers.PageIndex = e.NewPageIndex;
            bindUsers();
        }

        protected void btnSaveNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {

                if (saveUserLogin())
                {
                    ShowMessage("User Successfully Created!", MessageType.Success);
                    addUser.clearForm();
                }
                else
                {
                    ShowMessage("Something went wrong and the user could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }

            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict();  $('#addUserModal').modal('show');</script>", false);
            if (users != null)
                bindUsers(); //needed for modal performance
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (updateUserLogin())
                {
                    ShowMessage("User Successfully Updated!", MessageType.Success);
                }
                else
                {
                    ShowMessage("Something went wrong and the user could not be updated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }

            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict();  $('#editUserModal').modal('show');</script>", false);

            if (users != null)
                bindUsers(); //needed for modal performance
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (saveUserLogin())
                {
                    ShowMessage("User Successfully Created!", MessageType.Success);
                    addUser.clearForm();
                }
                else
                {
                    ShowMessage("Something went wrong and the user could not be created. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                    ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict();  $('#addUserModal').modal('show');</script>", false);
                }
            }

            if (users != null)
                bindUsers(); //needed for modal performance
        }

        /*handles date selection for from and to checkboxes */
        protected void tbDateFrom_TextChanged(object sender, EventArgs e)
        {
            if (tbDateTo.Text == "")
            {
                tbDateTo.Enabled = true;
                tbDateTo.Text = tbDateFrom.Text;
                ibSearchDateTo.Enabled = true;
            }
            bindUsers(); //needed for modal performance
        }

        /*Search for user activity */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Date", typeof(DateTime));
                dt.Columns.Add("Activity", typeof(String));

                DateTime? from = tbDateFrom.Text != "" ? Convert.ToDateTime(tbDateFrom.Text + " 0:00:00.000") : (DateTime?)null;
                DateTime? to = tbDateTo.Text != "" ? Convert.ToDateTime(tbDateTo.Text + " 23:59:59.999") : (DateTime?)null;

                var model = new EDM.DataSource();

                try
                {
                    var query = model.sp_FilterHistory(userID, from, to);

                    var result = query.Distinct().ToList();
                    if (result.Count > 0)
                    {
                        foreach (var row in result)
                        {
                            dt.LoadDataRow(new object[] { row.DateTime, row.AuditProcess }, false);
                        }
                    }

                    //bind grid 
                    if (dt.Rows.Count > 0)
                        history = dt;
                    else
                        history = null;


                    bindHistory();
                    bindUsers(); //needed for modal performance
                }
                catch (System.Exception excec)
                {
                     MISC.writetoAlertLog(excec.ToString());
                    ShowMessage("Something went wrong and the search could not be completed. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
                }
            }
        }

        /*Data bound event for action history*/
        protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //forces asp.net to render the gridview header as a <thead> instead of within the <tbody>
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void gvHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHistory.PageIndex = e.NewPageIndex;
            bindHistory();
            bindUsers(); //needed for modal performance
        }

        /*Print the current search activity history*/
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (history != null)
            {
                gvHistory.AllowPaging = false;
                bindHistory();
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvHistory.RenderControl(hw);
                string gridHTML = sw.ToString().Replace("\"", "'")
                    .Replace(System.Environment.NewLine, "");
                string header = "<p style='text-align:center'><strong><img alt='' src='../../Images/logo.png' style='float:left' align='left' /></strong></p>" +
                                 "<p style='text-align:center'>&nbsp;</p><h2 style='text-align:center;line-height:100%'><strong>Activity History Report for </strong>" + Membership.GetUser().UserName + "</h2>" +
                                 "<h3 style='text-align:center;line-height:100%'><strong>Date From   </strong> " + tbDateFrom.Text + " <strong>  To  </strong> " + tbDateTo.Text + "</h3> <hr />";
                gridHTML = header + gridHTML;
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload = new function(){");
                sb.Append("var printWin = window.open('', '', 'left=0");
                sb.Append(",top=0,width=1000,height=600,status=0');");
                sb.Append("printWin.document.write(\"");
                sb.Append(gridHTML);
                sb.Append("\");");
                sb.Append("printWin.document.close();");
                sb.Append("printWin.focus();");
                sb.Append("printWin.print();");
                sb.Append("printWin.close();};");
                sb.Append("</script>");
                ClientScript.RegisterStartupScript(GetType(), "GridPrint", sb.ToString());
                gvHistory.AllowPaging = true;
                bindHistory();
            }
            else
            {
                ShowMessage("There is nothing here to print.", MessageType.Error);
            }

            if (users != null)
                bindUsers(); //needed for modal performance
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            if (users != null)
                bindUsers(); //needed for modal performance
        }

        protected void btnNewUser_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#addUserModal').modal('toggle');</script>", false);

            editUser.Visible = false;
            addUser.Visible = true;
            //password
            addUser.pwdvalidator.Enabled = true;
            addUser.confpwdvalidator.Enabled = true;

            //modal performance
            addUser.isNewUser = true;

            if (users != null)
                bindUsers(); //needed for modal performance
        }
    }
}