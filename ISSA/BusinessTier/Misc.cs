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
    public class Misc
    {
        public bool successfulCommit = false;
      
        public DataTable sp_Area_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Area_Sel4DDL";

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
                        writetoAlertLog(excec.ToString());
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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }


        public DataTable sp_Location_Sel4DDL_ByAreaID(object areaID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Location_Sel4DDL_ByAreaID";

                    SqlParameter param1 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param1.Value = areaID;

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Shift_Sel4DDL_ByPositionID(object positionID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Shift_Sel4DDL_ByPositionID";

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Shift_Sel4TAB_ByPositionID(object positionID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Shift_Sel4TAB_ByPositionID";

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Shift_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Shift_Sel4DDL";
                    
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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Employee_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_Sel4DDL";

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Employee_Sel4DDL_Report(object startDate, object endDate)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_Sel4DDL_Report";

                    SqlParameter param0 = cmd.Parameters.Add("@startDate", SqlDbType.Date);
                    param0.Value = startDate;
                    SqlParameter param1 = cmd.Parameters.Add("@endDate", SqlDbType.Date);
                    param1.Value = endDate;

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public bool checkGrantStatus(DateTime dateFrom, DateTime dateTo)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Mgr_DateTimeGrant
                           where (a.DateTimeFrom <= dateFrom && a.DateTimeTo >= dateTo)
                           select new
                           {
                               a.GrantID
                           }).ToList();

            if (results.Count > 0)
            {
                isDuplicate = true;
            }
            else
            {
                isDuplicate = false;
            }
            return isDuplicate;
        }

        public void writetoAlertLog(string errorMessage)
        {
            string path = @"C:\\inetpub\\wwwroot\\ISSA\\Log\\";
            var alertFilePath = path + "Alert_Log.txt";
            var files = new DirectoryInfo(path).GetFiles("Alert_Log.txt");

            string bakpath = @"C:\\inetpub\\wwwroot\\ISSA\\Log\\Backups\\";
            string date = DateTime.UtcNow.Year.ToString() + DateTime.UtcNow.ToString("MM") + DateTime.UtcNow.ToString("dd");
            string backup = bakpath + "Alert_Log_" + date + ".txt";

            ////If File older than 30 days, backup 
            //foreach (var file in files)
            //{
            //    if (DateTime.UtcNow - file.CreationTimeUtc > TimeSpan.FromDays(30))
            //    {
            //        //create backup location if not exists
            //        if (!Directory.Exists(bakpath))
            //            Directory.CreateDirectory(bakpath);

            //        //move the log file to the backup folder
            //        if (File.Exists(alertFilePath))
            //            File.Move(alertFilePath, backup);

            //        //recreate an empty alert log file 
            //        if (!File.Exists(alertFilePath))
            //            File.Create(alertFilePath);
            //    }
            //}

            //This part get any previous errors in the file so that the new error can be written to the top of the file
            string previousErrors = "";
            using (FileStream fs = new FileStream(alertFilePath, FileMode.OpenOrCreate))
            {
                using (StreamReader sReader = new StreamReader(fs))
                {
                    previousErrors = sReader.ReadToEnd();
                }
                File.Delete(alertFilePath);
                fs.Close();
            }

            //Write the new error to the file
            using (FileStream fs2 = new FileStream(alertFilePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sWriter = new StreamWriter(fs2))
                {
                    sWriter.BaseStream.Seek(0, SeekOrigin.End);
                    sWriter.WriteLine("------------------------START OF NEW ERROR LINE ------------------------------ ");
                    sWriter.Write("ISSA ERROR (" + DateTime.Now + "): " + errorMessage);
                    sWriter.WriteLine(DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString());
                    sWriter.WriteLine(" ");
                    if (previousErrors != "")
                        sWriter.WriteLine(previousErrors);
                    sWriter.Flush();
                    sWriter.Close();
                }
                fs2.Close();
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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public DataTable sp_Employee_Sel4DDL_ByPositionIDAndLocation(object positionID, object locationID, object areaID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Employee_Sel4DDL_ByPositionIDAndLocation";

                    SqlParameter param1 = cmd.Parameters.Add("@positionID", SqlDbType.TinyInt);
                    param1.Value = positionID;
                    SqlParameter param2 = cmd.Parameters.Add("@locationID", SqlDbType.SmallInt);
                    param2.Value = locationID;
                    SqlParameter param3 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                    param3.Value = areaID;

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        //Function to get all days of a months within a year
        public  List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }

        public byte getUserID()
        {
            byte userID = 0;
            string username = Membership.GetUser().UserName;
            var model = new EDM.DataSource();
            userID = (from b in model.Users
                             where b.UserName == username
                             select b.UID
                         ).FirstOrDefault();
            return userID;
        }

        //overload method
        public byte getUserID(string username)
        {
            byte userID = 0;
            var model = new EDM.DataSource();
            userID = (from b in model.Users
                      where b.UserName == username
                      select b.UID
                         ).FirstOrDefault();
            return userID;
        }

        //overload method
        public byte getPositionID(string username)
        {
            byte positionID = 0;
            var model = new EDM.DataSource();
            var posID = (from b in model.Users
                      join c in model.tbl_Sup_Employees on b.UID equals c.UID
                      where b.UserName == username
                      select c.PositionID
                         ).FirstOrDefault();
            if (positionID != null)
                positionID = Convert.ToByte(posID);
            return positionID;
        }

        public string[] getUserRoles()
        {
            string[] roleNames = Roles.GetRolesForUser();
            return roleNames;
        }

        public string getUserName()
        {
            string username = Membership.GetUser().UserName;
            return username;
        }

        public bool? isResetRequired(string username)
        {
            bool? required = false;

            var model = new EDM.DataSource();

            var query = (from a in model.tbl_Sup_Employees
                         join b in model.Users on a.UID equals b.UID
                         where (b.UserName == username)
                         select new
                         {
                             a.ResetNeeded
                         }).FirstOrDefault();

            if (query != null)
            {
                required = query.ResetNeeded != (bool?)null ? query.ResetNeeded : false;
            }

            return required;
        }

        public bool? isUserActive(string username)
        {
            bool? active = false;

            var model = new EDM.DataSource();

            var query = (from a in model.tbl_Sup_Employees
                         join b in model.Users on a.UID equals b.UID
                         where (b.UserName == username)
                         select new
                         {
                             a.IsActive
                         }).FirstOrDefault();

            if (query != null)
            {
                active = query.IsActive != (bool?)null ? query.IsActive : false;
            }

            return active;
        }

        public DataTable sp_Reports_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Reports_Sel4DDL";

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
                        writetoAlertLog(excec.ToString());
                    }
                    finally
                    {
                        ConnStr.Close();
                    }
                    return dt;
                }
            }
        }

        public int getFailCount(string username)
        {
            int count = 0;
            var model = new EDM.DataSource();
            count = (from m in model.Memberships
                     join u in model.Users on m.UserId equals u.UserId
                      where u.UserName == username
                      select m.FailedPasswordAttemptCount
                         ).FirstOrDefault();
            return count;
        }
    }
}