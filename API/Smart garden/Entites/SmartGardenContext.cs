using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
