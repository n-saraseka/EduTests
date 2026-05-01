using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class AddShowAnswersFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "show_correct_answers",
                table: "tests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "show_correct_answers",
                table: "tests");
        }
    }
}
