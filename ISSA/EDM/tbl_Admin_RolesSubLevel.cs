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
    
    public partial class tbl_Admin_RolesSubLevel
    {
        public tbl_Admin_RolesSubLevel()
        {
            this.tbl_Admin_User_Role = new HashSet<tbl_Admin_User_Role>();
        }
    
        public short RoleSubLevelID { get; set; }
        public Nullable<byte> RoleID { get; set; }
        public string RoleTitle { get; set; }
        public string RoleDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<byte> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<byte> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual tbl_Admin_RolesMasterLevel tbl_Admin_RolesMasterLevel { get; set; }
        public virtual ICollection<tbl_Admin_User_Role> tbl_Admin_User_Role { get; set; }
    }
}