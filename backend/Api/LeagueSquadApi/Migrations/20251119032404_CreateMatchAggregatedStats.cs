using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateMatchAggregatedStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "team_position",
                table: "participant",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "champion_id",
                table: "participant",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MatchAggregatedStats",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    stats_json = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchAggregatedStats");

            migrationBuilder.AlterColumn<string>(
                name: "team_position",
                table: "participant",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "champion_id",
                table: "participant",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
