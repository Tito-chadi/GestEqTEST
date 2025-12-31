using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La date et l'heure sont requises")]
        [DataType(DataType.DateTime)]
        public DateTime DateHeure { get; set; }

        [Required(ErrorMessage = "L'adversaire est requis")]
        [MaxLength(100)]
        public string Adversaire { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le lieu est requis")]
        [MaxLength(200)]
        public string Lieu { get; set; } = string.Empty;

        public bool? EstDomicile { get; set; } = true;

        [MaxLength(10)]
        public string? Score { get; set; } // Format "2-1"

        public int? ButsPour { get; set; }
        public int? ButsContre { get; set; }

        // Propriétés de navigation
        public virtual ICollection<Presence>? Presences { get; set; }
        public virtual ICollection<Composition>? Compositions { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }

        [NotMapped]
        public string Resultat
        {
            get
            {
                if (!ButsPour.HasValue || !ButsContre.HasValue)
                    return "À jouer";

                if (ButsPour > ButsContre)
                    return "Victoire";
                else if (ButsPour < ButsContre)
                    return "Défaite";
                else
                    return "Nul";
            }
        }
    }
}