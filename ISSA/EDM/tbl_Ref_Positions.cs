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
    
    public partial class tbl_Ref_Positions
    {
        public tbl_Ref_Positions()
        {
            this.tbl_Ref_Shifts = new HashSet<tbl_Ref_Shifts>();
            this.tbl_Sup_Employees = new HashSet<tbl_Sup_Employees>();
        }
    
        public byte PositionID { get; set; }
        public string PositionName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual ICollection<tbl_Ref_Shifts> tbl_Ref_Shifts { get; set; }
        public virtual ICollection<tbl_Sup_Employees> tbl_Sup_Employees { get; set; }
    }
}
