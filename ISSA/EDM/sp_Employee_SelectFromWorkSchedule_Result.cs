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
    
    public partial class sp_Employee_SelectFromWorkSchedule_Result
    {
        public long WorkScheduleEmployeeID { get; set; }
        public Nullable<short> EmployeeID { get; set; }
        public string FullName { get; set; }
        public string employeenumber { get; set; }
        public Nullable<short> locationid { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<bool> Exclude { get; set; }
        public Nullable<short> StandyByEmployeeID { get; set; }
        public string createdOn { get; set; }
    }
}