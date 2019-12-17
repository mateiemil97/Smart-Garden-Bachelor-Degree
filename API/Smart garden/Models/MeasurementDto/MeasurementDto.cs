using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.MeasurementDto
{
    public class MeasurementDto
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public float Value { get; set; }
        public string Zone { get; set; }
        public DateTime DateTime { get; set; }
    }
}
