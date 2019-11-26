﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class Schedule
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("System")]
        public int  SystemId { get; set; }

        [Required]
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public float TemperatureStart {get; set; }
        public int MoistureStart { get; set; }
        public int MoistureReference { get; set; }
        
        public IrigationSystem System { get; set; }
        
    }
}
