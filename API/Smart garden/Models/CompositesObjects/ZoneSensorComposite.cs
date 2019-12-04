using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.CompositesObjects
{
    public class ZoneSensorComposite
    {
        public int SystemId { get; set; }
        public string Type { get; set; }
        public int PortId { get; set; }


        public string Name { get; set; }
        public int SensorId { get; set; }
        public int MoistureStart { get; set; }
        public int MoistureStop { get; set; }
    }
}
