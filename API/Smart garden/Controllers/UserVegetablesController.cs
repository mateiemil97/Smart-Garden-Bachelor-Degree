using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models.UserVegetablesDto;
using Smart_garden.Models.ZoneDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/")]
    public class UserVegetablesController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserVegetablesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{userId}/vegetables")]
        public IActionResult GetUserVegetables(int userId)
        {
            try
            {
                var user = _unitOfWork.UserGenericRepository.Exist(userId);
                if (user == null)
                    return NotFound("User not found");

                var vegetables = _unitOfWork.UserVegetablesRepository.GetUserVegetables(userId);
                if (vegetables == null)
                    return NotFound("Vegetables not found");
                var vegetablesToReturn = _mapper.Map<IEnumerable<UserVegetablesForGetDto>>(vegetables);

                return Ok(vegetablesToReturn);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Server error during getting the vegetables");
            }
        }

        [HttpGet("{userId}/vegetables/{id}")]
        public IActionResult GetUserVegetable(int userId, int id)
        {
            try
            {
                var user = _unitOfWork.UserGenericRepository.Exist(userId);
                if (user == null)
                    return NotFound("User not found");

                var vegetables = _unitOfWork.UserVegetablesRepository.GetUserVegetable(userId,id);
                if (vegetables == null)
                    return NotFound("Vegetables not found");
                var vegetablesToReturn = _mapper.Map<IEnumerable<UserVegetablesForGetDto>>(vegetables);

                return Ok(vegetablesToReturn);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Server error during getting the vegetables");
            }
        }

        [HttpPost("{userId}/vegetables",Name = "vegetable")]
        public IActionResult AddVegetables(int userId, [FromBody] UserVegetablesForCreationDto vegetable)
        {
            try
            {
                var user = _unitOfWork.UserGenericRepository.Exist(userId);
                if (user == null)
                    return NotFound("User not found");

                var vegetablesToAdd = _mapper.Map<UserVegetables>(vegetable);
                _unitOfWork.UserVegetablesRepository.Create(vegetablesToAdd);
                _unitOfWork.Save();
                return Created("vegetable", vegetablesToAdd);
            }
            catch 
            {
                return StatusCode(500, "Server error during creating vegetable");
            }
        }

        [HttpDelete("vegetables/{id}")]
        public IActionResult DeleteVegetable(int id)
        {
            try
            {
                var vegetable = _unitOfWork.UserVegetablesRepository.Exist(id);

                if (vegetable == null)
                    return NotFound();

                _unitOfWork.UserVegetablesRepository.Delete(vegetable);
                _unitOfWork.Save();

                return NoContent();

            }
            catch
            {
                return StatusCode(500, "Server error on deleting vegetable");
            }
            
        }
        [HttpPut("{userId}/vegetables/{vegetableId}")]
        public IActionResult UpdateVegetableWithZones(int vegetableId,int userId, [FromBody]ZoneForUpdateDto zone)
        {
            try
            {
                var vegetable = _unitOfWork.UserVegetablesRepository.GetUserVegetable(userId, vegetableId).FirstOrDefault();

                int initialStartMoisture = vegetable.StartMoisture;
                int initialStopMoisture = vegetable.StopMoisture;

                if (vegetable == null)
                    return NotFound();

                vegetable.StartMoisture = zone.MoistureStart;
                vegetable.StopMoisture = zone.MoistureStop;

                _unitOfWork.UserVegetablesRepository.Update(vegetable);

                var zones = _unitOfWork.ZoneRepository.GetZonesBySystem(zone.SystemId);

                foreach (var z in zones)
                {
                    if (z.MoistureStart == initialStartMoisture && z.MoistureStop == initialStopMoisture)
                    {
                        z.MoistureStart = zone.MoistureStart;
                        z.MoistureStop = zone.MoistureStop;
                        var zoneToUpdate = _mapper.Map<Zone>(z);
                        _unitOfWork.ZoneRepository.Update(zoneToUpdate);
                    }
                }

                _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Error on updating");
            }
            
        }
    }
}
