using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("role")]
        public string? Role { get; set; } = "User";

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
