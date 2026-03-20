using System;
using EduTests.Database.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class initial_create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:AccessType", "private,public,unlisted")
                .Annotation("Npgsql:Enum:QuestionType", "match_pairs,multiple_choice,number_input,sequence,single_choice,text_input")
                .Annotation("Npgsql:Enum:ReportStatus", "accepted,pending,rejected")
                .Annotation("Npgsql:Enum:UserGroup", "administrator,moderator,user");

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    group = table.Column<UserGroup>(type: "\"UserGroup\"", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "banned_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_banned_id = table.Column<int>(type: "integer", nullable: false),
                    banned_by_id = table.Column<int>(type: "integer", nullable: false),
                    ban_reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    date_banned = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_unbanned = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_banned_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_banned_users_users_banned_by_id",
                        column: x => x.banned_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_banned_users_users_user_banned_id",
                        column: x => x.user_banned_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    thumbnail_url = table.Column<string>(type: "text", nullable: true),
                    access_type = table.Column<AccessType>(type: "\"AccessType\"", nullable: false),
                    password = table.Column<string>(type: "text", nullable: true),
                    attempt_limit = table.Column<int>(type: "integer", nullable: true),
                    time_limit = table.Column<TimeSpan>(type: "interval", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tests", x => x.id);
                    table.CheckConstraint("attempt_limit_check", "attempt_limit IS NULL OR (attempt_limit >= 0 AND attempt_limit <= 10)");
                    table.CheckConstraint("time_limit_check", "time_limit IS NULL OR (time_limit >= '0 hours'::interval AND time_limit <= '5 hours'::interval)");
                    table.ForeignKey(
                        name: "fk_tests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    commenter_id = table.Column<int>(type: "integer", nullable: false),
                    user_profile_id = table.Column<int>(type: "integer", nullable: true),
                    test_id = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.CheckConstraint("SingleTargetCheckComment", "(test_id IS NULL AND user_profile_id IS NOT NULL) OR (test_id IS NOT NULL AND user_profile_id IS NULL)");
                    table.ForeignKey(
                        name: "fk_comments_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_comments_users_commenter_id",
                        column: x => x.commenter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_users_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<QuestionType>(type: "\"QuestionType\"", nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    correct_data = table.Column<string>(type: "jsonb", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.CheckConstraint("order_index_check", "order_index >= 1");
                    table.ForeignKey(
                        name: "fk_questions_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tag_test",
                columns: table => new
                {
                    tags_id = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag_test", x => new { x.tags_id, x.test_id });
                    table.ForeignKey(
                        name: "fk_tag_test_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tag_test_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_completions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    completion_percentage = table.Column<float>(type: "real", nullable: false),
                    completion_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_completions", x => x.id);
                    table.CheckConstraint("answer_count_check", "correct_answers >= 0");
                    table.CheckConstraint("completion_percentage_check", "completion_percentage >= 0 AND completion_percentage <= 100");
                    table.ForeignKey(
                        name: "fk_test_completions_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_test_completions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    percentage_threshold = table.Column<float>(type: "real", nullable: false),
                    result = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_results", x => x.id);
                    table.CheckConstraint("threshold_check", "percentage_threshold >= 0 AND percentage_threshold <= 100");
                    table.ForeignKey(
                        name: "fk_test_results_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_rating",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    is_positive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_rating", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_rating_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_rating_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    test_id = table.Column<int>(type: "integer", nullable: true),
                    comment_id = table.Column<int>(type: "integer", nullable: true),
                    text = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    report_status = table.Column<ReportStatus>(type: "\"ReportStatus\"", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reports", x => x.id);
                    table.CheckConstraint("SingleTargetCheckReport", "(test_id IS NOT NULL AND user_id IS NULL AND comment_id IS NULL) OR (test_id IS NULL AND user_id IS NOT NULL AND comment_id IS NULL) OR (test_id IS NULL AND user_id IS NULL AND comment_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_reports_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reports_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reports_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_answers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    test_completion_id = table.Column<int>(type: "integer", nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    answers = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_answers_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_answers_test_completions_test_completion_id",
                        column: x => x.test_completion_id,
                        principalTable: "test_completions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_banned_users_banned_by_id",
                table: "banned_users",
                column: "banned_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_banned_users_user_banned_id",
                table: "banned_users",
                column: "user_banned_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_commenter_id",
                table: "comments",
                column: "commenter_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_test_id",
                table: "comments",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_profile_id",
                table: "comments",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_test_id",
                table: "questions",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_comment_id",
                table: "reports",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_test_id",
                table: "reports",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_user_id",
                table: "reports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_test_test_id",
                table: "tag_test",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_test_completions_test_id",
                table: "test_completions",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_completions_user_id",
                table: "test_completions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_results_test_id",
                table: "test_results",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_tests_description",
                table: "tests",
                column: "description");

            migrationBuilder.CreateIndex(
                name: "ix_tests_name",
                table: "tests",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_tests_user_id",
                table: "tests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_question_id",
                table: "user_answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_test_completion_id_question_id",
                table: "user_answers",
                columns: new[] { "test_completion_id", "question_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_rating_test_id_is_positive",
                table: "user_rating",
                columns: new[] { "test_id", "is_positive" });

            migrationBuilder.CreateIndex(
                name: "ix_user_rating_test_id_user_id",
                table: "user_rating",
                columns: new[] { "test_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_rating_user_id",
                table: "user_rating",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_login",
                table: "users",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banned_users");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "tag_test");

            migrationBuilder.DropTable(
                name: "test_results");

            migrationBuilder.DropTable(
                name: "user_answers");

            migrationBuilder.DropTable(
                name: "user_rating");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "test_completions");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
