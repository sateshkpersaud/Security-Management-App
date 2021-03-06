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
    
    public partial class tbl_Sup_Employees
    {
        public tbl_Sup_Employees()
        {
            this.tbl_Mgr_WorkSchedule_Employees = new HashSet<tbl_Mgr_WorkSchedule_Employees>();
            this.tbl_Mgr_WorkSchedule_Employees1 = new HashSet<tbl_Mgr_WorkSchedule_Employees>();
            this.tbl_Op_Complaints = new HashSet<tbl_Op_Complaints>();
            this.tbl_Sup_ShiftReportChecks_Officers = new HashSet<tbl_Sup_ShiftReportChecks_Officers>();
            this.tbl_Op_DailyLogsPersonWorking = new HashSet<tbl_Op_DailyLogsPersonWorking>();
            this.tbl_Op_CallIn_Absent = new HashSet<tbl_Op_CallIn_Absent>();
            this.tbl_Op_CallIn_Absent1 = new HashSet<tbl_Op_CallIn_Absent>();
            this.tbl_Op_CallIn_Absent2 = new HashSet<tbl_Op_CallIn_Absent>();
        }
    
        public short EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public Nullable<System.DateTime> DateHired { get; set; }
        public Nullable<byte> AreaID { get; set; }
        public Nullable<short> LocationID { get; set; }
        public Nullable<byte> DepartmentID { get; set; }
        public Nullable<byte> PositionID { get; set; }
        public Nullable<byte> ShiftID { get; set; }
        public Nullable<byte> UID { get; set; }
        public Nullable<bool> ResetNeeded { get; set; }
        public Nullable<bool> IsStandbyStaff { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string EmployeeNumber { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public Nullable<bool> IsEmployee { get; set; }
    
        public virtual ICollection<tbl_Mgr_WorkSchedule_Employees> tbl_Mgr_WorkSchedule_Employees { get; set; }
        public virtual ICollection<tbl_Mgr_WorkSchedule_Employees> tbl_Mgr_WorkSchedule_Employees1 { get; set; }
        public virtual ICollection<tbl_Op_Complaints> tbl_Op_Complaints { get; set; }
        public virtual tbl_Ref_Areas tbl_Ref_Areas { get; set; }
        public virtual tbl_Ref_Departments tbl_Ref_Departments { get; set; }
        public virtual tbl_Ref_Locations tbl_Ref_Locations { get; set; }
        public virtual tbl_Ref_Positions tbl_Ref_Positions { get; set; }
        public virtual tbl_Ref_Shifts tbl_Ref_Shifts { get; set; }
        public virtual ICollection<tbl_Sup_ShiftReportChecks_Officers> tbl_Sup_ShiftReportChecks_Officers { get; set; }
        public virtual ICollection<tbl_Op_DailyLogsPersonWorking> tbl_Op_DailyLogsPersonWorking { get; set; }
        public virtual ICollection<tbl_Op_CallIn_Absent> tbl_Op_CallIn_Absent { get; set; }
        public virtual ICollection<tbl_Op_CallIn_Absent> tbl_Op_CallIn_Absent1 { get; set; }
        public virtual ICollection<tbl_Op_CallIn_Absent> tbl_Op_CallIn_Absent2 { get; set; }
    }
}
