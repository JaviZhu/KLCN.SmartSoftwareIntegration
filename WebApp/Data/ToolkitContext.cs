using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class ToolkitContext:DbContext
    {
        public ToolkitContext(DbContextOptions<ToolkitContext> options): base(options)
        {

        }
        public DbSet<HardDisk> HardDisk { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HardDisk>().ToTable("HardDisk");
        }
    }
}
