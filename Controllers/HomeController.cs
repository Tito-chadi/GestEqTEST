using Microsoft.AspNetCore.Mvc;
using GestionnaireFootball.Filters;
using GestionnaireFootball.Models;

namespace GestionnaireFootball.Controllers
{
    [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR, Role.JOUEUR)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View();
        }

        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public IActionResult Dashboard()
        {
            return View();
        }

        [AuthorizeRole(Role.JOUEUR)]
        public IActionResult MonEspace()
        {
            return View();
        }
    }
}