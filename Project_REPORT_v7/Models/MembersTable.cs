//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_REPORT_v7.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class MembersTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MembersTable()
        {
            this.ReportTable = new HashSet<ReportTable>();
            this.ReportTable1 = new HashSet<ReportTable>();
        }

        [DisplayName( "Member ID" )]
        public int MemberID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [DisplayName( "Shift" )]
        public Nullable<int> ShiftID { get; set; }
        
        public Nullable<int> SendEmail { get; set; }
        [DisplayName( "Receive Email" )]
        public bool SetEmail
        {
            get
            {
                return SendEmail == 1;
            }
            set
            {
                SendEmail = value ? 1 : 0;
            }
        }

        public virtual ShiftTable ShiftTable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportTable> ReportTable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportTable> ReportTable1 { get; set; }
    }
}
