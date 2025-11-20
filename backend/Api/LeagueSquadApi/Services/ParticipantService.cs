using LeagueSquadApi.Data;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeagueSquadApi.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly AppDbContext db;

        public ParticipantService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ServiceResult<List<ParticipantResponse>>> GetAllAsync(
            string matchId,
            CancellationToken ct
        )
        {
            var participants = await db
                .Participant.Where(p => p.MatchId == matchId)
                .Select(p => new ParticipantResponse(
                    p.MatchId,
                    p.ParticipantId,
                    p.Puuid,
                    p.TeamId,
                    p.TeamPosition,
                    p.ChampionId,
                    p.Kills,
                    p.Deaths,
                    p.Assists,
                    p.Win,
                    p.ParticipantsJson
                ))
                .ToListAsync(ct);
            if (!participants.Any() || participants == null)
                return ServiceResult<List<ParticipantResponse>>.Fail(ResultStatus.NotFound);
            return ServiceResult<List<ParticipantResponse>>.Ok(participants);
        }
    }
}
