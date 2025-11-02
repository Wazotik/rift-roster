using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Models
{
    [Table("squad_member")]
    [PrimaryKey(nameof(SquadId), nameof(MatchId))]
    public class SquadMember
    {
        [Column("squad_id")]
        public long SquadId { get; set; }

        [Column("match_id")]
        public string MatchId { get; set; } = null!;

        [Column("alias")]
        public string? Alias { get; set; } = null!;

        [Column("added_at")]
        public DateTimeOffset AddedAt { get; set; }
    }
}
