using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Models.ScheduleDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems/{systemId}")]
    public class ScheduleController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ScheduleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("schedules")]
        public IActionResult GetSchedules(int systemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return BadRequest("Irrigation system not found!");
            }

            var schedulesFromRepo = _unitOfWork.ScheduleRepository.GetSchedules(systemId);

            var schedulesMapped = _mapper.Map<IEnumerable<ScheduleDto>>(schedulesFromRepo);

            return Ok(schedulesMapped);
        }

        [HttpGet("schedule")]
        public IActionResult GetSchedule(int systemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return BadRequest("Irrigation system not found!");
            }

            var schedulesFromRepo = _unitOfWork.ScheduleRepository.GetSchedule(systemId);

            var schedulesMapped = _mapper.Map<ScheduleDto>(schedulesFromRepo);

            return Ok(schedulesMapped);
        }
    }
}
