using System.ComponentModel.DataAnnotations;

namespace GestionnaireFootball.Models
{
    public class Personnel : Utilisateur
    {
        [Required]
        public Fonction Fonction { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateEmbauche { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Specialite { get; set; }
    }
}