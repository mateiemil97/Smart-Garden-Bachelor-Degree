using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Models.MeasurementDto;
using Smart_garden.Repository.MeasurementRepository;

namespace Smart_garden.Repository.BoardsKeyRepository
{
    public class MeasurementRepository: Repository<Measurement>, IMeasurementRepository
    {
        private SmartGardenContext _context;
        public MeasurementRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public MeasurementDto GetLatestMeasurementValueByType(int systemId,int sensorId)
        {
            var measurement = (from sys in _context.IrigationSystem
                join sns in _context.Sensor
                    on sys.Id equals sns.SystemId
                join measure in _context.Measurement
                   on sns.Id equals  measure.SensorId
                join snsPort in _context.SensorPort
                    on sns.PortId equals snsPort.Id
                join zone in _context.Zone
                    on sns.Id equals zone.SensorId
                where sns.Id == sensorId && sys.Id == systemId
                orderby measure.DateTime descending
                select new MeasurementDto()
                {
                    DateTime = measure.DateTime,
                    Id = measure.Id,
                    SensorId = measure.SensorId,
                    Value = measure.Value,
                    Zone = zone.Name
                }
                ).Take(1).SingleOrDefault();

            return measurement;
        }

        public MeasurementDto GetLatestMeasurementOfTemperature(int systemId)
        {
            var temperature = (from sys in _context.IrigationSystem
                join sns in _context.Sensor
                    on sys.Id equals sns.SystemId
                join measure in _context.Measurement
                    on sns.Id equals measure.SensorId
                where sys.Id == systemId && sns.Type == "Temperature"
                orderby measure.DateTime descending
                select new MeasurementDto()
                {
                    DateTime = measure.DateTime,
                    Id = measure.Id,
                    SensorId = measure.SensorId,
                    Value = measure.Value,
                    Zone = "Environment"
                }
                ).Take(1).SingleOrDefault();

            return temperature;
        }

    }
}
