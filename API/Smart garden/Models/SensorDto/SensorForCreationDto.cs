using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.SensorDto
{
    public class SensorForCreationDto
    {
        public int SystemId { get; set; }
        public string Type { get; set; }
        public float Value { get; set; }
        public DateTime DateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
