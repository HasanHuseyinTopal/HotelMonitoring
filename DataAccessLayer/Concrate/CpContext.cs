using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace DataAccessLayer.Concrate
{
    public class CpContext : IdentityDbContext<IdentityUser>
    {
        public CpContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseQueryTrackingBehavior(queryTrackingBehavior: QueryTrackingBehavior.NoTrackingWithIdentityResolution);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var item in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                item.DeleteBehavior = DeleteBehavior.Cascade;
            }
            modelBuilder.Entity<Payment>().Navigation(x => x.Visitor).AutoInclude();
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<FinancialManagement> FinancialManagements { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}
