using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Models.ScheduleDto;

namespace Smart_garden.Repository.ScheduleRepository
{
    public interface IScheduleRepository: IRepository<Schedule>
    {
        IQueryable<Schedule> GetSchedules(int systemId);
        Schedule GetSchedule(int systemId);
    }
}
