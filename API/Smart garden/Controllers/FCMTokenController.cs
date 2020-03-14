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

    [Route("api/systems/{systemId}")]
    public class FCMTokenController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public FCMTokenController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("fcmtoken")]
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

        [HttpPost(Name="token")]
        public IActionResult SaveFCMToken(int irrigationSystem, [FromBody] FCMTokenForCreateDto token)
        {
            var tokenMapped = _mapper.Map<FCMToken>(token);

            _unitOfWork.FCMTokenRepository.Create(tokenMapped);

            if (!_unitOfWork.Save())
                return StatusCode(500);

            return Created("token", tokenMapped);

        }
    }
}
