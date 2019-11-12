using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.MeasurementRepository
{
    public class MeasurementRepository: Repository<Measurement>, IMeasurementRepository
    {
        private readonly SmartGardenContext _context;

        public MeasurementRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public override IEnumerable GetAll()
        {
            var measurements = (from measure in _context.Measurement
                    join sns in _context.Sensor on measure.SensorId equals sns.Id
                    select new
                    {
                        Id = measure.Id,
                        SensorId = sns.Id,
                        Type = sns.Type,
                        Value = sns.Value,
                        DateTime = measure.DateTime
                    }
                );
            return measurements;
        }
    }
}
