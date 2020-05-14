using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models.FCMTokenDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    //FCM = Firebase Cloud Messages

    [Route("api/systems")]
    public class FCMTokenController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public FCMTokenController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{systemId}/fcmtoken")]
        public IActionResult GetToken(int systemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return NotFound("Irrigation system not found");
            }

            var token = _unitOfWork.FCMTokenRepository.GetTokenByIrrigationSystem(systemId);

            if (token == null)
            {
                return NotFound("Token not found");
            }

            return Ok(token);
        }

        [HttpPost("{irrigationSystemId}/fcmtoken", Name="token")]
        public IActionResult SaveFCMToken(int irrigationSystemId, [FromBody] FCMTokenForCreateDto token)
        {
            var tokenMapped = _mapper.Map<FCMToken>(token);

            _unitOfWork.FCMTokenRepository.Create(tokenMapped);

            if (!_unitOfWork.Save())
                return StatusCode(500);

            return Created("token", tokenMapped);

        }

        [HttpDelete("{irrigationSystemId}/fcmtoken")]
        public IActionResult DeleteFCMToken(int irrigationSystemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(irrigationSystemId);

            if (!system)
            {
                return NotFound("Irrigation system not found");
            }

            _unitOfWork.FCMTokenRepository.DeleteTokenByIrrigationSystemId(irrigationSystemId);

            if (!_unitOfWork.Save())
                return StatusCode(500);

            return NoContent();

        }
    }
}
