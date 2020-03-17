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
    public class SpecialOperations
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        //Update employee
        public void update_SO(int specialOperationsID, byte areaID, Int16 locationID, DateTime date, bool escortService, bool cashInGT, bool cashOutGT, bool extraOfficers, byte amountOfficers, string reportNumber, string comments, byte userID, byte shiftID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the record to update
                        var a = (from y in model.tbl_Mgr_SpecialOperations
                                 where y.SpecialOperationsID == specialOperationsID
                                 select y).First();
                        a.AreaID = areaID == 0 ? null : (byte?)areaID;
                        a.LocationID = locationID == 0 ? null : (byte?)locationID;
                        a.Date = date;
                        a.EscortService = escortService;
                        a.CashInGT = cashInGT;
                        a.CashOutGT = cashOutGT;
                        a.ExtraOfficers = extraOfficers;
                        a.AmountOfficers = amountOfficers == 0 ? null : (byte?)amountOfficers;
                        a.ReportNumber = reportNumber;
                        a.Comments = comments == "" ? null : comments;
                        a.ShiftID = shiftID == 0 ? null : (byte?)shiftID;
                        a.LastModifiedBy = userID;
                        a.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = specialOperationsID;
                            z.TableName = "tbl_Mgr_SpecialOperations";
                            z.AuditProcess = "Modified Special Operations Log - " + reportNumber;
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
        public int insert_SO(byte areaID, Int16 locationID, DateTime date, bool escortService, bool cashInGT, bool cashOutGT, bool extraOfficers, byte amountOfficers, string reportNumber, string comments, byte userID, byte shiftID)
        {
            int soID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert transaction
                        EDM.tbl_Mgr_SpecialOperations a = new EDM.tbl_Mgr_SpecialOperations();
                        {
                            a.AreaID = areaID == 0 ? null : (byte?)areaID;
                            a.LocationID = locationID == 0 ? null : (byte?)locationID;
                            a.Date = date;
                            a.EscortService = escortService;
                            a.CashInGT = cashInGT;
                            a.CashOutGT = cashOutGT;
                            a.ExtraOfficers = extraOfficers;
                            a.AmountOfficers = amountOfficers == 0 ? null : (byte?)amountOfficers;
                            a.ReportNumber = reportNumber;
                            a.Comments = comments == "" ? null : comments;
                            a.ShiftID = shiftID == 0 ? null : (byte?)shiftID;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Mgr_SpecialOperations.Add(a);
                        model.SaveChanges();

                        //Get the absenceid
                        soID = (from b in model.tbl_Mgr_SpecialOperations
                                      where b.ReportNumber == reportNumber
                                      select b.SpecialOperationsID
                         ).FirstOrDefault();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = soID;
                            z.TableName = "tbl_Mgr_SpecialOperations";
                            z.AuditProcess = "Created SO Report - " + reportNumber;
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
                soID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return soID;
        }

        public DataTable sp_SpecialOperations_Search(object date, object areaID, object locationID, object escortService, object cit, object extraOfficers)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_SpecialOperations_Search";

                    if (date != null && date.ToString() != "")
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                        param1.Value = date;
                    }
                    if (areaID != null && areaID.ToString() != "")
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param2.Value = areaID;
                    }
                    if (locationID != null && locationID.ToString() != "")
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param3.Value = locationID;
                    }
                    if (escortService != null && escortService.ToString() != "")
                    {
                        bool val = false;
                        if (escortService.ToString() == "1")
                            val = true;
                        else
                            val = false;
                        SqlParameter param4 = cmd.Parameters.Add("@escortService", SqlDbType.Bit);
                        param4.Value = val;
                    }
                    if (cit != null && cit.ToString() != "")
                    {
                        bool val = false;
                        if (cit.ToString() == "1")
                        {
                            val = true;
                            SqlParameter param5 = cmd.Parameters.Add("@citInGT", SqlDbType.Bit);
                            param5.Value = val;
                        }
                        else
                        {
                            val = true;
                            SqlParameter param5 = cmd.Parameters.Add("@citOutGT", SqlDbType.Bit);
                            param5.Value = val;
                        }
                    }
                    if (extraOfficers != null && extraOfficers.ToString() != "")
                    {
                        bool val = false;
                        if (extraOfficers.ToString() == "1")
                            val = true;
                        else
                            val = false;
                        SqlParameter param6 = cmd.Parameters.Add("@extraOfficers", SqlDbType.Bit);
                        param6.Value = val;
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