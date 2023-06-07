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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class MainTaskTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MainTaskTable()
        {
            this.MT_IMAGES = new HashSet<MT_IMAGES>();
        }
    
        public System.Guid MainTaskID { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan Time { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan Duration { get; set; }
        [Required]
        public string Shop { get; set; }
        [Required]
        [MaxLength( 150, ErrorMessage = "Maximum length can't be more than 150 characters." )]
        public string System { get; set; }
        [Required]
        [DisplayName("What happened")]
        public string Problem { get; set; }
        [Required]
        [DisplayName("How it was solved")]
        public string Solution { get; set; }
        public string Cooperation { get; set; }
        public System.Guid ReportID { get; set; }
    
        public virtual ReportTable ReportTable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MT_IMAGES> MT_IMAGES { get; set; }
    }
}
