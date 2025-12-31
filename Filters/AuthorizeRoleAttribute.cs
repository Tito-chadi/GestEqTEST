using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestionnaireFootball.Models;

namespace GestionnaireFootball.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Role[] _allowedRoles;

        public AuthorizeRoleAttribute(params Role[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Récupérer le rôle depuis la session
            var roleSession = context.HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(roleSession))
            {
                // Non connecté, rediriger vers login
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Parser le rôle
            if (!Enum.TryParse<Role>(roleSession, out var userRole))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Vérifier si le rôle est autorisé
            if (!_allowedRoles.Contains(userRole))
            {
                // Accès interdit
                context.Result = new ViewResult
                {
                    ViewName = "AccessDenied"
                };
            }
        }
    }
}