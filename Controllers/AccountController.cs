using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Models;
using GestionnaireFootball.Helpers;

namespace GestionnaireFootball.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si déjà connecté, rediriger vers la page appropriée
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Chercher l'utilisateur par email
                Utilisateur? utilisateur = (Utilisateur?)await _context.Joueurs
            .FirstOrDefaultAsync(u => u.Email == model.Email)
            ?? (Utilisateur?)await _context.Personnels
            .FirstOrDefaultAsync(u => u.Email == model.Email);
                if (utilisateur != null && PasswordHasher.VerifyPassword(model.MotDePasse, utilisateur.MotDePasseHash))
                {
                    // Stocker les informations de session
                    HttpContext.Session.SetString("UserId", utilisateur.Id.ToString());
                    HttpContext.Session.SetString("UserEmail", utilisateur.Email);
                    HttpContext.Session.SetString("UserRole", utilisateur.Role.ToString());
                    HttpContext.Session.SetString("UserName", utilisateur.NomComplet);

                    // Redirection selon le rôle
                    return utilisateur.Role switch
                    {
                        Role.ADMIN => RedirectToAction("Index", "Admin"),
                        Role.ENTRAINEUR => RedirectToAction("Index", "Entraineur"),
                        Role.JOUEUR => RedirectToAction("Index", "Joueur"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Méthode pour initialiser un administrateur (à exécuter une fois)
        [HttpGet]
        public async Task<IActionResult> InitializeAdmin()
        {
            // Vérifier si un admin existe déjà
            var adminExists = await _context.Personnels
                .AnyAsync(p => p.Email == "admin@club.com");

            if (!adminExists)
            {
                var admin = new Personnel
                {
                    Email = "admin@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Admin123!"),
                    Role = Role.ADMIN,
                    Nom = "Admin",
                    Prenom = "System",
                    Fonction = Fonction.EntraineurPrincipal,
                    DateEmbauche = DateTime.Now
                };

                _context.Personnels.Add(admin);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Administrateur créé avec succès ! Email: admin@club.com, Mot de passe: Admin123!";
            }
            else
            {
                TempData["InfoMessage"] = "Un administrateur existe déjà.";
            }

            return RedirectToAction("Login");
        }
    }
}