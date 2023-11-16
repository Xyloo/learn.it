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
                name: "achievements",
                schema: "learnitdb",
                columns: table => new
                {
                    achievement_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements_achievement_id", x => x.achievement_id);
                });

            migrationBuilder.CreateTable(
                name: "GroupUser",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => new { x.GroupId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "learnitdb",
                columns: table => new
                {
                    permission_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions_permission_id", x => x.permission_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "learnitdb",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    last_login = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValueSql: "(N'default.png')"),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_user_id", x => x.user_id);
                    table.ForeignKey(
                        name: "users$fk_users_permissions1",
                        column: x => x.PermissionId,
                        principalSchema: "learnitdb",
                        principalTable: "permissions",
                        principalColumn: "permission_id");
                });

            migrationBuilder.CreateTable(
                name: "groups",
                schema: "learnitdb",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups_group_id", x => x.group_id);
                    table.ForeignKey(
                        name: "groups$fk_groups_users1",
                        column: x => x.CreatorId,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "logins",
                schema: "learnitdb",
                columns: table => new
                {
                    login_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    is_successful = table.Column<bool>(type: "bit", nullable: false),
                    user_agent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logins_login_id", x => x.login_id);
                    table.ForeignKey(
                        name: "logins$fk_logins_users",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                schema: "learnitdb",
                columns: table => new
                {
                    achievement_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements_user_id", x => new { x.user_id, x.achievement_id });
                    table.ForeignKey(
                        name: "user_achievements$fk_user_achievements_achievements1",
                        column: x => x.achievement_id,
                        principalSchema: "learnitdb",
                        principalTable: "achievements",
                        principalColumn: "achievement_id");
                    table.ForeignKey(
                        name: "user_achievements$fk_user_achievements_users1",
                        column: x => x.user_id,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                schema: "learnitdb",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    high_contrast_mode = table.Column<short>(type: "smallint", nullable: false),
                    email_reminders = table.Column<short>(type: "smallint", nullable: false),
                    auto_tts = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences_user_id", x => x.user_id);
                    table.ForeignKey(
                        name: "user_preferences$fk_user_preferences_users1",
                        column: x => x.user_id,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                schema: "learnitdb",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    sets_completed = table.Column<int>(type: "int", nullable: false),
                    total_login_days = table.Column<int>(type: "int", nullable: false),
                    total_flashcards_mastered = table.Column<int>(type: "int", nullable: false),
                    consecutive_login_days = table.Column<int>(type: "int", nullable: false),
                    sets_added = table.Column<int>(type: "int", nullable: false),
                    flashcards_added = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats_user_id", x => x.user_id);
                    table.ForeignKey(
                        name: "user_stats$fk_user_stats_users1",
                        column: x => x.user_id,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "group_join_request",
                schema: "learnitdb",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_join_requests_user_id", x => new { x.user_id, x.group_id });
                    table.ForeignKey(
                        name: "group_join_requests$fk_group_join_requests_groups1",
                        column: x => x.group_id,
                        principalSchema: "learnitdb",
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "group_join_requests$fk_group_join_requests_users1",
                        column: x => x.CreatorId,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "study_sets",
                schema: "learnitdb",
                columns: table => new
                {
                    study_set_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    visibility = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_sets_study_set_id", x => x.study_set_id);
                    table.ForeignKey(
                        name: "study_sets$fk_study_sets_groups1",
                        column: x => x.GroupId,
                        principalSchema: "learnitdb",
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "study_sets$fk_study_sets_users1",
                        column: x => x.CreatorId,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "users_has_groups",
                schema: "learnitdb",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    group_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_has_groups_user_id", x => new { x.user_id, x.group_id });
                    table.ForeignKey(
                        name: "users_has_groups$fk_users_has_groups_groups1",
                        column: x => x.group_id,
                        principalSchema: "learnitdb",
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "users_has_groups$fk_users_has_groups_users1",
                        column: x => x.user_id,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "flashcards",
                schema: "learnitdb",
                columns: table => new
                {
                    flashcard_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    term = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    definition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    is_term_text = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "((1))"),
                    StudySetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flashcards_flashcard_id", x => x.flashcard_id);
                    table.ForeignKey(
                        name: "flashcards$fk_flashcards_study_sets1",
                        column: x => x.StudySetId,
                        principalSchema: "learnitdb",
                        principalTable: "study_sets",
                        principalColumn: "study_set_id");
                });

            migrationBuilder.CreateTable(
                name: "answers",
                schema: "learnitdb",
                columns: table => new
                {
                    answer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_correct = table.Column<short>(type: "smallint", nullable: false),
                    answer_time = table.Column<int>(type: "int", nullable: false),
                    FlashcardId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers_answer_id", x => x.answer_id);
                    table.ForeignKey(
                        name: "answers$fk_answers_flashcards1",
                        column: x => x.FlashcardId,
                        principalSchema: "learnitdb",
                        principalTable: "flashcards",
                        principalColumn: "flashcard_id");
                    table.ForeignKey(
                        name: "answers$fk_answers_users1",
                        column: x => x.UserId,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "flashcard_user_progress",
                schema: "learnitdb",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    flashcard_id = table.Column<int>(type: "int", nullable: false),
                    consecutive_correct_answers = table.Column<int>(type: "int", nullable: false),
                    is_mastered = table.Column<short>(type: "smallint", nullable: false),
                    mastered_timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    needs_more_repetitions = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flashcard_user_progress_user_id", x => new { x.user_id, x.flashcard_id });
                    table.ForeignKey(
                        name: "flashcard_user_progress$fk_flashcard_user_progress_flashcards1",
                        column: x => x.flashcard_id,
                        principalSchema: "learnitdb",
                        principalTable: "flashcards",
                        principalColumn: "flashcard_id");
                    table.ForeignKey(
                        name: "flashcard_user_progress$fk_flashcard_user_progress_users1",
                        column: x => x.user_id,
                        principalSchema: "learnitdb",
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "permissions",
                columns: new[] { "permission_id", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });
            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "users",
                columns: new[]
                {
                    "user_id", "username", "email", "password", "create_time", "last_login", "avatar", "PermissionId"
                },
                values: new object[,]
                {
                    { -1, "testAdmin", "test@admin.com", "AQAAAAIAAYagAAAAEHgdRpl0GvK6IgRIWMN1SJfmJ2yirgRsimXNDeQx0LuSyPTqEQRenosehwnQfQSpGA==", DateTime.UtcNow, null, null, 1 }
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "user_stats",
                columns: new[]
                {
                    "user_id", "sets_completed", "total_login_days", "total_flashcards_mastered",
                    "consecutive_login_days", "sets_added", "flashcards_added"
                },
                values: new object[,]
                {
                    { -1, 0, 0, 0, 0, 0, 0 }
                });

            migrationBuilder.InsertData(
                schema: "learnitdb",
                table: "user_preferences",
                columns: new[]
                    { "user_id", "high_contrast_mode", "email_reminders", "auto_tts" },
                values: new object[,]
                {
                    { -1, 0, 0, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "fk_answers_flashcards1_idx",
                schema: "learnitdb",
                table: "answers",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "fk_answers_users1_idx",
                schema: "learnitdb",
                table: "answers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_flashcard_user_progress_flashcards1_idx",
                schema: "learnitdb",
                table: "flashcard_user_progress",
                column: "flashcard_id");

            migrationBuilder.CreateIndex(
                name: "fk_flashcards_study_sets1_idx",
                schema: "learnitdb",
                table: "flashcards",
                column: "StudySetId");

            migrationBuilder.CreateIndex(
                name: "IX_group_join_request_CreatorId",
                schema: "learnitdb",
                table: "group_join_request",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_group_join_request_group_id",
                schema: "learnitdb",
                table: "group_join_request",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "fk_groups_users1_idx",
                schema: "learnitdb",
                table: "groups",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "fk_logins_users_idx",
                schema: "learnitdb",
                table: "logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "fk_study_sets_groups1_idx",
                schema: "learnitdb",
                table: "study_sets",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "fk_study_sets_users1_idx",
                schema: "learnitdb",
                table: "study_sets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "fk_user_achievements_achievements1",
                schema: "learnitdb",
                table: "user_achievements",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "fk_user_achievements_users1_idx",
                schema: "learnitdb",
                table: "user_achievements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_user_preferences_users1_idx",
                schema: "learnitdb",
                table: "user_preferences",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_user_stats_users1_idx",
                schema: "learnitdb",
                table: "user_stats",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_users_permissions1_idx",
                schema: "learnitdb",
                table: "users",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "users$email_UNIQUE",
                schema: "learnitdb",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users$username_UNIQUE",
                schema: "learnitdb",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_users_has_groups_groups1_idx",
                schema: "learnitdb",
                table: "users_has_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "fk_users_has_groups_users1_idx",
                schema: "learnitdb",
                table: "users_has_groups",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answers",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "flashcard_user_progress",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "group_join_request",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "GroupUser");

            migrationBuilder.DropTable(
                name: "logins",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "user_achievements",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "user_preferences",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "user_stats",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "users_has_groups",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "flashcards",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "achievements",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "study_sets",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "groups",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "users",
                schema: "learnitdb");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "learnitdb");
        }
    }
}
