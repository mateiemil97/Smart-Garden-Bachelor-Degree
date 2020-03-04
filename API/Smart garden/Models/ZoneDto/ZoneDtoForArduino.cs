using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.ZoneDto
{
    public class ZoneDtoForArduino
    {
        public int MoistureStart { get; set; }
        public int MoistureStop { get; set; }
        public bool WaterSwitch { get; set; }
    }
}
