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
    
    public partial class tbl_Ref_Shifts
    {
        public tbl_Ref_Shifts()
        {
            this.tbl_Mgr_WorkSchedule = new HashSet<tbl_Mgr_WorkSchedule>();
            this.tbl_Op_DailyLogs = new HashSet<tbl_Op_DailyLogs>();
            this.tbl_Sup_Employees = new HashSet<tbl_Sup_Employees>();
            this.tbl_Sup_ShiftReport = new HashSet<tbl_Sup_ShiftReport>();
            this.tbl_Sup_DailyPostAssignment = new HashSet<tbl_Sup_DailyPostAssignment>();
            this.tbl_Mgr_WeeklySecurityReportComparison = new HashSet<tbl_Mgr_WeeklySecurityReportComparison>();
        }
    
        public byte ShiftID { get; set; }
        public Nullable<byte> PositionID { get; set; }
        public Nullable<System.TimeSpan> FromTime { get; set; }
        public Nullable<System.TimeSpan> ToTime { get; set; }
        public string HoursInShift { get; set; }
        public Nullable<bool> IsDay { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual ICollection<tbl_Mgr_WorkSchedule> tbl_Mgr_WorkSchedule { get; set; }
        public virtual ICollection<tbl_Op_DailyLogs> tbl_Op_DailyLogs { get; set; }
        public virtual tbl_Ref_Positions tbl_Ref_Positions { get; set; }
        public virtual ICollection<tbl_Sup_Employees> tbl_Sup_Employees { get; set; }
        public virtual ICollection<tbl_Sup_ShiftReport> tbl_Sup_ShiftReport { get; set; }
        public virtual ICollection<tbl_Sup_DailyPostAssignment> tbl_Sup_DailyPostAssignment { get; set; }
        public virtual ICollection<tbl_Mgr_WeeklySecurityReportComparison> tbl_Mgr_WeeklySecurityReportComparison { get; set; }
    }
}
