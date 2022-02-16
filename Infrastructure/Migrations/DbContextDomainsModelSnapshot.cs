﻿// <auto-generated />
using System;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DbContextDomains))]
    partial class DbContextDomainsModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domains.Activity", b =>
                {
                    b.Property<string>("ActivityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoryId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChallengeType")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Location")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VideoLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("ActivityId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("Domains.ActivityCategory", b =>
                {
                    b.Property<string>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ImageLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("CategoryId");

                    b.ToTable("ActivityCategories");
                });

            modelBuilder.Entity("Domains.Challenge", b =>
                {
                    b.Property<string>("ChallengeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChallengeType")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Location")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("VideoLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("ChallengeId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("Domains.Comment", b =>
                {
                    b.Property<string>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Text")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TimelinePostId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CommentId");

                    b.HasIndex("TimelinePostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Domains.Follower", b =>
                {
                    b.Property<string>("FollowerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FollowedUserUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserFollower")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FollowerId");

                    b.HasIndex("FollowedUserUserId");

                    b.ToTable("Follower");
                });

            modelBuilder.Entity("Domains.Like", b =>
                {
                    b.Property<string>("LikeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TimelinePostId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LikeId");

                    b.HasIndex("TimelinePostId");

                    b.HasIndex("UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("Domains.Notification", b =>
                {
                    b.Property<string>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChallengeId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("NotificationType")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeOfNotification")
                        .HasColumnType("datetime2");

                    b.Property<string>("TimelinePostId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ToUserUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserSenderId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("NotificationId");

                    b.HasIndex("ToUserUserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Domains.PushToken", b =>
                {
                    b.Property<string>("PushTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DeviceType")
                        .HasColumnType("int");

                    b.Property<bool>("NotificationEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Token")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PushTokenId");

                    b.HasIndex("UserId");

                    b.ToTable("PushTokens");
                });

            modelBuilder.Entity("Domains.SubscribedChallenge", b =>
                {
                    b.Property<string>("SubscribedChallengeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ActivityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChallengeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChallengeProgress")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("SubscribedChallengeId");

                    b.HasIndex("ActivityId");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("UserId");

                    b.ToTable("SubscribedChallenge");
                });

            modelBuilder.Entity("Domains.TimelinePost", b =>
                {
                    b.Property<string>("TimelinePostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Image")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Video")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("TimelinePostId");

                    b.HasIndex("UserId");

                    b.ToTable("TimelinePosts");
                });

            modelBuilder.Entity("Domains.User", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Firstname")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("HasAdminPriviledges")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("JobTitle")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Lastname")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("Location")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<string>("ProfilePicture")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domains.UserRecoveryToken", b =>
                {
                    b.Property<string>("RecoveryTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimeCreated")
                        .HasMaxLength(100)
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RecoveryTokenId");

                    b.ToTable("RecoveryTokens");
                });

            modelBuilder.Entity("Domains.Activity", b =>
                {
                    b.HasOne("Domains.ActivityCategory", "Category")
                        .WithMany("Activities")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domains.Comment", b =>
                {
                    b.HasOne("Domains.TimelinePost", "TimelinePost")
                        .WithMany("Comments")
                        .HasForeignKey("TimelinePostId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Domains.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("TimelinePost");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domains.Follower", b =>
                {
                    b.HasOne("Domains.User", "FollowedUser")
                        .WithMany("Followers")
                        .HasForeignKey("FollowedUserUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("FollowedUser");
                });

            modelBuilder.Entity("Domains.Like", b =>
                {
                    b.HasOne("Domains.TimelinePost", "TimelinePost")
                        .WithMany("Likes")
                        .HasForeignKey("TimelinePostId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Domains.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("TimelinePost");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domains.Notification", b =>
                {
                    b.HasOne("Domains.User", "ToUser")
                        .WithMany("Notifications")
                        .HasForeignKey("ToUserUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ToUser");
                });

            modelBuilder.Entity("Domains.PushToken", b =>
                {
                    b.HasOne("Domains.User", "User")
                        .WithMany("PushTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domains.SubscribedChallenge", b =>
                {
                    b.HasOne("Domains.Activity", null)
                        .WithMany("SubscribedChallenges")
                        .HasForeignKey("ActivityId");

                    b.HasOne("Domains.Challenge", "Challenge")
                        .WithMany("SubscribedChallenges")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domains.User", "User")
                        .WithMany("SubscribedChallenges")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Challenge");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domains.TimelinePost", b =>
                {
                    b.HasOne("Domains.User", "User")
                        .WithMany("TimelinePosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domains.Activity", b =>
                {
                    b.Navigation("SubscribedChallenges");
                });

            modelBuilder.Entity("Domains.ActivityCategory", b =>
                {
                    b.Navigation("Activities");
                });

            modelBuilder.Entity("Domains.Challenge", b =>
                {
                    b.Navigation("SubscribedChallenges");
                });

            modelBuilder.Entity("Domains.TimelinePost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Domains.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Followers");

                    b.Navigation("Likes");

                    b.Navigation("Notifications");

                    b.Navigation("PushTokens");

                    b.Navigation("SubscribedChallenges");

                    b.Navigation("TimelinePosts");
                });
#pragma warning restore 612, 618
        }
    }
}
