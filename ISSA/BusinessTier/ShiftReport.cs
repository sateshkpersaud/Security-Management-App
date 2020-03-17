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
    public class ShiftReport
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public int insert_ShiftReport(DateTime date, byte areaID, byte shiftID, Int16 supervisorID, string reportNumber, byte userID)
        {
            int shiftReportID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Sup_ShiftReport a = new EDM.tbl_Sup_ShiftReport();
                        {
                            a.Date = date;
                            a.AreaID = areaID;
                            a.ShiftID = shiftID;
                            a.ReportNumber = reportNumber;
                            a.SupervisorID = supervisorID;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Sup_ShiftReport.Add(a);
                        model.SaveChanges();

                        //Get the id
                        shiftReportID = (from b in model.tbl_Sup_ShiftReport
                                where b.ReportNumber == reportNumber
                                select b.ShiftReportID
                         ).FirstOrDefault();

                        if (shiftReportID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = shiftReportID;
                                z.TableName = "tbl_Sup_ShiftReport";
                                z.AuditProcess = "Created Shift Report " + reportNumber;
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            shiftReportID = 0;
                            successfulCommit = false;
                            MISC.writetoAlertLog("No shift report ID returned after insert!!");
                        }
                    }
                    if (shiftReportID != 0)
                    {
                        //commit transaction
                        ts.Complete();
                        successfulCommit = true;
                    }
                }
            }
            catch (Exception exec)
            {
                shiftReportID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return shiftReportID;
        }

        public void update_ShiftReport(int shiftReportID, byte areaID, byte shiftID, Int16 supervisorID, string reportNumber, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from a in model.tbl_Sup_ShiftReport
                                  where a.ShiftReportID == shiftReportID
                                  select a).First();
                        dl.SupervisorID = supervisorID;
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                      
                        //Insert AuditTrail
                        EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                        {
                            x.SourceID = shiftReportID;
                            x.TableName = "tbl_Sup_ShiftReport";
                            x.AuditProcess = "Modified Shift Report " + reportNumber;
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

        public DataTable sp_ShiftReports_Search(object ReportNumber, object date, object areaID, object shiftID, object supervisorID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_ShiftReports_Search";

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
                    if (supervisorID != null && supervisorID.ToString() != "")
                    {
                        SqlParameter param4 = cmd.Parameters.Add("@supervisorID", SqlDbType.SmallInt);
                        param4.Value = supervisorID;
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

        public void insert_Check(int shiftReportID, Int16 locationID, byte checkSequence, string generalObservations, string correctionsMade, string reportNumber, GridView Officers, byte userID)
        {
            try
            {
                Int64 checksID = new Int64();

                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //update header
                        var dl = (from a in model.tbl_Sup_ShiftReport
                                  where a.ShiftReportID == shiftReportID
                                  select a).First();
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert Chesk
                        EDM.tbl_Sup_ShiftReportChecks b = new EDM.tbl_Sup_ShiftReportChecks();
                        {
                            b.ShiftReportID = shiftReportID;
                            b.LocationID = locationID;
                            b.CheckSequence = checkSequence;
                            b.GeneralObservations = generalObservations;
                            b.CorrectionsMadeSuggested = correctionsMade;
                            b.CreatedBy = userID;
                            b.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Sup_ShiftReportChecks.Add(b);
                        model.SaveChanges();

                         //Get the id
                        checksID = b.ChecksID;

                        if (checksID != 0)
                        {
                            //insert officers
                            for (int i = 0; i < Officers.Rows.Count; i++)
                            {
                                DropDownList ddlOfficerName = (DropDownList)Officers.Rows[i].FindControl("ddlOfficerName");
                                TextBox tbTime = (TextBox)Officers.Rows[i].FindControl("tbTime");

                                if ((ddlOfficerName.SelectedIndex > 0) && (tbTime.Text != ""))
                                {
                                    Int16 empID = Convert.ToInt16(ddlOfficerName.SelectedValue);
                                    TimeSpan time = new TimeSpan();
                                    TimeSpan.TryParse(tbTime.Text, out time);

                                    EDM.tbl_Sup_ShiftReportChecks_Officers c = new EDM.tbl_Sup_ShiftReportChecks_Officers();
                                    {
                                        c.ChecksID = checksID;
                                        c.EmployeeID = empID;
                                        c.Time = time;
                                        c.CreatedBy = userID;
                                        c.CreatedOn = DateTime.Now;
                                    };
                                    model.tbl_Sup_ShiftReportChecks_Officers.Add(c);
                                    model.SaveChanges();
                                }
                            }

                            //Insert AuditTrail
                            EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                            {
                                x.SourceID = shiftReportID;
                                x.TableName = "tbl_Sup_ShiftReport";
                                x.AuditProcess = "Inserted Check for Shift Report " + reportNumber;
                                x.DateTime = DateTime.Now;
                                x.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(x);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No Checks ID returned after insert!!");
                        }
                    }
                    if (checksID != 0)
                    {
                        //commit transaction
                        ts.Complete();
                        successfulCommit = true;
                    }
                }
            }
            catch (Exception exec)
            {
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
        }

        public void update_Check(int checksID, int shiftReportID, Int16 locationID, byte checkSequence, string generalObservations, string correctionsMade, string reportNumber, GridView Officers, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from a in model.tbl_Sup_ShiftReport
                                  where a.ShiftReportID == shiftReportID
                                  select a).First();
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Update Checks table
                        var checks = (from b in model.tbl_Sup_ShiftReportChecks
                                      where b.ChecksID == checksID
                                      select b).First();
                        checks.LocationID = locationID;
                        checks.CheckSequence = checkSequence;
                        checks.GeneralObservations = generalObservations;
                        checks.CorrectionsMadeSuggested = correctionsMade;
                        checks.LastModifiedBy = userID;
                        checks.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Select all officers
                        var officers = (from c in model.tbl_Sup_ShiftReportChecks_Officers
                                        where c.ChecksID == checksID
                                        select c).ToList();

                        if (officers != null)
                        {
                            //remove selected officers
                            foreach (var val in officers)
                            {
                                model.tbl_Sup_ShiftReportChecks_Officers.Remove(val);
                                model.SaveChanges();
                            }

                            //insert new officers
                            for (int i = 0; i < Officers.Rows.Count; i++)
                            {
                                DropDownList ddlOfficerName = (DropDownList)Officers.Rows[i].FindControl("ddlOfficerName");
                                TextBox tbTime = (TextBox)Officers.Rows[i].FindControl("tbTime");

                                if ((ddlOfficerName.SelectedIndex > 0) && (tbTime.Text != ""))
                                {
                                    Int16 empID = Convert.ToInt16(ddlOfficerName.SelectedValue);
                                    TimeSpan time = new TimeSpan();
                                    TimeSpan.TryParse(tbTime.Text, out time);

                                    EDM.tbl_Sup_ShiftReportChecks_Officers c = new EDM.tbl_Sup_ShiftReportChecks_Officers();
                                    {
                                        c.ChecksID = checksID;
                                        c.EmployeeID = empID;
                                        c.Time = time;
                                        c.CreatedBy = userID;
                                        c.CreatedOn = DateTime.Now;
                                    };
                                    model.tbl_Sup_ShiftReportChecks_Officers.Add(c);
                                    model.SaveChanges();
                                }
                            }
                        }
                        //Insert AuditTrail
                        EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                        {
                            x.SourceID = shiftReportID;
                            x.TableName = "tbl_Sup_ShiftReport";
                            x.AuditProcess = "Updated Check for Shift Report " + reportNumber;
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

        public DataTable sp_shiftReport_Checks_Select(object shiftReportID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_shiftReport_Checks_Select";

                    SqlParameter param2 = cmd.Parameters.Add("@shiftReportID", SqlDbType.Int);
                    param2.Value = shiftReportID;

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

        public DataTable sp_ShiftReport_Checks_GetOfficers(object checksID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_ShiftReport_Checks_GetOfficers";

                    SqlParameter param2 = cmd.Parameters.Add("@checksID", SqlDbType.BigInt);
                    param2.Value = checksID;

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