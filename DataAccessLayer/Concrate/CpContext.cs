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
            modelBuilder.Entity<Visitor>().Navigation(x => x.Agency).AutoInclude(true);
            foreach (var item in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                item.DeleteBehavior = DeleteBehavior.Cascade;
            }

            modelBuilder.Entity<Room>().HasData(new List<Room>()
            {
                new Room(){RoomId=1, RoomIsClean=true,RoomNumber=101,RoomState=false},
                new Room(){RoomId=2, RoomIsClean=true,RoomNumber=102,RoomState=false},
                new Room(){RoomId=3, RoomIsClean=true,RoomNumber=103,RoomState=false},
                new Room(){RoomId=4, RoomIsClean=true,RoomNumber=104,RoomState=false},
                new Room(){RoomId=5, RoomIsClean=true,RoomNumber=201,RoomState=false},
                new Room(){RoomId=6, RoomIsClean=true,RoomNumber=202,RoomState=false},
                new Room(){RoomId=7, RoomIsClean=true,RoomNumber=203,RoomState=false},
                new Room(){RoomId=8, RoomIsClean=true,RoomNumber=204,RoomState=false},
                new Room(){RoomId=9, RoomIsClean=true,RoomNumber=301,RoomState=false},
                new Room(){RoomId=10, RoomIsClean=true,RoomNumber=302,RoomState=false},
                new Room(){RoomId=11, RoomIsClean=true,RoomNumber=303,RoomState=false},
                new Room(){RoomId=12, RoomIsClean=true,RoomNumber=304,RoomState=false},
                new Room(){RoomId=13, RoomIsClean=true,RoomNumber=401,RoomState=false},
                new Room(){RoomId=14, RoomIsClean=true,RoomNumber=402,RoomState=false},
                new Room(){RoomId=15, RoomIsClean=true,RoomNumber=403,RoomState=false},
                new Room(){RoomId=16, RoomIsClean=true,RoomNumber=404,RoomState=false},
                new Room(){RoomId=17, RoomIsClean=true,RoomNumber=501,RoomState=false},
                new Room(){RoomId=18, RoomIsClean=true,RoomNumber=502,RoomState=false},
                new Room(){RoomId=19, RoomIsClean=true,RoomNumber=503,RoomState=false},
                new Room(){RoomId=20, RoomIsClean=true,RoomNumber=601,RoomState=false},
                new Room(){RoomId=21, RoomIsClean=true,RoomNumber=602,RoomState=false},
                new Room(){RoomId=22, RoomIsClean=true,RoomNumber=603,RoomState=false},
                new Room(){RoomId=23, RoomIsClean=true,RoomNumber=701,RoomState=false},
                new Room(){RoomId=24, RoomIsClean=true,RoomNumber=702,RoomState=false}
            });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<VisitorHistory> VisitorHistories { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<VisitorProperty> VisitorProperties { get; set; }
        public DbSet<FinancialManagement> FinancialManagements { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Agency> Agencies { get; set; }
    }
}
