using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTests.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tag_test_tests_test_id",
                table: "tag_test");

            migrationBuilder.RenameColumn(
                name: "test_id",
                table: "tag_test",
                newName: "tests_id");

            migrationBuilder.RenameIndex(
                name: "ix_tag_test_test_id",
                table: "tag_test",
                newName: "ix_tag_test_tests_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tag_test_tests_tests_id",
                table: "tag_test",
                column: "tests_id",
                principalTable: "tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tag_test_tests_tests_id",
                table: "tag_test");

            migrationBuilder.RenameColumn(
                name: "tests_id",
                table: "tag_test",
                newName: "test_id");

            migrationBuilder.RenameIndex(
                name: "ix_tag_test_tests_id",
                table: "tag_test",
                newName: "ix_tag_test_test_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tag_test_tests_test_id",
                table: "tag_test",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
