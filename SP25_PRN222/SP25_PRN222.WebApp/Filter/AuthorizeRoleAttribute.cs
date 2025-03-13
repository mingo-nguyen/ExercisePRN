using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SP25_PRN222.WebApp.Filter
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public AuthorizeRoleAttribute(string role)
        {
            _role = role;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("UserRole");
            var sessionId = context.HttpContext.Session.GetString("sessionId");
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(userRole) || userRole!= _role)
            {
                context.Result = new RedirectToActionResult("Login","Auth", null);

                return;
            }
            base.OnActionExecuting(context);

        }

    }
}
