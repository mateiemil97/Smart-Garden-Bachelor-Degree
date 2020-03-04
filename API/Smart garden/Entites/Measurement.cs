using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class Measurement
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Sensor")]
        public int SensorId { get; set; }
        public float Value { get; set; }

        public DateTime DateTime { get; set; }
        public Sensor Sensor { get; set; }
    }
}
