using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Models;
using GestionnaireFootball.Filters;

namespace GestionnaireFootball.Controllers
{
    [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
    public class JoueursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JoueursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Joueurs
        public async Task<IActionResult> Index()
        {
            var joueurs = await _context.Joueurs
                .OrderBy(j => j.Nom)
                .ThenBy(j => j.Prenom)
                .ToListAsync();

            // Calculer les stats pour chaque joueur
            foreach (var joueur in joueurs)
            {
                joueur.Presences = await _context.Presences
                    .Where(p => p.JoueurId == joueur.Id)
                    .ToListAsync();
            }

            return View(joueurs);
        }

        // GET: Joueurs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var joueur = await _context.Joueurs
                .Include(j => j.Presences)
                    .ThenInclude(p => p.Match)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (joueur == null) return NotFound();

            // Statistiques avancées
            ViewBag.TotalMatchs = joueur.Presences?.Count ?? 0;
            ViewBag.MatchsJoues = joueur.Presences?.Count(p => p.Statut == StatutPresence.Present) ?? 0;
            ViewBag.TauxPresence = joueur.TauxPresence;

            return View(joueur);
        }

        // GET: Joueurs/Creer
        public IActionResult Creer()
        {
            return View();
        }

        // POST: Joueurs/Creer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer(Joueur joueur)
        {
            if (ModelState.IsValid)
            {
                // Hasher le mot de passe
                joueur.MotDePasseHash = Helpers.PasswordHasher.HashPassword("Joueur123!"); // Mot de passe par défaut
                joueur.Role = Role.JOUEUR;

                _context.Add(joueur);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Joueur créé avec succès !";
                return RedirectToAction(nameof(Index));
            }
            return View(joueur);
        }

        // GET: Joueurs/Editer/5
        public async Task<IActionResult> Editer(int id)
        {
            var joueur = await _context.Joueurs.FindAsync(id);
            if (joueur == null) return NotFound();

            return View(joueur);
        }

        // POST: Joueurs/Editer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editer(int id, Joueur joueur)
        {
            if (id != joueur.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Ne pas modifier le mot de passe ici
                    var existing = await _context.Joueurs.FindAsync(id);
                    if (existing != null)
                    {
                        existing.Nom = joueur.Nom;
                        existing.Prenom = joueur.Prenom;
                        existing.Email = joueur.Email;
                        existing.Poste = joueur.Poste;
                        existing.NumeroMaillot = joueur.NumeroMaillot;
                        existing.NoteGlobale = joueur.NoteGlobale;
                        existing.DateNaissance = joueur.DateNaissance;
                        existing.Telephone = joueur.Telephone;

                        _context.Update(existing);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JoueurExists(id))
                        return NotFound();
                    else
                        throw;
                }

                TempData["SuccessMessage"] = "Joueur mis à jour avec succès !";
                return RedirectToAction(nameof(Index));
            }
            return View(joueur);
        }

        // GET: Joueurs/Supprimer/5
        public async Task<IActionResult> Supprimer(int id)
        {
            var joueur = await _context.Joueurs
                .FirstOrDefaultAsync(j => j.Id == id);

            if (joueur == null) return NotFound();

            return View(joueur);
        }

        // POST: Joueurs/Supprimer/5
        [HttpPost, ActionName("Supprimer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirme(int id)
        {
            var joueur = await _context.Joueurs.FindAsync(id);
            if (joueur != null)
            {
                _context.Joueurs.Remove(joueur);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Joueur supprimé avec succès !";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool JoueurExists(int id)
        {
            return _context.Joueurs.Any(e => e.Id == id);
        }
    }
}