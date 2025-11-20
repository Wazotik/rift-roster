using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class TempRemoveMatchAggregatedStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchAggregatedStats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchAggregatedStats",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    stats_json = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchAggregatedStats", x => x.match_id);
                    table.ForeignKey(
                        name: "FK_MatchAggregatedStats_match_match_id",
                        column: x => x.match_id,
                        principalTable: "match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
