using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model.User;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;

namespace SourceName.Api.Controllers
{
    [Authorize(Roles = "Administrator,User")]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IUserCapabilitiesService _userCapabilitiesService;
        private readonly IUserContextService _userContextService;
        private readonly IUserService _userService;

        public UsersController(IMapper mapper, IUserAuthenticationService userAuthenticationService, IUserCapabilitiesService userCapabilitiesService, IUserContextService userContextService, IUserService userService)
        {
            _mapper = mapper;
            _userAuthenticationService = userAuthenticationService;
            _userCapabilitiesService = userCapabilitiesService;
            _userContextService = userContextService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateUserRequest request)
        {
            var token = _userAuthenticationService.Authenticate(request.Username, request.Password);
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized();
            }

            var user = _userService.GetByUsername(request.Username);
            return Ok(new AuthenticateUserResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] CreateUserRequest request)
        {
            var newUser = _userService.CreateUser(_mapper.Map<User>(request));
            return CreatedAtAction(nameof(Authenticate), _mapper.Map<UserResource>(newUser));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] Guid id)
        {
            _userService.DeleteUser(id);
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<List<UserResource>>(_userService.GetAll()));
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var user = _userService.GetById(id);
            return Ok(_mapper.Map<UserResource>(user));
        }

        [Authorize]
        [HttpGet("capabilities")]
        public IActionResult GetUserCapabilities()
        {
            var capabilities = _userCapabilitiesService.GetUserCapabilities(_userContextService.UserId.Value);
            return Ok(_mapper.Map<UserCapabilitiesResource>(capabilities));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            user.Id = id;
            return Ok(_mapper.Map<UserResource>(_userService.UpdateUser(user)));
        }
    }
}