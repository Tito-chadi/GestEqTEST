using System.ComponentModel.DataAnnotations;
using GestionnaireFootball.Models;

namespace GestionnaireFootball.ViewModels
{
    public class NotificationViewModel
    {
        [Required(ErrorMessage = "Le message est requis")]
        [StringLength(500, ErrorMessage = "Le message ne peut pas dépasser 500 caractères")]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Type de notification")]
        public string TypeSelectionne { get; set; } = "Annonce";

        [Display(Name = "Date d'envoi")]
        [DataType(DataType.DateTime)]
        public DateTime DateEnvoi { get; set; } = DateTime.Now;

        [Display(Name = "Lier à un match")]
        public int? MatchId { get; set; }

        [Display(Name = "Envoyer à tous les joueurs")]
        public bool PourTous { get; set; } = true;

        [Display(Name = "Destinataires spécifiques")]
        public List<int>? DestinataireIds { get; set; }

        // Pour les listes déroulantes
        public List<string> Types { get; set; } = new();
        public List<DestinataireViewModel> Destinataires { get; set; } = new();
        public List<Match> Matchs { get; set; } = new();
    }
}