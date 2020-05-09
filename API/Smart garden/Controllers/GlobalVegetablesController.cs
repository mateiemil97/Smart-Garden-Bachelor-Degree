using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/globalVegetables")]
    public class GlobalVegetablesController: Controller
    {
        private IUnitOfWork _unitOfWork;

        public GlobalVegetablesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet()]
        public IActionResult GetAllGlobalVegetables()
        {
            try
            {
                var vegetables = _unitOfWork.GlobalVegetablesRepository.GetAll();
                if (vegetables == null)
                    return NotFound();

                return Ok(vegetables);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

    }
}
