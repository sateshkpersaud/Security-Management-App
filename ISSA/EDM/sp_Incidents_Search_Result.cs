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
    
    public partial class sp_Incidents_Search_Result
    {
        public int IncidentID { get; set; }
        public string IncidentNumber { get; set; }
        public Nullable<byte> AreaID { get; set; }
        public string Area { get; set; }
        public Nullable<short> LocationID { get; set; }
        public string LocationName { get; set; }
        public string DateOccured { get; set; }
        public string TimeOccured { get; set; }
        public string IncidentType { get; set; }
        public Nullable<byte> IncidentTypeID { get; set; }
        public string OtherTypeDescription { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
    }
}