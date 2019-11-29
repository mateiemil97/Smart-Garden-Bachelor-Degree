using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Remotion.Linq.Clauses;
using Smart_garden.Entites;
using Smart_garden.Models.ScheduleDto;

namespace Smart_garden.Repository.ScheduleRepository
{
    public class ScheduleRepository: Repository<Schedule>, IScheduleRepository
    {
        private SmartGardenContext _context;
        public ScheduleRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable<Schedule> GetSchedules(int systemId)
        {
            var schedules = (from sys in _context.IrigationSystem
                join sch in _context.Schedule
                    on sys.Id equals sch.SystemId
                where sys.Id == systemId
                select sch);

            return schedules;
        }

        public Schedule GetSchedule(int systemId)
        {
            var schedule = (from sys in _context.IrigationSystem
                join sch in _context.Schedule
                    on sys.Id equals sch.SystemId
                where sys.Id == systemId
                orderby sch.Id descending
                select sch).Take(1).SingleOrDefault();

            return schedule;
        }
    }
}
