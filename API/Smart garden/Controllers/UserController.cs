using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
       // private readonly ApplicationSettings _appSettings;


        public UserController(
            IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager
            //ApplicationSettings appSettings
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
          //  _appSettings = appSettings;
        }

       
        [HttpGet()]
        [Authorize]
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
                UserName = user.LastName + user.FirstName
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

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody]UserForLogin userLogin)
        {
            var user = await _userManager.FindByEmailAsync(userLogin.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("userId", user.Id.ToString())
                    }),
                    SigningCredentials =
                        new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456")),
                            SecurityAlgorithms.HmacSha256Signature),

                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new {token});
            }
            else
            {
                return BadRequest(new {message = "Email or password is incorrect!"});
            }
        }
    }
}
