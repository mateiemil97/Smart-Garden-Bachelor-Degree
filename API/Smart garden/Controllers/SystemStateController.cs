using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    }
}
