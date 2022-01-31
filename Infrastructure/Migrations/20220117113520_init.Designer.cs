﻿// <auto-generated />
using System;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DbContextDomains))]
    [Migration("20220117113520_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domains.Challenge", b =>
                {
                    b.Property<string>("ChallengeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChallengeType")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VideoLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChallengeId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("Domains.DAL.CommentDAL", b =>
                {
                    b.Property<string>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimelinePostId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CommentId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Domains.DAL.LikeDAL", b =>
                {
                    b.Property<string>("LikeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TimelinePostId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LikeId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("Domains.DAL.NotificationDAL", b =>
                {
                    b.Property<string>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChallengeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NotificationType")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeOfNotification")
                        .HasColumnType("datetime2");

                    b.Property<string>("TimelinePostId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ToUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NotificationId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Domains.DAL.PushTokenDAL", b =>
                {
                    b.Property<string>("PushTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DeviceType")
                        .HasColumnType("int");

                    b.Property<bool>("NotificationEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PushTokenId");

                    b.ToTable("PushTokens");
                });

            modelBuilder.Entity("Domains.DAL.TimelinePostDAL", b =>
                {
                    b.Property<string>("TimelinePostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Video")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TimelinePostId");

                    b.ToTable("TimelinePosts");
                });

            modelBuilder.Entity("Domains.Follower", b =>
                {
                    b.Property<string>("FollowerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserFollowerId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FollowerId");

                    b.HasIndex("UserId");

                    b.ToTable("Follower");
                });

            modelBuilder.Entity("Domains.SubscribedChallenge", b =>
                {
                    b.Property<string>("SubscribedChallengeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChallengeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChallengeProgress")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("SubscribedChallengeId");

                    b.HasIndex("UserId");

                    b.ToTable("SubscribedChallenge");
                });

            modelBuilder.Entity("Domains.User", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Firstname")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("HasAdminPriviledges")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("JobTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lastname")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domains.Follower", b =>
                {
                    b.HasOne("Domains.User", null)
                        .WithMany("Followers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Domains.SubscribedChallenge", b =>
                {
                    b.HasOne("Domains.User", null)
                        .WithMany("SubscribedChallenges")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Domains.User", b =>
                {
                    b.Navigation("Followers");

                    b.Navigation("SubscribedChallenges");
                });
#pragma warning restore 612, 618
        }
    }
}
