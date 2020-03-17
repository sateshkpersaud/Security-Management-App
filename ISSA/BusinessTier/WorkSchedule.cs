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
    public class WorkSchedule
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_Employee_SelectForWorkSchedule(object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_SelectForWorkSchedule";

                    SqlParameter param1 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param1.Value = areaID;
                    SqlParameter param2 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                    param2.Value = shiftID;

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

        public DataTable sp_Employee_SelectFromWorkSchedule(object workScheduleID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_SelectFromWorkSchedule";

                    SqlParameter param1 = cmd.Parameters.Add("@workScheduleID", SqlDbType.Int);
                    param1.Value = workScheduleID;

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

        public DataTable sp_StandByEmployee_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_StandByEmployee_Sel4DDL";

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

        //insert workschedule
        public int insert_WorkSchedule(string year, string month, byte areaID, byte shiftID, GridView gvEmployees, string areaName, string shifttime, byte userID)
        {
            int wsID = 0;
            int count = 0;

            string connString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert workschedule header
                        EDM.tbl_Mgr_WorkSchedule a = new EDM.tbl_Mgr_WorkSchedule();
                        {
                            a.Year = year;
                            a.Month = month;
                            a.AreaID = areaID;
                            a.ShiftID = shiftID;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Mgr_WorkSchedule.Add(a);
                        model.SaveChanges();

                        //Get the id
                        wsID = (from b in model.tbl_Mgr_WorkSchedule
                                where b.Year == year && b.Month == month && b.AreaID == areaID && b.ShiftID == shiftID
                                select b.WorkScheduleID
                         ).FirstOrDefault();

                        //insert employees
                        for (int i = 0; i < gvEmployees.Rows.Count; i++)
                        {
                            Label lblEmployeeID = (Label)gvEmployees.Rows[i].FindControl("lblEmployeeID");
                            CheckBox cbComplete = (CheckBox)gvEmployees.Rows[i].FindControl("cbComplete");
                            DropDownList ddlLocation = (DropDownList)gvEmployees.Rows[i].FindControl("ddlLocation");
                            DropDownList ddlStandByEmployee = (DropDownList)gvEmployees.Rows[i].FindControl("ddlStandByEmployee");
                            CheckBox cbExclude = (CheckBox)gvEmployees.Rows[i].FindControl("cbExclude");
                            TextBox tbDayOff1 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff1");
                            TextBox tbDayOff2 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff2");
                            TextBox tbDayOff3 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff3");
                            TextBox tbDayOff4 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff4");

                            Int16 empID = new Int16();
                            empID = Convert.ToInt16(lblEmployeeID.Text);

                            Int16 standbyEmpID = new Int16();
                            if (ddlStandByEmployee.SelectedIndex > 0)
                                standbyEmpID = Convert.ToInt16(ddlStandByEmployee.SelectedValue);

                            Int16 locationID = new Int16();
                            if (ddlLocation.SelectedIndex > 0)
                                locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                            //insert employee data
                            EDM.tbl_Mgr_WorkSchedule_Employees c = new EDM.tbl_Mgr_WorkSchedule_Employees();
                            {
                                c.WorkScheduleID = wsID;
                                c.EmployeeID = empID;
                                c.LocationID = locationID == 0 ? null : (Int16?)locationID;
                                c.StandyByEmployeeID = standbyEmpID == 0 ? null : (Int16?)standbyEmpID;
                                c.IsComplete = cbComplete.Checked;
                                c.Exclude = cbExclude.Checked;
                                c.CreatedBy = userID;
                                c.CreatedOn = DateTime.Now;
                            };
                            model.tbl_Mgr_WorkSchedule_Employees.Add(c);
                            model.SaveChanges();

                            //Only insert days if the employee is not checked as excluded from the schedule
                            if (cbExclude.Checked == false)
                            {
                                //Get the id
                                Int64 wsEmpID = new Int64();
                                wsEmpID = (from d in model.tbl_Mgr_WorkSchedule_Employees
                                           where d.WorkScheduleID == wsID && d.EmployeeID == empID
                                           select d.WorkScheduleEmployeeID
                                 ).FirstOrDefault();

                                //Create a datatable to store the dates to be inserted
                                DataTable daysTOWork = new DataTable();
                                daysTOWork.Columns.Add("WorkScheduleEmployeeID", typeof(Int64));
                                daysTOWork.Columns.Add("Date", typeof(DateTime));
                                daysTOWork.Columns.Add("IsDayOff", typeof(bool));
                                daysTOWork.Columns.Add("CreatedBy", typeof(byte));
                                daysTOWork.Columns.Add("CreatedOn", typeof(DateTime));

                                //Get the dates for the month
                                var dates = new List<DateTime>();
                                dates = MISC.GetDates(Convert.ToInt32(year), Convert.ToInt32(month));

                                //Populate the dates in the datatable
                                foreach (var val in dates)
                                {
                                    DataRow dr = daysTOWork.NewRow();
                                    dr["WorkScheduleEmployeeID"] = wsEmpID;
                                    dr["Date"] = val.Date;
                                    dr["IsDayOff"] = false;
                                    dr["CreatedBy"] = userID;
                                    dr["CreatedOn"] = DateTime.Now;
                                    daysTOWork.Rows.Add(dr);
                                }

                                //create a datatable for the day offs, these will be used to update the dates datatable and set the IsDayOff to true
                                DataTable dayOffs = new DataTable();
                                dayOffs.Columns.Add("Date", typeof(DateTime));
                                if (tbDayOff1.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff1.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }
                                if (tbDayOff2.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff2.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }
                                if (tbDayOff3.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff3.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }
                                if (tbDayOff4.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff4.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }

                                //Merge the two tables and set dayoff to true for the dayoff dates
                                var commonRows = from dt1 in daysTOWork.AsEnumerable()
                                                 join dt2 in dayOffs.AsEnumerable() on dt1.Field<DateTime>("Date") equals dt2.Field<DateTime>("Date")
                                                 select new { dt1 };
                                foreach (var commonRowInfo in commonRows)
                                {
                                    commonRowInfo.dt1.SetField("IsDayOff", true);
                                }
                                conn.Open();
                                //Bulk insert the days 
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                                {
                                    bulkCopy.ColumnMappings.Add("WorkScheduleEmployeeID", "WorkScheduleEmployeeID");
                                    bulkCopy.ColumnMappings.Add("Date", "Date");
                                    bulkCopy.ColumnMappings.Add("IsDayOff", "IsDayOff");
                                    bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                    bulkCopy.ColumnMappings.Add("CreatedOn", "CreatedOn");
                                    bulkCopy.BatchSize = 10000;
                                    bulkCopy.DestinationTableName = "tbl_Mgr_WorkSchedule_Days";
                                    bulkCopy.WriteToServer(daysTOWork.CreateDataReader());
                                    conn.Close();
                                }
                                count++;
                                //if no days are available, set count to zero so that the records will not be appended
                                if (daysTOWork.Rows.Count == 0)
                                {
                                    count = 0;
                                    break;
                                }
                            }
                        }
                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = wsID;
                            z.TableName = "tbl_Mgr_WorkSchedule";
                            z.AuditProcess = "Created Work Schedule for " + month + "/" + year + " - " + areaName + " -- " + shifttime;
                            z.DateTime = DateTime.Now;
                            z.UserID = userID;
                        };
                        model.tbl_AuditLog.Add(z);
                        model.SaveChanges();

                    }
                    //commit transaction
                    if (count > 0)
                    {
                        ts.Complete();
                        successfulCommit = true;
                    }
                    else
                    {
                        successfulCommit = false;
                        MISC.writetoAlertLog("No days avaiable to save for employee schedule!!");
                    }
                }
            }
            catch (Exception exec)
            {
                wsID = 0;
                conn.Close();
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return wsID;
        }

        //insert unscheduled employees
        public int insert_UnscheduledWorkSchedule(int wsID, string year, string month, byte areaID, byte shiftID, GridView gvUnscheduledEmps, string areaName, string shifttime, byte userID)
        {
            string connString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);

            int count = 0;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                       //insert employees
                        for (int i = 0; i < gvUnscheduledEmps.Rows.Count; i++)
                        {
                            Label lblEmployeeID = (Label)gvUnscheduledEmps.Rows[i].FindControl("lblEmployeeID");
                            CheckBox cbSelect = (CheckBox)gvUnscheduledEmps.Rows[i].FindControl("cbSelect");
                            DropDownList ddlLocation = (DropDownList)gvUnscheduledEmps.Rows[i].FindControl("ddlLocation");

                            if (cbSelect.Checked)
                            {
                                Int16 empID = new Int16();
                                empID = Convert.ToInt16(lblEmployeeID.Text);

                                Int16 locationID = new Int16();
                                if (ddlLocation.SelectedIndex > 0)
                                    locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                                //insert employee data
                                EDM.tbl_Mgr_WorkSchedule_Employees c = new EDM.tbl_Mgr_WorkSchedule_Employees();
                                {
                                    c.WorkScheduleID = wsID;
                                    c.EmployeeID = empID;
                                    c.LocationID = locationID == 0 ? null : (Int16?)locationID;
                                    c.WasUnscheduled = true;
                                    c.IsComplete = false;
                                    c.Exclude = false;
                                    c.CreatedBy = userID;
                                    c.CreatedOn = DateTime.Now;
                                };
                                model.tbl_Mgr_WorkSchedule_Employees.Add(c);
                                model.SaveChanges();

                                //Get the id
                                Int64 wsEmpID = new Int64();
                                wsEmpID = (from d in model.tbl_Mgr_WorkSchedule_Employees
                                           where d.WorkScheduleID == wsID && d.EmployeeID == empID
                                           select d.WorkScheduleEmployeeID
                                 ).FirstOrDefault();

                                //Create a datatable to store the dates to be inserted
                                DataTable daysTOWork = new DataTable();
                                daysTOWork.Columns.Add("WorkScheduleEmployeeID", typeof(Int64));
                                daysTOWork.Columns.Add("Date", typeof(DateTime));
                                daysTOWork.Columns.Add("IsDayOff", typeof(bool));
                                daysTOWork.Columns.Add("CreatedBy", typeof(byte));
                                daysTOWork.Columns.Add("CreatedOn", typeof(DateTime));

                                //Get the dates for the month
                                var dates = new List<DateTime>();
                                dates = MISC.GetDates(Convert.ToInt32(year), Convert.ToInt32(month));

                                //Populate the dates in the datatable
                                foreach (var val in dates)
                                {
                                    DataRow dr = daysTOWork.NewRow();
                                    dr["WorkScheduleEmployeeID"] = wsEmpID;
                                    dr["Date"] = val.Date;
                                    dr["IsDayOff"] = false;
                                    dr["CreatedBy"] = userID;
                                    dr["CreatedOn"] = DateTime.Now;
                                    daysTOWork.Rows.Add(dr);
                                }

                                //Delete days preceeding today's date
                                if (Convert.ToDateTime(daysTOWork.Rows[0]["Date"].ToString()) <= DateTime.Now.Date)
                                {
                                    daysTOWork.Rows.Cast<DataRow>().Where(r => Convert.ToDateTime(r.ItemArray[1]) < DateTime.Now.Date).ToList().ForEach(r => r.Delete());
                                }

                                conn.Open();
                                //Bulk insert the days 
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                                {
                                    bulkCopy.ColumnMappings.Add("WorkScheduleEmployeeID", "WorkScheduleEmployeeID");
                                    bulkCopy.ColumnMappings.Add("Date", "Date");
                                    bulkCopy.ColumnMappings.Add("IsDayOff", "IsDayOff");
                                    bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                    bulkCopy.ColumnMappings.Add("CreatedOn", "CreatedOn");
                                    bulkCopy.BatchSize = 10000;
                                    bulkCopy.DestinationTableName = "tbl_Mgr_WorkSchedule_Days";
                                    bulkCopy.WriteToServer(daysTOWork.CreateDataReader());
                                    conn.Close();
                                }
                                count++;

                                //if no days are available, set count to zer so that the records will not be appended
                                if (daysTOWork.Rows.Count == 0)
                                {
                                    count = 0;
                                    break;
                                }
                            }
                        }
                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = wsID;
                            z.TableName = "tbl_Mgr_WorkSchedule";
                            z.AuditProcess = "Appended employees to Work Schedule for " + month + "/" + year + " - " + areaName + " -- " + shifttime;
                            z.DateTime = DateTime.Now;
                            z.UserID = userID;
                        };
                        model.tbl_AuditLog.Add(z);
                        model.SaveChanges();
                    }
                    //commit transaction
                    if (count > 0)
                    {
                        ts.Complete();
                        successfulCommit = true;
                    }
                    else
                    {
                        successfulCommit = false;
                        MISC.writetoAlertLog("No days avaiable to save for employee schedule!!");
                    }
                }
            }
            catch (Exception exec)
            {
                count = 0;
                conn.Close();
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return count;
        }

        public void update_WorkSchedule(int wsID, string year, string month, GridView gvEmployees, string areaName, string shifttime, byte userID)
        {
            string connString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var WS = (from a in model.tbl_Mgr_WorkSchedule
                                              where a.WorkScheduleID == wsID
                                              select a).First();
                        WS.LastModifiedBy = userID;
                        WS.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //update employees
                        for (int i = 0; i < gvEmployees.Rows.Count; i++)
                        {
                            Label lblWorkScheduleEmployeeID = (Label)gvEmployees.Rows[i].FindControl("lblWorkScheduleEmployeeID");
                            CheckBox cbComplete = (CheckBox)gvEmployees.Rows[i].FindControl("cbComplete");
                            DropDownList ddlLocation = (DropDownList)gvEmployees.Rows[i].FindControl("ddlLocation");
                            DropDownList ddlStandByEmployee = (DropDownList)gvEmployees.Rows[i].FindControl("ddlStandByEmployee");
                            CheckBox cbExclude = (CheckBox)gvEmployees.Rows[i].FindControl("cbExclude");
                            TextBox tbDayOff1 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff1");
                            TextBox tbDayOff2 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff2");
                            TextBox tbDayOff3 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff3");
                            TextBox tbDayOff4 = (TextBox)gvEmployees.Rows[i].FindControl("tbDayOff4");

                            Int64 wsEmpID = new Int64();
                            wsEmpID = Convert.ToInt64(lblWorkScheduleEmployeeID.Text);

                            Int16 standbyEmpID = new Int16();
                            if (ddlStandByEmployee.SelectedIndex > 0)
                                standbyEmpID = Convert.ToInt16(ddlStandByEmployee.SelectedValue);

                            Int16 locationID = new Int16();
                            if (ddlLocation.SelectedIndex > 0)
                                locationID = Convert.ToInt16(ddlLocation.SelectedValue);

                            //Get the record to update
                            var recordToUpdate = (from a in model.tbl_Mgr_WorkSchedule_Employees
                                                  where a.WorkScheduleEmployeeID == wsEmpID
                                                  select a).First();
                            recordToUpdate.LocationID = locationID == 0 ? null : (Int16?)locationID;
                            recordToUpdate.StandyByEmployeeID = standbyEmpID == 0 ? null : (Int16?)standbyEmpID;
                            recordToUpdate.IsComplete = cbComplete.Checked;
                            recordToUpdate.Exclude = cbExclude.Checked;
                            recordToUpdate.LastModifiedBy = userID;
                            recordToUpdate.LastModifiedOn = DateTime.Now;
                            model.SaveChanges();

                            //Only insert days if the employee is not checked as excluded from the schedule
                            if (cbExclude.Checked == false)
                            {
                                //Create a datatable to store the dates to be inserted
                                DataTable daysTOWork = new DataTable();
                                daysTOWork.Columns.Add("WorkScheduleEmployeeID", typeof(Int64));
                                daysTOWork.Columns.Add("Date", typeof(DateTime));
                                daysTOWork.Columns.Add("IsDayOff", typeof(bool));
                                daysTOWork.Columns.Add("CreatedBy", typeof(byte));
                                daysTOWork.Columns.Add("CreatedOn", typeof(DateTime));
                                daysTOWork.Columns.Add("LastModifiedBy", typeof(byte));
                                daysTOWork.Columns.Add("LastModifiedOn", typeof(DateTime));

                                //Get the dates for the month
                                var dates = new List<DateTime>();
                                dates = MISC.GetDates(Convert.ToInt32(year), Convert.ToInt32(month));

                                //Populate the dates in the datatable
                                foreach (var val in dates)
                                {
                                    DataRow dr = daysTOWork.NewRow();
                                    dr["WorkScheduleEmployeeID"] = wsEmpID;
                                    dr["Date"] = val.Date;
                                    dr["IsDayOff"] = false;
                                    dr["CreatedBy"] = userID;
                                    dr["CreatedOn"] = DateTime.Now;
                                    dr["LastModifiedBy"] = userID;
                                    dr["LastModifiedOn"] = DateTime.Now;
                                    daysTOWork.Rows.Add(dr);
                                }

                                //create a datatable for the day offs, these will be used to update the dates datatable and set the IsDayOff to true
                                DataTable dayOffs = new DataTable();
                                dayOffs.Columns.Add("Date", typeof(DateTime));
                                if (tbDayOff1.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff1.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }
                                if (tbDayOff2.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff2.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }
                                if (tbDayOff3.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff3.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }
                                if (tbDayOff4.Text.Length > 0)
                                {
                                    DateTime dayOff = Convert.ToDateTime(tbDayOff4.Text);
                                    DataRow dr = dayOffs.NewRow();
                                    dr["Date"] = dayOff;
                                    dayOffs.Rows.Add(dr);
                                }

                                //Merge the two tables and set isdayoff to true for the dayoff dates
                                var commonRows = from dt1 in daysTOWork.AsEnumerable()
                                                 join dt2 in dayOffs.AsEnumerable() on dt1.Field<DateTime>("Date") equals dt2.Field<DateTime>("Date")
                                                 select new { dt1 };
                                foreach (var commonRowInfo in commonRows)
                                {
                                    commonRowInfo.dt1.SetField("IsDayOff", true);
                                }

                                //Delete days preceeding today's date
                                Label lblCreatedOn = (Label)gvEmployees.Rows[i].FindControl("lblCreatedOn");
                                if (lblCreatedOn.Text != "")
                                {
                                    if (Convert.ToDateTime(daysTOWork.Rows[0]["Date"].ToString()) <= DateTime.Now.Date)
                                    {
                                        daysTOWork.Rows.Cast<DataRow>().Where(r => Convert.ToDateTime(r.ItemArray[1]) < Convert.ToDateTime(lblCreatedOn.Text)).ToList().ForEach(r => r.Delete());
                                    }
                                }

                                //String to create a temp table to store the dates
                                string tmpTable = "IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo]. " +                                                                     "[#tbl_Mgr_WorkSchedule_Days_Temp]') AND TYPE IN (N'U')) " +
                                                  "DROP TABLE [dbo].[#tbl_Mgr_WorkSchedule_Days_Temp] " +
                                                  "CREATE TABLE #tbl_Mgr_WorkSchedule_Days_Temp (WorkScheduleDateID BIGINT IDENTITY(1,1), " +                                                        "WorkScheduleEmployeeID BIGINT, [Date] DATE, IsDayOff BIT, CreatedBy TINYINT, " + 
                                                  "CreatedOn DATETIME, LastModifiedBy TINYINT, LastModifiedOn DATETIME)";

                                conn.Open();

                                //Execute the command to create the temp table
                                SqlCommand cmd = new SqlCommand(tmpTable, conn);
                                cmd.ExecuteNonQuery();

                                bool success = false;
                                //Bulk insert into the temp table
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                                {
                                    bulkCopy.ColumnMappings.Add("WorkScheduleEmployeeID", "WorkScheduleEmployeeID");
                                    bulkCopy.ColumnMappings.Add("Date", "Date");
                                    bulkCopy.ColumnMappings.Add("IsDayOff", "IsDayOff");
                                    bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                    bulkCopy.ColumnMappings.Add("CreatedOn", "CreatedOn");
                                    bulkCopy.ColumnMappings.Add("LastModifiedBy", "LastModifiedBy");
                                    bulkCopy.ColumnMappings.Add("LastModifiedOn", "LastModifiedOn");
                                    bulkCopy.BatchSize = 10000;
                                    bulkCopy.DestinationTableName = "#tbl_Mgr_WorkSchedule_Days_Temp";
                                    bulkCopy.WriteToServer(daysTOWork.CreateDataReader());
                                    success = true;
                                }
                                if (success == true)
                                {
                                    //Statement to merge the temp table with the permanent one and either update changed records or insert new ones
                                    string mergeSql =   "MERGE INTO tbl_Mgr_WorkSchedule_Days AS Tgt " +
                                                        "USING #tbl_Mgr_WorkSchedule_Days_Temp AS Src " +
                                                        "ON " +
                                                        "Tgt.WorkScheduleEmployeeID=Src.WorkScheduleEmployeeID AND " +
                                                        "Tgt.[Date]=Src.[Date] " +
                                                        "WHEN MATCHED AND Tgt.IsDayOff != Src.IsDayOff " +
                                                        "THEN " +
                                                            "UPDATE SET Tgt.IsDayOff = Src.IsDayOff, " +
                                                                "Tgt.LastModifiedBy = Src.LastModifiedBy, " +
                                                                "Tgt.LastModifiedOn = Src.LastModifiedOn " +
                                                        "WHEN NOT MATCHED BY TARGET " +
                                                            "THEN " +
                                                                "INSERT (WorkScheduleEmployeeID, [Date], IsDayOff, CreatedBy, CreatedOn) " +
                                                                "VALUES (Src.WorkScheduleEmployeeID, Src.[Date], Src.IsDayOff, Src.CreatedBy, " +                                                                  "Src.CreatedOn);";

                                    cmd.CommandText = mergeSql;
                                    cmd.ExecuteNonQuery();

                                    //Clean up the temp table
                                    cmd.CommandText = "drop table #tbl_Mgr_WorkSchedule_Days_Temp";
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                            }
                        }
                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = wsID;
                            z.TableName = "tbl_Mgr_WorkSchedule";
                            z.AuditProcess = "Updated Work Schedule for " + month + "/" + year + " - " + areaName + " -- " + shifttime;
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

        public void transferEmployee(int oldWSID, int newWSID, byte areaID, byte shiftID, string year, string month, string transferFor, Int16 employeeID, Int16 locationID, string oldAreaName, string oldShifttime, string newAreaName, string newShifttime, string employeeName, string comments, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Transferring for only this schedule
                        if (transferFor == "1")
                        {
                            //Change the schedule the employee is on
                            var recordToUpdate = (from a in model.tbl_Mgr_WorkSchedule_Employees
                                                  where a.WorkScheduleID == oldWSID && a.EmployeeID == employeeID
                                                  select a).First();
                            recordToUpdate.WorkScheduleID = newWSID;
                            recordToUpdate.IsTransfer = true;
                            recordToUpdate.Comments = comments == "" ? null : comments ;
                            if (locationID != 0)
                                recordToUpdate.LocationID = locationID;
                            recordToUpdate.LastModifiedBy = userID;
                            recordToUpdate.LastModifiedOn = DateTime.Now;
                            model.SaveChanges();

                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = newWSID;
                                z.TableName = "tbl_Mgr_WorkSchedule_Employees";
                                z.AuditProcess = "Transfer employee " + employeeName + " only for this schedule from " + month + "/" + year + " - " + oldAreaName + " -- " + oldShifttime + " to " + month + "/" + year + " - " + newAreaName + " -- " + newShifttime;
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        //Transferring for this and all subsequent schedules. This would have to change the employee data too
                        else if (transferFor == "2")
                        {
                            //Change the work schedule
                            var recordToUpdate = (from a in model.tbl_Mgr_WorkSchedule_Employees
                                                  where a.WorkScheduleID == oldWSID && a.EmployeeID == employeeID
                                                  select a).First();
                            recordToUpdate.WorkScheduleID = newWSID;
                            recordToUpdate.IsTransfer = true;
                            recordToUpdate.Comments = comments ;
                            if (locationID != 0)
                                recordToUpdate.LocationID = locationID;
                            recordToUpdate.LastModifiedBy = userID;
                            recordToUpdate.LastModifiedOn = DateTime.Now;
                            model.SaveChanges();

                            //Change the employee data
                            var employeeData = (from b in model.tbl_Sup_Employees
                                                  where b.EmployeeID == employeeID
                                                  select b).First();
                            if (((oldAreaName != newAreaName) && (newAreaName != "")) || locationID != null)
                            {
                                employeeData.AreaID = areaID;
                                employeeData.LocationID = locationID;
                            }
                            if ((oldShifttime != newShifttime) && (newShifttime != ""))
                                employeeData.ShiftID = shiftID;
                            employeeData.LastModifiedBy = userID;
                            employeeData.LastModifiedOn = DateTime.Now;
                            model.SaveChanges();

                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = newWSID;
                                z.TableName = "tbl_Mgr_WorkSchedule_Employees";
                                z.AuditProcess = "Transfer employee " + employeeName + " for this and all subsequent schedules from " + month + "/" + year + " - " + oldAreaName + " -- " + oldShifttime + " to " + month + "/" + year + " - " + newAreaName + " -- " + newShifttime;
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }

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

        public DataTable sp_Employee_SelectDayOffs(object WorkScheduleEmployeeID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_SelectDayOffs";

                    SqlParameter param1 = cmd.Parameters.Add("@WorkScheduleEmployeeID", SqlDbType.BigInt);
                    param1.Value = WorkScheduleEmployeeID;

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

        public DataTable sp_Employee_UnscheduledSelectForWorkSchedule(object wsID, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_UnscheduledSelectForWorkSchedule";

                    SqlParameter param1 = cmd.Parameters.Add("@wsID", SqlDbType.Int);
                    param1.Value = wsID;
                    SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param3.Value = areaID;
                    SqlParameter param4 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                    param4.Value = shiftID;

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

        public DataTable sp_WorkSchedule_AuditSelect(object wsID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WorkSchedule_AuditSelect";

                    SqlParameter param1 = cmd.Parameters.Add("@wsID", SqlDbType.Int);
                    param1.Value = wsID;

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

        //Get illegals
        public List<int> sp_Alerts_Attendance_IllegalEmployees(object daysBeforedateFrom, object dateTo, object areaID, object shiftID, object daysCount, object dateFrom)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_IllegalEmployeesSelect";

                    SqlParameter param0 = cmd.Parameters.Add("@daysBeforedateFrom", SqlDbType.Date);
                    param0.Value = daysBeforedateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }
                    SqlParameter param4 = cmd.Parameters.Add("@daysCount", SqlDbType.TinyInt);
                    param4.Value = daysCount;
                    SqlParameter param5 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param5.Value = dateFrom;

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    List<int> result = new List<int>();

                    try
                    {
                        ConnStr.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds, "ss");
                        dt = ds.Tables["ss"];

                        if ((dt != null) && (dt.Rows.Count != 0))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                result.Add(Convert.ToInt32(dr["EmployeeID"].ToString()));
                            }
                        }
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
                    return result;
                }
            }
        }
    }
}