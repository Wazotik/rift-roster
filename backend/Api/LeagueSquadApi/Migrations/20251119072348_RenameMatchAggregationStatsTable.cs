using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameMatchAggregationStatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchAggregatedStats_match_match_id",
                table: "MatchAggregatedStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchAggregatedStats",
                table: "MatchAggregatedStats");

            migrationBuilder.RenameTable(
                name: "MatchAggregatedStats",
                newName: "match_aggregated_stats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_match_aggregated_stats",
                table: "match_aggregated_stats",
                column: "match_id");

            migrationBuilder.AddForeignKey(
                name: "FK_match_aggregated_stats_match_match_id",
                table: "match_aggregated_stats",
                column: "match_id",
                principalTable: "match",
                principalColumn: "match_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_aggregated_stats_match_match_id",
                table: "match_aggregated_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_match_aggregated_stats",
                table: "match_aggregated_stats");

            migrationBuilder.RenameTable(
                name: "match_aggregated_stats",
                newName: "MatchAggregatedStats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchAggregatedStats",
                table: "MatchAggregatedStats",
                column: "match_id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchAggregatedStats_match_match_id",
                table: "MatchAggregatedStats",
                column: "match_id",
                principalTable: "match",
                principalColumn: "match_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
