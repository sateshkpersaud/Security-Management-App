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
    public class ReferenceData
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        //AREA FUNCTIONS
        public DataTable sp_Ref_Area_Search(object searchText, object statusID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Ref_Area_Search ";

                    if (searchText != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@searchText", SqlDbType.VarChar, 100);
                        param1.Value = searchText;
                    }
                    SqlParameter param3 = cmd.Parameters.Add("@statusID", SqlDbType.Bit);
                    param3.Value = statusID;

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

        public void update_Area(byte areaID, string area, bool isActive, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        var record = (from a in model.tbl_Ref_Areas
                                   where a.AreaID == areaID
                                   select a).First();
                        record.Area = firstCharToUpper(area);
                        record.LastModifiedBy = userID;
                        record.IsActive = isActive;
                        record.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = areaID;
                            z.TableName = "tbl_Ref_Areas";
                            z.AuditProcess = "Updated Area (" + area + ")";
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

        public void insert_Area(string area, bool isActive, byte userID)
        {
            int areaID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the row count
                        int ct = (from n in model.tbl_Ref_Areas
                                  select n
                         ).Count();

                        ct = ct + 1;
                        byte seq = Convert.ToByte(ct);

                        EDM.tbl_Ref_Areas a = new EDM.tbl_Ref_Areas();
                        {
                            a.Area = firstCharToUpper(area);
                            a.Seq = seq;
                            a.IsActive = isActive;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Ref_Areas.Add(a);
                        model.SaveChanges();

                        //Get the id
                        areaID = a.AreaID;
                        if (areaID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = areaID;
                                z.TableName = "tbl_Ref_Areas";
                                z.AuditProcess = "Created Area (" + area + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No area ID returned after insert!!");
                        }
                    }
                    if (areaID != 0)
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

        //DEPARTMENT FUNCTIONS
        public DataTable sp_Ref_Department_Search(object searchText, object statusID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Ref_Department_Search";

                    if (searchText != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@searchText", SqlDbType.VarChar, 100);
                        param1.Value = searchText;
                    }
                    SqlParameter param3 = cmd.Parameters.Add("@statusID", SqlDbType.Bit);
                    param3.Value = statusID;

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

        public void update_Department(byte departmentID, string department, bool isActive, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        var record = (from a in model.tbl_Ref_Departments
                                      where a.DepartmentID == departmentID
                                      select a).First();
                        record.Department = firstCharToUpper(department);
                        record.LastModifiedBy = userID;
                        record.IsActive = isActive;
                        record.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = departmentID;
                            z.TableName = "tbl_Ref_Departments";
                            z.AuditProcess = "Updated Department (" + department + ")";
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

        public void insert_Department(string department, bool isActive, byte userID)
        {
            int departmentID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Ref_Departments a = new EDM.tbl_Ref_Departments();
                        {
                            a.Department = firstCharToUpper(department);
                            a.IsActive = isActive;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Ref_Departments.Add(a);
                        model.SaveChanges();

                        //Get the id
                        departmentID = a.DepartmentID;
                        if (departmentID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = departmentID;
                                z.TableName = "tbl_Ref_Departments";
                                z.AuditProcess = "Created Department (" + department + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No department ID returned after insert!!");
                        }
                    }
                    if (departmentID != 0)
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


        //LOCATION FUNCTIONS
        public DataTable sp_Ref_Location_Search(object searchText, object areaID, object statusID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Ref_Location_Search";

                    if (searchText != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@searchText", SqlDbType.VarChar, 100);
                        param1.Value = searchText;
                    }
                    if ((areaID != null) && (areaID.ToString() != ""))
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@areaID", SqlDbType.TinyInt);
                        param2.Value = areaID;
                    }
                    SqlParameter param3 = cmd.Parameters.Add("@statusID", SqlDbType.Bit);
                    param3.Value = statusID;

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

        public void update_Location(byte areaID, Int16 locationID, string Location, bool isActive, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        var record = (from a in model.tbl_Ref_Locations
                                      where a.LocationID == locationID
                                      select a).First();
                        record.AreaID = areaID;
                        record.LocationName = firstCharToUpper(Location);
                        record.IsActive = isActive;
                        record.LastModifiedBy = userID;
                        record.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = locationID;
                            z.TableName = "tbl_Ref_Locations";
                            z.AuditProcess = "Updated Location (" + Location + ")";
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

        public void insert_Location(byte areaID, string location, bool isActive, byte userID)
        {
            int locationID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Ref_Locations a = new EDM.tbl_Ref_Locations();
                        {
                            a.AreaID = areaID;
                            a.LocationName = firstCharToUpper(location);
                            a.IsActive = isActive;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Ref_Locations.Add(a);
                        model.SaveChanges();

                        //Get the id
                        locationID = a.LocationID;
                        if (locationID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = locationID;
                                z.TableName = "tbl_Ref_Locations";
                                z.AuditProcess = "Created Location (" + location + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No location ID returned after insert!!");
                        }
                    }
                    if (locationID != 0)
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



        //POSITION FUNCTIONS
        public DataTable sp_Ref_Position_Search(object searchText, object statusID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Ref_Position_Search";

                    if (searchText != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@searchText", SqlDbType.VarChar, 100);
                        param1.Value = searchText;
                    }
                    SqlParameter param3 = cmd.Parameters.Add("@statusID", SqlDbType.Bit);
                    param3.Value = statusID;

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

        public void update_Position(byte positionID, string position, bool isActive, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        var record = (from a in model.tbl_Ref_Positions
                                      where a.PositionID == positionID
                                      select a).First();
                        record.PositionName = firstCharToUpper(position);
                        record.LastModifiedBy = userID;
                        record.IsActive = isActive;
                        record.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = positionID;
                            z.TableName = "tbl_Ref_Positions";
                            z.AuditProcess = "Updated Position (" + position + ")";
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

        public void insert_Position(string position, bool isActive, byte userID)
        {
            int positionID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Ref_Positions a = new EDM.tbl_Ref_Positions();
                        {
                            a.PositionName = firstCharToUpper(position);
                            a.IsActive = isActive;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Ref_Positions.Add(a);
                        model.SaveChanges();

                        //Get the id
                        positionID = a.PositionID;
                        if (positionID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = positionID;
                                z.TableName = "tbl_Ref_Positions";
                                z.AuditProcess = "Created Position (" + position + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No position ID returned after insert!!");
                        }
                    }
                    if (positionID != 0)
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


        //SHIFT FUNCTIONS
        public DataTable sp_Ref_Shift_Search(object fromTime, object toTime, object positionID, object statusID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Ref_Shift_Search";

                    if (fromTime != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@fromTime", SqlDbType.Time);
                        param1.Value = fromTime;
                    }
                    if (toTime != null)
                    {
                        SqlParameter param3 = cmd.Parameters.Add("@toTime", SqlDbType.Time);
                        param3.Value = toTime;
                    }
                    if ((positionID != null) && (positionID.ToString() != ""))
                    {
                        SqlParameter param2 = cmd.Parameters.Add("@positionID", SqlDbType.TinyInt);
                        param2.Value = positionID;
                    }
                    SqlParameter param4 = cmd.Parameters.Add("@statusID", SqlDbType.Bit);
                    param4.Value = statusID;

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

        public void update_Shift(byte shiftID, byte positionID, TimeSpan fromTime, TimeSpan toTime, string shiftName, string hrsInShift, bool isActive, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        var record = (from a in model.tbl_Ref_Shifts
                                      where a.ShiftID == shiftID
                                      select a).First();
                        record.PositionID = positionID;
                        record.FromTime = fromTime;
                        record.ToTime = toTime;
                        record.IsActive = isActive;
                        record.HoursInShift = hrsInShift;
                        record.LastModifiedBy = userID;
                        record.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = shiftID;
                            z.TableName = "tbl_Ref_Shifts";
                            z.AuditProcess = "Updated Shift (" + shiftName + ")";
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

        public void insert_Shift(byte positionID, TimeSpan fromTime, TimeSpan toTime, string shiftName, string hrsInShift, bool isActive, byte userID)
        {
            int shiftID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert header
                        EDM.tbl_Ref_Shifts a = new EDM.tbl_Ref_Shifts();
                        {
                            a.PositionID = positionID;
                            a.FromTime = fromTime;
                            a.ToTime = toTime;
                            a.HoursInShift = hrsInShift;
                            a.IsActive = isActive;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Ref_Shifts.Add(a);
                        model.SaveChanges();

                        //Get the id
                        shiftID = a.ShiftID;
                        if (shiftID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = shiftID;
                                z.TableName = "tbl_Ref_Shifts";
                                z.AuditProcess = "Created Shift (" + shiftName + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No Shift ID returned after insert!!");
                        }
                    }
                    if (shiftID != 0)
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


        public string firstCharToUpper(string input)
        {
            string val = "";
            if (!String.IsNullOrEmpty(input))
                val = input.First().ToString().ToUpper() + input.Substring(1);

            return val;
        }
    }
}