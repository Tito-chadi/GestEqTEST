using System.ComponentModel.DataAnnotations;

namespace GestionnaireFootball.ViewModels
{
    public class DestinataireViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nom complet")]
        public string NomComplet { get; set; } = string.Empty;

        [Display(Name = "Type")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Rôle")]
        public string? Role { get; set; }

        [Display(Name = "Poste")]
        public string? Poste { get; set; } // Pour les joueurs

        [Display(Name = "Fonction")]
        public string? Fonction { get; set; } // Pour le personnel

        [Display(Name = "Numéro maillot")]
        public int? NumeroMaillot { get; set; } // Pour les joueurs

        // Pour les cases à cocher dans les formulaires
        public bool EstSelectionne { get; set; }

        // Pour l'affichage dans les listes déroulantes
        [Display(Name = "Affichage")]
        public string AffichageComplet => $"{NomComplet} ({Type})";

        // Pour le groupement dans les listes déroulantes
        public string Groupe => Type;
    }

    // Optionnel : Pour gérer les groupes de destinataires
    public class DestinataireGroupeViewModel
    {
        public string NomGroupe { get; set; } = string.Empty;
        public List<DestinataireViewModel> Destinataires { get; set; } = new();
    }

    // Pour la réponse de l'API de sélection de destinataires
    public class DestinatairesResponseViewModel
    {
        public List<DestinataireGroupeViewModel> Groupes { get; set; } = new();
        public int TotalDestinataires { get; set; }
        public int TotalJoueurs { get; set; }
        public int TotalPersonnel { get; set; }
    }

    // Pour la création de notification avec validation
    public class CreationNotificationViewModel
    {
        [Required(ErrorMessage = "Le message est obligatoire")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Le message doit contenir entre 5 et 500 caractères")]
        [Display(Name = "Message de la notification")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le type de notification est obligatoire")]
        [Display(Name = "Type de notification")]
        public string TypeNotification { get; set; } = "Information";

        [Display(Name = "Match concerné (optionnel)")]
        public int? MatchId { get; set; }

        [Display(Name = "Envoyer à tous les joueurs")]
        public bool EnvoyerATousLesJoueurs { get; set; } = true;

        [Display(Name = "Envoyer à tout le personnel")]
        public bool EnvoyerAToutLePersonnel { get; set; } = false;

        [Display(Name = "Destinataires spécifiques")]
        public List<int> DestinatairesIds { get; set; } = new();

        [Display(Name = "Envoyer maintenant")]
        public bool EnvoyerMaintenant { get; set; } = true;

        [Display(Name = "Date d'envoi programmée")]
        [DataType(DataType.DateTime)]
        public DateTime? DateEnvoiProgrammee { get; set; }

        // Options pour les types de notifications
        public List<string> TypesDisponibles { get; set; } = new List<string>
        {
            "Information",
            "Rappel",
            "Urgent",
            "Félicitations",
            "Avertissement",
            "Annonce",
            "Changement"
        };
    }
}