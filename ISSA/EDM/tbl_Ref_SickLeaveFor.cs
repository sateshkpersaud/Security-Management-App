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
    
    public partial class tbl_Ref_SickLeaveFor
    {
        public tbl_Ref_SickLeaveFor()
        {
            this.tbl_Op_CallIn_Absent = new HashSet<tbl_Op_CallIn_Absent>();
        }
    
        public byte SickLeaveForID { get; set; }
        public string LeaveFor { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<byte> Seq { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual ICollection<tbl_Op_CallIn_Absent> tbl_Op_CallIn_Absent { get; set; }
    }
}