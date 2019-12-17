using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models.MeasurementDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems/{systemId}/measurements")]
    public class MeasurementController: Controller
    {

        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MeasurementController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{sensorId}")]
        public IActionResult GetSensorValueByType(int systemId, int sensorId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
            if (!system)
            {
                return BadRequest("Irigation system not found");
            }
            var measurement = _unitOfWork.MeasurementRepository.GetLatestMeasurementValueByType(systemId, sensorId);
            
            var mappedMeasurement = _mapper.Map<MeasurementDto>(measurement);

            return Ok(mappedMeasurement);
        }

        [HttpGet("temperature")]
        public IActionResult GetTemperatureValueBySystem(int systemId, int sensorId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
            if (!system)
            {
                return BadRequest("Irigation system not found");
            }

            var measurement = _unitOfWork.MeasurementRepository.GetLatestMeasurementOfTemperature(systemId);
            var mappedMeasurement = _mapper.Map<MeasurementDto>(measurement);

            return Ok(mappedMeasurement);
        }

        [HttpPost("{port}",Name = "measurement")]
        public IActionResult PostMeasurement(int systemId, [FromBody] MeasurementForCreationDto measurement, string port)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);
            if (!system)
            {
                return NotFound("Irrigation system not found");
            }

            var sensor = _unitOfWork.SensorRepository.GetSensorBySystemAndPortName(systemId, port);
            if (sensor == null)
            {
                return NotFound("Sensor not found");
            }

            measurement.SensorId = sensor.Id;
            measurement.DateTime = DateTime.UtcNow.ToLocalTime();


            var measurementMapped = _mapper.Map<Measurement>(measurement);
            _unitOfWork.MeasurementRepository.Create(measurementMapped);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return CreatedAtRoute("measurement", measurementMapped);

        }
    }
}
