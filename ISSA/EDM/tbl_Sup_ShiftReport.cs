//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISSA.EDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Sup_ShiftReport
    {
        public tbl_Sup_ShiftReport()
        {
            this.tbl_Sup_ShiftReportChecks = new HashSet<tbl_Sup_ShiftReportChecks>();
        }
    
        public int ShiftReportID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<byte> AreaID { get; set; }
        public Nullable<byte> ShiftID { get; set; }
        public Nullable<short> SupervisorID { get; set; }
        public string ReportNumber { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual tbl_Ref_Areas tbl_Ref_Areas { get; set; }
        public virtual tbl_Ref_Shifts tbl_Ref_Shifts { get; set; }
        public virtual ICollection<tbl_Sup_ShiftReportChecks> tbl_Sup_ShiftReportChecks { get; set; }
    }
}