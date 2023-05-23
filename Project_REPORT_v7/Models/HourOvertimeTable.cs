//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Project_REPORT_v7.Models
{

    public partial class HourOvertimeTable
    {
        public System.Guid OvertimeID { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan Time { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan Duration { get; set; }
        [Required]
        public string Shop { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Description { get; set; }
        public string Cooperation { get; set; }
        public System.Guid ReportID { get; set; }
    
        public virtual ReportTable ReportTable { get; set; }
    }
}
