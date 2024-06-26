﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using learn.it.Models;

#nullable disable

namespace learn.it.Migrations
{
    [DbContext(typeof(LearnitDbContext))]
    partial class LearnitDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupUser", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.Achievement", b =>
                {
                    b.Property<int>("AchievementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AchievementId"));

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Predicate")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AchievementId")
                        .HasName("PK_achievements_achievement_id");

                    b.HasIndex(new[] { "Predicate" }, "predicate_UNIQUE")
                        .IsUnique()
                        .HasFilter("[Predicate] IS NOT NULL");

                    b.ToTable("Achievements", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AnswerId"));

                    b.Property<int>("AnswerTime")
                        .HasColumnType("int");

                    b.Property<DateTime>("AnswerTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FlashcardId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("AnswerId")
                        .HasName("PK_answers_answer_id");

                    b.HasIndex(new[] { "FlashcardId" }, "fk_answers_flashcards1_idx");

                    b.HasIndex(new[] { "UserId" }, "fk_answers_users1_idx");

                    b.ToTable("Answers", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.Flashcard", b =>
                {
                    b.Property<int>("FlashcardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FlashcardId"));

                    b.Property<string>("Definition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsTermText")
                        .HasColumnType("bit");

                    b.Property<int?>("StudySetId")
                        .HasColumnType("int");

                    b.Property<string>("Term")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FlashcardId")
                        .HasName("PK_flashcards_flashcard_id");

                    b.HasIndex(new[] { "StudySetId" }, "fk_flashcards_study_sets1_idx");

                    b.ToTable("Flashcards", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.FlashcardUserProgress", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("FlashcardId")
                        .HasColumnType("int");

                    b.Property<int>("ConsecutiveCorrectAnswers")
                        .HasColumnType("int");

                    b.Property<bool>("IsMastered")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("MasteredTimestamp")
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)");

                    b.Property<bool>("NeedsMoreRepetitions")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "FlashcardId")
                        .HasName("PK_flashcard_user_progress_user_id");

                    b.HasIndex(new[] { "FlashcardId" }, "fk_flashcard_user_progress_flashcards1_idx");

                    b.ToTable("FlashcardUserProgress", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GroupId")
                        .HasName("PK_groups_group_id");

                    b.HasIndex(new[] { "CreatorId" }, "fk_groups_users1_idx");

                    b.ToTable("Groups", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.GroupJoinRequest", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiresAt")
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)");

                    b.HasKey("UserId", "GroupId")
                        .HasName("PK_group_join_requests_user_id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupJoinRequest", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.Login", b =>
                {
                    b.Property<int>("LoginId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LoginId"));

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginId")
                        .HasName("PK_logins_login_id");

                    b.HasIndex(new[] { "UserId" }, "fk_logins_users_idx");

                    b.ToTable("Logins", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PermissionId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.HasKey("PermissionId")
                        .HasName("PK_permissions_permission_id");

                    b.ToTable("Permissions", "learnitdb");

                    b.HasData(
                        new
                        {
                            PermissionId = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            PermissionId = 2,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("learn.it.Models.StudySet", b =>
                {
                    b.Property<int>("StudySetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudySetId"));

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Visibility")
                        .HasColumnType("int");

                    b.HasKey("StudySetId")
                        .HasName("PK_study_sets_study_set_id");

                    b.HasIndex(new[] { "GroupId" }, "fk_study_sets_groups1_idx");

                    b.HasIndex(new[] { "CreatorId" }, "fk_study_sets_users1_idx");

                    b.ToTable("StudySets", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Avatar")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValueSql("(N'default.png')");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastLogin")
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId")
                        .HasName("PK_users_user_id");

                    b.HasIndex(new[] { "PermissionId" }, "fk_users_permissions1_idx");

                    b.HasIndex(new[] { "Email" }, "users$email_UNIQUE")
                        .IsUnique();

                    b.HasIndex(new[] { "Username" }, "users$username_UNIQUE")
                        .IsUnique();

                    b.ToTable("Users", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.UserAchievements", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("AchievementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(0)
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("UserId", "AchievementId")
                        .HasName("PK_user_achievements_user_id");

                    b.HasIndex(new[] { "AchievementId" }, "fk_user_achievements_achievements1");

                    b.HasIndex(new[] { "UserId" }, "fk_user_achievements_users1_idx");

                    b.ToTable("UserAchievements", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.UserPreferences", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("AutoTts")
                        .HasColumnType("bit");

                    b.Property<bool>("HighContrastMode")
                        .HasColumnType("bit");

                    b.HasKey("UserId")
                        .HasName("PK_user_preferences_user_id");

                    b.HasIndex(new[] { "UserId" }, "fk_user_preferences_users1_idx");

                    b.ToTable("UserPreferences", "learnitdb");
                });

            modelBuilder.Entity("learn.it.Models.UserStats", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ConsecutiveLoginDays")
                        .HasColumnType("int");

                    b.Property<int>("TotalFlashcardsAdded")
                        .HasColumnType("int");

                    b.Property<int>("TotalFlashcardsMastered")
                        .HasColumnType("int");

                    b.Property<int>("TotalLoginDays")
                        .HasColumnType("int");

                    b.Property<int>("TotalSetsAdded")
                        .HasColumnType("int");

                    b.Property<int>("TotalSetsMastered")
                        .HasColumnType("int");

                    b.HasKey("UserId")
                        .HasName("PK_user_stats_user_id");

                    b.HasIndex(new[] { "UserId" }, "fk_user_stats_users1_idx");

                    b.ToTable("UserStats", "learnitdb");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("learn.it.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("learn.it.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("learn.it.Models.Answer", b =>
                {
                    b.HasOne("learn.it.Models.Flashcard", "Flashcard")
                        .WithMany("Answers")
                        .HasForeignKey("FlashcardId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .HasConstraintName("answers$fk_answers_flashcards1");

                    b.HasOne("learn.it.Models.User", "User")
                        .WithMany("Answers")
                        .HasForeignKey("UserId")
                        .HasConstraintName("answers$fk_answers_users1");

                    b.Navigation("Flashcard");

                    b.Navigation("User");
                });

            modelBuilder.Entity("learn.it.Models.Flashcard", b =>
                {
                    b.HasOne("learn.it.Models.StudySet", "StudySet")
                        .WithMany("Flashcards")
                        .HasForeignKey("StudySetId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .HasConstraintName("flashcards$fk_flashcards_study_sets1");

                    b.Navigation("StudySet");
                });

            modelBuilder.Entity("learn.it.Models.FlashcardUserProgress", b =>
                {
                    b.HasOne("learn.it.Models.Flashcard", "Flashcard")
                        .WithMany("FlashcardUserProgress")
                        .HasForeignKey("FlashcardId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("flashcard_user_progress$fk_flashcard_user_progress_flashcards1");

                    b.HasOne("learn.it.Models.User", "User")
                        .WithMany("FlashcardUserProgress")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("flashcard_user_progress$fk_flashcard_user_progress_users1");

                    b.Navigation("Flashcard");

                    b.Navigation("User");
                });

            modelBuilder.Entity("learn.it.Models.Group", b =>
                {
                    b.HasOne("learn.it.Models.User", "Creator")
                        .WithMany("GroupCreator")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .HasConstraintName("groups$fk_groups_users1");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("learn.it.Models.GroupJoinRequest", b =>
                {
                    b.HasOne("learn.it.Models.User", "Creator")
                        .WithMany("GroupJoinRequests")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("group_join_requests$fk_group_join_requests_users1");

                    b.HasOne("learn.it.Models.Group", "Group")
                        .WithMany("GroupJoinRequests")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("group_join_requests$fk_group_join_requests_groups1");

                    b.Navigation("Creator");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("learn.it.Models.Login", b =>
                {
                    b.HasOne("learn.it.Models.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("logins$fk_logins_users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("learn.it.Models.StudySet", b =>
                {
                    b.HasOne("learn.it.Models.User", "Creator")
                        .WithMany("StudySets")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("study_sets$fk_study_sets_users1");

                    b.HasOne("learn.it.Models.Group", "Group")
                        .WithMany("StudySets")
                        .HasForeignKey("GroupId")
                        .HasConstraintName("study_sets$fk_study_sets_groups1");

                    b.Navigation("Creator");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("learn.it.Models.User", b =>
                {
                    b.HasOne("learn.it.Models.Permission", "Permissions")
                        .WithMany("Users")
                        .HasForeignKey("PermissionId")
                        .IsRequired()
                        .HasConstraintName("users$fk_users_permissions1");

                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("learn.it.Models.UserAchievements", b =>
                {
                    b.HasOne("learn.it.Models.Achievement", "Achievement")
                        .WithMany("UserAchievements")
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("user_achievements$fk_user_achievements_achievements1");

                    b.HasOne("learn.it.Models.User", "User")
                        .WithMany("UserAchievements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("user_achievements$fk_user_achievements_users1");

                    b.Navigation("Achievement");

                    b.Navigation("User");
                });

            modelBuilder.Entity("learn.it.Models.UserPreferences", b =>
                {
                    b.HasOne("learn.it.Models.User", "User")
                        .WithOne("UserPreferences")
                        .HasForeignKey("learn.it.Models.UserPreferences", "UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("user_preferences$fk_user_preferences_users1");

                    b.Navigation("User");
                });

            modelBuilder.Entity("learn.it.Models.UserStats", b =>
                {
                    b.HasOne("learn.it.Models.User", "User")
                        .WithOne("UserStats")
                        .HasForeignKey("learn.it.Models.UserStats", "UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("user_stats$fk_user_stats_users1");

                    b.Navigation("User");
                });

            modelBuilder.Entity("learn.it.Models.Achievement", b =>
                {
                    b.Navigation("UserAchievements");
                });

            modelBuilder.Entity("learn.it.Models.Flashcard", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("FlashcardUserProgress");
                });

            modelBuilder.Entity("learn.it.Models.Group", b =>
                {
                    b.Navigation("GroupJoinRequests");

                    b.Navigation("StudySets");
                });

            modelBuilder.Entity("learn.it.Models.Permission", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("learn.it.Models.StudySet", b =>
                {
                    b.Navigation("Flashcards");
                });

            modelBuilder.Entity("learn.it.Models.User", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("FlashcardUserProgress");

                    b.Navigation("GroupCreator");

                    b.Navigation("GroupJoinRequests");

                    b.Navigation("Logins");

                    b.Navigation("StudySets");

                    b.Navigation("UserAchievements");

                    b.Navigation("UserPreferences")
                        .IsRequired();

                    b.Navigation("UserStats")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
