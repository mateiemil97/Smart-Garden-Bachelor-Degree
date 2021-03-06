﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Smart_garden.Entites;
using Smart_garden.Models.SensorDto;
using Smart_garden.Repository;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems")]
    public class SensorsController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public SensorsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{systemid}/sensors")]
        public IActionResult GetSensorsBySystem(int systemid)
        {
            var systemRepo = _unitOfWork.IrigationSystemRepository.Exist(systemid);

            if (systemRepo== null)
            {
                return NotFound("Irigation system not found");
            }

            var sensorsFromRepo = _unitOfWork.SensorRepository.GetSensorsBySystem(systemid);

            return Ok(sensorsFromRepo);
        }

        [HttpGet("{systemid}/sensors/{sensorid}")]
        public IActionResult GetSensorBySystem(int systemid, int sensorid)
        {
            var systemRepo = _unitOfWork.IrigationSystemRepository.Exist(systemid);

            if (systemRepo == null)
            {
                return NotFound("Irigation system not found");
            }

            var sensorFromRepo = _unitOfWork.SensorRepository.GetSensorBySystem(systemid, sensorid);

            if (sensorFromRepo == null)
            {
                return NotFound("Sensor not found");
            }

            return Ok(sensorFromRepo);
        }

        [HttpGet("{systemId}/sensors/statistics")]
        public IActionResult GetSensorValueByType(int systemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
            if (!system)
            {
                return BadRequest("Irigation system not found");
            }

            var sensors = _unitOfWork.SensorRepository.GetInfoForStatistics(systemId);

            return Ok(sensors);
        }

        [HttpPost("{systemid}/sensors", Name = "sensor")]

        public IActionResult CreateSensor([FromBody] SensorForCreationDto sensor, int systemid)
        {
            var systemRepo = _unitOfWork.IrigationSystemRepository.Exist(systemid);

            if (systemRepo == null)
            {
                return NotFound("Irigation system not found");
            }


            var sensorMapped = _mapper.Map<Sensor>(sensor);

           _unitOfWork.SensorRepository.Create(sensorMapped);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return CreatedAtRoute(sensor, sensorMapped);
        }

        [HttpDelete("{systemid}/sensors/{sensorid}")]
        public IActionResult DeleteSensor(int systemid,int sensorid)
        {

            var systemRepo = _unitOfWork.IrigationSystemRepository.Exist(systemid);

            if (systemRepo == null)
            {
                return NotFound("Irigation system not found");
            }

            var sensorFromRepo = _unitOfWork.SensorRepository.Get(sensorid);

            if (sensorFromRepo == null)
            {
                return NotFound();
            }

//            var sensorMapped = _mapper.Map<Sensor>(sensorFromRepo);

            _unitOfWork.SensorRepository.Delete(sensorFromRepo);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return NoContent();
        }
    }
}
