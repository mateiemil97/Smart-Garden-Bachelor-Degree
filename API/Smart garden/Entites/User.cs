using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Smart_garden.Repository;

namespace Smart_garden.Entites
{
    public class User: IdentityUser
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(70)]
        [Required]
        public string FirstName { get; set; }

        public  ICollection<IrigationSystem> System { get; set; } = new List<IrigationSystem>();

    }
}
