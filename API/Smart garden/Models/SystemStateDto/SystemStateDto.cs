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
        public DateTime DateTime { get; set; }
    }
}
