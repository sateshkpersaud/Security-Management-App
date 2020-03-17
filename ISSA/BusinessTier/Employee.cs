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

namespace ISSA.BusinessTier
{
    public class Employee
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_Department_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Department_Sel4DDL";

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();

                    try
                    {
                        ConnStr.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds, "ss");
                        dt = ds.Tables["ss"];
                        successfulCommit = true;
                    }
                    catch (System.Exception excec)
                    {
                        successfulCommit = false;
                        MISC.writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Position_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Position_Sel4DDL";

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();

                    try
                    {
                        ConnStr.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds, "ss");
                        dt = ds.Tables["ss"];
                        successfulCommit = true;
                    }
                    catch (System.Exception excec)
                    {
                        successfulCommit = false;
                        MISC.writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        //Update employee
        public void update_Employee(Int16 employeeID, string firstName, string lastName, string otherName, DateTime dateHired, byte areaID, Int16 locationID, byte departmentID, byte positionID, byte shiftID, bool isStandbyEmployee, bool isActive, string employeeNumber, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the record to update
                        var a = (from y in model.tbl_Sup_Employees
                                              where y.EmployeeID == employeeID
                                              select y).First();
                        a.FirstName = firstName == "" ? null : firstCharToUpper(firstName);
                        a.LastName = firstCharToUpper(lastName);
                        a.OtherName = otherName == "" ? null : firstCharToUpper(otherName);
                        a.DateHired = dateHired;
                        a.AreaID = areaID == 0 ? null : (byte?)areaID;
                        a.LocationID = locationID == 0 ? null : (Int16?)locationID;
                        a.DepartmentID = departmentID == 0 ? null : (byte?)departmentID;
                        a.PositionID = positionID == 0 ? null : (byte?)positionID;
                        a.ShiftID = shiftID == 0 ? null : (byte?)shiftID;
                        a.IsStandbyStaff = isStandbyEmployee;
                        a.IsActive = isActive;
                        a.EmployeeNumber = employeeNumber;
                        a.LastModifiedBy = userID;
                        a.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = employeeID;
                            z.TableName = "tbl_Sup_Employees";
                            z.AuditProcess = "Modified Employee Record - " + employeeNumber;
                            z.DateTime = DateTime.Now;
                            z.UserID = userID;
                        };
                        model.tbl_AuditLog.Add(z);
                        model.SaveChanges();

                    }
                    //commit transaction
                    ts.Complete();
                    successfulCommit = true;
                }
            }
            catch (Exception exec)
            {
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
        }

        //insert employee
        public short insert_Employee(string firstName, string lastName, string otherName, DateTime dateHired, byte areaID, Int16 locationID, byte departmentID, byte positionID, byte shiftID, bool isStandbyEmployee, bool isActive, string employeeNumber, byte userID)
        {
            short employeeID = 0;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert transaction
                        EDM.tbl_Sup_Employees a = new EDM.tbl_Sup_Employees();
                        {
                            a.FirstName = firstName == "" ? null : firstCharToUpper(firstName);
                            a.LastName = firstCharToUpper(lastName);
                            a.OtherName = otherName == "" ? null : firstCharToUpper(otherName);
                            a.DateHired = dateHired;
                            a.AreaID = areaID == 0 ? null : (byte?)areaID;
                            a.LocationID = locationID == 0 ? null : (Int16?)locationID;
                            a.DepartmentID = departmentID == 0 ? null : (byte?)departmentID;
                            a.PositionID = positionID == 0 ? null : (byte?)positionID;
                            a.ShiftID = shiftID == 0 ? null : (byte?)shiftID;
                            a.IsStandbyStaff = isStandbyEmployee;
                            a.IsActive = isActive;
                            a.EmployeeNumber = employeeNumber;
                            a.IsEmployee = true;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Sup_Employees.Add(a);
                        model.SaveChanges();

                        //Get the absenceid
                        employeeID = (from b in model.tbl_Sup_Employees
                                     where b.EmployeeNumber == employeeNumber
                                     select b.EmployeeID
                         ).FirstOrDefault();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = employeeID;
                            z.TableName = "tbl_Sup_Employees";
                            z.AuditProcess = "Created Employee Record - " + employeeNumber;
                            z.DateTime = DateTime.Now;
                            z.UserID = userID;
                        };
                        model.tbl_AuditLog.Add(z);
                        model.SaveChanges();
                    }
                    //commit transaction
                    ts.Complete();
                    successfulCommit = true;
                }
            }
            catch (Exception exec)
            {
                employeeID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return employeeID;
        }

        public DataTable sp_Employee_Search(object Name, object date, object departmentID, object positionID, object areaID, object locationID, object shiftID, object isStandByStaff, object isActive)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_Search";

                    if (Name != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@Name", SqlDbType.VarChar, 20);
                        param1.Value = Name;
                    }
                    if (date != null && date.ToString() != "")
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@date", SqlDbType.Date);
                        param2.Value = date;
                    }
                    if (departmentID != null && departmentID.ToString() != "")
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@departmentID", SqlDbType.TinyInt);
                        param3.Value = departmentID;
                    }
                    if (positionID != null && positionID.ToString() != "")
                    {
                        SqlParameter param4 = cmd.Parameters.Add("@positionID", SqlDbType.TinyInt);
                        param4.Value = positionID;
                    }
                    if (areaID != null && areaID.ToString() != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param5.Value = areaID;
                    }
                    if (locationID != null && locationID.ToString() != "")
                    {
                        SqlParameter param6 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param6.Value = locationID;
                    }
                    if (shiftID != null && shiftID.ToString() != "")
                    {
                        SqlParameter param7 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param7.Value = shiftID;
                    }
                    if (isStandByStaff != null && isStandByStaff.ToString() != "")
                    {
                        bool isStandy = false;
                        if (isStandByStaff.ToString() == "1")
                            isStandy = true;
                        else
                            isStandy = false;
                        SqlParameter param8 = cmd.Parameters.Add("@isStandByStaff", SqlDbType.Bit);
                        param8.Value = isStandy;
                    }
                    if (isActive != null && isActive.ToString() != "")
                    {
                        bool isActiv = false;
                        if (isActive.ToString() == "1")
                            isActiv = true;
                        else
                            isActiv = false;
                        SqlParameter param9 = cmd.Parameters.Add("@isActive", SqlDbType.Bit);
                        param9.Value = isActiv;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();

                    try
                    {
                        ConnStr.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds, "ss");
                        dt = ds.Tables["ss"];
                        successfulCommit = true;
                    }
                    catch (System.Exception excec)
                    {
                        successfulCommit = false;
                        MISC.writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public string firstCharToUpper(string input)
        {
            string val = "";
            if (!String.IsNullOrEmpty(input))
                val = input.First().ToString().ToUpper() + input.Substring(1);

            return val;
        }

        public void employee_Delete(int employeeID)
        {
            var model = new EDM.DataSource();
            try
            {
                var emp = (from a in model.tbl_Sup_Employees
                                  where a.EmployeeID == employeeID
                                  select a).SingleOrDefault();

                if (emp != null)
                {
                    model.tbl_Sup_Employees.Remove(emp);
                    model.SaveChanges();
                    successfulCommit = true;
                }
            }
            catch (System.Exception excec)
            {
                successfulCommit = false;
                MISC.writetoAlertLog(excec.ToString());
            }
        }

    }
}