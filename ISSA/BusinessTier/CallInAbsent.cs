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
    public class CallInAbsent
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_CallTakenBy_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_CallTakenBy_Sel4DDL";

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

        public DataTable sp_CallForwardedTo_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_CallForwardedTo_Sel4DDL";

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

        public DataTable sp_SickLeaveFor_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_SickLeaveFor_Sel4DDL";

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

        

        //Update complaint
        public void update_Absence(int callInOffID, bool isCallIn, Int16 employeeID, DateTime date, TimeSpan time, Int16 callTakenByID, Int16 callForwardedToID, string workSchedule, string phoneNumber, byte absentType, string otherTypeDescription, byte sickLeaveForID, string otherSickLeaveForDescription, string comments, byte userID, string reportNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the record to update
                        var recordToUpdate = (from a in model.tbl_Op_CallIn_Absent
                                              where a.CallInOffID == callInOffID
                                              select a).First();
                        recordToUpdate.IsCallIn = isCallIn;
                        recordToUpdate.EmployeeID = employeeID;
                        recordToUpdate.Date = date;
                        recordToUpdate.Time = time;
                        recordToUpdate.CallTakenByID = callTakenByID == 0 ? null : (Int16?)callTakenByID;
                        recordToUpdate.ForwardedToID = callForwardedToID == 0 ? null : (Int16?)callForwardedToID;
                        recordToUpdate.WorkSchedule = workSchedule == "" ? null : firstCharToUpper(workSchedule);
                        recordToUpdate.PhoneNumber = phoneNumber == "" ? null : phoneNumber;
                        recordToUpdate.AbsentTypeID = absentType;
                        recordToUpdate.OtherTypeDescription = otherTypeDescription == "" ? null : firstCharToUpper(otherTypeDescription);
                        recordToUpdate.SickLeaveForID = sickLeaveForID == 0 ? null : (byte?)sickLeaveForID;
                        recordToUpdate.OtherSickLeaveForTypeDescription = otherSickLeaveForDescription == "" ? null : firstCharToUpper(otherSickLeaveForDescription);
                        recordToUpdate.Comments = comments == "" ? null : firstCharToUpper(comments);
                        recordToUpdate.StartDate = startDate.Date;
                        recordToUpdate.EndDate = endDate.Date;
                        //recordToUpdate.ShiftID = shiftID;
                        recordToUpdate.LastModifiedBy = userID;
                        recordToUpdate.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = callInOffID;
                            z.TableName = "tbl_Op_CallIn_Absent";
                            z.AuditProcess = "Modified Call In/Absent Report - " + reportNumber;
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

        //Insert new absence
        public int insert_Absence(string reportNumber, bool isCallIn, Int16 employeeID, DateTime date, TimeSpan time, Int16 callTakenByID, Int16 callForwardedToID, string workSchedule, string phoneNumber, byte absentType, string otherTypeDescription, byte sickLeaveForID, string otherSickLeaveForDescription, string comments, byte userID, DateTime startDate, DateTime endDate)
        {
            int absenceID = 0;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert transaction
                        EDM.tbl_Op_CallIn_Absent a = new EDM.tbl_Op_CallIn_Absent();
                        {
                            a.ReportNumber = reportNumber;
                            a.IsCallIn = isCallIn;
                            a.EmployeeID = employeeID;
                            a.Date = date;
                            a.Time = time;
                            a.CallTakenByID = callTakenByID == 0 ? null : (Int16?)callTakenByID;
                            a.ForwardedToID = callForwardedToID == 0 ? null : (Int16?)callForwardedToID;
                            a.WorkSchedule = workSchedule == "" ? null : firstCharToUpper(workSchedule);
                            a.PhoneNumber = phoneNumber == "" ? null : phoneNumber;
                            a.AbsentTypeID = absentType;
                            a.OtherTypeDescription = otherTypeDescription == "" ? null : firstCharToUpper(otherTypeDescription);
                            a.SickLeaveForID = sickLeaveForID == 0 ? null : (byte?)sickLeaveForID;
                            a.OtherSickLeaveForTypeDescription = otherSickLeaveForDescription == "" ? null : firstCharToUpper(otherSickLeaveForDescription);
                            a.Comments = comments == "" ? null : firstCharToUpper(comments);
                            a.StartDate = startDate.Date;
                            a.EndDate = endDate.Date;
                            //a.ShiftID = shiftID;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_CallIn_Absent.Add(a);
                        model.SaveChanges();

                        //Get the absenceid
                        absenceID = (from b in model.tbl_Op_CallIn_Absent
                                       where b.ReportNumber == reportNumber
                                       select b.CallInOffID
                         ).FirstOrDefault();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = absenceID;
                            z.TableName = "tbl_Op_CallIn_Absent";
                            z.AuditProcess = "Created Call In/Absent Report - " + reportNumber;
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
                absenceID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return absenceID;
        }

        public DataTable sp_CallInOff_Search(object ReportNumber, object date, object takenByID, object forwardedToID, object employeeID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_CallInOff_Search";

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
                    if (takenByID != null && takenByID.ToString() != "")
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@takenByID", SqlDbType.SmallInt);
                        param3.Value = takenByID;
                    }
                    if (forwardedToID != null && forwardedToID.ToString() != "")
                    {
                        SqlParameter param4 = cmd.Parameters.Add("@forwardedToID", SqlDbType.SmallInt);
                        param4.Value = forwardedToID;
                    }
                    if (employeeID != null && employeeID.ToString() != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@employeeID", SqlDbType.SmallInt);
                        param5.Value = employeeID;
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

    }
}