using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMatchIdWithPuuidSquadMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "match_id",
                table: "squad_member",
                newName: "puuid");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "squad_member",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "squad_member");

            migrationBuilder.RenameColumn(
                name: "puuid",
                table: "squad_member",
                newName: "match_id");
        }
    }
}
