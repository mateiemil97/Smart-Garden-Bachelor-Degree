﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.MeasurementDto
{
    public class MeasurementForCreationDto
    { 
        public int SensorId { get; set; }
        public float Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
