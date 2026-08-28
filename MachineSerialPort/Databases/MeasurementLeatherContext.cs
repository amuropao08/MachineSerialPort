using MachineSerialPort.Databases.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases
{
    public class MeasurementLeatherContext(DbContextOptions<MeasurementLeatherContext> options) : DbContext(options)
    {
        public DbSet<MsData> MsDatas { get; set; }
        public DbSet<MsMatDoc> MsMatDocs { get; set; }
        public DbSet<MsSummary> MsSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Table Names (Your previous fix)
            modelBuilder.Entity<MsSummary>().ToTable("MsSummary").HasKey(s => s.MsId);
            modelBuilder.Entity<MsMatDoc>().ToTable("MsMatDoc").HasKey(m => m.MsMdId);
            modelBuilder.Entity<MsData>().ToTable("MsData").HasKey(d => d.MsDId);

            // 2. Relationship: Summary -> MatDoc
            modelBuilder.Entity<MsMatDoc>()
                .HasOne(m => m.Summary)          // MatDoc has one Summary
                .WithMany(s => s.MatDocs)        // Summary has many MatDocs
                .HasForeignKey(m => m.MsId);     // The link is the 'MsId' column

            // 3. Relationship: MatDoc -> Data
            modelBuilder.Entity<MsData>()
                .HasOne(d => d.MatDoc)           // Data has one MatDoc
                .WithMany(m => m.Details)        // MatDoc has many Details
                .HasForeignKey(d => d.MsMdId);   // The link is the 'MsMdId' column

            // 4. Precision for Decimal
            modelBuilder.Entity<MsData>()
                .Property(p => p.Area)
                .HasPrecision(18, 3);

            base.OnModelCreating(modelBuilder);
        }
    }
}
