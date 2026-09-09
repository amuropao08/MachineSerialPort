using MachineSerialPort.Databases.Tables;
using Microsoft.EntityFrameworkCore;

namespace MachineSerialPort.Databases
{
    public class MaterialInfoDbContext(DbContextOptions<MaterialInfoDbContext> options) : DbContext(options)
    {
        public DbSet<material_creation_history> material_creation_history { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<material_creation_history>()
                .HasNoKey();
        }
    }
}
