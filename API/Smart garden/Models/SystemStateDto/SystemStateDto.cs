using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.SystemStateDto
{
    public class SystemStateDto
    {
        public int Id { get; set; }
        public bool Working { get; set; }
        public bool Manual { get; set; }
        public bool AutomationMode { get; set; }
        public DateTime DateTime { get; set; }
    }
}
