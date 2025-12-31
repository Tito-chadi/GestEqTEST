using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Data;
using GestionnaireFootball.Models;
using GestionnaireFootball.ViewModels;
using GestionnaireFootball.Filters;

namespace GestionnaireFootball.Controllers
{
    [AuthorizeRole(Role.ADMIN, Role.ENTRAINEUR)]
    public class CompositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Compositions
        public async Task<IActionResult> Index()
        {
            var compositions = await _context.Compositions
                .Include(c => c.Match)
                .Include(c => c.PostesTerrain)
                    .ThenInclude(pt => pt.Joueur)
                .OrderByDescending(c => c.DateCreation)
                .ToListAsync();

            return View(compositions);
        }

        // GET: Compositions/Visualiser/5
        public async Task<IActionResult> Visualiser(int id)
        {
            var composition = await _context.Compositions
                .Include(c => c.Match)
                .Include(c => c.PostesTerrain)
                    .ThenInclude(pt => pt.Joueur)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (composition == null)
            {
                return NotFound();
            }

            // Générer la vue modèle pour l'affichage visuel
            var viewModel = new CompositionViewModel
            {
                Id = composition.Id,
                Nom = composition.Nom,
                Formation = composition.Formation,
                MatchId = composition.MatchId,
                Postes = composition.PostesTerrain
                    .Select(pt => new PosteTerrainViewModel
                    {
                        Id = pt.Id,
                        NomPoste = pt.NomPoste,
                        PositionX = pt.PositionX,
                        PositionY = pt.PositionY,
                        JoueurId = pt.JoueurId,
                        JoueurNom = pt.Joueur?.NomComplet,
                        JoueurNumero = pt.Joueur?.NumeroMaillot?.ToString(),
                        PosteJoueur = pt.Joueur?.Poste.ToString()
                    })
                    .ToList()
            };

            ViewBag.Formation = composition.Formation;
            return View(viewModel);
        }

        // GET: Compositions/Creer
        public async Task<IActionResult> Creer()
        {
            var viewModel = new CompositionViewModel
            {
                JoueursDisponibles = await GetJoueursDisponibles()
            };

            // Créer les postes par défaut selon la formation 4-4-2
            viewModel.Postes = GenererPostesParDefaut("4-4-2");

            return View(viewModel);
        }

        // POST: Compositions/Creer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer(CompositionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var composition = new Composition
                {
                    Nom = viewModel.Nom,
                    Formation = viewModel.Formation,
                    MatchId = viewModel.MatchId,
                    DateCreation = DateTime.Now
                };

                // Ajouter les postes
                foreach (var posteVm in viewModel.Postes)
                {
                    composition.PostesTerrain.Add(new PosteTerrain
                    {
                        NomPoste = posteVm.NomPoste,
                        PositionX = posteVm.PositionX,
                        PositionY = posteVm.PositionY,
                        JoueurId = posteVm.JoueurId,
                        Ordre = composition.PostesTerrain.Count + 1
                    });
                }

                _context.Add(composition);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Composition créée avec succès !";
                return RedirectToAction(nameof(Visualiser), new { id = composition.Id });
            }

            viewModel.JoueursDisponibles = await GetJoueursDisponibles();
            return View(viewModel);
        }

        // GET: Compositions/Editer/5
        public async Task<IActionResult> Editer(int id)
        {
            var composition = await _context.Compositions
                .Include(c => c.PostesTerrain)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (composition == null)
            {
                return NotFound();
            }

            var viewModel = new CompositionViewModel
            {
                Id = composition.Id,
                Nom = composition.Nom,
                Formation = composition.Formation,
                MatchId = composition.MatchId,
                JoueursDisponibles = await GetJoueursDisponibles(),
                Postes = composition.PostesTerrain
                    .Select(pt => new PosteTerrainViewModel
                    {
                        Id = pt.Id,
                        NomPoste = pt.NomPoste,
                        PositionX = pt.PositionX,
                        PositionY = pt.PositionY,
                        JoueurId = pt.JoueurId,
                        JoueurNom = pt.Joueur?.NomComplet
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: Compositions/Editer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editer(int id, CompositionViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var composition = await _context.Compositions
                    .Include(c => c.PostesTerrain)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (composition == null)
                {
                    return NotFound();
                }

                composition.Nom = viewModel.Nom;
                composition.Formation = viewModel.Formation;
                composition.MatchId = viewModel.MatchId;

                // Mettre à jour les postes existants
                foreach (var posteVm in viewModel.Postes)
                {
                    var poste = composition.PostesTerrain.FirstOrDefault(p => p.Id == posteVm.Id);
                    if (poste != null)
                    {
                        poste.JoueurId = posteVm.JoueurId;
                    }
                }

                try
                {
                    _context.Update(composition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompositionExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["SuccessMessage"] = "Composition mise à jour avec succès !";
                return RedirectToAction(nameof(Visualiser), new { id = composition.Id });
            }

            viewModel.JoueursDisponibles = await GetJoueursDisponibles();
            return View(viewModel);
        }

        // GET: Compositions/ChangerFormation
        public IActionResult ChangerFormation(string formationActuelle = "4-4-2")
        {
            var formations = new List<string>
            {
                "4-4-2", "4-3-3", "4-2-3-1", "3-5-2", "3-4-3", "5-3-2", "4-5-1"
            };

            ViewBag.Formations = formations;
            ViewBag.FormationActuelle = formationActuelle;

            return View();
        }

        // POST: Compositions/GenererNouvelleFormation
        [HttpPost]
        public async Task<IActionResult> GenererNouvelleFormation(string nouvelleFormation)
        {
            var postes = GenererPostesParDefaut(nouvelleFormation);

            return Json(new
            {
                success = true,
                postes = postes,
                formation = nouvelleFormation
            });
        }

        private bool CompositionExists(int id)
        {
            return _context.Compositions.Any(e => e.Id == id);
        }

        private async Task<List<JoueurSelectItem>> GetJoueursDisponibles()
        {
            return await _context.Joueurs
                .OrderBy(j => j.Nom)
                .Select(j => new JoueurSelectItem
                {
                    Id = j.Id,
                    NomComplet = j.NomComplet,
                    NumeroMaillot = j.NumeroMaillot,
                    Poste = j.Poste
                })
                .ToListAsync();
        }

        private List<PosteTerrainViewModel> GenererPostesParDefaut(string formation)
        {
            var postes = new List<PosteTerrainViewModel>();
            var parts = formation.Split('-').Select(int.Parse).ToArray();

            // Position des lignes selon la formation
            var positions = new Dictionary<string, (int x, int y, string nom)[]>();

            switch (formation)
            {
                case "4-4-2":
                    postes.AddRange(new[]
                    {
                        new PosteTerrainViewModel { NomPoste = "Gardien", PositionX = 50, PositionY = 5 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Droit", PositionX = 75, PositionY = 25 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Central Droit", PositionX = 60, PositionY = 25 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Central Gauche", PositionX = 40, PositionY = 25 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Gauche", PositionX = 25, PositionY = 25 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Droit", PositionX = 75, PositionY = 45 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Central Droit", PositionX = 60, PositionY = 45 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Central Gauche", PositionX = 40, PositionY = 45 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Gauche", PositionX = 25, PositionY = 45 },
                        new PosteTerrainViewModel { NomPoste = "Attaquant Droit", PositionX = 70, PositionY = 70 },
                        new PosteTerrainViewModel { NomPoste = "Attaquant Gauche", PositionX = 30, PositionY = 70 }
                    });
                    break;

                case "4-3-3":
                    postes.AddRange(new[]
                    {
                        new PosteTerrainViewModel { NomPoste = "Gardien", PositionX = 50, PositionY = 5 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Droit", PositionX = 75, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Central Droit", PositionX = 60, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Central Gauche", PositionX = 40, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Gauche", PositionX = 25, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Défensif", PositionX = 50, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Droit", PositionX = 70, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Gauche", PositionX = 30, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Ailier Droit", PositionX = 80, PositionY = 65 },
                        new PosteTerrainViewModel { NomPoste = "Avant-Centre", PositionX = 50, PositionY = 75 },
                        new PosteTerrainViewModel { NomPoste = "Ailier Gauche", PositionX = 20, PositionY = 65 }
                    });
                    break;

                case "3-5-2":
                    postes.AddRange(new[]
                    {
                        new PosteTerrainViewModel { NomPoste = "Gardien", PositionX = 50, PositionY = 5 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Droit", PositionX = 75, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Central", PositionX = 50, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Défenseur Gauche", PositionX = 25, PositionY = 20 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Droit", PositionX = 80, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Central Droit", PositionX = 60, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Central", PositionX = 50, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Central Gauche", PositionX = 40, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Milieu Gauche", PositionX = 20, PositionY = 40 },
                        new PosteTerrainViewModel { NomPoste = "Attaquant Droit", PositionX = 60, PositionY = 70 },
                        new PosteTerrainViewModel { NomPoste = "Attaquant Gauche", PositionX = 40, PositionY = 70 }
                    });
                    break;

                default:
                    // Formation générique
                    int totalJoueurs = 11;
                    int yBase = 10;

                    for (int i = 0; i < totalJoueurs; i++)
                    {
                        int y = yBase + (i * 8);
                        int x = 50 + ((i % 2 == 0 ? 1 : -1) * 20 * ((i / 2) + 1));

                        postes.Add(new PosteTerrainViewModel
                        {
                            NomPoste = $"Joueur {i + 1}",
                            PositionX = Math.Clamp(x, 10, 90),
                            PositionY = Math.Clamp(y, 5, 85)
                        });
                    }
                    postes[0].NomPoste = "Gardien";
                    break;
            }

            return postes;
        }
    }
}