﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ReportDBEntities1 : DbContext
    {
        public ReportDBEntities1()
            : base("name=ReportDBEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<HourOvertimeTable> HourOvertimeTable { get; set; }
        public virtual DbSet<MainTaskTable> MainTaskTable { get; set; }
        public virtual DbSet<MembersTable> MembersTable { get; set; }
        public virtual DbSet<PasswordTable> PasswordTable { get; set; }
        public virtual DbSet<PreCheckTable> PreCheckTable { get; set; }
        public virtual DbSet<PrintersTable> PrintersTable { get; set; }
        public virtual DbSet<ReIssueTable> ReIssueTable { get; set; }
        public virtual DbSet<ReportTable> ReportTable { get; set; }
        public virtual DbSet<PermisionTable> PermisionTable { get; set; }
        public virtual DbSet<LogTable> LogTable { get; set; }
        public virtual DbSet<MT_IMAGES> MT_IMAGES { get; set; }
    }
}
