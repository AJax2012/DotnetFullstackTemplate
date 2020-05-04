using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SourceName.Api.Core.Authorization
{
    public class AuthorizeResourceAttribute : TypeFilterAttribute
    {
        public AuthorizeResourceAttribute(Type type) : base(typeof(AuthorizeResourceFilter))
        {
            Arguments = new object[] { type };
        }

        private class AuthorizeResourceFilter : IAsyncActionFilter
        {
            private readonly IAuthorizationService _authorizationService;
            private readonly Type _requirementType;

            public AuthorizeResourceFilter(IAuthorizationService authorizationService, Type requirementType)
            {
                _authorizationService = authorizationService;
                _requirementType = requirementType;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var requirement = Activator.CreateInstance(_requirementType) as IAuthorizationRequirement;
                var authorizationResult = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, requirement);

                if (!authorizationResult.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                await next();
            }
        }
    }
}