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
    
    public partial class tbl_Ref_Locations
    {
        public tbl_Ref_Locations()
        {
            this.tbl_Mgr_SpecialOperations = new HashSet<tbl_Mgr_SpecialOperations>();
            this.tbl_Mgr_WorkSchedule_Employees = new HashSet<tbl_Mgr_WorkSchedule_Employees>();
            this.tbl_Op_Complaints = new HashSet<tbl_Op_Complaints>();
            this.tbl_Op_DailyLogs = new HashSet<tbl_Op_DailyLogs>();
            this.tbl_Sup_Employees = new HashSet<tbl_Sup_Employees>();
            this.tbl_Sup_ShiftReportChecks = new HashSet<tbl_Sup_ShiftReportChecks>();
            this.tbl_Op_Incidents = new HashSet<tbl_Op_Incidents>();
        }
    
        public short LocationID { get; set; }
        public Nullable<byte> AreaID { get; set; }
        public string LocationName { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public Nullable<byte> RegionID { get; set; }
        public string ContactNumberOne { get; set; }
        public string ContactNumberTwo { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual ICollection<tbl_Mgr_SpecialOperations> tbl_Mgr_SpecialOperations { get; set; }
        public virtual ICollection<tbl_Mgr_WorkSchedule_Employees> tbl_Mgr_WorkSchedule_Employees { get; set; }
        public virtual ICollection<tbl_Op_Complaints> tbl_Op_Complaints { get; set; }
        public virtual ICollection<tbl_Op_DailyLogs> tbl_Op_DailyLogs { get; set; }
        public virtual tbl_Ref_Areas tbl_Ref_Areas { get; set; }
        public virtual tbl_Ref_Regions tbl_Ref_Regions { get; set; }
        public virtual ICollection<tbl_Sup_Employees> tbl_Sup_Employees { get; set; }
        public virtual ICollection<tbl_Sup_ShiftReportChecks> tbl_Sup_ShiftReportChecks { get; set; }
        public virtual ICollection<tbl_Op_Incidents> tbl_Op_Incidents { get; set; }
    }
}
