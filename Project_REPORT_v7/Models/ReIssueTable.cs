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
    
    public partial class ReIssueTable
    {
        public System.Guid ReIssueID { get; set; }
        public System.TimeSpan Time { get; set; }
        public string User { get; set; }
        public string Objective { get; set; }
        public string BodyNum { get; set; }
        public System.Guid ReportID { get; set; }
    
        public virtual ReportTable ReportTable { get; set; }
    }
}
