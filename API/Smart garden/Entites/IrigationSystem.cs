﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Repository;

namespace Smart_garden.Entites
{
    public class IrigationSystem 
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("BoardKey")]
        public int BoardKeyId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public User User { get; set; }
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<SystemState> SystemState = new List<SystemState>();
        public ICollection<Schedule> Schedule = new List<Schedule>();
        public BoardsKeys BoardKey { get; set; }
        public FCMToken FCMToken { get; set; }



    }
}
