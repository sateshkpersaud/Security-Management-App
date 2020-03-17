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
    public class Alert
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        //Load the call logs
        public DataTable sp_Alerts_CallLog_Select(object date, object areaID, object locationID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_CallLog_Select";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
                    if ((areaID.ToString() != "") && (areaID != null))
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param2.Value = areaID;
                    }
                    if ((locationID.ToString() != "") && (locationID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param3.Value = locationID;
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

        //Load the incidents/complaints logs
        public DataTable sp_Alerts_IncidentsComplaints_Select(object date, object areaID, object locationID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_IncidentsComplaints_Select ";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
                    if ((areaID.ToString() != "") && (areaID != null))
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param2.Value = areaID;
                    }
                    if ((locationID.ToString() != "") && (locationID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param3.Value = locationID;
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

        //Load incidents details for location
        public DataTable sp_Alerts_IncidentsSelect(object date, object locationID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_IncidentsSelect ";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
                    if ((locationID.ToString() != "") && (locationID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param3.Value = locationID;
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

        //Load complaints details for location
        public DataTable sp_Alerts_ComplaintsSelect(object date, object locationID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_ComplaintsSelect ";

                    SqlParameter param1 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param1.Value = date;
                    if ((locationID.ToString() != "") && (locationID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                        param3.Value = locationID;
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

        //get standby used
        public int sp_Alerts_Attendance_StandByUsed(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_StandByUsed";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }
                    
                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        //Get sentHomes
        public int sp_Alerts_Attendance_SentHome(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_SentHome";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        //get day off employees
        public int sp_Alerts_Attendance_DayOffs(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_DayOffs";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        //get persons absent who are not on day off
        public int sp_Alerts_Attendance_Absent(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_Absent";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        //get total persons working from personsworking, switches and doubles
        public int sp_Alerts_Attendance_PersonsWorking(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_PersonsWorking";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        //get total persons scheduled to work
        public int sp_Alerts_Attendance_TotalScheduled(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_TotalScheduled";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        //Get illegals
        public int sp_Alerts_Attendance_IllegalEmployees(object daysBeforedateFrom, object dateTo, object areaID, object shiftID, object daysCount, object dateFrom)
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
                    int result = 0;

                    try
                    {
                        ConnStr.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds, "ss");
                        dt = ds.Tables["ss"];

                        if ((dt != null) && (dt.Rows.Count != 0))
                        {
                            result = dt.Rows.Count;
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

        //get doubles
        public int sp_Alerts_Attendance_Doubles(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_Doubles";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
                    }

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    int result = 0;

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
                                result += Convert.ToInt32(dr["resultCount"]);
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

        public DataTable sp_Alerts_Attendance_AbsentSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_AbsentSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        public DataTable sp_Alerts_Attendance_DayOffsSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_DayOffsSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        //Get illegals
        public DataTable sp_Alerts_Attendance_IllegalEmployeesSelect(object daysBeforedateFrom,  object dateTo, object areaID, object shiftID, object daysCount,object dateFrom)
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

        public DataTable sp_Alerts_Attendance_DoublesSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_DoublesSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        public DataTable sp_Alerts_Attendance_SentHomeSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_SentHomeSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        public DataTable sp_Alerts_Attendance_StandByUnusedSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_StandByUnusedSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        public DataTable sp_Alerts_Attendance_ShortageSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_ShortageSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        public DataTable sp_Alerts_Attendance_StandByUsedSelect(object dateFrom, object dateTo, object areaID, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_Attendance_StandByUsedSelect ";

                    SqlParameter param0 = cmd.Parameters.Add("@dateFrom", SqlDbType.Date);
                    param0.Value = dateFrom;
                    SqlParameter param1 = cmd.Parameters.Add("@dateTo", SqlDbType.Date);
                    param1.Value = dateTo;
                    SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param2.Value = areaID;
                    if ((shiftID.ToString() != "") && (shiftID != null))
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@shiftID", SqlDbType.TinyInt);
                        param3.Value = shiftID;
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

        public DataTable sp_Alerts_DailyOperationsReport(object date, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_DailyOperationsReport ";

                    SqlParameter param0 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param0.Value = date;
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

        public DataTable sp_Alerts_DailyOperationsReport_Supervisors(object date, object shiftID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Alerts_DailyOperationsReport_Supervisors ";

                    SqlParameter param0 = cmd.Parameters.Add("@date", SqlDbType.Date);
                    param0.Value = date;
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
    }
}