using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    public class PosteTerrain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompositionId { get; set; }

        [ForeignKey("CompositionId")]
        public virtual Composition? Composition { get; set; }

        [Required]
        [MaxLength(50)]
        public string NomPoste { get; set; } = string.Empty; // Ex: "Gardien", "Défenseur central gauche"

        // Coordonnées pour l'affichage visuel (0-100 pourcentage)
        [Range(0, 100)]
        public int PositionX { get; set; } = 50;

        [Range(0, 100)]
        public int PositionY { get; set; } = 50;

        // Joueur assigné (optionnel)
        public int? JoueurId { get; set; }

        [ForeignKey("JoueurId")]
        public virtual Joueur? Joueur { get; set; }

        public int? Ordre { get; set; } // Pour trier les postes
    }
}