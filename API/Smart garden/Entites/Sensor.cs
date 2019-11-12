using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        [Required]
        public float Value { get; set; }

        public IrigationSystem System { get; set; }

        public ICollection<Measurement> Measurement = new List<Measurement>();

    }
}
