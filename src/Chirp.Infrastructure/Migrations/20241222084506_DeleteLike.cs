using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CheepId",
                table: "Likes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Likes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "CheepId1",
                table: "Likes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CheepId1",
                table: "Likes",
                column: "CheepId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Cheeps_CheepId1",
                table: "Likes",
                column: "CheepId1",
                principalTable: "Cheeps",
                principalColumn: "CheepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Cheeps_CheepId1",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_CheepId1",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "CheepId1",
                table: "Likes");

            migrationBuilder.AlterColumn<string>(
                name: "CheepId",
                table: "Likes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Likes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
