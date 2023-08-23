//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Entity Framework Version:6.1.3
//
//     Edited by: Jiri Kukuczka
//     Added Attributes: Required, DataType, DisplayFormat, DisplayName, MaxLength
//          Required - 4x (Time, FullName, UserID, System) - Prevents from saving empty values
//          DataType - 1x (Time) - Sets the type of the data
//          DisplayFormat - 1x (Time) - Sets the format of the data
//          DisplayName - 3x (Name, User ID, System) - Sets the display name of the data
//          MaxLength - 3x (Name, User ID, System) - Sets the maximum length of the data
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project_REPORT_v7.Models
{

    public partial class PreCheckTable
    {
        public System.Guid PreCheckID { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan Time { get; set; }
        [Required]
        [DisplayName("System")]
        [MaxLength( 50, ErrorMessage = "Maximum length can't be more than 50 characters." )]
        public string System { get; set; }
        [Required]
        [DisplayName("Check")]
        [MaxLength( 5, ErrorMessage = "Maximum length can't be more than 5 characters." )]
        public string Check { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public string EmailTime { get; set; }
        public System.Guid ReportID { get; set; }
    
        public virtual ReportTable ReportTable { get; set; }
    }
}
