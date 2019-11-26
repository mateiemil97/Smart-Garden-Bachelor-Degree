using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Smart_garden.Entites
{
    public class BoardsKeys
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string SeriesKey { get; set; }

        [Required]
        public bool Registered { get; set; }

        public IrigationSystem IrigationSystem { get; set; }

    }
}
