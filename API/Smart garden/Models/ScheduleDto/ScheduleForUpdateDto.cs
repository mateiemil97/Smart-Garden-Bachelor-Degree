﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.ScheduleDto
{
    public class ScheduleForUpdateDto
    {
        public int SystemId { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public float TemperatureMin { get; set; }
        public float TemperatureMax { get; set; }
    }
}
