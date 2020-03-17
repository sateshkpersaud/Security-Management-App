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
    public class Incident
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        //Insert new incidents
        public int insert_Incident(byte areaID, Int16 locationID, DateTime dateOccured, TimeSpan timeOccured, byte incidentTypeID, string otherTypeDescription, string description, string incidentNumber, byte userID, byte shiftID, string actionTaken)
        {
            int incidentID = 0;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert transaction
                        EDM.tbl_Op_Incidents a = new EDM.tbl_Op_Incidents();
                        {
                            a.AreaID = areaID;
                            a.LocationID = locationID;
                            a.DateOccured = dateOccured;
                            a.TimeOccured = timeOccured;
                            a.IncidentTypeID = incidentTypeID;
                            a.OtherTypeDescription = otherTypeDescription.Length > 0 ? firstCharToUpper(otherTypeDescription) : null;
                            a.Description = description.Length > 0 ? firstCharToUpper(description) : null;
                            a.ActionTaken = actionTaken.Length > 0 ? firstCharToUpper(actionTaken) : null;
                            a.IncidentNumber = incidentNumber;
                            a.ShiftID = shiftID == 0 ? null : (byte?)shiftID;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_Incidents.Add(a);
                        model.SaveChanges();

                        //Get the incidentID
                        incidentID = (from b in model.tbl_Op_Incidents
                                      where b.IncidentNumber == incidentNumber
                                    select b.IncidentID
                         ).FirstOrDefault();

                         //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = incidentID;
                            z.TableName = "tbl_Op_Incidents";
                            z.AuditProcess = "Created Incident Report - " + incidentNumber;
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
                incidentID = 0;
                successfulCommit = false;
                MISC.writetoAlertLog(exec.ToString());
            }
            return incidentID;
        }

        public void insert_IncidentPersonAffected(byte affectedTypeID,string firstName, string lastName, string description, string address, string otherTypeDescription, int incidentID, string phoneNumber, byte userID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Insert transaction
                        EDM.tbl_Op_IncidentPersonsAffected a = new EDM.tbl_Op_IncidentPersonsAffected();
                        {
                            a.AffectedTypeID = affectedTypeID;
                            a.FirstName = firstName.Length > 0 ? firstCharToUpper(firstName) : null;
                            a.LastName = firstCharToUpper(lastName);
                            a.Description = firstCharToUpper(description);
                            a.Address = address.Length > 0 ? firstCharToUpper(address) : null;
                            a.OtherTypeDescription = otherTypeDescription.Length > 0 ? firstCharToUpper(otherTypeDescription) : null;
                            a.IncidentID = incidentID;
                            a.PhoneNumber = phoneNumber.Length > 0 ? phoneNumber : null;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Op_IncidentPersonsAffected.Add(a);
                        model.SaveChanges();

                        //Get the incidentID
                        string incidentNumber = (from b in model.tbl_Op_Incidents
                                      where b.IncidentID == incidentID
                                      select b.IncidentNumber
                         ).FirstOrDefault();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = incidentID;
                            z.TableName = "tbl_Op_IncidentPersonsAffected";
                            z.AuditProcess = "Created Person Affected - " + incidentNumber;
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

        public void update_Incident(int incidentID, byte areaID, Int16 locationID, DateTime dateOccured, TimeSpan timeOccured, byte incidentTypeID, string otherTypeDescription, string description, string incidentNumber, byte userID, byte shiftID, string actionTaken)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the record to update
                        var recordToUpdate = (from a in model.tbl_Op_Incidents
                                              where a.IncidentID == incidentID
                                              select a).First();
                        recordToUpdate.AreaID = areaID;
                        recordToUpdate.LocationID = locationID;
                        recordToUpdate.DateOccured = dateOccured;
                        recordToUpdate.TimeOccured = timeOccured;
                        recordToUpdate.IncidentTypeID = incidentTypeID;
                        recordToUpdate.OtherTypeDescription = otherTypeDescription.Length > 0 ? firstCharToUpper(otherTypeDescription) : null;
                        recordToUpdate.Description = firstCharToUpper(description);
                        recordToUpdate.ActionTaken = firstCharToUpper(actionTaken);
                        recordToUpdate.IncidentNumber = incidentNumber;
                        recordToUpdate.ShiftID = shiftID == 0 ? null : (byte?)shiftID;
                        recordToUpdate.LastModifiedBy = userID;
                        recordToUpdate.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = incidentID;
                            z.TableName = "tbl_Op_Incidents";
                            z.AuditProcess = "Modified Incident Report - " + incidentNumber;
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

        public void update_PersonAffected(int personAffectedID, byte affectedTypeID, string firstName, string lastName, string description, string address, string otherTypeDescription, string phoneNumber, byte userID, string incidentNumber, int incidentID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        //Get the record to update
                        var recordToUpdate = (from a in model.tbl_Op_IncidentPersonsAffected
                                              where a.PersonAffectedID == personAffectedID
                                              select a).First();
                        recordToUpdate.AffectedTypeID = affectedTypeID;
                        recordToUpdate.FirstName = firstName.Length > 0 ? firstCharToUpper(firstName) : null;
                        recordToUpdate.LastName = firstCharToUpper(lastName);
                        recordToUpdate.Description = firstCharToUpper(description);
                        recordToUpdate.Address = address.Length > 0 ? firstCharToUpper(address) : null;
                        recordToUpdate.PhoneNumber = phoneNumber.Length > 0 ? phoneNumber : null;
                        recordToUpdate.OtherTypeDescription = otherTypeDescription.Length > 0 ? firstCharToUpper(otherTypeDescription) : null;
                        recordToUpdate.LastModifiedBy = userID;
                        recordToUpdate.LastModifiedOn = DateTime.Now;
                        model.SaveChanges();

                        //Insert AuditTrail
                        EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                        {
                            z.SourceID = incidentID;
                            z.TableName = "tbl_Op_Incidents";
                            z.AuditProcess = "Modified Person Affected - " + incidentNumber;
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

        public void delete_Incident(int incidentID)
        {
            var model = new EDM.DataSource();
            try
            {

                var results = (from a in model.tbl_Op_Incidents
                               where a.IncidentID == incidentID
                               select a).SingleOrDefault();

                if (results != null)
                {
                    model.tbl_Op_Incidents.Remove(results);
                    model.SaveChanges();
                    successfulCommit = true;
                }
            }
            catch (System.Exception excec)
            {
                successfulCommit = false;
                MISC.writetoAlertLog(excec.ToString());
            }

        }

        public bool duplicateCheck_personsAffected(string firstName, string lastName, int incidentID)
        {
            bool isDuplicate = false;

            var model = new EDM.DataSource();
            var results = (from a in model.tbl_Op_IncidentPersonsAffected
                           join b in model.tbl_Op_Incidents on a.IncidentID equals b.IncidentID
                           where a.FirstName == firstName && a.LastName == lastName && b.IncidentID == incidentID
                           select new
                           {
                               a
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

        public string firstCharToUpper(string input)
        {
            string val = "";
            if (!String.IsNullOrEmpty(input))
                val = input.First().ToString().ToUpper() + input.Substring(1);

            return val;
        }

        public DataTable sp_Incidents_Search(object ReportNumber, object date, object areaID, object locationID, object incidentTypeID)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Incidents_Search";

                    if (ReportNumber != null)
                    {
                        SqlParameter param1 = cmd.Parameters.Add("@ReportNumber", SqlDbType.VarChar,20);
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
                    if (incidentTypeID != null && incidentTypeID.ToString() != "")
                    {
                        SqlParameter param5 = cmd.Parameters.Add("@incidentTypeID", SqlDbType.TinyInt);
                        param5.Value = incidentTypeID;
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

        public DataTable sp_IncidentType_Sel4DDL()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_IncidentType_Sel4DDL";

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