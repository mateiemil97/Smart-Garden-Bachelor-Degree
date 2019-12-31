using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Models.CompositesObjects;
using Smart_garden.Models.SystemStateDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems/{systemid}")]
    public class SystemStateController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public SystemStateController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("systemStates")]
        public IActionResult GetAllSystemState(int systemid)
        {
            var isSystem = _unitOfWork.IrigationSystemRepository.Exist(systemid);

            if (isSystem == null)
            {
                return BadRequest("System doesn't exist!");
            }

            var systemStateFromRepo = _unitOfWork.SystemStateRepository.GetSystemStatesBySystem(systemid);

            return Ok(systemStateFromRepo);
        }

        [HttpGet("currentState")]
        public IActionResult GetCurrentState(int systemId)
        {
            var isSystem = _unitOfWork.IrigationSystemRepository.Exist(systemId);

            if (isSystem == null)
            {
                return BadRequest("System doesn't exist!");
            }
            
            var currentStateFromRepo = _unitOfWork.SystemStateRepository.GetCurrentState(systemId);

            var currentStateMapped = _mapper.Map<SystemStateDto>(currentStateFromRepo);

            return Ok(currentStateMapped);
        }

        [HttpPut("systemState")]
        public IActionResult UpdateSystemState(int systemId, [FromBody] ScheduleSystemStateForUpdate schStateForUpdate)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
            if (!system)
            {
                return NotFound("Irrigation system not found");
            }

            var schedule = _unitOfWork.ScheduleRepository.GetSchedule(systemId);
            if (schedule == null)
            {
                return NotFound("Schedule not found");
            }

            schedule.Manual = schStateForUpdate.Manual;
            _unitOfWork.ScheduleRepository.Update(schedule);

            var systemState = _unitOfWork.SystemStateRepository.GetCurrentState(systemId);
            if (systemState == null)
            {
                return NotFound("System state not found");
            }

            systemState.Working = schStateForUpdate.Manual;
            _unitOfWork.SystemStateRepository.Update(systemState);

            if (!_unitOfWork.Save())
            { 
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return NoContent();

        }

    }
}
