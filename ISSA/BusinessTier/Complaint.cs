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
    public class Complaint
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_Acknowledgement_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Acknowledgement_Sel4DDL";

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

        public DataTable sp_ComplaintMode_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_ComplaintMode_Sel4DDL";

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

        public DataTable sp_Employee_Sel4DDL_ByPositionID(object positionID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_Sel4DDL_ByPositionID";

                    SqlParameter param1 = cmd.Parameters.Add("@positionID", SqlDbType.TinyInt);
                    param1.Value = positionID;

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

        public DataTable sp_ComplaintReceivedBy_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_ComplaintReceivedBy_Sel4DDL";

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

        public DataTable sp_Complaints_Search(object ReportNumber, object date, object areaID, object locationID, object complaintModeID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Complaints_Search";

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
                    if (complaintModeID != null && complaintModeID.ToString() != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@channelID", SqlDbType.TinyInt);
                        param5.Value = complaintModeID;
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

        //Insert new complaint
        public int insert_Complaint(byte complaintChannelID, string personMakingReport, string contactNumber, byte areaID, Int16 locationID, DateTime dateOfIncident, TimeSpan timeofIncident, Int16 receivedByID, DateTime dateReceived, byte acknowledgementID, string clientcomplaint, bool feedbackProvided, DateTime dateOfFeedback, string actionTaken, string complaintNumber, byte userID, byte shiftID)
        {
            int complaintID = 0;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert transaction
                        EDM.tbl_Op_Complaints a = new EDM.tbl_Op_Complaints();
                        {
                            a.ComplaintNumber = complaintNumber;
                            a.ComplaintChannelID = complaintChannelID;
                            a.DateOfIncident = dateOfIncident;
                            a.TimeOfIncident = timeofIncident;
                            a.AreaID = areaID;
                            a.LocationID = locationID;
                            a.PersonMakingReport = firstCharToUpper(personMakingReport);
                            a.ContactNumber = contactNumber.Length > 0 ? contactNumber : null;
                            a.ReceivedByID = receivedByID;
                            a.DateReceived = dateReceived;
                            a.ShiftID = shiftID;
                            a.AcknowledgementID = acknowledgementID == 0 ? null : (byte?)acknowledgementID;
                            a.ClientComplaint = clientcomplaint.Length > 0 ? firstCharToUpper(clientcomplaint) : null;
                            a.FeedbackProvided = feedbackProvided;
                            a.DateOfFeedback = dateOfFeedback.ToString() == "1/1/0001 12:00:00 AM" ? null : (DateTime?)dateOfFeedback;
                            a.ActionTaken = actionTaken.Length > 0 ? firstCharToUpper(actionTaken) : null;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_Complaints.Add(a);
                        model.SaveChanges();

                        //Get the incidentID
                        complaintID = (from b in model.tbl_Op_Complaints
                                      where b.ComplaintNumber == complaintNumber
                                      select b.ComplaintID
                         ).FirstOrDefault();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = complaintID;
                            z.TableName = "tbl_Op_Complaints";
                            z.AuditProcess = "Created Complaint Report - " + complaintNumber;
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
                complaintID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return complaintID;
        }

        //Update complaint
        public void update_Complaint(int complaintID, byte complaintChannelID, string personMakingReport, string contactNumber, byte areaID, Int16 locationID, DateTime dateOfIncident, TimeSpan timeofIncident, Int16 receivedByID, DateTime dateReceived, byte acknowledgementID, string clientcomplaint, bool feedbackProvided, DateTime dateOfFeedback, string actionTaken, byte userID, string complaintNumber, byte shiftID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the record to update
                        var recordToUpdate = (from a in model.tbl_Op_Complaints
                                              where a.ComplaintID == complaintID
                                              select a).First();
                        recordToUpdate.ComplaintChannelID = complaintChannelID;
                        recordToUpdate.DateOfIncident = dateOfIncident;
                        recordToUpdate.TimeOfIncident = timeofIncident;
                        recordToUpdate.AreaID = areaID;
                        recordToUpdate.LocationID = locationID;
                        recordToUpdate.PersonMakingReport = firstCharToUpper(personMakingReport);
                        recordToUpdate.ContactNumber = contactNumber.Length > 0 ? contactNumber : null;
                        recordToUpdate.ReceivedByID = receivedByID;
                        recordToUpdate.DateReceived = dateReceived;
                        recordToUpdate.ShiftID = shiftID;
                        recordToUpdate.AcknowledgementID = acknowledgementID == 0 ? null : (byte?)acknowledgementID;
                        recordToUpdate.ClientComplaint = clientcomplaint.Length > 0 ? firstCharToUpper(clientcomplaint) : null;
                        recordToUpdate.FeedbackProvided = feedbackProvided;
                        recordToUpdate.DateOfFeedback = dateOfFeedback.ToString() == "1/1/0001 12:00:00 AM" ? null : (DateTime?)dateOfFeedback;
                        recordToUpdate.ActionTaken = actionTaken.Length > 0 ? firstCharToUpper(actionTaken) : null;
                        recordToUpdate.LastModifiedBy = userID;
                        recordToUpdate.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = complaintID;
                            z.TableName = "tbl_Op_Complaints";
                            z.AuditProcess = "Modified Complaint Report - " + complaintNumber;
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

        public string firstCharToUpper(string input)
        {
            string val = "";
            if (!String.IsNullOrEmpty(input))
                val = input.First().ToString().ToUpper() + input.Substring(1);

            return val;
        }

    }
}