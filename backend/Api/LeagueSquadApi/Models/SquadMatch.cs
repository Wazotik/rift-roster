using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Models
{
    [Table("sqaud_match")]
    [PrimaryKey(nameof(SquadId), nameof(MatchId))]
    public class SquadMatch
    {
        [Column("squad_id")]
        public long SquadId { get; set; }

        [Column("match_id")]
        public string MatchId { get; set; } = null!;

        [Column("reason_for_addition")]
        public string? ReasonForAddition { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
