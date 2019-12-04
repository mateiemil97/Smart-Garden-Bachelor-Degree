using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Models.SensorPortDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems/{systemId}/ports")]
    public class SensorPort: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public SensorPort(IUnitOfWork unitOfWork, IMapper mapper)
        {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
        }

        [HttpGet("availables")]
        public IActionResult GetUnUsedPorts(int systemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
        
            if (!system)
            {
                return NotFound("Irrigation system not found!");
            }

            var allPorts = _unitOfWork.SensorPortRepository.GetAllPorts();
            var usedPorts = _unitOfWork.SensorPortRepository.GetUsedPorts(systemId);
           
            var unUsedPorts = allPorts.Except(usedPorts);

            var portsMapped = _mapper.Map<IEnumerable<SensorPortDto>>(unUsedPorts);

            return Ok(portsMapped);
        }
//        [HttpGet()]
//        public IActionResult GetUsedPorts(int systemId)
//        {
//            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
//
//            if (!system)
//            {
//                return NotFound("Irrigation system not found!");
//            }
//
//            var portsFromRepo = _unitOfWork.SensorPortRepository.GetAll();
//            var portsMapped = _mapper.Map<IEnumerable<SensorPortDto>>(portsFromRepo);
//
//            return Ok(portsMapped);
//        }
    }
}
