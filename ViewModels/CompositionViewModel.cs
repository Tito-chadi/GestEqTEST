using GestionnaireFootball.Models;
using System.ComponentModel.DataAnnotations;

namespace GestionnaireFootball.ViewModels
{
    public class CompositionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom de la composition")]
        public string Nom { get; set; } = "Nouvelle Composition";

        [Required]
        [Display(Name = "Formation")]
        [RegularExpression(@"^\d{1,2}-\d{1,2}-\d{1,2}$", ErrorMessage = "Format invalide. Exemple: 4-4-2")]
        public string Formation { get; set; } = "4-4-2";

        public int? MatchId { get; set; }

        [Display(Name = "Joueurs disponibles")]
        public List<JoueurSelectItem> JoueursDisponibles { get; set; } = new();

        [Display(Name = "Postes sur le terrain")]
        public List<PosteTerrainViewModel> Postes { get; set; } = new();
    }

    public class PosteTerrainViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom du poste")]
        public string NomPoste { get; set; } = string.Empty;

        [Range(0, 100)]
        [Display(Name = "Position X")]
        public int PositionX { get; set; } = 50;

        [Range(0, 100)]
        [Display(Name = "Position Y")]
        public int PositionY { get; set; } = 50;

        [Display(Name = "Joueur assigné")]
        public int? JoueurId { get; set; }

        public string? JoueurNom { get; set; }
        public string? JoueurNumero { get; set; }
        public string? PosteJoueur { get; set; }
    }

    public class JoueurSelectItem
    {
        public int Id { get; set; }
        public string NomComplet { get; set; } = string.Empty;
        public int? NumeroMaillot { get; set; }
        public Poste Poste { get; set; }
        public bool EstSelectionnable { get; set; } = true;
    }
}