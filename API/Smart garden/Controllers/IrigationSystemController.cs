using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Models.SystemDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems")]
    public class IrigationSystemController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public IrigationSystemController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult GetSystemById(int id)
        {

            var systemRepo = _unitOfWork.IrigationSystemRepository.Get(id);
            if (systemRepo == null)
            {
                return NotFound();
            }
            var systemMapped = _mapper.Map<IEnumerable<IrigationSystemDto>>(systemRepo);

            return Ok(systemMapped);
        }

       
        [HttpGet]
        public IActionResult GetAllSystems()
        {
            var systemsRepo = _unitOfWork.IrigationSystemRepository.GetAll();

            var systemsMapped = _mapper.Map<IEnumerable<IrigationSystemDto>>(systemsRepo);

            return Ok(systemsMapped);
        }



        [HttpGet("users/{userid}")]
         public IActionResult GetSystemByUser(int userid)
         {
             var user = _unitOfWork.IrigationSystemRepository.ExistUser(userid);

            if (user == false)
            {
                return BadRequest();
            }

            var systemsByUser = _unitOfWork.IrigationSystemRepository.GetSystemsByUser(userid);

            if (systemsByUser == null)
            {
                return NotFound();
            }

            return Ok(systemsByUser);
        }


        [HttpPost("system/users", Name = "system")]
         public IActionResult CreateSystem([FromBody] IrigationSystemForCreationDto system)
         {
             var user = _unitOfWork.IrigationSystemRepository.ExistUser(system.UserId);
             
             if (user == false)
             {
                 return BadRequest();
             }

             var existSeries = _unitOfWork.IrigationSystemRepository.ExistSeries(system.SeriesKey);

             if (existSeries)
             {
                 return BadRequest("Already registered an irigation system with this series key");
             }

             var irigationSystemForCreation = _mapper.Map<Entites.IrigationSystem>(system);


             _unitOfWork.IrigationSystemRepository.Create(irigationSystemForCreation);

                if (!_unitOfWork.Save())
                {
                    return StatusCode(500, "A problem happened with handling your request. Try again!");
                }

                return CreatedAtRoute("system", irigationSystemForCreation);
         }
    }
}
