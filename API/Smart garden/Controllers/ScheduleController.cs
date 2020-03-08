using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
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

        [HttpPost("schedules", Name = "Schedule")]
        public IActionResult CreateSchedule(int systemId, [FromBody] ScheduleForCreationDto scheduleForCreation)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return BadRequest("Irrigation system not found");
            }

            var schedueleMapped = _mapper.Map<Schedule>(scheduleForCreation);

            _unitOfWork.ScheduleRepository.Create(schedueleMapped);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return CreatedAtRoute("Schedule",schedueleMapped);
        }

        [HttpPut("schedule")]
        public IActionResult UpdateSchedule(int systemId, [FromBody] ScheduleForUpdateDto scheduleForUpdate)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return BadRequest("Irrigation system not found");
            }

            var schedule = _unitOfWork.ScheduleRepository.GetSchedule(systemId);

            if (schedule != null)
            {
                schedule.TemperatureMin = scheduleForUpdate.TemperatureMin;
                schedule.TemperatureMax = scheduleForUpdate.TemperatureMax;
                schedule.Start = scheduleForUpdate.Start;
                schedule.Stop = scheduleForUpdate.Stop;
                _unitOfWork.ScheduleRepository.Update(schedule);
            }
            else
            {
                var scheduleForCreation = _mapper.Map<Schedule>(scheduleForUpdate);
                _unitOfWork.ScheduleRepository.Create(scheduleForCreation);
            }

            
            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            if (schedule != null)
                return NoContent();
            return StatusCode(201);
        }
    }
}
