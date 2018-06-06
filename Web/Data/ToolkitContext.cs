using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
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
    public class FinanceContext : DbContext
    {
        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        {

        }
        public DbSet<KLHKG_PriceBook> KLHKG_PriceBook { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KLHKG_PriceBook>().ToTable("KLHKG_PriceBookA300");
        }
    }
    public class KnowledgeBaseContext : DbContext
    {
        public KnowledgeBaseContext(DbContextOptions<KnowledgeBaseContext> options) : base(options)
        {

        }
        public DbSet<EBook> EBook { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EBook>().ToTable("EBook");
        }
    }
}
