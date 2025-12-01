using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyConstraintForSquadStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_squad_aggregated_stats_squad_id",
                table: "squad_aggregated_stats",
                column: "squad_id");

            migrationBuilder.AddForeignKey(
                name: "FK_squad_aggregated_stats_squad_squad_id",
                table: "squad_aggregated_stats",
                column: "squad_id",
                principalTable: "squad",
                principalColumn: "squad_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_squad_aggregated_stats_squad_squad_id",
                table: "squad_aggregated_stats");

            migrationBuilder.DropIndex(
                name: "IX_squad_aggregated_stats_squad_id",
                table: "squad_aggregated_stats");
        }
    }
}
