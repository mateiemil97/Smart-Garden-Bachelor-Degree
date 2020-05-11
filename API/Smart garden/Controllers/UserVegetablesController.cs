using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models.UserVegetablesDto;
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
    }
}
