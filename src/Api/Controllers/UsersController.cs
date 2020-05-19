using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model.User;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;

namespace SourceName.Api.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserPasswordValidationService _userPasswordValidationService;
        private readonly IUserValidationService _userValidationService;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IUserCapabilitiesService _userCapabilitiesService;
        private readonly IUserContextService _userContextService;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMapper mapper, 
                               IUserPasswordValidationService userPasswordValidationService,
                               IUserValidationService userValidationService,
                               IUserAuthenticationService userAuthenticationService,
                               IUserCapabilitiesService userCapabilitiesService,
                               IUserContextService userContextService,
                               IUserService userService,
                               ILogger<UsersController> logger)
        {
            _mapper = mapper;
            _userPasswordValidationService = userPasswordValidationService;
            _userValidationService = userValidationService;
            _userAuthenticationService = userAuthenticationService;
            _userCapabilitiesService = userCapabilitiesService;
            _userContextService = userContextService;
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateUserRequest request)
        {
            var token = _userAuthenticationService.Authenticate(request.Username, request.Password);

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation($"Invalid user login. Username: {request.Username}");
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
            var user = _mapper.Map<User>(request);
            var validationObject = _userValidationService.ValidateUser(user);

            if (!validationObject.IsValid)
            {
                return Ok(validationObject);
            }

            var newUser = _userService.CreateUser(_mapper.Map<User>(request));
            _logger.LogInformation($"New User being created: {newUser.Id}");

            var response = new CreateUserResponse
            {
                IsUserCreated = true,
                UserResource = _mapper.Map<UserResource>(newUser)
            };

            return CreatedAtAction(nameof(Authenticate), response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            if (_userService.GetById(id) == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"User {id} is being deleted.");
            _userService.DeleteUser(id);
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<List<UserResource>>(_userService.GetAll()));
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserResource>(user));
        }

        [HttpGet("capabilities")]
        public IActionResult GetUserCapabilities()
        {
            var capabilities = _userCapabilitiesService.GetUserCapabilities(_userContextService.UserId.Value);
            return Ok(_mapper.Map<UserCapabilitiesResource>(capabilities));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            if (_userService.GetById(id) == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"User is being updated: {id}");
            var user = _mapper.Map<User>(request);
            user.Id = id;
            return Ok(_mapper.Map<UserResource>(_userService.UpdateUser(user)));
        }

        [HttpPatch("{id}/password")]
        public IActionResult UpdatePassword([FromRoute] int id, [FromBody] UpdatePasswordRequest request)
        {
            var validationObject = _userPasswordValidationService.Validate(request.Password);

            if (!validationObject.IsValid)
            {
                return BadRequest(validationObject);
            }

            if (_userService.GetById(id) == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"User {id} is updating their password.");
            return Ok(_mapper.Map<UserResource>(
                _userService.UpdateUserPassword(id, request.Password)));
        }
    }
}