using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTestCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completion_percentage",
                table: "test_completions");

            migrationBuilder.DropColumn(
                name: "completion_time",
                table: "test_completions");

            migrationBuilder.DropColumn(
                name: "correct_answers",
                table: "test_completions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "completed_at",
                table: "test_completions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "started_at",
                table: "test_completions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "started_at",
                table: "test_completions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "completed_at",
                table: "test_completions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "completion_percentage",
                table: "test_completions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "completion_time",
                table: "test_completions",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "correct_answers",
                table: "test_completions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
