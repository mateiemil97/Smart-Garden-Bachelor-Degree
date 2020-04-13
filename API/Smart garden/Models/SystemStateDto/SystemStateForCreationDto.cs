using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.SystemStateDto
{
    public class SystemStateForCreationDto
    {
        public bool Working { get; set; }
        public DateTime DateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        public bool Manual { get; set; }
        public bool AutomationMode { get; set; }

    }
}
