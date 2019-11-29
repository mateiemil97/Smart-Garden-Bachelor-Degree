using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.ScheduleDto
{
    public class ScheduleForCreationDto
    {
        public int SystemId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public float TemperatureStart { get; set; }
        public bool Manual { get; set; }
    }
}
