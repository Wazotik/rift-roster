using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteForSquadAndTimelineTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "iconUrl",
                table: "squad",
                newName: "icon_url");

            migrationBuilder.AddForeignKey(
                name: "FK_match_timeline_match_match_id",
                table: "match_timeline",
                column: "match_id",
                principalTable: "match",
                principalColumn: "match_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_squad_match_squad_squad_id",
                table: "squad_match",
                column: "squad_id",
                principalTable: "squad",
                principalColumn: "squad_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_squad_member_squad_squad_id",
                table: "squad_member",
                column: "squad_id",
                principalTable: "squad",
                principalColumn: "squad_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_timeline_match_match_id",
                table: "match_timeline");

            migrationBuilder.DropForeignKey(
                name: "FK_squad_match_squad_squad_id",
                table: "squad_match");

            migrationBuilder.DropForeignKey(
                name: "FK_squad_member_squad_squad_id",
                table: "squad_member");

            migrationBuilder.RenameColumn(
                name: "icon_url",
                table: "squad",
                newName: "iconUrl");
        }
    }
}
