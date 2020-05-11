using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Models.ZoneDto;

namespace Smart_garden.Repository.ZoneRepository
{
    public interface IZoneRepository: IRepository<Zone>
    {
        IEnumerable<ZoneDtoForGet> GetZonesBySystem(int systemId);
        ZoneDtoForGet GetZoneBySystem(int systemId, int id);
    }
}
