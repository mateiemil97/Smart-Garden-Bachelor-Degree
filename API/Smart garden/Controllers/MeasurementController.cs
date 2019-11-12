using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Models;
using Smart_garden.Models.SensorDto;
using Smart_garden.Repository.MeasurementRepository;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/systems/measurements")]
    public class MeasurementController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public MeasurementController( IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetAllMeasurements()
        {
            var measurementsFromRepo = _unitOfWork.MeasurementRepository.GetAll();

            return Ok(measurementsFromRepo);
            
        }
    }
}
