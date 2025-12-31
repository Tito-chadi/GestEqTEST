using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Models;
using GestionnaireFootball.Filters;

namespace GestionnaireFootball.Controllers
{
    [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR, Role.JOUEUR)]
    public class MatchsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Matchs - Vue calendrier
        public async Task<IActionResult> Index(string? mois = null)
        {
            DateTime dateReference = mois != null ?
                DateTime.Parse(mois) :
                DateTime.Now;

            var debutMois = new DateTime(dateReference.Year, dateReference.Month, 1);
            var finMois = debutMois.AddMonths(1).AddDays(-1);

            var matchs = await _context.Matchs
                .Where(m => m.DateHeure >= debutMois && m.DateHeure <= finMois)
                .OrderBy(m => m.DateHeure)
                .ToListAsync();

            ViewBag.MoisCourant = dateReference;
            ViewBag.MoisPrecedent = debutMois.AddMonths(-1);
            ViewBag.MoisSuivant = debutMois.AddMonths(1);

            return View(matchs);
        }

        // GET: Matchs/Details/5 avec composition
        public async Task<IActionResult> Details(int id)
        {
            var match = await _context.Matchs
                .Include(m => m.Compositions)
                    .ThenInclude(c => c.PostesTerrain)
                        .ThenInclude(pt => pt.Joueur)
                .Include(m => m.Presences)
                    .ThenInclude(p => p.Joueur)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
            {
                return NotFound();
            }

            // Statistiques de présence
            var totalJoueurs = await _context.Joueurs.CountAsync();
            var presents = match.Presences?.Count(p => p.Statut == StatutPresence.Present) ?? 0;
            var absents = match.Presences?.Count(p => p.Statut == StatutPresence.Absent) ?? 0;

            ViewBag.TotalJoueurs = totalJoueurs;
            ViewBag.Presents = presents;
            ViewBag.Absents = absents;
            ViewBag.TauxPresence = totalJoueurs > 0 ? (presents * 100 / totalJoueurs) : 0;

            return View(match);
        }

        // GET: Matchs/Creer
        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public IActionResult Creer()
        {
            return View();
        }

        // POST: Matchs/Creer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public async Task<IActionResult> Creer(Match match)
        {
            // Vérifier l'unicité de la date
            var matchExistant = await _context.Matchs
                .FirstOrDefaultAsync(m => m.DateHeure.Date == match.DateHeure.Date);

            if (matchExistant != null)
            {
                ModelState.AddModelError("DateHeure", "Un match est déjà prévu à cette date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(match);
                await _context.SaveChangesAsync();

                // Créer automatiquement une composition vide pour ce match
                var composition = new Composition
                {
                    Nom = $"Équipe vs {match.Adversaire}",
                    Formation = "4-4-2",
                    MatchId = match.Id,
                    DateCreation = DateTime.Now
                };

                _context.Add(composition);
                await _context.SaveChangesAsync();

                // Générer les 11 postes vides
                var postes = GenererPostesParDefaut("4-4-2");
                foreach (var poste in postes)
                {
                    poste.CompositionId = composition.Id;
                    _context.PostesTerrain.Add(poste);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Match créé avec succès ! Une composition a été générée automatiquement.";
                return RedirectToAction(nameof(Details), new { id = match.Id });
            }
            return View(match);
        }

        // GET: Matchs/Editer/5
        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public async Task<IActionResult> Editer(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            return View(match);
        }

        // POST: Matchs/Editer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public async Task<IActionResult> Editer(int id, Match match)
        {
            if (id != match.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Match mis à jour avec succès !";
                return RedirectToAction(nameof(Index));
            }
            return View(match);
        }

        // GET: Matchs/Supprimer/5
        [AuthorizeRole(Role.ADMIN)]
        public async Task<IActionResult> Supprimer(int id)
        {
            var match = await _context.Matchs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Matchs/Supprimer/5
        [HttpPost, ActionName("Supprimer")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(Role.ADMIN)]
        public async Task<IActionResult> SupprimerConfirme(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match != null)
            {
                _context.Matchs.Remove(match);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Match supprimé avec succès !";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Matchs/Calendrier - Vue mensuelle complète
        public async Task<IActionResult> Calendrier(int? annee, int? mois)
        {
            int year = annee ?? DateTime.Now.Year;
            int month = mois ?? DateTime.Now.Month;

            var debutMois = new DateTime(year, month, 1);
            var finMois = debutMois.AddMonths(1).AddDays(-1);

            var matchs = await _context.Matchs
                .Where(m => m.DateHeure >= debutMois && m.DateHeure <= finMois)
                .OrderBy(m => m.DateHeure)
                .ToListAsync();

            // Préparer le calendrier
            var calendrier = new List<List<DateTime?>>();
            var premierJour = debutMois.DayOfWeek;
            var joursDansMois = DateTime.DaysInMonth(year, month);

            // Ajuster pour lundi premier jour
            int decalage = premierJour == DayOfWeek.Sunday ? 6 : (int)premierJour - 1;

            var jour = 1;
            for (int semaine = 0; semaine < 6; semaine++)
            {
                var semaineCal = new List<DateTime?>();
                for (int jourSemaine = 0; jourSemaine < 7; jourSemaine++)
                {
                    if ((semaine == 0 && jourSemaine < decalage) || jour > joursDansMois)
                    {
                        semaineCal.Add(null);
                    }
                    else
                    {
                        var date = new DateTime(year, month, jour);
                        semaineCal.Add(date);
                        jour++;
                    }
                }
                calendrier.Add(semaineCal);
                if (jour > joursDansMois) break;
            }

            ViewBag.Calendrier = calendrier;
            ViewBag.Matchs = matchs.ToDictionary(m => m.DateHeure.Date);
            ViewBag.MoisCourant = debutMois;
            ViewBag.MoisPrecedent = debutMois.AddMonths(-1);
            ViewBag.MoisSuivant = debutMois.AddMonths(1);

            return View();
        }

        // GET: Matchs/RemplirScore/5
        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public async Task<IActionResult> RemplirScore(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            return View(match);
        }

        // POST: Matchs/RemplirScore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
        public async Task<IActionResult> RemplirScore(int id, int butsPour, int butsContre)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            match.ButsPour = butsPour;
            match.ButsContre = butsContre;
            match.Score = $"{butsPour}-{butsContre}";

            _context.Update(match);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Score enregistré : {match.Score}";
            return RedirectToAction(nameof(Details), new { id = match.Id });
        }

        private bool MatchExists(int id)
        {
            return _context.Matchs.Any(e => e.Id == id);
        }

        private List<PosteTerrain> GenererPostesParDefaut(string formation)
        {
            var postes = new List<PosteTerrain>();

            switch (formation)
            {
                case "4-4-2":
                    postes.AddRange(new[]
                    {
                        new PosteTerrain { NomPoste = "Gardien", PositionX = 50, PositionY = 5, Ordre = 1 },
                        new PosteTerrain { NomPoste = "Défenseur Droit", PositionX = 75, PositionY = 25, Ordre = 2 },
                        new PosteTerrain { NomPoste = "Défenseur Central Droit", PositionX = 60, PositionY = 25, Ordre = 3 },
                        new PosteTerrain { NomPoste = "Défenseur Central Gauche", PositionX = 40, PositionY = 25, Ordre = 4 },
                        new PosteTerrain { NomPoste = "Défenseur Gauche", PositionX = 25, PositionY = 25, Ordre = 5 },
                        new PosteTerrain { NomPoste = "Milieu Droit", PositionX = 75, PositionY = 45, Ordre = 6 },
                        new PosteTerrain { NomPoste = "Milieu Central Droit", PositionX = 60, PositionY = 45, Ordre = 7 },
                        new PosteTerrain { NomPoste = "Milieu Central Gauche", PositionX = 40, PositionY = 45, Ordre = 8 },
                        new PosteTerrain { NomPoste = "Milieu Gauche", PositionX = 25, PositionY = 45, Ordre = 9 },
                        new PosteTerrain { NomPoste = "Attaquant Droit", PositionX = 70, PositionY = 70, Ordre = 10 },
                        new PosteTerrain { NomPoste = "Attaquant Gauche", PositionX = 30, PositionY = 70, Ordre = 11 }
                    });
                    break;
                // Ajouter d'autres formations si besoin
                default:
                    // Génération générique
                    for (int i = 1; i <= 11; i++)
                    {
                        postes.Add(new PosteTerrain
                        {
                            NomPoste = $"Joueur {i}",
                            PositionX = 50,
                            PositionY = 5 + (i * 8),
                            Ordre = i
                        });
                    }
                    break;
            }

            return postes;
        }
    }
}