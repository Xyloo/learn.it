using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace learn.it.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "learnitdb");

            migrationBuilder.CreateTable(
                name: "Achievements",
                schema: "learnitdb",
                columns: table => new
                {
                    AchievementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Predicate = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements_achievement_id", x => x.AchievementId);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "learnitdb",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions_permission_id", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "learnitdb",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    LastLogin = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValueSql: "(N'default.png')"),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_user_id", x => x.UserId);
                    table.ForeignKey(
                        name: "users$fk_users_permissions1",
                        column: x => x.PermissionId,
                        principalSchema: "learnitdb",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "learnitdb",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups_group_id", x => x.GroupId);
                    table.ForeignKey(
                        name: "groups$fk_groups_users1",
                        column: x => x.CreatorId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                schema: "learnitdb",
                columns: table => new
                {
                    LoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logins_login_id", x => x.LoginId);
                    table.ForeignKey(
                        name: "logins$fk_logins_users",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                schema: "learnitdb",
                columns: table => new
                {
                    AchievementId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements_user_id", x => new { x.UserId, x.AchievementId });
                    table.ForeignKey(
                        name: "user_achievements$fk_user_achievements_achievements1",
                        column: x => x.AchievementId,
                        principalSchema: "learnitdb",
                        principalTable: "Achievements",
                        principalColumn: "AchievementId");
                    table.ForeignKey(
                        name: "user_achievements$fk_user_achievements_users1",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                schema: "learnitdb",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HighContrastMode = table.Column<bool>(type: "bit", nullable: false),
                    AutoTts = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences_user_id", x => x.UserId);
                    table.ForeignKey(
                        name: "user_preferences$fk_user_preferences_users1",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserStats",
                schema: "learnitdb",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalSetsMastered = table.Column<int>(type: "int", nullable: false),
                    TotalLoginDays = table.Column<int>(type: "int", nullable: false),
                    TotalFlashcardsMastered = table.Column<int>(type: "int", nullable: false),
                    ConsecutiveLoginDays = table.Column<int>(type: "int", nullable: false),
                    TotalSetsAdded = table.Column<int>(type: "int", nullable: false),
                    TotalFlashcardsAdded = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats_user_id", x => x.UserId);
                    table.ForeignKey(
                        name: "user_stats$fk_user_stats_users1",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "GroupJoinRequest",
                schema: "learnitdb",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_join_requests_user_id", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "group_join_requests$fk_group_join_requests_groups1",
                        column: x => x.GroupId,
                        principalSchema: "learnitdb",
                        principalTable: "Groups",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "group_join_requests$fk_group_join_requests_users1",
                        column: x => x.CreatorId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "GroupUser",
                schema: "learnitdb",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupUser_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "learnitdb",
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUser_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySets",
                schema: "learnitdb",
                columns: table => new
                {
                    StudySetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_sets_study_set_id", x => x.StudySetId);
                    table.ForeignKey(
                        name: "study_sets$fk_study_sets_groups1",
                        column: x => x.GroupId,
                        principalSchema: "learnitdb",
                        principalTable: "Groups",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "study_sets$fk_study_sets_users1",
                        column: x => x.CreatorId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Flashcards",
                schema: "learnitdb",
                columns: table => new
                {
                    FlashcardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Term = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTermText = table.Column<bool>(type: "bit", nullable: false),
                    StudySetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flashcards_flashcard_id", x => x.FlashcardId);
                    table.ForeignKey(
                        name: "flashcards$fk_flashcards_study_sets1",
                        column: x => x.StudySetId,
                        principalSchema: "learnitdb",
                        principalTable: "StudySets",
                        principalColumn: "StudySetId");
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                schema: "learnitdb",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    AnswerTime = table.Column<int>(type: "int", nullable: false),
                    AnswerTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlashcardId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers_answer_id", x => x.AnswerId);
                    table.ForeignKey(
                        name: "answers$fk_answers_flashcards1",
                        column: x => x.FlashcardId,
                        principalSchema: "learnitdb",
                        principalTable: "Flashcards",
                        principalColumn: "FlashcardId");
                    table.ForeignKey(
                        name: "answers$fk_answers_users1",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "FlashcardUserProgress",
                schema: "learnitdb",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FlashcardId = table.Column<int>(type: "int", nullable: false),
                    ConsecutiveCorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    IsMastered = table.Column<bool>(type: "bit", nullable: false),
                    MasteredTimestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    NeedsMoreRepetitions = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flashcard_user_progress_user_id", x => new { x.UserId, x.FlashcardId });
                    table.ForeignKey(
                        name: "flashcard_user_progress$fk_flashcard_user_progress_flashcards1",
                        column: x => x.FlashcardId,
                        principalSchema: "learnitdb",
                        principalTable: "Flashcards",
                        principalColumn: "FlashcardId");
                    table.ForeignKey(
                        name: "flashcard_user_progress$fk_flashcard_user_progress_users1",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "Permissions",
                columns: new[] { "PermissionId", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "Users",
                columns: new[]
                {
                    "UserId", "Username", "Email", "Password", "CreateTime", "LastLogin", "Avatar", "PermissionId"
                },
                values: new object[,]
                {
                    { -1, "testAdmin", "test@admin.com", "AQAAAAIAAYagAAAAEHgdRpl0GvK6IgRIWMN1SJfmJ2yirgRsimXNDeQx0LuSyPTqEQRenosehwnQfQSpGA==", DateTime.UtcNow, null, null, 1 }
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "UserStats",
                columns: new[]
                {
                    "UserId", "TotalSetsMastered", "TotalLoginDays", "TotalFlashcardsMastered",
                    "ConsecutiveLoginDays", "TotalSetsAdded", "TotalFlashcardsAdded"
                },
                values: new object[,]
                {
                    { -1, 0, 0, 0, 0, 0, 0 }
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "UserPreferences",
                columns: new[]
                    { "UserId", "HighContrastMode", "AutoTts" },
                values: new object[,]
                {
                    { -1, false, false }
                });

            migrationBuilder.CreateIndex(
                name: "predicate_UNIQUE",
                schema: "learnitdb",
                table: "Achievements",
                column: "Predicate",
                unique: true,
                filter: "[Predicate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "fk_answers_flashcards1_idx",
                schema: "learnitdb",
                table: "Answers",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "fk_answers_users1_idx",
                schema: "learnitdb",
                table: "Answers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_flashcards_study_sets1_idx",
                schema: "learnitdb",
                table: "Flashcards",
                column: "StudySetId");

            migrationBuilder.CreateIndex(
                name: "fk_flashcard_user_progress_flashcards1_idx",
                schema: "learnitdb",
                table: "FlashcardUserProgress",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupJoinRequest_CreatorId",
                schema: "learnitdb",
                table: "GroupJoinRequest",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupJoinRequest_GroupId",
                schema: "learnitdb",
                table: "GroupJoinRequest",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "fk_groups_users1_idx",
                schema: "learnitdb",
                table: "Groups",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_UserId",
                schema: "learnitdb",
                table: "GroupUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_logins_users_idx",
                schema: "learnitdb",
                table: "Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_study_sets_groups1_idx",
                schema: "learnitdb",
                table: "StudySets",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "fk_study_sets_users1_idx",
                schema: "learnitdb",
                table: "StudySets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "fk_user_achievements_achievements1",
                schema: "learnitdb",
                table: "UserAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "fk_user_achievements_users1_idx",
                schema: "learnitdb",
                table: "UserAchievements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_user_preferences_users1_idx",
                schema: "learnitdb",
                table: "UserPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_users_permissions1_idx",
                schema: "learnitdb",
                table: "Users",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "users$email_UNIQUE",
                schema: "learnitdb",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users$username_UNIQUE",
                schema: "learnitdb",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_user_stats_users1_idx",
                schema: "learnitdb",
                table: "UserStats",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "FlashcardUserProgress",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "GroupJoinRequest",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "GroupUser",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "Logins",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "UserAchievements",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "UserPreferences",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "UserStats",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "Flashcards",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "Achievements",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "StudySets",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "learnitdb");
        }
    }
}
