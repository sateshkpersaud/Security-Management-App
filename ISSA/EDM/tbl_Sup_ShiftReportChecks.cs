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
    
    public partial class tbl_Sup_ShiftReportChecks
    {
        public tbl_Sup_ShiftReportChecks()
        {
            this.tbl_Sup_ShiftReportChecks_Officers = new HashSet<tbl_Sup_ShiftReportChecks_Officers>();
        }
    
        public long ChecksID { get; set; }
        public Nullable<int> ShiftReportID { get; set; }
        public Nullable<short> LocationID { get; set; }
        public Nullable<byte> CheckSequence { get; set; }
        public string GeneralObservations { get; set; }
        public string CorrectionsMadeSuggested { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual tbl_Ref_Locations tbl_Ref_Locations { get; set; }
        public virtual tbl_Sup_ShiftReport tbl_Sup_ShiftReport { get; set; }
        public virtual ICollection<tbl_Sup_ShiftReportChecks_Officers> tbl_Sup_ShiftReportChecks_Officers { get; set; }
    }
}
