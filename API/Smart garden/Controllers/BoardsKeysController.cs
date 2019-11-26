﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Models.BoardKeyDto;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/boardsseries")]
    public class BoardsKeysController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BoardsKeysController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetBoardsSeries()
        {
            var seriesFromRepo = _unitOfWork.BoardsKeyRepository.GetAll();

            var seriesMapped = _mapper.Map<IEnumerable<BoardKeyDto>>(seriesFromRepo);

            return Ok(seriesMapped);
        }

        [HttpGet("{id}")]
        public IActionResult GetBoardSerie(int id)
        {
            var serieFromRepo = _unitOfWork.BoardsKeyRepository.Get(id);

            if (serieFromRepo == null)
            {
                return NotFound();
            }

            var serieMapped = _mapper.Map<BoardKeyDto>(serieFromRepo);

            return Ok(serieMapped);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBoardSeriesState(int id,[FromBody] BoardKeyForUpdateDto board)
        {
            var boardFromRepo = _unitOfWork.BoardsKeyRepository.Get(id);

            if (boardFromRepo == null)
            {
                return BadRequest();
            }

            boardFromRepo.Registered = board.Registered;
            
            _unitOfWork.BoardsKeyRepository.Update(boardFromRepo);

            if (!_unitOfWork.Save())
            {
                return StatusCode(500, "A problem happened with handling your request. Try again!");

            }

            return NoContent();
        }
        
    }
}
