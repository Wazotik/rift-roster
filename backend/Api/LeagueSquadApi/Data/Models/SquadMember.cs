using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("squad_member")]
    [PrimaryKey(nameof(SquadId), nameof(Puuid))]
    public class SquadMember
    {
        [Column("squad_id")]
        public long SquadId { get; set; }

        [Column("puuid")]
        public string Puuid { get; set; } = null!;

        [Column("role")]
        public string? Role { get; set; }

        [Column("alias")]
        public string? Alias { get; set; }

        [Column("added_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
