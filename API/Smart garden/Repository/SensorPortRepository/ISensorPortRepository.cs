using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.SensorPortRepository
{
    public interface ISensorPortRepository: IRepository<SensorPort>
    {
        IQueryable<SensorPort>GetUsedPorts(int systemId);
        IQueryable<SensorPort> GetAllPorts();
    }
}
