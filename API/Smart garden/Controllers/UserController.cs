﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models.UserDto;
using Smart_garden.Repository;
using Smart_garden.UnitOfWork;

namespace Smart_garden.Controllers
{
    [Route("api/users")]
    public class UserController: Controller
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private UserManager<User> _userManager;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            var users = _unitOfWork.UserGenericRepository.GetAll();

            var usersMapped = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(usersMapped);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var userFromRepo = _unitOfWork.UserGenericRepository.Get(id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            var mappedUser = _mapper.Map<UserDto>(userFromRepo);

            return Ok(mappedUser);
        }

        [HttpPost("register", Name = "register")]
        public async Task<IActionResult> Create([FromBody] UserForCreationDto user)
        {
            
            var userForRegister = new User()
            {
                LastName = user.LastName,
                FirstName =  user.FirstName,
                Email = user.Email,
                Country = user.Country,
                City = user.City,
                UserName = user.FirstName +  user.LastName
            };

            try
            {
                var result = await _userManager.CreateAsync(userForRegister, user.Password);
                return Created("register", result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
