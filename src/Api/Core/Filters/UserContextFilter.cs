using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using SourceName.Api.Core.Authorization;
using SourceName.Service.Users;

namespace SourceName.Api.Core.Filters
{
    public class UserContextFilter : IAsyncActionFilter
    {
        private readonly IUserContextService _userContextService;

        public UserContextFilter(IUserContextService userContextService)
        {
            _userContextService = userContextService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userIdClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name);
                _userContextService.SetCurrentUserId(userIdClaim?.Value);

                var claimRoleIds = claimsIdentity.Claims.SingleOrDefault(c => c.Type == SourceNameClaimTypes.Roles);
                _userContextService.SetUserRoleIds(claimRoleIds?.Value);
            }

            var resultContext = await next();
        }
    }
}