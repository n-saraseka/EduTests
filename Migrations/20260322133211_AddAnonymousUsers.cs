using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymousUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_test_completions_users_user_id",
                table: "test_completions");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "test_completions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "anonymous_user_id",
                table: "test_completions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "anonymous_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_anonymous_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_test_completions_anonymous_user_id",
                table: "test_completions",
                column: "anonymous_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_test_completions_anonymous_users_anonymous_user_id",
                table: "test_completions",
                column: "anonymous_user_id",
                principalTable: "anonymous_users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_test_completions_users_user_id",
                table: "test_completions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_test_completions_anonymous_users_anonymous_user_id",
                table: "test_completions");

            migrationBuilder.DropForeignKey(
                name: "fk_test_completions_users_user_id",
                table: "test_completions");

            migrationBuilder.DropTable(
                name: "anonymous_users");

            migrationBuilder.DropIndex(
                name: "ix_test_completions_anonymous_user_id",
                table: "test_completions");

            migrationBuilder.DropColumn(
                name: "anonymous_user_id",
                table: "test_completions");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "test_completions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_test_completions_users_user_id",
                table: "test_completions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
