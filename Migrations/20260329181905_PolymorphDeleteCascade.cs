using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class PolymorphDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_tests_test_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_profile_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_comments_comment_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_tests_test_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_users_user_id",
                table: "reports");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_tests_test_id",
                table: "comments",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_profile_id",
                table: "comments",
                column: "user_profile_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_comments_comment_id",
                table: "reports",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_tests_test_id",
                table: "reports",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_users_user_id",
                table: "reports",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_tests_test_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_profile_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_comments_comment_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_tests_test_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_users_user_id",
                table: "reports");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_tests_test_id",
                table: "comments",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_profile_id",
                table: "comments",
                column: "user_profile_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reports_comments_comment_id",
                table: "reports",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reports_tests_test_id",
                table: "reports",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reports_users_user_id",
                table: "reports",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
