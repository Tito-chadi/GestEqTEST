using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Models;
using GestionnaireFootball.Filters;
using GestionnaireFootball.ViewModels;

namespace GestionnaireFootball.Controllers
{
    [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
    public class PresencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PresencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Presences - Vue par match
        public async Task<IActionResult> Index(int? matchId)
        {
            var matchs = await _context.Matchs
                .Where(m => m.DateHeure >= DateTime.Now.AddDays(-7)) // 7 derniers jours
                .OrderBy(m => m.DateHeure)
                .ToListAsync();

            ViewBag.Matchs = matchs;
            ViewBag.SelectedMatchId = matchId;

            if (matchId.HasValue)
            {
                var match = await _context.Matchs
                    .Include(m => m.Presences)
                        .ThenInclude(p => p.Joueur)
                    .FirstOrDefaultAsync(m => m.Id == matchId);

                if (match != null)
                {
                    // Récupérer tous les joueurs pour les marquer présents/absents
                    var tousJoueurs = await _context.Joueurs
                        .OrderBy(j => j.Nom)
                        .ThenBy(j => j.Prenom)
                        .ToListAsync();

                    var viewModel = new PresenceViewModel
                    {
                        Match = match,
                        TousJoueurs = tousJoueurs,
                        Presences = match.Presences?.ToDictionary(p => p.JoueurId) ?? new Dictionary<int, Presence>()
                    };

                    return View(viewModel);
                }
            }

            return View(new PresenceViewModel());
        }

        // POST: Presences/MarquerPresence
        [HttpPost]
        public async Task<IActionResult> MarquerPresence(int matchId, int joueurId, StatutPresence statut, string? commentaire)
        {
            var presenceExistante = await _context.Presences
                .FirstOrDefaultAsync(p => p.MatchId == matchId && p.JoueurId == joueurId);

            if (presenceExistante != null)
            {
                // Mettre à jour
                presenceExistante.Statut = statut;
                presenceExistante.Commentaire = commentaire;
                presenceExistante.DateSaisie = DateTime.Now;

                if (statut == StatutPresence.Present)
                {
                    presenceExistante.HeureArrivee = DateTime.Now;
                }
                else
                {
                    presenceExistante.HeureArrivee = null;
                }
            }
            else
            {
                // Créer nouvelle présence
                var presence = new Presence
                {
                    MatchId = matchId,
                    JoueurId = joueurId,
                    Statut = statut,
                    Commentaire = commentaire,
                    DateSaisie = DateTime.Now,
                    HeureArrivee = statut == StatutPresence.Present ? DateTime.Now : null
                };
                _context.Presences.Add(presence);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, statut = statut.ToString() });
        }

        // GET: Presences/Stats
        public async Task<IActionResult> Stats(int? joueurId, int? mois, int? annee)
        {
            var moisCourant = mois ?? DateTime.Now.Month;
            var anneeCourante = annee ?? DateTime.Now.Year;

            var debutMois = new DateTime(anneeCourante, moisCourant, 1);
            var finMois = debutMois.AddMonths(1).AddDays(-1);

            var query = _context.Presences
                .Include(p => p.Joueur)
                .Include(p => p.Match)
                .Where(p => p.Match.DateHeure >= debutMois && p.Match.DateHeure <= finMois);

            if (joueurId.HasValue)
            {
                query = query.Where(p => p.JoueurId == joueurId);
            }

            var presences = await query.ToListAsync();

            // Statistiques globales
            var totalMatchs = await _context.Matchs
                .CountAsync(m => m.DateHeure >= debutMois && m.DateHeure <= finMois);

            var statsParJoueur = await _context.Joueurs
                .Select(j => new
                {
                    Joueur = j,
                    Presences = j.Presences.Where(p =>
                        p.Match.DateHeure >= debutMois &&
                        p.Match.DateHeure <= finMois).ToList()
                })
                .Where(x => x.Presences.Any())
                .Select(x => new JoueurStatsViewModel
                {
                    JoueurId = x.Joueur.Id,
                    JoueurNom = x.Joueur.NomComplet,
                    TotalPresences = x.Presences.Count,
                    Presents = x.Presences.Count(p => p.Statut == StatutPresence.Present),
                    Absents = x.Presences.Count(p => p.Statut == StatutPresence.Absent),
                    TauxPresence = x.Presences.Count > 0 ?
                        (x.Presences.Count(p => p.Statut == StatutPresence.Present) * 100 / x.Presences.Count) : 0
                })
                .OrderByDescending(s => s.TauxPresence)
                .ToListAsync();

            ViewBag.MoisCourant = debutMois;
            ViewBag.Joueurs = await _context.Joueurs.OrderBy(j => j.Nom).ToListAsync();
            ViewBag.SelectedJoueurId = joueurId;
            ViewBag.StatsParJoueur = statsParJoueur;
            ViewBag.TotalMatchs = totalMatchs;

            return View(presences);
        }

        // GET: Presences/ExportExcel
        public async Task<IActionResult> ExportExcel(int? matchId, int? mois, int? annee)
        {
            var query = _context.Presences
                .Include(p => p.Joueur)
                .Include(p => p.Match)
                .AsQueryable();

            if (matchId.HasValue)
            {
                query = query.Where(p => p.MatchId == matchId);
            }
            else if (mois.HasValue && annee.HasValue)
            {
                var debutMois = new DateTime(annee.Value, mois.Value, 1);
                var finMois = debutMois.AddMonths(1).AddDays(-1);
                query = query.Where(p => p.Match.DateHeure >= debutMois && p.Match.DateHeure <= finMois);
            }

            var presences = await query
                .OrderBy(p => p.Match.DateHeure)
                .ThenBy(p => p.Joueur.Nom)
                .ToListAsync();

            // Générer CSV simple
            var csv = "Date;Joueur;Statut;Commentaire;Heure Arrivée\n";

            foreach (var presence in presences)
            {
                csv += $"{presence.Match.DateHeure:dd/MM/yyyy HH:mm};" +
                      $"{presence.Joueur.NomComplet};" +
                      $"{presence.Statut};" +
                      $"{presence.Commentaire ?? ""};" +
                      $"{presence.HeureArrivee:HH:mm}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"presences_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        // GET: Presences/RapportMensuel
        public async Task<IActionResult> RapportMensuel(int mois, int annee)
        {
            var debutMois = new DateTime(annee, mois, 1);
            var finMois = debutMois.AddMonths(1).AddDays(-1);

            var matchs = await _context.Matchs
                .Where(m => m.DateHeure >= debutMois && m.DateHeure <= finMois)
                .OrderBy(m => m.DateHeure)
                .ToListAsync();

            var joueurs = await _context.Joueurs
                .OrderBy(j => j.Nom)
                .ToListAsync();

            var presences = await _context.Presences
                .Include(p => p.Match)
                .Include(p => p.Joueur)
                .Where(p => p.Match.DateHeure >= debutMois && p.Match.DateHeure <= finMois)
                .ToListAsync();

            var viewModel = new RapportMensuelViewModel
            {
                Mois = debutMois,
                Matchs = matchs,
                Joueurs = joueurs,
                Presences = presences
            };

            return View(viewModel);
        }
    }

    public class PresenceViewModel
    {
        public Match? Match { get; set; }
        public List<Joueur> TousJoueurs { get; set; } = new();
        public Dictionary<int, Presence> Presences { get; set; } = new();
    }

    public class JoueurStatsViewModel
    {
        public int JoueurId { get; set; }
        public string JoueurNom { get; set; } = string.Empty;
        public int TotalPresences { get; set; }
        public int Presents { get; set; }
        public int Absents { get; set; }
        public int TauxPresence { get; set; }
    }

    public class RapportMensuelViewModel
    {
        public DateTime Mois { get; set; }
        public List<Match> Matchs { get; set; } = new();
        public List<Joueur> Joueurs { get; set; } = new();
        public List<Presence> Presences { get; set; } = new();
    }
}