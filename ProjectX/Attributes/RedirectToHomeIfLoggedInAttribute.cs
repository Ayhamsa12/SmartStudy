using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ProjectX.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RedirectToHomeIfLoggedInAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // User is authenticated, redirect to home or another page
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}