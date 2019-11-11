using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class Environment
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  int Id { get; set; }

        [ForeignKey("Sensor")]
        [Required]
        public int SensorId { get; set; }

        public DateTime DateTime { get; set; }

        public ICollection<Sensor> Sensor = new List<Sensor>();
    }
}
