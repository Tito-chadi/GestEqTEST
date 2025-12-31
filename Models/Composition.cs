using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireFootball.Models
{
    public class Composition
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la composition est requis")]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{1,2}-\d{1,2}-\d{1,2}$", ErrorMessage = "Format invalide. Utilisez X-X-X (ex: 4-4-2)")]
        public string Formation { get; set; } = "4-4-2";

        [DataType(DataType.DateTime)]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        public int? MatchId { get; set; }

        [ForeignKey("MatchId")]
        public virtual Match? Match { get; set; }

        // Collection exactement de 11 PosteTerrain
        public virtual ICollection<PosteTerrain> PostesTerrain { get; set; } = new List<PosteTerrain>();

        [NotMapped]
        public int NombreJoueursAssignes => PostesTerrain?.Count(pt => pt.JoueurId.HasValue) ?? 0;
    }
}