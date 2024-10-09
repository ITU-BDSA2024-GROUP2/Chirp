using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class Cheeps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "cheeps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_cheeps_AuthorId",
                table: "cheeps",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_cheeps_authors_AuthorId",
                table: "cheeps",
                column: "AuthorId",
                principalTable: "authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cheeps_authors_AuthorId",
                table: "cheeps");

            migrationBuilder.DropIndex(
                name: "IX_cheeps_AuthorId",
                table: "cheeps");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "cheeps");
        }
    }
}
