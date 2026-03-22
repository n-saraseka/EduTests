using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class LinkReportsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "reporting_anonymous_user_id",
                table: "reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reporting_user_id",
                table: "reports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_reports_reporting_anonymous_user_id",
                table: "reports",
                column: "reporting_anonymous_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_reporting_user_id",
                table: "reports",
                column: "reporting_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reports_anonymous_users_reporting_anonymous_user_id",
                table: "reports",
                column: "reporting_anonymous_user_id",
                principalTable: "anonymous_users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reports_users_reporting_user_id",
                table: "reports",
                column: "reporting_user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reports_anonymous_users_reporting_anonymous_user_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_users_reporting_user_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_reports_reporting_anonymous_user_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_reports_reporting_user_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "reporting_anonymous_user_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "reporting_user_id",
                table: "reports");
        }
    }
}
