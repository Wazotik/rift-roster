using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("participant")]
    [PrimaryKey(nameof(MatchId), nameof(ParticipantId))]
    [Index(nameof(MatchId), nameof(Puuid), IsUnique = true)]
    public class Participant
    {
        [Column("match_id")]
        public string MatchId { get; set; } = null!;

        [Column("participant_id")]
        public int ParticipantId { get; set; }

        [Column("puuid")]
        public string Puuid { get; set; } = null!;

        [Column("team_id")]
        public int TeamId { get; set; }

        [Column("team_position")]
        public string? TeamPosition { get; set; }

        [Column("champion_id")]
        public int? ChampionId { get; set; }

        [Column("kills")]
        public int Kills { get; set; }

        [Column("deaths")]
        public int Deaths { get; set; }

        [Column("assists")]
        public int Assists { get; set; }

        [Column("win")]
        public bool Win { get; set; }

        [Column("participants_json")]
        public string ParticipantsJson { get; set; } = "[]";

    }
}
