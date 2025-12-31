namespace GestionnaireFootball.Models
{
    public enum Role
    {
        ADMIN,
        ENTRAINEUR,
        JOUEUR
    }

    public enum Poste
    {
        Gardien,
        Defenseur,
        Milieu,
        Attaquant
    }

    public enum Fonction
    {
        EntraineurPrincipal,
        EntraineurAdjoint,
        PreparateurPhysique,
        Kinesitherapeute
    }

    public enum StatutPresence
    {
        Present,
        Absent,
        EnRetard,
        Blesse
    }
}