using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Smart_garden.Entites;

namespace Smart_garden.Repository.SystemStateRepository
{
    public class SystemStateRepository: Repository<SystemState>, ISystemStateRepository
    {
        private readonly SmartGardenContext _context;

        public SystemStateRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetSystemStatesBySystem(int systemId)
        {
            var states = (from systemState in _context.SystemState
                    join irigationSystem in _context.IrigationSystem
                        on systemState.SystemId equals irigationSystem.Id
                    where systemState.SystemId == systemId
                    select new
                    {
                        systemState.Id,
                        systemState.Working,
                        systemState.DateTime
                    }
                );
            return states;
        }

       public  SystemState GetCurrentState(int systemId)
        {
            var state = (from systemState in _context.SystemState
                join irigationSystem in _context.IrigationSystem
                    on systemState.SystemId equals irigationSystem.Id
                orderby systemState.Id descending
                select systemState).Take(1).SingleOrDefault();

            return state;
        }
    }
}
