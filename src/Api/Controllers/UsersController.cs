using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model;
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
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateUserRequest request)
        {
            var token = await _userAuthenticationService.AuthenticateAsync(request.Username, request.Password);

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation($"Invalid user login. Username: {request.Username}");
                return Unauthorized();
            }

            var user = await _userService.GetByUsernameAsync(request.Username);

            return Ok(new AuthenticateUserResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var validationObject = await _userValidationService.ValidateUserAsync(user);

            if (!validationObject.IsValid)
            {
                return Ok(validationObject);
            }

            var newUser = await _userService.CreateUserAsync(_mapper.Map<User>(request));
            _logger.LogInformation($"New User being created: {newUser.Id}");

            var response = new CreateUserResponse
            {
                IsUserCreated = true,
                UserResource = _mapper.Map<UserResource>(newUser)
            };

            return CreatedAtAction(nameof(AuthenticateAsync), response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (await _userService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"User {id} is being deleted.");
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 0, int resultsPerPage = 10, bool removeInactive = true)
        {
            var results = _mapper.Map <SearchResultResource<UserResource>>(await _userService.GetAllPaginatedAsync(pageNumber, resultsPerPage, removeInactive));
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserResource>(user));
        }

        [HttpGet("capabilities")]
        public async Task<IActionResult> GetUserCapabilities()
        {
            var capabilities = await _userCapabilitiesService.GetUserCapabilitiesAsync(_userContextService.UserId.Value);
            return Ok(_mapper.Map<UserCapabilitiesResource>(capabilities));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            if (await _userService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"User is being updated: {id}");
            var user = _mapper.Map<User>(request);
            user.Id = id;
            return Ok(_mapper.Map<UserResource>(await _userService.UpdateUserAsync(user)));
        }

        [HttpPatch("{id}/roles")]
        public async Task<IActionResult> UpdateUserRoles([FromRoute] int id, [FromBody] List<int> roleIds)
        {
            return Ok(_mapper.Map<UserResource>(await _userService.UpdateUserRolesAsync(id, roleIds)));
        }

        [HttpPatch("{id}/password")]
        public async Task<IActionResult> UpdatePassword([FromRoute] int id, [FromBody] UpdatePasswordRequest request)
        {
            var validationObject = _userPasswordValidationService.Validate(request.Password);

            if (!validationObject.IsValid)
            {
                return BadRequest(validationObject);
            }

            if (await _userService.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"User {id} is updating their password.");
            return Ok(_mapper.Map<UserResource>(
                await _userService.UpdateUserPasswordAsync(id, request.Password)));
        }
    }
}