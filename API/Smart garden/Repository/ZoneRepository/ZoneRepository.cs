using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Smart_garden.Entites;

namespace Smart_garden.Repository.ZoneRepository
{
    public class ZoneRepository: Repository<Zone>,IZoneRepository
    {
       private SmartGardenContext _context;

        public ZoneRepository(SmartGardenContext context): base(context)
        {
            _context = context;
        }

        public  IEnumerable<Zone> GetZonesBySystem(int systemId)
        {
            var zones = from zone in _context.Zone
                join sns in _context.Sensor on zone.SensorId equals sns.Id
                join sys in _context.IrigationSystem on sns.SystemId equals sys.Id
                where sys.Id == systemId
                select zone;

            return zones;
        }

        public Zone GetZoneBySystem(int systemId, int id)
        {
            var zoneToReturn = (from zone in _context.Zone
                join sns in _context.Sensor on zone.SensorId equals sns.Id
                join sys in _context.IrigationSystem on sns.SystemId equals sys.Id
                where sys.Id == systemId && zone.Id == id
                select zone).FirstOrDefault();

            return zoneToReturn;
        }
    }
}
