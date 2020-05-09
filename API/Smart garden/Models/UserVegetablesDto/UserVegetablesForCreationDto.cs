using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Models.UserVegetablesDto
{
    public class UserVegetablesForCreationDto
    {
        public string Name { get; set; }
        public int StartMoisture { get; set; }
        public int StopMoisture { get; set; }
        public int UserId { get; set; }
    }
}
