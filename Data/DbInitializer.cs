using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Models;
using GestionnaireFootball.Helpers;

namespace GestionnaireFootball.Data
{
    public static class DbInitializer
    {
        public static async void Initialize(ApplicationDbContext context)
        {
            // Vérifier si la base de données existe, la créer si nécessaire
            context.Database.EnsureCreated();

            // ✅ Vérifier si des UTILISATEURS existent déjà (pas Joueurs/Personnels séparément)
            if (context.Utilisateurs.Any())
            {
                Console.WriteLine("Base de données déjà initialisée.");
                return;
            }

            Console.WriteLine("Initialisation de la base de données...");

            // 1. CRÉER LES ADMINISTRATEURS ET STAFF (Personnel)
            // -------------------------------------------------
            var admin = new Personnel
            {
                Email = "admin@club.com",
                MotDePasseHash = PasswordHasher.HashPassword("Admin123!"),
                Nom = "Martin",
                Prenom = "Pierre",
                Role = Role.ADMIN,
                Fonction = Fonction.EntraineurPrincipal,
                DateEmbauche = DateTime.Now.AddYears(-2),
                Telephone = "0601020304",
                Specialite = "Administration système"
            };

            var entraineurPrincipal = new Personnel
            {
                Email = "coach@club.com",
                MotDePasseHash = PasswordHasher.HashPassword("Coach123!"),
                Nom = "Dubois",
                Prenom = "Marc",
                Role = Role.ENTRAINEUR,
                Fonction = Fonction.EntraineurPrincipal,
                DateEmbauche = DateTime.Now.AddYears(-1),
                Telephone = "0605060708",
                Specialite = "Formation 4-4-2, Défense organisée",
                DateNaissance = new DateTime(1980, 5, 15)
            };

            var preparateurPhysique = new Personnel
            {
                Email = "prepa@club.com",
                MotDePasseHash = PasswordHasher.HashPassword("Prepa123!"),
                Nom = "Leroy",
                Prenom = "Thomas",
                Role = Role.ENTRAINEUR,
                Fonction = Fonction.PreparateurPhysique,
                DateEmbauche = DateTime.Now.AddMonths(-6),
                Telephone = "0609080706",
                Specialite = "Préparation physique, Récupération",
                DateNaissance = new DateTime(1985, 8, 22)
            };

            var kinesitherapeute = new Personnel
            {
                Email = "kine@club.com",
                MotDePasseHash = PasswordHasher.HashPassword("Kine123!"),
                Nom = "Garcia",
                Prenom = "Sophie",
                Role = Role.ENTRAINEUR,
                Fonction = Fonction.Kinesitherapeute,
                DateEmbauche = DateTime.Now.AddMonths(-3),
                Telephone = "0611121314",
                Specialite = "Rééducation, Prévention des blessures",
                DateNaissance = new DateTime(1990, 3, 10)
            };

            // 2. CRÉER LES JOUEURS
            // ---------------------
            var joueurs = new Joueur[]
            {
                // GARDIENS
                new() {
                    Email = "g.bernard@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Bernard",
                    Prenom = "Gabriel",
                    Role = Role.JOUEUR,
                    Poste = Poste.Gardien,
                    NumeroMaillot = 1,
                    DateNaissance = new DateTime(1990, 5, 15),
                    Telephone = "0612345678",
                    NoteGlobale = 7.5f
                },
                new() {
                    Email = "r.petit@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Petit",
                    Prenom = "Romain",
                    Role = Role.JOUEUR,
                    Poste = Poste.Gardien,
                    NumeroMaillot = 16,
                    DateNaissance = new DateTime(1995, 8, 22),
                    Telephone = "0623456789",
                    NoteGlobale = 6.0f
                },

                // DÉFENSEURS
                new() {
                    Email = "l.moreau@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Moreau",
                    Prenom = "Lucas",
                    Role = Role.JOUEUR,
                    Poste = Poste.Defenseur,
                    NumeroMaillot = 2,
                    DateNaissance = new DateTime(1992, 3, 10),
                    Telephone = "0634567890",
                    NoteGlobale = 7.0f
                },
                new() {
                    Email = "a.durand@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Durand",
                    Prenom = "Alexandre",
                    Role = Role.JOUEUR,
                    Poste = Poste.Defenseur,
                    NumeroMaillot = 3,
                    DateNaissance = new DateTime(1993, 11, 5),
                    Telephone = "0645678901",
                    NoteGlobale = 7.2f
                },
                new() {
                    Email = "m.leroy@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Leroy",
                    Prenom = "Mathieu",
                    Role = Role.JOUEUR,
                    Poste = Poste.Defenseur,
                    NumeroMaillot = 4,
                    DateNaissance = new DateTime(1991, 7, 18),
                    Telephone = "0656789012",
                    NoteGlobale = 7.8f
                },
                new() {
                    Email = "s.lambert@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Lambert",
                    Prenom = "Simon",
                    Role = Role.JOUEUR,
                    Poste = Poste.Defenseur,
                    NumeroMaillot = 5,
                    DateNaissance = new DateTime(1994, 9, 30),
                    Telephone = "0667890123",
                    NoteGlobale = 6.5f
                },

                // MILIEUX
                new() {
                    Email = "t.robert@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Robert",
                    Prenom = "Thomas",
                    Role = Role.JOUEUR,
                    Poste = Poste.Milieu,
                    NumeroMaillot = 6,
                    DateNaissance = new DateTime(1989, 12, 8),
                    Telephone = "0678901234",
                    NoteGlobale = 8.0f
                },
                new() {
                    Email = "j.richard@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Richard",
                    Prenom = "Julien",
                    Role = Role.JOUEUR,
                    Poste = Poste.Milieu,
                    NumeroMaillot = 7,
                    DateNaissance = new DateTime(1996, 4, 25),
                    Telephone = "0689012345",
                    NoteGlobale = 7.3f
                },
                new() {
                    Email = "n.petit@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Petit",
                    Prenom = "Nicolas",
                    Role = Role.JOUEUR,
                    Poste = Poste.Milieu,
                    NumeroMaillot = 8,
                    DateNaissance = new DateTime(1993, 6, 12),
                    Telephone = "0690123456",
                    NoteGlobale = 7.6f
                },
                new() {
                    Email = "d.simon@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Simon",
                    Prenom = "David",
                    Role = Role.JOUEUR,
                    Poste = Poste.Milieu,
                    NumeroMaillot = 10,
                    DateNaissance = new DateTime(1990, 2, 28),
                    Telephone = "0601234567",
                    NoteGlobale = 8.5f
                },

                // ATTAQUANTS
                new() {
                    Email = "c.michel@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Michel",
                    Prenom = "Charles",
                    Role = Role.JOUEUR,
                    Poste = Poste.Attaquant,
                    NumeroMaillot = 9,
                    DateNaissance = new DateTime(1995, 1, 14),
                    Telephone = "0611121314",
                    NoteGlobale = 8.2f
                },
                new() {
                    Email = "f.laurent@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Laurent",
                    Prenom = "François",
                    Role = Role.JOUEUR,
                    Poste = Poste.Attaquant,
                    NumeroMaillot = 11,
                    DateNaissance = new DateTime(1997, 10, 7),
                    Telephone = "0622232425",
                    NoteGlobale = 7.9f
                },
                new() {
                    Email = "p.henry@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Henry",
                    Prenom = "Paul",
                    Role = Role.JOUEUR,
                    Poste = Poste.Attaquant,
                    NumeroMaillot = 14,
                    DateNaissance = new DateTime(1998, 3, 18),
                    Telephone = "0633343536",
                    NoteGlobale = 7.0f
                },
                new() {
                    Email = "e.roger@club.com",
                    MotDePasseHash = PasswordHasher.HashPassword("Joueur123!"),
                    Nom = "Roger",
                    Prenom = "Émile",
                    Role = Role.JOUEUR,
                    Poste = Poste.Attaquant,
                    NumeroMaillot = 17,
                    DateNaissance = new DateTime(1996, 7, 9),
                    Telephone = "0644454647",
                    NoteGlobale = 6.8f
                }
            };

            // 3. CRÉER DES MATCHS DE TEST
            // ---------------------------
            var matchs = new Match[]
            {
                new() {
                    DateHeure = DateTime.Now.AddDays(7).AddHours(15),
                    Adversaire = "FC Les Champions",
                    Lieu = "Stade Municipal",
                    EstDomicile = true,
                    Score = null,
                    ButsPour = null,
                    ButsContre = null
                },
                new() {
                    DateHeure = DateTime.Now.AddDays(14).AddHours(18),
                    Adversaire = "AS Victoire",
                    Lieu = "Stade des Olympes",
                    EstDomicile = false,
                    Score = null,
                    ButsPour = null,
                    ButsContre = null
                },
                new() {
                    DateHeure = DateTime.Now.AddDays(-7).AddHours(15),
                    Adversaire = "Équipe United",
                    Lieu = "Stade Municipal",
                    EstDomicile = true,
                    Score = "2-1",
                    ButsPour = 2,
                    ButsContre = 1
                },
                new() {
                    DateHeure = DateTime.Now.AddDays(21).AddHours(16),
                    Adversaire = "Red Star FC",
                    Lieu = "Complexe Sportif",
                    EstDomicile = true,
                    Score = null,
                    ButsPour = null,
                    ButsContre = null
                }
            };

            // 4. AJOUTER TOUT AU CONTEXTE
            // ----------------------------
            Console.WriteLine("Ajout des utilisateurs...");

            // Personnel (Admin & Staff)
            context.Personnels.Add(admin);
            context.Personnels.Add(entraineurPrincipal);
            context.Personnels.Add(preparateurPhysique);
            context.Personnels.Add(kinesitherapeute);

            // Joueurs
            context.Joueurs.AddRange(joueurs);

            // Sauvegarder pour obtenir les IDs
            await context.SaveChangesAsync();
            Console.WriteLine($"{joueurs.Length + 4} utilisateurs créés.");

            // Matchs
            Console.WriteLine("Ajout des matchs...");
            context.Matchs.AddRange(matchs);
            await context.SaveChangesAsync();
            Console.WriteLine($"{matchs.Length} matchs créés.");

            // 5. CRÉER UNE COMPOSITION PAR DÉFAUT
            // -----------------------------------
            Console.WriteLine("Création d'une composition par défaut...");
            var compositionDefaut = new Composition
            {
                Nom = "Équipe Type Saison 2024",
                Formation = "4-4-2",
                DateCreation = DateTime.Now,
                MatchId = matchs[0].Id // Lier au premier match
            };

            context.Compositions.Add(compositionDefaut);
            await context.SaveChangesAsync();

            // 6. CRÉER LES POSTES DE TERRAIN POUR LA COMPOSITION
            // ---------------------------------------------------
            var joueursList = context.Joueurs.ToList();

            var postesTerrain = new List<PosteTerrain>
            {
                // Gardien
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Gardien",
                    PositionX = 50,
                    PositionY = 5,
                    JoueurId = joueursList[0].Id, // Premier joueur (gardien)
                    Ordre = 1
                },
                
                // Défense (4 joueurs)
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Défenseur Droit",
                    PositionX = 75,
                    PositionY = 25,
                    JoueurId = joueursList[2].Id,
                    Ordre = 2
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Défenseur Central Droit",
                    PositionX = 60,
                    PositionY = 25,
                    JoueurId = joueursList[3].Id,
                    Ordre = 3
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Défenseur Central Gauche",
                    PositionX = 40,
                    PositionY = 25,
                    JoueurId = joueursList[4].Id,
                    Ordre = 4
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Défenseur Gauche",
                    PositionX = 25,
                    PositionY = 25,
                    JoueurId = joueursList[5].Id,
                    Ordre = 5
                },
                
                // Milieu (4 joueurs)
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Milieu Droit",
                    PositionX = 75,
                    PositionY = 45,
                    JoueurId = joueursList[7].Id,
                    Ordre = 6
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Milieu Central Droit",
                    PositionX = 60,
                    PositionY = 45,
                    JoueurId = joueursList[6].Id,
                    Ordre = 7
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Milieu Central Gauche",
                    PositionX = 40,
                    PositionY = 45,
                    JoueurId = joueursList[8].Id,
                    Ordre = 8
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Milieu Gauche",
                    PositionX = 25,
                    PositionY = 45,
                    JoueurId = joueursList[9].Id,
                    Ordre = 9
                },
                
                // Attaque (2 joueurs)
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Attaquant Droit",
                    PositionX = 70,
                    PositionY = 70,
                    JoueurId = joueursList[10].Id,
                    Ordre = 10
                },
                new() {
                    CompositionId = compositionDefaut.Id,
                    NomPoste = "Attaquant Gauche",
                    PositionX = 30,
                    PositionY = 70,
                    JoueurId = joueursList[11].Id,
                    Ordre = 11
                }
            };

            context.PostesTerrain.AddRange(postesTerrain);
            await context.SaveChangesAsync();
            Console.WriteLine("11 postes de terrain créés.");

            // 7. CRÉER DES PRÉSENCES POUR LE DERNIER MATCH
            // --------------------------------------------
            Console.WriteLine("Création des présences...");
            var dernierMatch = matchs[2]; // Match déjà joué
            var presences = new List<Presence>();
            var random = new Random();

            foreach (var joueur in joueursList.Take(13)) // 13 joueurs sur 14
            {
                var statut = random.Next(0, 10) > 1 ? StatutPresence.Present : StatutPresence.Absent;

                presences.Add(new Presence
                {
                    JoueurId = joueur.Id,
                    MatchId = dernierMatch.Id,
                    Statut = statut,
                    DateSaisie = dernierMatch.DateHeure.AddHours(-2),
                    HeureArrivee = statut == StatutPresence.Present ?
                        dernierMatch.DateHeure.AddHours(-1).AddMinutes(random.Next(-30, 30)) :
                        null,
                    Commentaire = statut == StatutPresence.Absent ? "Blessé" : null
                });
            }

            context.Presences.AddRange(presences);
            await context.SaveChangesAsync();
            Console.WriteLine($"{presences.Count} présences créées.");

            // 8. CRÉER DES NOTIFICATIONS DE TEST
            // ----------------------------------
            Console.WriteLine("Création des notifications...");
            var notifications = new Notification[]
            {
                new() {
                    Message = "Rappel : Match contre FC Les Champions ce samedi à 15h. Présence obligatoire.",
                    DateEnvoi = DateTime.Now.AddDays(-1),
                    Type = "Rappel",
                    MatchId = matchs[0].Id,
                    DestinataireId = null, // Notification générale
                    EstLue = false
                },
                new() {
                    Message = "Félicitations à Charles Michel pour ses 2 buts lors du dernier match !",
                    DateEnvoi = DateTime.Now.AddDays(-2),
                    Type = "Félicitations",
                    DestinataireId = joueursList[10].Id, // Charles Michel
                    EstLue = true
                },
                new() {
                    Message = "Entraînement annulé demain pour cause de mauvais temps.",
                    DateEnvoi = DateTime.Now.AddHours(-3),
                    Type = "Changement",
                    DestinataireId = null, // Pour tout le monde
                    EstLue = false
                },
                new() {
                    Message = "Veuillez mettre à jour vos informations personnelles dans votre profil.",
                    DateEnvoi = DateTime.Now.AddDays(-5),
                    Type = "Information",
                    DestinataireId = null,
                    EstLue = true
                }
            };

            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();
            Console.WriteLine($"{notifications.Length} notifications créées.");

            // 9. MESSAGE FINAL
            // ----------------
            Console.WriteLine("\n✅ Base de données initialisée avec succès !");
            Console.WriteLine("==========================================");
            Console.WriteLine("Comptes de test disponibles :");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("ADMIN : admin@club.com / Admin123!");
            Console.WriteLine("ENTRAÎNEUR : coach@club.com / Coach123!");
            Console.WriteLine("PRÉPARATEUR : prepa@club.com / Prepa123!");
            Console.WriteLine("KINÉ : kine@club.com / Kine123!");
            Console.WriteLine("JOUEURS : g.bernard@club.com / Joueur123!");
            Console.WriteLine("          l.moreau@club.com / Joueur123!");
            Console.WriteLine("          (tous les joueurs : Joueur123!)");
            Console.WriteLine("==========================================\n");
        }
    }
}