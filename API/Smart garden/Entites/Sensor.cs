using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class Sensor
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("System")]
        public int SystemId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        [ForeignKey("SensorPort")]
        public int PortId { get; set; }

        
        public IrigationSystem System { get; set; }
        public SensorPort SensorPort { get; set; }
        public ICollection<Measurement> Measurement { get; set; }
        public Zone Zone {get;set;}
    }
}
