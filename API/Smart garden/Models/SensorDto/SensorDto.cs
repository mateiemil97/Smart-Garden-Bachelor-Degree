using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.SensorDto
{
    public class SensorDto
    {
        public int Id { get; set; }
        public int SystemId { get; set; }
        public string Type { get; set; }
        public int SensorPortId { get; set; }
    }
}
