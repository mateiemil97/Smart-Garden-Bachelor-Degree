using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.SystemStateRepository
{
    public interface ISystemStateRepository: IRepository<SystemState>
    {
        IQueryable GetSystemStatesBySystem(int systemId);
        SystemState GetCurrentState(int systemId);
    }
}
