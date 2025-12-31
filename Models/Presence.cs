using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    public class Presence
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JoueurId { get; set; }

        [ForeignKey("JoueurId")]
        public virtual Joueur? Joueur { get; set; }

        // Clé étrangère pour Match
        public int? MatchId { get; set; }

        [ForeignKey("MatchId")]
        public virtual Match? Match { get; set; }

        // Clé étrangère pour Entrainement (on le traitera plus tard)
        public int? EntrainementId { get; set; }

        [Required]
        public StatutPresence Statut { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? HeureArrivee { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Commentaire { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateSaisie { get; set; } = DateTime.Now;
    }
}