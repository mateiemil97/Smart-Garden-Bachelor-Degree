using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models.CompositesObjects;
using Smart_garden.Models.SensorDto;
using Smart_garden.Models.ZoneDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems/{systemId}/zones")]
    public class ZoneController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ZoneController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetZones(int systemId)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return NotFound("Irrigation system not found");
            }

            var zonesFromRepo = _unitOfWork.ZoneRepository.GetZonesBySystem(systemId);

            var zoneMapped = _mapper.Map<IEnumerable<ZoneDto>>(zonesFromRepo);

            return Ok(zoneMapped);
        }

        [HttpGet("{id}")]
        public IActionResult GetZone(int systemId, int id)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return NotFound("Irrigation system not found!");
            }

            var zoneFromRepo = _unitOfWork.ZoneRepository.GetZoneBySystem(systemId, id);

            if (zoneFromRepo == null)
            {
                return NotFound("Zone not found!");
            }

            var zoneMapped = _mapper.Map<ZoneDto>(zoneFromRepo);

            return Ok(zoneMapped);
        }

        [HttpPost(Name="zone")]
        public IActionResult CreateZone(int systemId, [FromBody] ZoneSensorComposite zoneSensor)
        {
            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return NotFound("Irrigation system not found!");
            }


            var sensorForCreation = _mapper.Map<SensorForCreationDto>(zoneSensor);
            sensorForCreation.SystemId = systemId;
            var sensorMapped = _mapper.Map<Sensor>(sensorForCreation);

            _unitOfWork.SensorRepository.Create(sensorMapped);



            var zoneForCreation = _mapper.Map<ZoneForCreationDto>(zoneSensor);

            //Getting id of sensor
            zoneForCreation.SensorId = sensorMapped.Id;

            var zoneMapped = _mapper.Map<Zone>(zoneForCreation);

            _unitOfWork.ZoneRepository.Create(zoneMapped);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return CreatedAtRoute("zone", zoneMapped);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteZone(int systemId, int id)
        {

            var system = _unitOfWork.IrigationSystemRepository.ExistIrigationSystem(systemId);

            if (!system)
            {
                return NotFound("Irrigation system not found!");
            }

            var zone = _unitOfWork.ZoneRepository.GetZoneBySystem(systemId,id);
            if (zone == null)
            {
                return NotFound("Zone not found!");
            }

            _unitOfWork.ZoneRepository.Delete(zone);

            var sensor = _unitOfWork.SensorRepository.GetSensorById(zone.SensorId);
            _unitOfWork.SensorRepository.Delete(sensor);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");
            }

            return NoContent();
        }
    }
}
