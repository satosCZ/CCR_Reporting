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
    using System.ComponentModel;

    public partial class LogTable
    {
        public int Id { get; set; }
        [DisplayName("Date")]
        public Nullable<System.DateTime> L_DATE { get; set; }
        [DisplayName("Type")]
        public string L_TYPE { get; set; }
        [DisplayName("Message")]
        public string L_MESSAGE { get; set; }
        [DisplayName("User ID")]
        public Nullable<int> L_USER_ID { get; set; }
    }
}
