using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.ZoneRepository
{
    public interface IZoneRepository: IRepository<Zone>
    {
        IEnumerable<Zone> GetZonesBySystem(int systemId);
        Zone GetZoneBySystem(int systemId, int id);
    }
}
