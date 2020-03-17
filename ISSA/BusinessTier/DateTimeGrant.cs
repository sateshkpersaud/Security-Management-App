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
    public class DateTimeGrant
    {
        public bool successfulCommit = false;
        public BusinessTier.Misc MISC = new BusinessTier.Misc();

        public DataTable sp_DateTimeGrant_Select()
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (SqlConnection ConnStr = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = ConnStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DateTimeGrant_Select ";

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

        public void insert_Grant(DateTime dtDateFrom, DateTime dtDateTo, string comments, byte userID)
        {
            int grantID = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (EDM.DataSource model = new EDM.DataSource())
                    {
                        EDM.tbl_Mgr_DateTimeGrant a = new EDM.tbl_Mgr_DateTimeGrant();
                        {
                            a.DateTimeFrom = dtDateFrom;
                            a.DateTimeTo = dtDateTo;
                            a.Comments = comments;
                            a.CreatedBy = userID;
                            a.CreatedOn = DateTime.Now;
                        };
                        model.tbl_Mgr_DateTimeGrant.Add(a);
                        model.SaveChanges();

                        //Get the id
                        grantID = a.GrantID;
                        if (grantID != 0)
                        {
                            //Insert AuditTrail
                            EDM.tbl_AuditLog z = new EDM.tbl_AuditLog();
                            {
                                z.SourceID = grantID;
                                z.TableName = "tbl_Mgr_DateTimeGrant";
                                z.AuditProcess = "Created Date/Time Grant for period " + dtDateFrom + " to " + dtDateTo + ")";
                                z.DateTime = DateTime.Now;
                                z.UserID = userID;
                            };
                            model.tbl_AuditLog.Add(z);
                            model.SaveChanges();
                        }
                        else
                        {
                            successfulCommit = false;
                            MISC.writetoAlertLog("No grant ID returned after insert!!");
                        }
                    }
                    if (grantID != 0)
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
    }
}