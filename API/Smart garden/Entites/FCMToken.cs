using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Entites
{
    public class FCMToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("IrigationSystem")]
        public int SystemId { get; set; }
        public string Token { get; set; }

        public IrigationSystem IrigationSystem { get; set; }


    }
}
