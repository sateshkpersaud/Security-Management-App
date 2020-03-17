using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISSA.BusinessTier;

namespace ISSA.Pages.Administrator
{
    public partial class UserForm : System.Web.UI.UserControl
    {
        Misc misc = new Misc();
        public enum MessageType { Success, Error, Info, Warning };

        public List<string> roles
        {
            get { return ViewState["roles"] as List<string>; }
            set { ViewState["roles"] = value; }
        }

        public bool? isNewUser
        {
            get { return ViewState["form"] as bool?; }
            set { ViewState["form"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindEmployeeDDL();
                bindRoleDDL();
            }
        }

        /*Methods*/
        //display the error 
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void bindEmployeeDDL()
        {
            var model = new EDM.DataSource();
            var results = (model.sp_Employee_Sel4DDL_2()).ToList();
            ddlEmployee.DataSource = results;
            ddlEmployee.DataValueField = "EmployeeID";
            ddlEmployee.DataTextField = "FullName";
            ddlEmployee.DataBind();
        }

        protected void bindRoleDDL()
        {
            var model = new EDM.DataSource();
            var results = (from a in model.Roles
                           select new
                           {
                               a.RoleId,
                               a.RoleName
                           }).OrderBy(Seq => Seq).ToList();
            cbkRole.DataSource = results;
            cbkRole.DataTextField = "RoleName";
            cbkRole.DataValueField = "RoleName";
            cbkRole.DataBind();
            //ddlRole.Items.Insert(0, new ListItem() { Text = "...Select...", Value = "" });
        }

        //returns the details for teh employee to be autoinserted
        protected DataTable getEmployeeDetails(Int16 empId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeID", typeof(Int16));
            dt.Columns.Add("FirstName", typeof(String));
            dt.Columns.Add("LastName", typeof(String));

            var model = new EDM.DataSource();

            try
            {
                var query = (from a in model.tbl_Sup_Employees
                             where a.EmployeeID == empId
                             select new
                             {
                                 a.EmployeeID,
                                 a.FirstName,
                                 a.LastName
                             }).FirstOrDefault();

                if (query != null)
                {
                    dt.LoadDataRow(new object[] { query.EmployeeID, query.FirstName, query.LastName }, false);
                }
                else
                {
                    dt = null;
                }

            }
            catch (System.Exception excec)
            {
                 misc.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the employee details could not be retrieved. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }

            return dt;
        }

        //creates the user name based on the first and last names, also checks the DB to ensure that the usernme is not already taken. 
        protected void createUserName()
        {
            string fName = tbFName.Text;
            string lName = tbLName.Text;
            string userName = "";

            if (fName != "" && lName != "")
            {
                //create username
                userName = lName + fName.Substring(0, 1);

                //check if username exists
                int no = validateUserName(userName);
                if (no > 0)
                    userName = userName + no;

                //update username if exists
                tbUserName.Text = userName;
            }
        }

        //validate the username
        protected int validateUserName(string username)
        {
            int number = 0;
            var model = new EDM.DataSource();
            try
            {
                var query = (from a in model.Users
                             where a.UserName.Contains(username)
                             select new
                             {
                                 a.UserName
                             }).ToList();

                number = query.Count;
            }
            catch (System.Exception excec)
            {
                misc.writetoAlertLog(excec.ToString());
                ShowMessage("Something went wrong and the userName could not be validated. Kindly try again and if the error persists, refer to the error log for details.", MessageType.Error);
            }
            return number;
        }

        //reset the fields
        public void clearForm()
        {
            foreach (ListItem li in cbkRole.Items)
                li.Selected = false;

            //ddlEmployee.SelectedIndex = 0;
            ddl_Employee.ClearSelection();
            tbUserName.Text = "";
            tbPassword.Text = "";
            tbConfPassword.Text = "";
            tbEmail.Text = "";
            tbFName.Text = "";
            tbFName.Enabled = true;
            tbLName.Text = "";
            tbLName.Enabled = true;
            ddlStatus.ClearSelection();
            //ddlStatus.SelectedIndex = 0;
            cbkPwdReset.Checked = false;

            bindEmployeeDDL();//reset dropdownbox
        }

        /*Get and set variables*/

        public Int16 Employee
        {
            get
            {
                if (ddlEmployee.SelectedItem.Value == "")
                    return 0;
                else
                    return Convert.ToInt16(ddlEmployee.SelectedItem.Value);
            }
            set
            {
            }
        }
        public string FirstName { get { return tbFName.Text; } set { tbFName.Text = value; } }
        public string LastName { get { return tbLName.Text; } set { tbLName.Text = value; } }
        public List<string> Roles
        {
            get
            {
                List<string> sl = new List<string>();
                foreach (ListItem li in cbkRole.Items)
                {
                    if (li.Selected == true)
                        sl.Add(li.Text);
                }
                roles = sl;
                return sl;
            }
            set
            {
                List<string> ls = value;
                foreach (string s in ls)
                {
                    cbkRole.Items.FindByText(s).Selected = true;
                }
            }
        }
        public string UserName { get { return tbUserName.Text; } set { tbUserName.Text = value; } }
        public string Password { get { return tbPassword.Text; } set { } }
        public string confPassword { get { return tbConfPassword.Text; } set { } }
        public string Status { get { return ddlStatus.SelectedValue; } set { ddlStatus.ClearSelection(); ddlStatus.Items.FindByValue(value).Selected = true; } }
        public string Email { get { return tbEmail.Text; } set { tbEmail.Text = value; } }
        public bool Reset { get { return cbkPwdReset.Checked; } set { } }
        public DropDownList ddl_Employee { get { return ddlEmployee; } }
        public Label lbl_Employee { get { return lblEmployee; } }
        public RequiredFieldValidator pwdvalidator
        {
            get { return RequiredFieldValidator8; }
            set { }
        }
        public RequiredFieldValidator confpwdvalidator
        {
            get { return RequiredFieldValidator9; }
            set { }
        }

        /*Events*/
        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmployee.SelectedItem.Value != "")
            {
                Int16 employeeId = Convert.ToInt16(ddlEmployee.SelectedItem.Value);
                DataTable dt = getEmployeeDetails(employeeId);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        tbFName.Text = row["FirstName"].ToString();
                        tbFName.Enabled = false;
                        tbLName.Text = row["LastName"].ToString();
                        tbLName.Enabled = false;
                        createUserName();
                    }
                }
            }
            else
            {
                tbFName.Text = "";
                tbFName.Enabled = true;
                tbLName.Text = "";
                tbLName.Enabled = true;
            }

            if (isNewUser != null && isNewUser == true)
                Page.ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#addUserModal').modal('show');</script>", false);
            else
                Page.ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#editUserModal').modal('show');</script>", false);

        }

        protected void tbName_TextChanged(object sender, EventArgs e)
        {
            if (isNewUser != null && isNewUser == true)
            {
                createUserName();
                Page.ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#addUserModal').modal('show');</script>", false);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Show", "<script> jQuery.noConflict(); $('#editUserModal').modal('show');</script>", false);
            }
        }
    }
}