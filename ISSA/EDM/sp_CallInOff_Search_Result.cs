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
    
    public partial class sp_CallInOff_Search_Result
    {
        public string ReportNumber { get; set; }
        public int CallInOffID { get; set; }
        public string isCallIn { get; set; }
        public Nullable<short> EmployeeID { get; set; }
        public string EmpFullName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public Nullable<short> CallTakenByID { get; set; }
        public string CallTakenBy { get; set; }
        public Nullable<short> ForwardedToID { get; set; }
        public string ForwardedTo { get; set; }
        public string WorkSchedule { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<byte> AbsentTypeID { get; set; }
        public string AbsentType { get; set; }
        public Nullable<byte> SickLeaveForID { get; set; }
        public string SickLeaveFor { get; set; }
        public string Comments { get; set; }
        public string OtherTypeDescription { get; set; }
        public string OtherSickLeaveForTypeDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
    }
}
