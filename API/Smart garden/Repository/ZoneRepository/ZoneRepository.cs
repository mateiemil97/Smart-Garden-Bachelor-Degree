using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Smart_garden.Entites;
using Smart_garden.Models.ZoneDto;

namespace Smart_garden.Repository.ZoneRepository
{
    public class ZoneRepository: Repository<Zone>,IZoneRepository
    {
       private SmartGardenContext _context;

        public ZoneRepository(SmartGardenContext context): base(context)
        {
            _context = context;
        }

        public  IEnumerable<ZoneDtoForGet> GetZonesBySystem(int systemId)
        {
            var zones = from zone in _context.Zone
                join sns in _context.Sensor on zone.SensorId equals sns.Id
                join sys in _context.IrigationSystem on sns.SystemId equals sys.Id
                join veg in _context.UserVegetableses on zone.UserVegetableId equals veg.Id
                where sys.Id == systemId
                select new ZoneDtoForGet()
                {
                    Id = zone.Id,
                    MoistureStart = zone.MoistureStart,
                    MoistureStop = zone.MoistureStop,
                    Name = zone.Name,
                    SensorId = zone.SensorId,
                    UserVegetableId = zone.UserVegetableId,
                    UserVegetableName = veg.Name,
                    WaterSwitch = zone.WaterSwitch
                };

            return zones;
        }

        public ZoneDtoForGet GetZoneBySystem(int systemId, int id)
        {
            var zoneToReturn = (from zone in _context.Zone
                join sns in _context.Sensor on zone.SensorId equals sns.Id
                join sys in _context.IrigationSystem on sns.SystemId equals sys.Id
                join veg in _context.UserVegetableses on zone.UserVegetableId equals veg.Id
                where sys.Id == systemId && zone.Id == id
                select new ZoneDtoForGet()
                {
                    Id = zone.Id,
                    MoistureStart = zone.MoistureStart,
                    MoistureStop = zone.MoistureStop,
                    Name = zone.Name,
                    SensorId = zone.SensorId,
                    UserVegetableId = zone.UserVegetableId,
                    UserVegetableName = veg.Name,
                    WaterSwitch = zone.WaterSwitch
                }).FirstOrDefault();

            return zoneToReturn;
        }
    }
}
