using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class LimitAuthorInfoStringLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cheeps_authors_AuthorId",
                table: "cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cheeps",
                table: "cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_authors",
                table: "authors");

            migrationBuilder.RenameTable(
                name: "cheeps",
                newName: "Cheeps");

            migrationBuilder.RenameTable(
                name: "authors",
                newName: "Authors");

            migrationBuilder.RenameIndex(
                name: "IX_cheeps_AuthorId",
                table: "Cheeps",
                newName: "IX_Cheeps_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cheeps",
                table: "Cheeps",
                column: "CheepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cheeps",
                table: "Cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.RenameTable(
                name: "Cheeps",
                newName: "cheeps");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "authors");

            migrationBuilder.RenameIndex(
                name: "IX_Cheeps_AuthorId",
                table: "cheeps",
                newName: "IX_cheeps_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cheeps",
                table: "cheeps",
                column: "CheepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_authors",
                table: "authors",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_cheeps_authors_AuthorId",
                table: "cheeps",
                column: "AuthorId",
                principalTable: "authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
