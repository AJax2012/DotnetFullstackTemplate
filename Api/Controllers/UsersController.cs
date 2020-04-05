using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SourceName.Api.Controllers
{
    [Authorize(Roles = "Administrator,User")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
    }
}