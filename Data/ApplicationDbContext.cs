using Microsoft.EntityFrameworkCore;
using GestionnaireFootball.Models;

namespace GestionnaireFootball.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Joueur> Joueurs { get; set; }
        public DbSet<Personnel> Personnels { get; set; }
        public DbSet<Match> Matchs { get; set; }
        public DbSet<Presence> Presences { get; set; }
        public DbSet<Composition> Compositions { get; set; }
        public DbSet<PosteTerrain> PostesTerrain { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Configuration TPH SIMPLIFIÉE pour MySQL
            modelBuilder.Entity<Utilisateur>()
                .ToTable("Utilisateurs")
                .HasDiscriminator<string>("TypeUtilisateur")
                .HasValue<Joueur>("Joueur")
                .HasValue<Personnel>("Personnel");

            // Contraintes d'unicité
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Match>()
                .HasIndex(m => m.DateHeure)
                .IsUnique();

            // Configuration des relations (gardez-les)
            modelBuilder.Entity<Presence>()
                .HasOne(p => p.Joueur)
                .WithMany(j => j.Presences)
                .HasForeignKey(p => p.JoueurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Presence>()
                .HasOne(p => p.Match)
                .WithMany(m => m.Presences)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PosteTerrain>()
                .HasOne(pt => pt.Composition)
                .WithMany(c => c.PostesTerrain)
                .HasForeignKey(pt => pt.CompositionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PosteTerrain>()
                .HasOne(pt => pt.Joueur)
                .WithMany(j => j.PostesOccupes)
                .HasForeignKey(pt => pt.JoueurId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Composition>()
                .HasOne(c => c.Match)
                .WithMany(m => m.Compositions)
                .HasForeignKey(c => c.MatchId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Destinataire)
                .WithMany()
                .HasForeignKey(n => n.DestinataireId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Match)
                .WithMany(m => m.Notifications)
                .HasForeignKey(n => n.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Noms de tables
            modelBuilder.Entity<Match>().ToTable("Matchs");
            modelBuilder.Entity<Presence>().ToTable("Presences");
            modelBuilder.Entity<Composition>().ToTable("Compositions");
            modelBuilder.Entity<PosteTerrain>().ToTable("PostesTerrain");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
        }
    }
}