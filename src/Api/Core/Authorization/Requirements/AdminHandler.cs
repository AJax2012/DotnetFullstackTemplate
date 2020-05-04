using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using SourceName.Api.Model.Roles;

namespace SourceName.Api.Core.Authorization.Requirements
{
    public class AdminHandler : AuthorizationHandler<AdminRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var rolesClaim = context.User.Claims
                .SingleOrDefault(c => c.Type == SourceNameClaimTypes.Roles);

            if (rolesClaim?.Value?
                .Split(",")?
                .Select(int.Parse)?
                .Contains((int)Roles.Administrator) != true)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class AdminRequirement : IAuthorizationRequirement { }
}