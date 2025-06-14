using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace back.Config
{
    public class AuthorizeAdminEmailAttribute : Attribute, IAuthorizationFilter
    {
        private const string AdminEmail = "admin@admin.com";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userEmail = context.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail == null || !userEmail.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult(); // Ou UnauthorizedResult si tu préfères
            }
        }
    }

}
