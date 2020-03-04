using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Smart_garden.Entites
{
    public class SmartGardenContext: DbContext
    {
        public SmartGardenContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<IrigationSystem> IrigationSystem { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<SystemState> SystemState { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<BoardsKeys> BoardKey { get; set; }
        public DbSet<SensorPort> SensorPort { get; set; }
        public DbSet<Measurement> Measurement { get; set; }
        public DbSet<Zone> Zone { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);
//            modelBuilder.Entity<User>()
//                .Ignore(c => c.AccessFailedCount)
//                .Ignore(c => c.LockoutEnabled)
//                .Ignore(c => c.TwoFactorEnabled)
//                .Ignore(c => c.ConcurrencyStamp)
//                .Ignore(c => c.LockoutEnd)
//                .Ignore(c => c.UserName)
//                .Ignore(c => c.NormalizedUserName)
//                .Ignore(c => c.NormalizedEmail)
//                .Ignore(c => c.EmailConfirmed)
//                .Ignore(c => c.TwoFactorEnabled)
//                .Ignore(c => c.LockoutEnd)
//                .Ignore(c => c.AccessFailedCount)
//                .Ignore(c => c.PhoneNumberConfirmed);
//
//
//            modelBuilder.Entity<User>().ToTable("Users");//to change the name of table.
//
//        }
    }
}
