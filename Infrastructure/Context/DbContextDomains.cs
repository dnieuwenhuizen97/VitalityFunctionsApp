using Domains;
using Domains.DAL;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Context
{
    public class DbContextDomains : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<PushTokenDAL> PushTokens { get; set; }
        public DbSet<TimelinePostDAL> TimelinePosts { get; set; }
        public DbSet<NotificationDAL> Notifications { get; set; }
        public DbSet<LikeDAL> Likes { get; set; }
        public DbSet<CommentDAL> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlServer"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.HasMany(d => d.SubscribedChallenges);
            }
            );

            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.Property(e => e.ChallengeId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TimelinePostDAL>(entity =>
            {
                entity.Property(e => e.TimelinePostId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<NotificationDAL>(entity =>
            {
                entity.Property(e => e.NotificationId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PushTokenDAL>(entity =>
            {
                entity.Property(e => e.PushTokenId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<LikeDAL>(entity =>
            {
                entity.Property(e => e.LikeId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CommentDAL>(entity =>
            {
                entity.Property(e => e.CommentId).ValueGeneratedOnAdd();
            });

        }
    }
}
