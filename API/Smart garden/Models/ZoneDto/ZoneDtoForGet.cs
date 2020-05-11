using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.ZoneDto
{
    public class ZoneDtoForGet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SensorId { get; set; }
        public int MoistureStart { get; set; }
        public int UserVegetableId { get; set; }
        public string UserVegetableName { get; set; }
        public int MoistureStop { get; set; }
        public bool WaterSwitch { get; set; }
    }
}
