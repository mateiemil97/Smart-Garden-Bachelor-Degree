using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotion.Linq.Clauses;
using Smart_garden.Entites;
using Smart_garden.Models.SensorDto;

namespace Smart_garden.Repository.SensorRepository
{
    public class SensorRepository:Repository<Sensor>, ISensorRepository
    {
        private readonly SmartGardenContext _context;
        public SensorRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public object GetSensorsBySystem(int systemId)
        {
            var sensors = (from sensor in _context.Sensor
                    join systems in _context.IrigationSystem
                        on sensor.SystemId equals systems.Id
                           where sensor.SystemId == systemId
                    select new
                    {
                        IrigationSystemId = systems.Id,
                        SensorId = sensor.Id,
                        Type = sensor.Type,
                        Value = sensor.Value
                    }
                );
            return sensors;
        }

        public IQueryable<Sensor> GetSensorBySystem(int systemId, int sensorId)
        {
            var sensor = (from sys in _context.IrigationSystem
                    join sns in _context.Sensor
                        on sys.Id equals sns.SystemId
                    where sns.SystemId == sys.Id && sns.Id == sensorId
                    select new Sensor
                    {
                        Id = sns.Id,
                        SystemId = sys.Id,
                        Type = sns.Type,
                        Value = sns.Value
                    }
                );
            return sensor;
        }
    }
}
