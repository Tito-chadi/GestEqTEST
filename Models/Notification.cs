using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime DateEnvoi { get; set; } = DateTime.Now;

        public bool EstLue { get; set; } = false;

        // Lien vers l'utilisateur destinataire
        public int? DestinataireId { get; set; }

        [ForeignKey("DestinataireId")]
        public virtual Utilisateur? Destinataire { get; set; }

        // Lien optionnel vers un match
        public int? MatchId { get; set; }

        [ForeignKey("MatchId")]
        public virtual Match? Match { get; set; }

        [MaxLength(100)]
        public string? Type { get; set; } // "Rappel", "Annonce", "Changement"
    }
}