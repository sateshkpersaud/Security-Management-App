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
    public class DailyLog
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_DailyLog_PersonWorking_Select(object date, object areaID, object locationID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_PersonWorking_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param2.Value = date;
                    SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param3.Value = areaID;
                    SqlParameter param4 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                    param4.Value = locationID;
                    SqlParameter param5 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                    param5.Value = shiftID;

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

        public DataTable sp_DailyLog_ActualPersonWorking_Select(object dlID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_ActualPersonWorking_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@dlID", SqlDbType.Int);
                    param2.Value = dlID;

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

        //Get all standby and other employees schedule d for shift
        public DataTable sp_DailyLog_Switches_Select(object date, object areaID, object locationID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_Switches_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param2.Value = date;
                    SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param3.Value = areaID;
                    SqlParameter param4 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                    param4.Value = locationID;
                    SqlParameter param5 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                    param5.Value = shiftID;

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

        //Get all scitches saved
        public DataTable sp_DailyLog_SwitchesWorking_Select(object dlID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_SwitchesWorking_Select";

                    SqlParameter param5 = cmd.Parameters.Add("@dlID", SqlDbType.Int);
                    param5.Value = dlID;

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

        //insert DAILYlOG
        public int insert_dailyLog(DateTime date, byte areaID, Int16 locationID, byte shiftID, GridView personsWorking, string areaName, string locationName, string shiftTime, string logNumber, byte userID, int workScheduleID)
        {
            int dlID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert dailylog header
                        EDM.tbl_Op_DailyLogs a = new EDM.tbl_Op_DailyLogs();
                        {
                            a.Date = date;
                            a.AreaID = areaID;
                            a.LocationID = locationID;
                            a.ShiftID = shiftID;
                            a.LogNumber = logNumber;
                            a.WorkScheduleID = workScheduleID;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_DailyLogs.Add(a);
                        model.SaveChanges();

                        //Get the id
                        dlID = (from b in model.tbl_Op_DailyLogs
                                where b.LogNumber == logNumber
                                select b.DailyLogID
                         ).FirstOrDefault();

                        if (dlID != 0)
                        {
                            //insert persons working
                            for (int i = 0; i < personsWorking.Rows.Count; i++)
                            {
                                Label lblEmployeeID = (Label)personsWorking.Rows[i].FindControl("lblEmployeeID");
                                Label lblIsPresent = (Label)personsWorking.Rows[i].FindControl("lblIsPresent");
                                Label lblFullName = (Label)personsWorking.Rows[i].FindControl("lblFullName");
                                CheckBox cbPresent = (CheckBox)personsWorking.Rows[i].FindControl("cbPresent");

                                Int16 empID = new Int16();
                                empID = Convert.ToInt16(lblEmployeeID.Text);

                                EDM.tbl_Op_DailyLogsPersonWorking c = new EDM.tbl_Op_DailyLogsPersonWorking();
                                {
                                    c.DailyLogID = dlID;
                                    c.EmployeeID = empID;
                                    c.IsPresent = cbPresent.Checked ? true : false;
                                    c.TimeClocked = DateTime.Now;
                                    c.IsDouble = false;
                                    c.IsSwitch = false;
                                    c.CreatedBy = userID;
                                    c.CreatedOn = DateTime.Now;
                                };
                                model.tbl_Op_DailyLogsPersonWorking.Add(c);
                                model.SaveChanges();

                                if (((lblIsPresent.Text == "1") || (lblIsPresent.Text == "True")) && (cbPresent.Checked == false))
                                {
                                    //Insert AuditTrail
                                    EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                                    {
                                        x.SourceID = dlID;
                                        x.TableName = "tbl_Op_DailyLogs";
                                        x.AuditProcess = "Checked " + lblFullName .Text + " as Present on Daily Log (" + logNumber + ") for " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                        x.DateTime = DateTime.Now;
                                        x.UserID = userID;
                                    };
                                    model.tbl_AuditLog.Add(x);
                                    model.SaveChanges();
                                }
                                else if (((lblIsPresent.Text == "0") || (lblIsPresent.Text == "False")) && (cbPresent.Checked == true))
                                     {
                                    //Insert AuditTrail
                                    EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                                    {
                                        x.SourceID = dlID;
                                        x.TableName = "tbl_Op_DailyLogs";
                                        x.AuditProcess = "Unchecked " + lblFullName .Text + " as Present from Daily Log (" + logNumber + ") for " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                        x.DateTime = DateTime.Now;
                                        x.UserID = userID;
                                    };
                                    model.tbl_AuditLog.Add(x);
                                    model.SaveChanges();
                                }
                            }

                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = dlID;
                                z.TableName = "tbl_Op_DailyLogs";
                                z.AuditProcess = "Created Daily Log (" + logNumber + ") for " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            dlID = 0;
                            successfulCommit = false;
                            MISC.writetoAlertLog("No daily log ID returned after insert!!");
                        }
                    }
                    if (dlID != 0)
                    {
                        //commit transaction
                        ts.Complete();
                        successfulCommit = true;
                    }
                }
            }
            catch (Exception exec)
            {
                dlID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return dlID;
        }

        public void update_DailyLog(int dailyLogID, DateTime date, byte areaID, Int16 locationID, byte shiftID, GridView personsWorking, string areaName, string locationName, string shiftTime, string logNumber, byte userID, int workScheduleID, GridView switches, GridView doubles)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from a in model.tbl_Op_DailyLogs
                                  where a.DailyLogID == dailyLogID
                                  select a).First();
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //update employees
                        for (int i = 0; i < personsWorking.Rows.Count; i++)
                        {
                            Label lblEmployeeID = (Label)personsWorking.Rows[i].FindControl("lblEmployeeID");
                            Label lblPersonWorkingID = (Label)personsWorking.Rows[i].FindControl("lblPersonWorkingID");
                            Label lblIsPresent = (Label)personsWorking.Rows[i].FindControl("lblIsPresent");
                            Label lblFullName = (Label)personsWorking.Rows[i].FindControl("lblFullName");
                            CheckBox cbPresent = (CheckBox)personsWorking.Rows[i].FindControl("cbPresent");

                            Int64 personWorkingID = new Int64();

                            if (lblPersonWorkingID.Text != "")
                            {
                                //Update employee if already exist
                                personWorkingID = Convert.ToInt64(lblPersonWorkingID.Text);
                                //Get the record to update
                                var recordToUpdate = (from b in model.tbl_Op_DailyLogsPersonWorking
                                                      where b.PersonWorkingID == personWorkingID
                                                      select b).First();
                                recordToUpdate.IsPresent = cbPresent.Checked;
                                recordToUpdate.LastModifiedBy = userID;
                                recordToUpdate.LastModifiedOn = DateTime.Now;
                                model.SaveChanges();
                            }
                            else
                            {
                                //Insert employee if they do not exist
                                Int16 empID = new Int16();
                                empID = Convert.ToInt16(lblEmployeeID.Text);

                                EDM.tbl_Op_DailyLogsPersonWorking c = new EDM.tbl_Op_DailyLogsPersonWorking();
                                {
                                    c.DailyLogID = dailyLogID;
                                    c.EmployeeID = empID;
                                    c.IsPresent = cbPresent.Checked ? true : false;
                                    c.TimeClocked = DateTime.Now;
                                    c.IsDouble = false;
                                    c.IsSwitch = false;
                                    c.CreatedBy = userID;
                                    c.CreatedOn = DateTime.Now;
                                };
                                model.tbl_Op_DailyLogsPersonWorking.Add(c);
                                model.SaveChanges();
                            }

                            //Check if employee was checked and is no unchecked and vice versa and save in audit trail
                            if (((lblIsPresent.Text == "1") || (lblIsPresent.Text == "True")) && (cbPresent.Checked == false))
                            {
                                //Insert AuditTrail
                                EDM.tbl_AuditLog xx = new EDM.tbl_AuditLog();
                                {
                                    xx.SourceID = dailyLogID;
                                    xx.TableName = "tbl_Op_DailyLogs";
                                    xx.AuditProcess = "Unchecked " + lblFullName.Text + " as Present from Daily Log (" + logNumber + ") for " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                    xx.DateTime = DateTime.Now;
                                    xx.UserID = userID;
                                };
                                model.tbl_AuditLog.Add(xx);
                                model.SaveChanges();
                            }
                            else if (((lblIsPresent.Text == "0") || (lblIsPresent.Text == "False")) && (cbPresent.Checked == true))
                            {
                                //Insert AuditTrail
                                EDM.tbl_AuditLog xx = new EDM.tbl_AuditLog();
                                {
                                    xx.SourceID = dailyLogID;
                                    xx.TableName = "tbl_Op_DailyLogs";
                                    xx.AuditProcess = "Checked " + lblFullName.Text + " as Present for Daily Log (" + logNumber + ") for " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                    xx.DateTime = DateTime.Now;
                                    xx.UserID = userID;
                                };
                                model.tbl_AuditLog.Add(xx);
                                model.SaveChanges();
                            }
                        }

                        //insert/delete switches
                        for (int i = 0; i < switches.Rows.Count; i++)
                        {
                            Label lblSwitchID = (Label)switches.Rows[i].FindControl("lblSwitchID");
                            Label lblEmployeeID = (Label)switches.Rows[i].FindControl("lblEmployeeID");
                            Label lblFullName = (Label)switches.Rows[i].FindControl("lblFullName");
                            CheckBox cbPresent = (CheckBox)switches.Rows[i].FindControl("cbPresent");

                            if ((lblSwitchID.Text == "") && (cbPresent.Checked))
                            {
                                Int16 empID = new Int16();
                                empID = Convert.ToInt16(lblEmployeeID.Text);

                                //insert
                                EDM.tbl_Op_DailyLogsPersonWorking c = new EDM.tbl_Op_DailyLogsPersonWorking();
                                {
                                    c.DailyLogID = dailyLogID;
                                    c.EmployeeID = empID;
                                    c.IsPresent = cbPresent.Checked;
                                    c.TimeClocked = DateTime.Now;
                                    c.IsDouble = false;
                                    c.IsSwitch = true;
                                    c.CreatedBy = userID;
                                    c.CreatedOn = DateTime.Now;
                                };
                                model.tbl_Op_DailyLogsPersonWorking.Add(c);
                                model.SaveChanges();

                                //Insert AuditTrail
                                EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                                {
                                    z.SourceID = dailyLogID;
                                    z.TableName = "tbl_Op_DailyLogs";
                                    z.AuditProcess = "Inserted Switch (" + lblFullName.Text + ") to Daily Log (" + logNumber + ") " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                    z.DateTime = DateTime.Now;
                                    z.UserID = userID;
                                };
                                model.tbl_AuditLog.Add(z);
                                model.SaveChanges();
                            }
                            else if ((lblSwitchID.Text != "") && (!cbPresent.Checked))
                            {
                                //delete
                                int switchID = new int();
                                switchID = Convert.ToInt32(lblSwitchID.Text);

                                var swEmp = (from a in model.tbl_Op_DailyLogsPersonWorking
                                                  where a.PersonWorkingID == switchID && a.IsSwitch == true
                                                  select a).SingleOrDefault();

                                if (swEmp != null)
                                {
                                    model.tbl_Op_DailyLogsPersonWorking.Remove(swEmp);
                                    model.SaveChanges();

                                    //Insert AuditTrail
                                    EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                                    {
                                        z.SourceID = dailyLogID;
                                        z.TableName = "tbl_Op_DailyLogs";
                                        z.AuditProcess = "Removed Switch (" + lblFullName.Text + ") from Daily Log (" + logNumber + ") " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                        z.DateTime = DateTime.Now;
                                        z.UserID = userID;
                                    };
                                    model.tbl_AuditLog.Add(z);
                                    model.SaveChanges();
                                }
                            }
                        }

                        //delete doubles
                        for (int i = 0; i < doubles.Rows.Count; i++)
                        {
                            Label lblDoubleID = (Label)doubles.Rows[i].FindControl("lblDoubleID");
                            Label lblEmployeeID = (Label)doubles.Rows[i].FindControl("lblEmployeeID");
                            Label lblFullName = (Label)doubles.Rows[i].FindControl("lblFullName");
                            CheckBox cbPresent = (CheckBox)doubles.Rows[i].FindControl("cbPresent");

                            if (!cbPresent.Checked)
                            {
                                //delete
                                int doubleID = new int();
                                doubleID = Convert.ToInt32(lblDoubleID.Text);

                                var swEmp = (from a in model.tbl_Op_DailyLogsPersonWorking
                                             where a.PersonWorkingID == doubleID && a.IsDouble == true
                                             select a).SingleOrDefault();

                                if (swEmp != null)
                                {
                                    model.tbl_Op_DailyLogsPersonWorking.Remove(swEmp);
                                    model.SaveChanges();

                                    //Insert AuditTrail
                                    EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                                    {
                                        z.SourceID = dailyLogID;
                                        z.TableName = "tbl_Op_DailyLogs";
                                        z.AuditProcess = "Removed Double (" + lblFullName.Text + ") from Daily Log (" + logNumber + ") " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                                        z.DateTime = DateTime.Now;
                                        z.UserID = userID;
                                    };
                                    model.tbl_AuditLog.Add(z);
                                    model.SaveChanges();
                                }
                            }
                        }

                        //Insert AuditTrail
                        EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                        {
                            x.SourceID = dailyLogID;
                            x.TableName = "tbl_Op_DailyLogs";
                            x.AuditProcess = "Modified Daily Log (" + logNumber + ") for " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                            x.DateTime = DateTime.Now;
                            x.UserID = userID;
                        };
                        model.tbl_AuditLog.Add(x);
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


         //insert Call Log
        public void insert_CallLog(int dailyLogID, TimeSpan time, string reports, bool supCheckLocation, DateTime date, string areaName, string locationName, string shiftTime, byte userID, string logNumber)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from t in model.tbl_Op_DailyLogs
                                  where t.DailyLogID == dailyLogID
                                  select t).First();
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert call log header
                        EDM.tbl_Op_DailyLogsCallLog a = new EDM.tbl_Op_DailyLogsCallLog();
                        {
                            a.DailyLogID = dailyLogID;
                            a.Time = time;
                            a.Reports = reports;
                            a.SupCheckLocation = supCheckLocation;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_DailyLogsCallLog.Add(a);
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = dailyLogID;
                            z.TableName = "tbl_Op_DailyLogs";
                            z.AuditProcess = "Created Call Log for Daily Log (" + logNumber + ") " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
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

        public void update_CallLog(Int64 callLogID, int dailyLogID, TimeSpan time, string reports, bool supCheckLocation, DateTime date, string areaName, string locationName, string shiftTime, byte userID, string logNumber)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from t in model.tbl_Op_DailyLogs
                                  where t.DailyLogID == dailyLogID
                                  select t).First();
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Update call log table
                        var callLog = (from a in model.tbl_Op_DailyLogsCallLog
                                       where a.CallLogID == callLogID
                                       select a).First();
                        callLog.SupCheckLocation = supCheckLocation;
                        callLog.Reports = reports;
                        callLog.LastModifiedBy = userID;
                        callLog.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();


                        //Insert AuditTrail
                        EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                        {
                            x.SourceID = dailyLogID;
                            x.TableName = "tbl_Op_DailyLogs";
                            x.AuditProcess = "Modified Call Log for Daily Log (" + logNumber + ") " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
                            x.DateTime = DateTime.Now;
                            x.UserID = userID;
                        };
                        model.tbl_AuditLog.Add(x);
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

        //Load the call logs
        public DataTable sp_DailyLog_CallLog_Select(object dailyLogID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_CallLog_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@dailyLogID", SqlDbType.Int);
                    param2.Value = dailyLogID;

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

        //Get last check location and time
        public DataTable sp_DailyLog_LastCheckLocationAndTime(object date, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_LastCheckLocationAndTime";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.DateTime);
                    param1.Value = date;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                    param3.Value = shiftID;

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

        //Get last time for selected daily log
        public DataTable sp_DailyLog_LastCheckTime(object dailyLogID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_LastCheckTime";

                    SqlParameter param2 = cmd.Parameters.Add("@dailyLogID", SqlDbType.Int);
                    param2.Value = dailyLogID;

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

        //Get the audit log details for the daily log
        public DataTable sp_DailyLog_Audit_Select(object dailyLogID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_Audit_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@dailyLogID", SqlDbType.Int);
                    param2.Value = dailyLogID;

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

        //Get all employees who worked the previous shift
        public DataTable sp_DailyLog_Doubles_Select(object date, object areaID, object fromTime, object toTime, string name)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_Doubles_Select";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.DateTime);
                    param1.Value = date;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    SqlParameter param3 = cmd.Parameters.Add("@fromTime", SqlDbType.Time,7);
                    param3.Value = fromTime;
                    SqlParameter param4 = cmd.Parameters.Add("@toTime", SqlDbType.Time,7);
                    param4.Value = toTime;
                    if (name != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        param5.Value = name;
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

        //Get all employees who worked the previous shift
        public DataTable sp_DailyLog_Doubles_SelectAllEmployees(string name)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_Doubles_SelectAllEmployees";

                    if (name != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        param5.Value = name;
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

        //Get all employees on doubles for selected log
        public DataTable sp_DailyLog_DoublesWorking_Select(object dailyLogID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_DoublesWorking_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@dailyLogID", SqlDbType.Int);
                    param2.Value = dailyLogID;

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

        //insert doubles
        public void insert_double(int dailyLogID, Int16 employeeID, DateTime date, string areaName, string locationName, string shiftTime, byte userID, string logNumber)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from t in model.tbl_Op_DailyLogs
                                  where t.DailyLogID == dailyLogID
                                  select t).First();
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert doubles
                        EDM.tbl_Op_DailyLogsPersonWorking a = new EDM.tbl_Op_DailyLogsPersonWorking();
                        {
                            a.DailyLogID = dailyLogID;
                            a.EmployeeID = employeeID;
                            a.IsPresent = true;
                            a.TimeClocked = DateTime.Now;
                            a.IsDouble = true;
                            a.IsSwitch = false;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_DailyLogsPersonWorking.Add(a);
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = dailyLogID;
                            z.TableName = "tbl_Op_DailyLogs";
                            z.AuditProcess = "Added Double to Daily Log (" + logNumber + ") " + date.Date.ToString("dd/MM/yyyy") + ", " + areaName + " - " + locationName + " -- " + shiftTime;
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

        //Search for daily logs created
        public DataTable sp_DailyLog_Search(object ReportNumber, object date, object areaID, object locationID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyLog_Search";

                    if (ReportNumber != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@ReportNumber", SqlDbType.VarChar, 20);
                        param1.Value = ReportNumber;
                    }
                    if (date != null && date.ToString() != "")
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@date", SqlDbType.Date);
                        param2.Value = date;
                    }
                    if (areaID != null && areaID.ToString() != "")
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param3.Value = areaID;
                    }
                    if (locationID != null && locationID.ToString() != "")
                    {
                        SqlParameter param4 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param4.Value = locationID;
                    }
                    if (shiftID != null && shiftID.ToString() != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param5.Value = shiftID;
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
    }
}