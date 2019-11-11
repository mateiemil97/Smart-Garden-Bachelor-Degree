using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class SystemState
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("System")]
        [Required]
        public int SystemId { get; set; }

        public bool Working { get; set; }
        public DateTime DateTime { get; set; }

        public IrigationSystem System { get; set; }
    }
}
