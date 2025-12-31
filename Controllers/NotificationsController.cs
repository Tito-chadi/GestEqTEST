using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Models;
using GestionnaireFootball.Filters;
using GestionnaireFootball.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace GestionnaireFootball.Controllers
{
    [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notifications
        public async Task<IActionResult> Index(string? type, bool? nonLues)
        {
            var query = _context.Notifications
                .Include(n => n.Destinataire)
                .Include(n => n.Match)
                .OrderByDescending(n => n.DateEnvoi)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type) && type != "Tous")
            {
                query = query.Where(n => n.Type == type);
            }

            if (nonLues == true)
            {
                query = query.Where(n => !n.EstLue);
            }

            var notifications = await query.ToListAsync();

            ViewBag.Types = new List<string> { "Annonce", "Rappel", "Changement", "Information", "Urgent", "Félicitations", "Avertissement" };
            ViewBag.SelectedType = type;
            ViewBag.NonLues = nonLues;

            return View(notifications);
        }

        // GET: Notifications/Creer
        public async Task<IActionResult> Creer()
        {
            var viewModel = new NotificationViewModel
            {
                DateEnvoi = DateTime.Now,
                Types = GetTypesNotification(),
                Destinataires = await GetDestinataires(),
                Matchs = await GetProchainsMatchs()
            };

            return View(viewModel);
        }

        // POST: Notifications/Creer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer(NotificationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Si "Tous" est sélectionné, créer une notification pour chaque joueur
                if (viewModel.PourTous)
                {
                    var tousLesJoueurs = await _context.Joueurs.ToListAsync();
                    var notifications = new List<Notification>();

                    foreach (var joueur in tousLesJoueurs)
                    {
                        var notification = new Notification
                        {
                            Message = viewModel.Message,
                            DateEnvoi = viewModel.DateEnvoi,
                            Type = viewModel.TypeSelectionne,
                            MatchId = viewModel.MatchId,
                            DestinataireId = joueur.Id,
                            EstLue = false
                        };
                        notifications.Add(notification);
                    }

                    _context.Notifications.AddRange(notifications);
                }
                else if (viewModel.DestinataireIds != null && viewModel.DestinataireIds.Any())
                {
                    // Notifications pour des destinataires spécifiques
                    var notifications = new List<Notification>();

                    foreach (var destinataireId in viewModel.DestinataireIds)
                    {
                        var notification = new Notification
                        {
                            Message = viewModel.Message,
                            DateEnvoi = viewModel.DateEnvoi,
                            Type = viewModel.TypeSelectionne,
                            MatchId = viewModel.MatchId,
                            DestinataireId = destinataireId,
                            EstLue = false
                        };
                        notifications.Add(notification);
                    }

                    _context.Notifications.AddRange(notifications);
                }
                else
                {
                    // Notification générale (sans destinataire spécifique)
                    var notification = new Notification
                    {
                        Message = viewModel.Message,
                        DateEnvoi = viewModel.DateEnvoi,
                        Type = viewModel.TypeSelectionne,
                        MatchId = viewModel.MatchId,
                        EstLue = false
                    };
                    _context.Notifications.Add(notification);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Notification(s) envoyée(s) avec succès !";
                return RedirectToAction(nameof(Index));
            }

            // Recharger les données pour la vue
            viewModel.Types = GetTypesNotification();
            viewModel.Destinataires = await GetDestinataires();
            viewModel.Matchs = await GetProchainsMatchs();

            return View(viewModel);
        }

        // GET: Notifications/MarquerCommeLue/5
        public async Task<IActionResult> MarquerCommeLue(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.EstLue = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Notifications/MarquerToutesCommeLues
        public async Task<IActionResult> MarquerToutesCommesLues()
        {
            var notificationsNonLues = await _context.Notifications
                .Where(n => !n.EstLue)
                .ToListAsync();

            foreach (var notification in notificationsNonLues)
            {
                notification.EstLue = true;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Toutes les notifications ({notificationsNonLues.Count}) ont été marquées comme lues.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Notifications/MesNotifications (pour les joueurs)
        [AuthorizeRole(Role.JOUEUR)]
        public async Task<IActionResult> MesNotifications()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);

            var notifications = await _context.Notifications
                .Include(n => n.Match)
                .Include(n => n.Destinataire)
                .Where(n => n.DestinataireId == userIdInt || n.DestinataireId == null)
                .OrderByDescending(n => n.DateEnvoi)
                .ToListAsync();

            // Marquer comme lues quand on les consulte
            var nonLues = notifications.Where(n => !n.EstLue).ToList();
            foreach (var notification in nonLues)
            {
                notification.EstLue = true;
            }
            await _context.SaveChangesAsync();

            return View(notifications);
        }

        // GET: Notifications/Supprimer/5
        public async Task<IActionResult> Supprimer(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.Destinataire)
                .Include(n => n.Match)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Supprimer/5
        [HttpPost, ActionName("Supprimer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirme(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Notification supprimée avec succès !";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Notifications/EnvoyerRappelMatch/5
        public async Task<IActionResult> EnvoyerRappelMatch(int matchId)
        {
            var match = await _context.Matchs
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return NotFound();
            }

            // Récupérer les joueurs
            var joueurs = await _context.Joueurs.ToListAsync();

            // Créer les notifications de rappel
            var notifications = new List<Notification>();
            foreach (var joueur in joueurs)
            {
                var notification = new Notification
                {
                    Message = $"Rappel : Match contre {match.Adversaire} le {match.DateHeure:dd/MM/yyyy à HH:mm}. Merci de déclarer votre présence.",
                    DateEnvoi = DateTime.Now,
                    Type = "Rappel",
                    MatchId = matchId,
                    DestinataireId = joueur.Id,
                    EstLue = false
                };
                notifications.Add(notification);
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Rappels envoyés à {joueurs.Count} joueur(s) !";
            return RedirectToAction(nameof(Index));
        }

        // ✅ API: GET Notifications/GetNonLuesCount
        [HttpGet]
        public async Task<IActionResult> GetNonLuesCount()
        {
            var userId = HttpContext.Session.GetString("UserId");
            int? userIdInt = null;

            if (!string.IsNullOrEmpty(userId))
            {
                userIdInt = int.Parse(userId);
            }

            var count = await _context.Notifications
                .CountAsync(n => !n.EstLue && (n.DestinataireId == userIdInt || n.DestinataireId == null));

            return Json(new { count });
        }

        // ✅ API: GET Notifications/GetRecentNotifications
        [HttpGet]
        public async Task<IActionResult> GetRecentNotifications()
        {
            var userId = HttpContext.Session.GetString("UserId");
            int? userIdInt = null;

            if (!string.IsNullOrEmpty(userId))
            {
                userIdInt = int.Parse(userId);
            }

            var notifications = await _context.Notifications
                .Include(n => n.Match)
                .Where(n => n.DestinataireId == userIdInt || n.DestinataireId == null)
                .OrderByDescending(n => n.DateEnvoi)
                .Take(5)
                .Select(n => new
                {
                    n.Id,
                    Message = n.Message.Length > 50 ? n.Message.Substring(0, 50) + "..." : n.Message,
                    n.Type,
                    n.DateEnvoi,
                    n.EstLue,
                    MatchAdversaire = n.Match != null ? n.Match.Adversaire : null
                })
                .ToListAsync();

            return Json(new { notifications });
        }

        // ✅ Méthode pour les joueurs pour marquer une notification comme lue
        [HttpPost]
        public async Task<IActionResult> MarquerLue(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.EstLue = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private List<string> GetTypesNotification()
        {
            return new List<string>
            {
                "Annonce",
                "Rappel",
                "Changement",
                "Information",
                "Urgent",
                "Félicitations",
                "Avertissement"
            };
        }

        private async Task<List<DestinataireViewModel>> GetDestinataires()
        {
            var joueurs = await _context.Joueurs
                .Select(j => new DestinataireViewModel
                {
                    Id = j.Id,
                    NomComplet = j.NomComplet,
                    Type = "Joueur"
                })
                .ToListAsync();

            var personnels = await _context.Personnels
                .Select(p => new DestinataireViewModel
                {
                    Id = p.Id,
                    NomComplet = p.NomComplet,
                    Type = "Personnel"
                })
                .ToListAsync();

            return joueurs.Concat(personnels).OrderBy(d => d.NomComplet).ToList();
        }

        private async Task<List<Match>> GetProchainsMatchs()
        {
            return await _context.Matchs
                .Where(m => m.DateHeure > DateTime.Now)
                .OrderBy(m => m.DateHeure)
                .Take(10)
                .ToListAsync();
        }
    }
}