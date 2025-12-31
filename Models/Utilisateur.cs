using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    [Table("Utilisateurs")]  
    public abstract class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Le mot de passe doit avoir au moins 6 caractères")]
        public string MotDePasseHash { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateNaissance { get; set; }

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        public string? Telephone { get; set; }

        [NotMapped]
        public string NomComplet => $"{Prenom} {Nom}";
    }
}