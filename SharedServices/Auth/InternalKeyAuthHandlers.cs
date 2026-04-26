using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SharedServices.Auth
{

    public class InternalOrJwtRequirement : IAuthorizationRequirement 
    {
        public static string Policy => "InternalOrJwt";
    }
    public class InternalOnlyRequirement : IAuthorizationRequirement 
    {
        public static string Policy => "InternalOnly";
    }

    // Succeeds when the request carries a valid X-Internal-Key header OR a valid JWT.
    public class InternalOrJwtHandler : AuthorizationHandler<InternalOrJwtRequirement>
    {
        private readonly InternalServiceSettings _settings;

        public InternalOrJwtHandler(InternalServiceSettings settings)
        {
            _settings = settings;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, InternalOrJwtRequirement requirement)
        {
            if (context.Resource is HttpContext http)
            {
                var key = http.Request.Headers["X-Internal-Key"].FirstOrDefault();
                if (!string.IsNullOrEmpty(key) && key == _settings.ApiKey)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            if (context.User.Identity?.IsAuthenticated == true)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    // Succeeds only when the request carries a valid X-Internal-Key header.
    public class InternalOnlyHandler : AuthorizationHandler<InternalOnlyRequirement>
    {
        private readonly InternalServiceSettings _settings;

        public InternalOnlyHandler(InternalServiceSettings settings)
            => _settings = settings;

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, InternalOnlyRequirement requirement)
        {
            if (context.Resource is HttpContext http)
            {
                var key = http.Request.Headers["X-Internal-Key"].FirstOrDefault();
                if (!string.IsNullOrEmpty(key) && key == _settings.ApiKey)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
