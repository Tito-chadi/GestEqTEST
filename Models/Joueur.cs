using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    public class Joueur : Utilisateur
    {
        [Required]
        public Poste Poste { get; set; }

        [Range(1, 99, ErrorMessage = "Le numéro doit être entre 1 et 99")]
        public int? NumeroMaillot { get; set; }

        [Range(0, 10)]
        public float NoteGlobale { get; set; } = 5.0f;

        // Propriétés de navigation
        public virtual ICollection<Presence>? Presences { get; set; }
        public virtual ICollection<PosteTerrain>? PostesOccupes { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }

        // Propriétés calculées (non mappées)
        [NotMapped]
        public int MatchsJoues => Presences?.Count(p => p.Statut == StatutPresence.Present) ?? 0;

        [NotMapped]
        public float TauxPresence
        {
            get
            {
                if (Presences == null || !Presences.Any())
                    return 0;

                var total = Presences.Count();
                var presents = Presences.Count(p => p.Statut == StatutPresence.Present);
                return (float)presents / total * 100;
            }
        }
    }
}