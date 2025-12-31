using GestionnaireFootball.Models;

namespace GestionnaireFootball.ViewModels
{
    public class PresenceMarquageViewModel
    {
        public int MatchId { get; set; }
        public string MatchTitre { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public List<JoueurPresenceViewModel> Joueurs { get; set; } = new();
    }

    public class JoueurPresenceViewModel
    {
        public int JoueurId { get; set; }
        public string NomComplet { get; set; } = string.Empty;
        public int? NumeroMaillot { get; set; }
        public Poste Poste { get; set; }
        public StatutPresence? Statut { get; set; }
        public string? Commentaire { get; set; }
        public bool EstPresent => Statut == StatutPresence.Present;
        public bool EstAbsent => Statut == StatutPresence.Absent;
    }

    public class StatistiquesViewModel
    {
        public int TotalJoueurs { get; set; }
        public int TotalMatchs { get; set; }
        public int TotalPresences { get; set; }
        public int TotalPresents { get; set; }
        public int TotalAbsents { get; set; }
        public double TauxPresenceGlobal { get; set; }
        public List<StatJoueurViewModel> StatsParJoueur { get; set; } = new();
    }

    public class StatJoueurViewModel
    {
        public int JoueurId { get; set; }
        public string JoueurNom { get; set; } = string.Empty;
        public int MatchsJoues { get; set; }
        public int MatchsManques { get; set; }
        public double TauxPresence { get; set; }
        public string? DernierStatut { get; set; }
        public DateTime? DernierePresence { get; set; }
    }
}