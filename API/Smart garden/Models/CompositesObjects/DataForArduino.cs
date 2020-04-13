using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Models.ZoneDto;

namespace Smart_garden.Models.CompositesObjects
{
    public class DataForArduino
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public float TemperatureMin { get; set; }
        public float TemperatureMax { get; set; }
        public bool Manual { get; set; }
        public bool Working { get; set; }
        public string FCMToken { get; set; }
        public bool AutomationMode { get; set; }
        private ICollection<ZoneDtoForArduino> Zone { get; set; }

    }
}
