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
    public class WeeklySecurityReport
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_WeeklySecurityReport_Computation_SelectFor(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WeeklySecurityReport_Computation_SelectFor ";

                    SqlParameter param1 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param1.Value = dateFrom;
                    SqlParameter param2 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param2.Value = dateTo;
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

        public DataTable sp_WeeklySecurityReport_OfficersPerShift_SelectFor(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WeeklySecurityReport_OfficersPerShift_SelectFor ";

                    SqlParameter param1 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param1.Value = dateFrom;
                    SqlParameter param2 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param2.Value = dateTo;
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

        public DataTable sp_WeeklySecurityReport_SpecialOperations_SelectFor(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WeeklySecurityReport_SpecialOperations_SelectFor ";

                    SqlParameter param1 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param1.Value = dateFrom;
                    SqlParameter param2 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param2.Value = dateTo;
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

        public DataTable sp_WeeklySecurityReport_Comparison_Select(object dateFrom, object dateTo, object areaID, object shiftID, object dayOfWeek)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WeeklySecurityReport_Comparison_Select ";

                    SqlParameter param1 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param1.Value = dateFrom;
                    SqlParameter param2 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param2.Value = dateTo;
                    SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param3.Value = areaID;
                    SqlParameter param4 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                    param4.Value = shiftID;
                    SqlParameter param5 = cmd.Parameters.Add("@dayOfWeek", SqlDbType.VarChar,10);
                    param5.Value = dayOfWeek;

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

        public int insert_WeeklySecurityReport(string reportNumber, DateTime weekStart, DateTime weekEnd, byte areaID, string comments, GridView gvShiftReport, byte userID)
        {
            int securityReportID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Mgr_WeeklySecurityReport a = new EDM.tbl_Mgr_WeeklySecurityReport();
                        {
                            a.ReportNumber = reportNumber;
                            a.AreaID = areaID;
                            a.WeekStart = weekStart;
                            a.WeekEnd = weekEnd;
                            a.Comments = comments == "" ? null : comments;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Mgr_WeeklySecurityReport.Add(a);
                        model.SaveChanges();

                        //Get the id
                        securityReportID = a.SecurityReportID;

                        if (securityReportID != 0)
                        {
                            for (int i = 0; i < gvShiftReport.Rows.Count; i++)
                            {
                                Label lblShiftID = (Label)gvShiftReport.Rows[i].FindControl("lblShiftID");
                                Label lblType = (Label)gvShiftReport.Rows[i].FindControl("lblType");

                                if (lblType.Text == "Scheduled")
                                {
                                    Label lblCashInGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashInGT");
                                    Label lblCashOutGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashOutGT");
                                    Label lblOther = (Label)gvShiftReport.Rows[i].FindControl("lblOther");
                                    Label lblMonday = (Label)gvShiftReport.Rows[i].FindControl("lblMonday");
                                    Label lblTuesday = (Label)gvShiftReport.Rows[i].FindControl("lblTuesday");
                                    Label lblWednesday = (Label)gvShiftReport.Rows[i].FindControl("lblWednesday");
                                    Label lblThursday = (Label)gvShiftReport.Rows[i].FindControl("lblThursday");
                                    Label lblFriday = (Label)gvShiftReport.Rows[i].FindControl("lblFriday");
                                    Label lblSaturday = (Label)gvShiftReport.Rows[i].FindControl("lblSaturday");
                                    Label lblSunday = (Label)gvShiftReport.Rows[i].FindControl("lblSunday");
                                    Label lblPresent = (Label)gvShiftReport.Rows[i].FindControl("lblPresent");
                                    Label lblAbsent = (Label)gvShiftReport.Rows[i].FindControl("lblAbsent");
                                    Label lblIncidentsComplaints = (Label)gvShiftReport.Rows[i].FindControl("lblIncidentsComplaints");
                                    Label lblSwitches = (Label)gvShiftReport.Rows[i].FindControl("lblSwitches");

                                    //insert shift comparison data
                                    EDM.tbl_Mgr_WeeklySecurityReportComparison b = new EDM.tbl_Mgr_WeeklySecurityReportComparison();
                                    {
                                        b.ShiftID = Convert.ToByte(lblShiftID.Text);
                                        b.SecurityReportID = securityReportID;
                                        b.CashInGT = lblCashInGT.Text;
                                        b.CashOutGT = lblCashOutGT.Text;
                                        b.Other = lblOther.Text;
                                        b.Monday = lblMonday.Text;
                                        b.Tuesday = lblTuesday.Text;
                                        b.Wednesday = lblWednesday.Text;
                                        b.Thursday = lblThursday.Text;
                                        b.Friday = lblFriday.Text;
                                        b.Saturday = lblSaturday.Text;
                                        b.Sunday = lblSunday.Text;
                                        b.Present = lblPresent.Text;
                                        b.Absent = lblAbsent.Text;
                                        b.IncidentComplaints = lblIncidentsComplaints.Text;
                                        b.Switches = lblSwitches.Text;
                                        b.IsScheduled = true;
                                        b.CreatedBy = userID;
                                        b.CreatedOn = DateTime.Now;
                                    };
                                    model.tbl_Mgr_WeeklySecurityReportComparison.Add(b);
                                    model.SaveChanges();
                                }
                                else if (lblType.Text == "Actual")
                                {
                                    Label lblCashInGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashInGT");
                                    Label lblCashOutGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashOutGT");
                                    Label lblOther = (Label)gvShiftReport.Rows[i].FindControl("lblOther");
                                    TextBox tbMonday = (TextBox)gvShiftReport.Rows[i].FindControl("tbMonday");
                                    TextBox tbTuesday = (TextBox)gvShiftReport.Rows[i].FindControl("tbTuesday");
                                    TextBox tbWednesday = (TextBox)gvShiftReport.Rows[i].FindControl("tbWednesday");
                                    TextBox tbThursday = (TextBox)gvShiftReport.Rows[i].FindControl("tbThursday");
                                    TextBox tbFriday = (TextBox)gvShiftReport.Rows[i].FindControl("tbFriday");
                                    TextBox tbSaturday = (TextBox)gvShiftReport.Rows[i].FindControl("tbSaturday");
                                    TextBox tbSunday = (TextBox)gvShiftReport.Rows[i].FindControl("tbSunday");
                                    TextBox tbPresent = (TextBox)gvShiftReport.Rows[i].FindControl("tbPresent");
                                    TextBox tbAbsent = (TextBox)gvShiftReport.Rows[i].FindControl("tbAbsent");
                                    TextBox tbIncidentsComplaints = (TextBox)gvShiftReport.Rows[i].FindControl("tbIncidentsComplaints");
                                    TextBox tbSwitches = (TextBox)gvShiftReport.Rows[i].FindControl("tbSwitches");

                                    //insert shift comparison data
                                    EDM.tbl_Mgr_WeeklySecurityReportComparison b = new EDM.tbl_Mgr_WeeklySecurityReportComparison();
                                    {
                                        b.ShiftID = Convert.ToByte(lblShiftID.Text);
                                        b.SecurityReportID = securityReportID;
                                        b.CashInGT = lblCashInGT.Text;
                                        b.CashOutGT = lblCashOutGT.Text;
                                        b.Other = lblOther.Text;
                                        b.Monday = tbMonday.Text;
                                        b.Tuesday = tbTuesday.Text;
                                        b.Wednesday = tbWednesday.Text;
                                        b.Thursday = tbThursday.Text;
                                        b.Friday = tbFriday.Text;
                                        b.Saturday = tbSaturday.Text;
                                        b.Sunday = tbSunday.Text;
                                        b.Present = tbPresent.Text;
                                        b.Absent = tbAbsent.Text;
                                        b.IncidentComplaints = tbIncidentsComplaints.Text;
                                        b.Switches = tbSwitches.Text;
                                        b.IsScheduled = false;
                                        b.CreatedBy = userID;
                                        b.CreatedOn = DateTime.Now;
                                    };
                                    model.tbl_Mgr_WeeklySecurityReportComparison.Add(b);
                                    model.SaveChanges();
                                }
                            }

                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = securityReportID;
                                z.TableName = "tbl_Mgr_WeeklySecurityReport";
                                z.AuditProcess = "Created Weekly Security Report for period " + weekStart + " to " + weekEnd + " (" + reportNumber + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            securityReportID = 0;
                            successfulCommit = false;
                            MISC.writetoAlertLog("No security report ID returned after insert!!");
                        }
                    }
                    if (securityReportID != 0)
                    {
                        //commit transaction
                        ts.Complete();
                        successfulCommit = true;
                    }
                }
            }
            catch (Exception exec)
            {
                securityReportID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return securityReportID;
        }

        public void update_WeeklySecurityReport(int securityReportID, string reportNumber, DateTime weekStart, DateTime weekEnd, byte areaID, string comments, GridView gvShiftReport, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Update header table
                        var wsr = (from a in model.tbl_Mgr_WeeklySecurityReport
                                   where a.SecurityReportID == securityReportID
                                   select a).First();
                        wsr.WeekStart = weekStart;
                        wsr.WeekEnd = weekEnd;
                        wsr.Comments = comments;
                        wsr.LastModifiedBy = userID;
                        wsr.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        ////Delete all shifts saved
                        //for (int j = 0; j < gvShiftReport.Rows.Count; j++)
                        //{
                        //    Label lblShiftID = (Label)gvShiftReport.Rows[j].FindControl("lblShiftID");
                        //    Label lblType = (Label)gvShiftReport.Rows[j].FindControl("lblType");
                        //    bool isScheduled = lblType.Text == "Scheduled" ? true : false;
                        //    byte shiftID = Convert.ToByte(lblShiftID.Text);
                        //    var wsrc = (from f in model.tbl_Mgr_WeeklySecurityReportComparison
                        //                where f.SecurityReportID == securityReportID &&  f.ShiftID == shiftID && f.IsScheduled == isScheduled
                        //                select f).SingleOrDefault();

                        //    if (wsrc != null)
                        //    {
                        //        model.tbl_Mgr_WeeklySecurityReportComparison.Remove(wsrc);
                        //        model.SaveChanges();
                        //        successfulCommit = true;
                        //    }
                        //}

                        for (int i = 0; i < gvShiftReport.Rows.Count; i++)
                        {
                            Label lblShiftID = (Label)gvShiftReport.Rows[i].FindControl("lblShiftID");
                            Label lblType = (Label)gvShiftReport.Rows[i].FindControl("lblType");
                            Label lblComparisonID = (Label)gvShiftReport.Rows[i].FindControl("lblComparisonID");

                            //Update shift if it has an ID
                            if (lblComparisonID.Text != "0")
                            {
                                Int32 comparisonID = Convert.ToInt32(lblComparisonID.Text);

                                //insert shift comparison data
                                if (lblType.Text == "Scheduled")
                                {
                                    Label lblCashInGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashInGT");
                                    Label lblCashOutGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashOutGT");
                                    Label lblOther = (Label)gvShiftReport.Rows[i].FindControl("lblOther");
                                    Label lblMonday = (Label)gvShiftReport.Rows[i].FindControl("lblMonday");
                                    Label lblTuesday = (Label)gvShiftReport.Rows[i].FindControl("lblTuesday");
                                    Label lblWednesday = (Label)gvShiftReport.Rows[i].FindControl("lblWednesday");
                                    Label lblThursday = (Label)gvShiftReport.Rows[i].FindControl("lblThursday");
                                    Label lblFriday = (Label)gvShiftReport.Rows[i].FindControl("lblFriday");
                                    Label lblSaturday = (Label)gvShiftReport.Rows[i].FindControl("lblSaturday");
                                    Label lblSunday = (Label)gvShiftReport.Rows[i].FindControl("lblSunday");
                                    Label lblPresent = (Label)gvShiftReport.Rows[i].FindControl("lblPresent");
                                    Label lblAbsent = (Label)gvShiftReport.Rows[i].FindControl("lblAbsent");
                                    Label lblIncidentsComplaints = (Label)gvShiftReport.Rows[i].FindControl("lblIncidentsComplaints");
                                    Label lblSwitches = (Label)gvShiftReport.Rows[i].FindControl("lblSwitches");


                                    var shi = (from h in model.tbl_Mgr_WeeklySecurityReportComparison
                                               where h.ComparisonID == comparisonID
                                               select h).First();
                                    shi.CashInGT = lblCashInGT.Text;
                                    shi.CashOutGT = lblCashOutGT.Text;
                                    shi.Other = lblOther.Text;
                                    shi.Monday = lblMonday.Text;
                                    shi.Tuesday = lblTuesday.Text;
                                    shi.Wednesday = lblWednesday.Text;
                                    shi.Thursday = lblThursday.Text;
                                    shi.Friday = lblFriday.Text;
                                    shi.Saturday = lblSaturday.Text;
                                    shi.Sunday = lblSunday.Text;
                                    shi.Present = lblPresent.Text;
                                    shi.Absent = lblAbsent.Text;
                                    shi.IncidentComplaints = lblIncidentsComplaints.Text;
                                    shi.Switches = lblSwitches.Text;
                                    shi.LastModifiedBy = userID;
                                    shi.LastModifiedOn = DateTime.Now;
                                    model.SaveChanges();
                                }
                                else if (lblType.Text == "Actual")
                                {
                                    Label lblCashInGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashInGT");
                                    Label lblCashOutGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashOutGT");
                                    Label lblOther = (Label)gvShiftReport.Rows[i].FindControl("lblOther");
                                    TextBox tbMonday = (TextBox)gvShiftReport.Rows[i].FindControl("tbMonday");
                                    TextBox tbTuesday = (TextBox)gvShiftReport.Rows[i].FindControl("tbTuesday");
                                    TextBox tbWednesday = (TextBox)gvShiftReport.Rows[i].FindControl("tbWednesday");
                                    TextBox tbThursday = (TextBox)gvShiftReport.Rows[i].FindControl("tbThursday");
                                    TextBox tbFriday = (TextBox)gvShiftReport.Rows[i].FindControl("tbFriday");
                                    TextBox tbSaturday = (TextBox)gvShiftReport.Rows[i].FindControl("tbSaturday");
                                    TextBox tbSunday = (TextBox)gvShiftReport.Rows[i].FindControl("tbSunday");
                                    TextBox tbPresent = (TextBox)gvShiftReport.Rows[i].FindControl("tbPresent");
                                    TextBox tbAbsent = (TextBox)gvShiftReport.Rows[i].FindControl("tbAbsent");
                                    TextBox tbIncidentsComplaints = (TextBox)gvShiftReport.Rows[i].FindControl("tbIncidentsComplaints");
                                    TextBox tbSwitches = (TextBox)gvShiftReport.Rows[i].FindControl("tbSwitches");

                                    var shi = (from h in model.tbl_Mgr_WeeklySecurityReportComparison
                                               where h.ComparisonID == comparisonID
                                               select h).First();
                                    shi.CashInGT = lblCashInGT.Text;
                                    shi.CashOutGT = lblCashOutGT.Text;
                                    shi.Other = lblOther.Text;
                                    shi.Monday = tbMonday.Text;
                                    shi.Tuesday = tbTuesday.Text;
                                    shi.Wednesday = tbWednesday.Text;
                                    shi.Thursday = tbThursday.Text;
                                    shi.Friday = tbFriday.Text;
                                    shi.Saturday = tbSaturday.Text;
                                    shi.Sunday = tbSunday.Text;
                                    shi.Present = tbPresent.Text;
                                    shi.Absent = tbAbsent.Text;
                                    shi.IncidentComplaints = tbIncidentsComplaints.Text;
                                    shi.Switches = tbSwitches.Text;
                                    shi.LastModifiedBy = userID;
                                    shi.LastModifiedOn = DateTime.Now;
                                    model.SaveChanges();
                                }
                            }
                            //insert shift comparison data
                            else
                            {
                                if (lblType.Text == "Scheduled")
                                {
                                    Label lblCashInGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashInGT");
                                    Label lblCashOutGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashOutGT");
                                    Label lblOther = (Label)gvShiftReport.Rows[i].FindControl("lblOther");
                                    Label lblMonday = (Label)gvShiftReport.Rows[i].FindControl("lblMonday");
                                    Label lblTuesday = (Label)gvShiftReport.Rows[i].FindControl("lblTuesday");
                                    Label lblWednesday = (Label)gvShiftReport.Rows[i].FindControl("lblWednesday");
                                    Label lblThursday = (Label)gvShiftReport.Rows[i].FindControl("lblThursday");
                                    Label lblFriday = (Label)gvShiftReport.Rows[i].FindControl("lblFriday");
                                    Label lblSaturday = (Label)gvShiftReport.Rows[i].FindControl("lblSaturday");
                                    Label lblSunday = (Label)gvShiftReport.Rows[i].FindControl("lblSunday");
                                    Label lblPresent = (Label)gvShiftReport.Rows[i].FindControl("lblPresent");
                                    Label lblAbsent = (Label)gvShiftReport.Rows[i].FindControl("lblAbsent");
                                    Label lblIncidentsComplaints = (Label)gvShiftReport.Rows[i].FindControl("lblIncidentsComplaints");
                                    Label lblSwitches = (Label)gvShiftReport.Rows[i].FindControl("lblSwitches");

                                    EDM.tbl_Mgr_WeeklySecurityReportComparison b = new EDM.tbl_Mgr_WeeklySecurityReportComparison();
                                    {
                                        b.ShiftID = Convert.ToByte(lblShiftID.Text);
                                        b.SecurityReportID = securityReportID;
                                        b.CashInGT = lblCashInGT.Text;
                                        b.CashOutGT = lblCashOutGT.Text;
                                        b.Other = lblOther.Text;
                                        b.Monday = lblMonday.Text;
                                        b.Tuesday = lblTuesday.Text;
                                        b.Wednesday = lblWednesday.Text;
                                        b.Thursday = lblThursday.Text;
                                        b.Friday = lblFriday.Text;
                                        b.Saturday = lblSaturday.Text;
                                        b.Sunday = lblSunday.Text;
                                        b.Present = lblPresent.Text;
                                        b.Absent = lblAbsent.Text;
                                        b.IncidentComplaints = lblIncidentsComplaints.Text;
                                        b.Switches = lblSwitches.Text;
                                        b.IsScheduled = true;
                                        b.CreatedBy = userID;
                                        b.CreatedOn = DateTime.Now;
                                    };
                                    model.tbl_Mgr_WeeklySecurityReportComparison.Add(b);
                                    model.SaveChanges();
                                }
                                else if (lblType.Text == "Actual")
                                {
                                    Label lblCashInGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashInGT");
                                    Label lblCashOutGT = (Label)gvShiftReport.Rows[i].FindControl("lblCashOutGT");
                                    Label lblOther = (Label)gvShiftReport.Rows[i].FindControl("lblOther");
                                    TextBox tbMonday = (TextBox)gvShiftReport.Rows[i].FindControl("tbMonday");
                                    TextBox tbTuesday = (TextBox)gvShiftReport.Rows[i].FindControl("tbTuesday");
                                    TextBox tbWednesday = (TextBox)gvShiftReport.Rows[i].FindControl("tbWednesday");
                                    TextBox tbThursday = (TextBox)gvShiftReport.Rows[i].FindControl("tbThursday");
                                    TextBox tbFriday = (TextBox)gvShiftReport.Rows[i].FindControl("tbFriday");
                                    TextBox tbSaturday = (TextBox)gvShiftReport.Rows[i].FindControl("tbSaturday");
                                    TextBox tbSunday = (TextBox)gvShiftReport.Rows[i].FindControl("tbSunday");
                                    TextBox tbPresent = (TextBox)gvShiftReport.Rows[i].FindControl("tbPresent");
                                    TextBox tbAbsent = (TextBox)gvShiftReport.Rows[i].FindControl("tbAbsent");
                                    TextBox tbIncidentsComplaints = (TextBox)gvShiftReport.Rows[i].FindControl("tbIncidentsComplaints");
                                    TextBox tbSwitches = (TextBox)gvShiftReport.Rows[i].FindControl("tbSwitches");

                                    //insert shift comparison data
                                    EDM.tbl_Mgr_WeeklySecurityReportComparison b = new EDM.tbl_Mgr_WeeklySecurityReportComparison();
                                    {
                                        b.ShiftID = Convert.ToByte(lblShiftID.Text);
                                        b.SecurityReportID = securityReportID;
                                        b.CashInGT = lblCashInGT.Text;
                                        b.CashOutGT = lblCashOutGT.Text;
                                        b.Other = lblOther.Text;
                                        b.Monday = tbMonday.Text;
                                        b.Tuesday = tbTuesday.Text;
                                        b.Wednesday = tbWednesday.Text;
                                        b.Thursday = tbThursday.Text;
                                        b.Friday = tbFriday.Text;
                                        b.Saturday = tbSaturday.Text;
                                        b.Sunday = tbSunday.Text;
                                        b.Present = tbPresent.Text;
                                        b.Absent = tbAbsent.Text;
                                        b.IncidentComplaints = tbIncidentsComplaints.Text;
                                        b.Switches = tbSwitches.Text;
                                        b.IsScheduled = false;
                                        b.CreatedBy = userID;
                                        b.CreatedOn = DateTime.Now;
                                    };
                                    model.tbl_Mgr_WeeklySecurityReportComparison.Add(b);
                                    model.SaveChanges();
                                }
                            }
                        }

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = securityReportID;
                            z.TableName = "tbl_Mgr_WeeklySecurityReport";
                            z.AuditProcess = "Updated Weekly Security Report for period " + weekStart + " to " + weekEnd + " (" + reportNumber + ")";
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

        public DataTable sp_WeeklySecurityReport_Search(object ReportNumber, object weekStart, object weekEnd, object areaID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WeeklySecurityReport_Search ";

                    if (ReportNumber != null)
                    {
                        SqlParameter param4 = cmd.Parameters.Add("@ReportNumber", SqlDbType.VarChar, 20);
                        param4.Value = ReportNumber;
                    }
                    if (weekStart != null && weekStart.ToString() != "")
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@weekStart", SqlDbType.Date);
                        param1.Value = weekStart;
                    }
                    if (weekEnd != null && weekEnd.ToString() != "")
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@weekEnd", SqlDbType.Date);
                        param2.Value = weekEnd;
                    }
                    if (areaID != null && areaID.ToString() != "")
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param3.Value = areaID;
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

        public DataTable sp_WeeklySecurityReport_ComparisonSelect(object securityReportID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_WeeklySecurityReport_ComparisonSelect ";

                    SqlParameter param1 = cmd.Parameters.Add("@securityReportID", SqlDbType.Int);
                    param1.Value = securityReportID;

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