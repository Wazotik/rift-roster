using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LeagueSquadApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    patch = table.Column<string>(type: "text", nullable: true),
                    queue = table.Column<int>(type: "integer", nullable: true),
                    has_timeline = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match", x => x.match_id);
                });

            migrationBuilder.CreateTable(
                name: "match_timeline",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    fetched_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    frame_interval_ms = table.Column<int>(type: "integer", nullable: true),
                    timeline_json = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_timeline", x => x.match_id);
                });

            migrationBuilder.CreateTable(
                name: "participant",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    participant_id = table.Column<int>(type: "integer", nullable: false),
                    puuid = table.Column<string>(type: "text", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    team_position = table.Column<string>(type: "text", nullable: true),
                    champion_id = table.Column<int>(type: "integer", nullable: true),
                    kills = table.Column<int>(type: "integer", nullable: false),
                    deaths = table.Column<int>(type: "integer", nullable: false),
                    assists = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participant", x => new { x.match_id, x.participant_id });
                });

            migrationBuilder.CreateTable(
                name: "player",
                columns: table => new
                {
                    puuid = table.Column<string>(type: "text", nullable: false),
                    game_name = table.Column<string>(type: "text", nullable: false),
                    tag_line = table.Column<string>(type: "text", nullable: false),
                    region = table.Column<string>(type: "text", nullable: true),
                    platform = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.puuid);
                });

            migrationBuilder.CreateTable(
                name: "sqaud_match",
                columns: table => new
                {
                    squad_id = table.Column<long>(type: "bigint", nullable: false),
                    match_id = table.Column<string>(type: "text", nullable: false),
                    reason_for_addition = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sqaud_match", x => new { x.squad_id, x.match_id });
                });

            migrationBuilder.CreateTable(
                name: "squad",
                columns: table => new
                {
                    squad_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_squad", x => x.squad_id);
                });

            migrationBuilder.CreateTable(
                name: "squad_member",
                columns: table => new
                {
                    squad_id = table.Column<long>(type: "bigint", nullable: false),
                    match_id = table.Column<string>(type: "text", nullable: false),
                    alias = table.Column<string>(type: "text", nullable: true),
                    added_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_squad_member", x => new { x.squad_id, x.match_id });
                });

            migrationBuilder.CreateIndex(
                name: "IX_participant_match_id_puuid",
                table: "participant",
                columns: new[] { "match_id", "puuid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match");

            migrationBuilder.DropTable(
                name: "match_timeline");

            migrationBuilder.DropTable(
                name: "participant");

            migrationBuilder.DropTable(
                name: "player");

            migrationBuilder.DropTable(
                name: "sqaud_match");

            migrationBuilder.DropTable(
                name: "squad");

            migrationBuilder.DropTable(
                name: "squad_member");
        }
    }
}
