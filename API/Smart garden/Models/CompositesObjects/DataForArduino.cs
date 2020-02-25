﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.CompositesObjects
{
    public class DataForArduino
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public float TemperatureMin { get; set; }
        public float TemperatureMax { get; set; }
        public bool Manual { get; set; }
        public DateTime CurrentTime { get; set; } 

        
    }
}