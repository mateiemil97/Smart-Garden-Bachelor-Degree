using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class Zone
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [ForeignKey("Sensor")]
        public int SensorId { get; set; }
        public int MoistureStart { get; set; }
        public int MoistureStop { get; set; }
        public bool WaterSwitch { get; set; }
        public Sensor Sensor { get; set; }
    }
}
