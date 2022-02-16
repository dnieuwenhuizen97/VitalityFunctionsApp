using Domains;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Context
{
    public class DbContextDomains : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ActivityCategory> ActivityCategories { get; set; }
        public DbSet<Activity> Activities { get; set; } 
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<PushToken> PushTokens { get; set; }
        public DbSet<TimelinePost> TimelinePosts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserRecoveryToken> RecoveryTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer("Server=tcp:vitality-app-sql-server.database.windows.net,1433;Initial Catalog=VitalityAppDb-tst;Persist Security Info=False;User ID=vitalityadmin;Password=*H@Y4oU@$TmwC9@OMZo6;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();

                entity.HasMany(e => e.SubscribedChallenges)
                                            .WithOne(e => e.User)
                                            .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.TimelinePosts)
                                            .WithOne(e => e.User)
                                            .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Notifications)
                                            .WithOne(e => e.ToUser)
                                            .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Likes)
                                            .WithOne(e => e.User)
                                            .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Followers)
                                            .WithOne(e => e.FollowedUser)
                                            .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Comments)
                                            .WithOne(e => e.User)
                                            .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ActivityCategory>(entity =>
            {
                entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();
                entity.HasMany(e => e.Activities)
                            .WithOne(e => e.Category)
                            .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.Property(e => e.ActivityId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.Property(e => e.ChallengeId).ValueGeneratedOnAdd();

                entity.HasMany(e => e.SubscribedChallenges)
                                            .WithOne(e => e.Challenge)
                                            .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TimelinePost>(entity =>
            {
                entity.Property(e => e.TimelinePostId).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Comments)
                                            .WithOne(e => e.TimelinePost)
                                            .OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.Likes)
                                            .WithOne(e => e.TimelinePost)
                                            .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.NotificationId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Follower>(entity =>
            {
                entity.Property(e => e.FollowerId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PushToken>(entity =>
            {
                entity.Property(e => e.PushTokenId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.LikeId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserRecoveryToken>(entity =>
            {
                entity.Property(e => e.RecoveryTokenId).ValueGeneratedOnAdd();
            });
        }
    }
}
