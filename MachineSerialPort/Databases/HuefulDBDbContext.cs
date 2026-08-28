using MachineSerialPort.Databases.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases
{
    public class HuefulDBDbContext(DbContextOptions<HuefulDBDbContext> options) : DbContext(options)
    {
        public DbSet<MstrArticle> MstrArticle { get; set; }
        public DbSet<MstrCustomer> MstrCustomer { get; set; }
        public DbSet<MstrThickness> MstrThickness { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MstrArticle>()
                .HasNoKey();
            modelBuilder.Entity<MstrCustomer>()
                .HasNoKey();
            modelBuilder.Entity<MstrThickness>()
                .HasNoKey();
        }
    }
}
