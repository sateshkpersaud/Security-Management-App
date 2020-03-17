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
    public class DailyPostAssignment
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        //Get officers who called in for day off
        public int sp_DailyPostStats_OfficersCalledInForDayOff(object date, object shiftID)
        {
            int count = 0;

            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_OfficersCalledInForDayOff";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            count = Convert.ToInt32(dt.Rows[0]["officersCount"]);
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
                    return count;
                }
            }
        }

        //Get officers present
        public int sp_DailyPostStats_OfficersPresent(object date, object shiftID)
        {
            int count = 0;

            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_OfficersPresent";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            count = Convert.ToInt32(dt.Rows[0]["officersCount"]);
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
                    return count;
                }
            }
        }

        //get officers called in to work on day off
        public int sp_DailyPostStats_OfficersCalledInOnDayOff(object date, object shiftID)
        {
            int count = 0;

            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_OfficersCalledInOnDayOff";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            count = Convert.ToInt32(dt.Rows[0]["officersCount"]);
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
                    return count;
                }
            }
        }

        //get officers scheduled
        public int sp_DailyPostStats_OfficersScheduled(object date, object shiftID)
        {
            int count = 0;

            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_OfficersScheduled";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            count = Convert.ToInt32(dt.Rows[0]["officersCount"]);
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
                    return count;
                }
            }
        }

        //Get officers absent. Those who are not working and did not call in to report off and also does not have a day off
        public int sp_DailyPostStats_OfficersAbsent(object date, object shiftID)
        {
            int count = 0;

            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_OfficersAbsent";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            count = Convert.ToInt32(dt.Rows[0]["officersCount"]);
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
                    return count;
                }
            }
        }

        //Get officers on day off who are not working
        public int sp_DailyPostStats_OfficersOnDayOff(object date, object shiftID)
        {
            int count = 0;

            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_OfficersOnDayOff";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            count = Convert.ToInt32(dt.Rows[0]["officersCount"]);
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
                    return count;
                }
            }
        }

        //Get the daily master roster
        public DataTable sp_DailyPostStats_Masterroster(object date, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostStats_Masterroster";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
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

        //Insert dailyPost
        public int insert_DailyPostAssignment(DateTime date, byte shiftID, Int16 lateArrivals, Int16 callsForAssistance, Int16 officersPresent, Int16 officersAbsent, Int16 officersAssigned, Int16 officersCalledOut, Int16 officersCallInForDayOff, Int16 officersOnDayOff, Int16 notDressedProperly, string shiftSummary, string shiftBriefingNotes, string PassedOnFrom, string passedOnTo, string summaryOfIncidents, string reportNumber, byte userID, Int16 sleeping)
        {
            int postAssignmentID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Sup_DailyPostAssignment a = new EDM.tbl_Sup_DailyPostAssignment();
                        {
                            a.Date = date;
                            a.ShiftID = shiftID;
                            a.LateArrivals = lateArrivals;
                            a.CallsForAssistance = callsForAssistance;
                            a.OfficersPresent = officersPresent;
                            a.OfficersAbsent = officersAbsent;
                            a.OfficersAssigned = officersAssigned;
                            a.OfficersCalledOut = officersCalledOut;
                            a.OfficersCallInForDayOff = officersCallInForDayOff;
                            a.OfficersOnDayOff = officersOnDayOff;
                            a.NotDressedProperly = notDressedProperly;
                            a.ShiftSummary = shiftSummary;
                            a.ShiftBriefingNotes = shiftBriefingNotes;
                            a.PassedOnFrom = PassedOnFrom;
                            a.PassedOnTo = passedOnTo;
                            a.SummaryOfIncidents = summaryOfIncidents;
                            a.ReportNumber = reportNumber;
                            a.Sleeping = sleeping;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Sup_DailyPostAssignment.Add(a);
                        model.SaveChanges();

                        //Get the id
                        postAssignmentID = a.PostAssignmentID;

                        if (postAssignmentID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = postAssignmentID;
                                z.TableName = "tbl_Sup_DailyPostAssignment";
                                z.AuditProcess = "Created Daily Post Assignment for " + reportNumber;
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            postAssignmentID = 0;
                            successfulCommit = false;
                            MISC.writetoAlertLog("No post assignment ID returned after insert!!");
                        }
                    }
                    if (postAssignmentID != 0)
                    {
                        //commit transaction
                        ts.Complete();
                        successfulCommit = true;
                    }
                }
            }
            catch (Exception exec)
            {
                postAssignmentID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return postAssignmentID;
        }

        public void update_dailyPostAssignment(int postAssignmentID, DateTime date, byte shiftID, Int16 lateArrivals, Int16 callsForAssistance, Int16 officersPresent, Int16 officersAbsent, Int16 officersAssigned, Int16 officersCalledOut, Int16 officersCallInForDayOff, Int16 officersOnDayOff, Int16 notDressedProperly, string shiftSummary, string shiftBriefingNotes, string PassedOnFrom, string passedOnTo, string summaryOfIncidents, string reportNumber, byte userID, Int16 sleeping)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var dl = (from a in model.tbl_Sup_DailyPostAssignment
                                  where a.PostAssignmentID == postAssignmentID
                                  select a).First();
                        dl.LateArrivals = lateArrivals;
                        dl.CallsForAssistance = callsForAssistance;
                        dl.OfficersPresent = officersPresent;
                        dl.OfficersAbsent = officersAbsent;
                        dl.OfficersAssigned = officersAssigned;
                        dl.OfficersCalledOut = officersCalledOut;
                        dl.OfficersCallInForDayOff = officersCallInForDayOff;
                        dl.OfficersOnDayOff = officersOnDayOff;
                        dl.NotDressedProperly = notDressedProperly;
                        dl.ShiftSummary = shiftSummary;
                        dl.ShiftBriefingNotes = shiftBriefingNotes;
                        dl.PassedOnFrom = PassedOnFrom;
                        dl.PassedOnTo = passedOnTo;
                        dl.SummaryOfIncidents = summaryOfIncidents;
                        dl.ReportNumber = reportNumber;
                        dl.Sleeping = sleeping;
                        dl.LastModifiedBy = userID;
                        dl.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog x = new EDM.tbl_AuditLog();
                        {
                            x.SourceID = postAssignmentID;
                            x.TableName = "tbl_Sup_DailyPostAssignment";
                            x.AuditProcess = "Modified Daily Post Assignment " + reportNumber;
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

        public DataTable sp_DailyPostAssignment_Search(object ReportNumber, object date, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DailyPostAssignment_Search";

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