using MachineSerialPort.Databases.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases
{
    public class SapDbContext(DbContextOptions<SapDbContext> options) : DbContext(options)
    {
        public DbSet<Rfc_pass> Rfc_pass { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rfc_pass>()
                .HasNoKey(); // Declares this entity as keyless
        }
    }
}
