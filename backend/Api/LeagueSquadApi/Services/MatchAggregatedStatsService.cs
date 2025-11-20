using System.Text.Json;
using System.Text.Json.Nodes;
using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services
{
    public class MatchAggregatedStatsService : IMatchAggregatedStatsService
    {
        private readonly AppDbContext db;
        private static readonly JsonSerializerOptions camelCase = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public MatchAggregatedStatsService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ServiceResult<MatchAggregatedStatsResponse>> GetAsync(
            string matchId,
            IParticipantService ps,
            CancellationToken ct
        )
        {
            var existingStats = await db.MatchAggregatedStats.FindAsync(matchId, ct);
            if (existingStats != null)
            {
                return ServiceResult<MatchAggregatedStatsResponse>.Ok(
                    new MatchAggregatedStatsResponse(
                        existingStats.MatchId,
                        existingStats.StatsJson,
                        existingStats.CreatedAt
                    )
                );
            }

            Console.WriteLine("getting aggregated stats");
            // Get all match participants
            var participants = (await ps.GetAllAsync(matchId, ct)).Value;
            if (participants == null)
                return ServiceResult<MatchAggregatedStatsResponse>.Fail(ResultStatus.NotFound);

            var participantsStatsStr = participants.Select(p => p.ParticipantsJson).ToList();

            List<RiotRawParticipantForStats> participantsStatsRaw = new();
            foreach (var jsonStr in participantsStatsStr)
            {
                var participant = JsonSerializer.Deserialize<RiotRawParticipantForStats>(
                    jsonStr,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (participant != null)
                    participantsStatsRaw.Add(participant);
            }

            Console.WriteLine("raw stats:");

            foreach (var p in participantsStatsRaw)
            {
                Console.WriteLine("new player");
                Console.WriteLine(p.Kills);
                Console.WriteLine(p.GoldEarned);
                Console.WriteLine(p.TotalMinionsKilled);
                Console.WriteLine(p.Challenges.DragonTakedowns);
                Console.WriteLine(p.Challenges.RiftHeraldTakedowns);
                Console.WriteLine(p.Challenges.BaronTakedowns);
            }

            // Calculate total kills and total gold earned per team
            var teamKillsCount = new Dictionary<int, int>();
            var teamGoldEarnedCount = new Dictionary<int, int>();
            var teamObjectivesCount = new Dictionary<int, int>();
            var teamDragonsCount = new Dictionary<int, int>();
            var teamBaronsCount = new Dictionary<int, int>();
            var teamHeraldCount = new Dictionary<int, int>();

            foreach (var p in participantsStatsRaw)
            {
                if (!teamKillsCount.ContainsKey(p.TeamId))
                {
                    teamKillsCount.Add(p.TeamId, p.Kills);
                }
                else
                {
                    teamKillsCount[p.TeamId] = teamKillsCount[p.TeamId] + p.Kills;
                }

                if (!teamGoldEarnedCount.ContainsKey(p.TeamId))
                {
                    teamGoldEarnedCount[p.TeamId] = p.GoldEarned;
                }
                else
                {
                    teamGoldEarnedCount[p.TeamId] = teamGoldEarnedCount[p.TeamId] + p.GoldEarned;
                }

                if (!teamDragonsCount.ContainsKey(p.TeamId))
                {
                    teamDragonsCount[p.TeamId] = p.Challenges.DragonTakedowns;
                }
                else
                {
                    teamDragonsCount[p.TeamId] =
                        teamDragonsCount[p.TeamId] + p.Challenges.DragonTakedowns;
                }

                if (!teamBaronsCount.ContainsKey(p.TeamId))
                {
                    teamBaronsCount[p.TeamId] = p.Challenges.BaronTakedowns;
                }
                else
                {
                    teamBaronsCount[p.TeamId] =
                        teamBaronsCount[p.TeamId] + p.Challenges.BaronTakedowns;
                }

                if (!teamHeraldCount.ContainsKey(p.TeamId))
                {
                    teamHeraldCount[p.TeamId] = p.Challenges.RiftHeraldTakedowns;
                }
                else
                {
                    teamHeraldCount[p.TeamId] =
                        teamBaronsCount[p.TeamId] + p.Challenges.RiftHeraldTakedowns;
                }

                if (!teamObjectivesCount.ContainsKey(p.TeamId))
                {
                    teamObjectivesCount[p.TeamId] = (
                        p.Challenges.DragonTakedowns
                        + p.Challenges.BaronTakedowns
                        + p.Challenges.DragonTakedowns
                    );
                }
                else
                {
                    teamObjectivesCount[p.TeamId] =
                        teamObjectivesCount[p.TeamId]
                        + (
                            p.Challenges.DragonTakedowns
                            + p.Challenges.BaronTakedowns
                            + p.Challenges.DragonTakedowns
                        );
                }
            }

            foreach (var p in participantsStatsRaw) { }

            // Calculate team kill share and team gold share per participant
            // Calculate total CS and CS/min
            foreach (var p in participantsStatsRaw)
            {
                if (teamKillsCount.ContainsKey(p.TeamId) && teamKillsCount[p.TeamId] > 0)
                {
                    p.KillShare = p.Kills / teamKillsCount[p.TeamId];
                }

                if (teamGoldEarnedCount.ContainsKey(p.TeamId) && teamGoldEarnedCount[p.TeamId] > 0)
                {
                    p.GoldShare = p.GoldEarned / teamGoldEarnedCount[p.TeamId];
                }

                p.TotalCs = p.TotalMinionsKilled + p.NeutralMinionsKilled;

                var timePlayedMinutes = p.TimePlayed / 60;
                if (timePlayedMinutes > 0)
                {
                    p.CsPerMinute = p.TotalCs / timePlayedMinutes;
                }
                else
                {
                    p.CsPerMinute = 0;
                }

                if (p.TotalDamageDealtToChampions > 0)
                {
                    p.DamageTakenToDealtRatio = p.TotalDamageTaken / p.TotalDamageDealtToChampions;
                }
                else
                {
                    p.DamageTakenToDealtRatio = 0;
                }

                if (teamObjectivesCount.ContainsKey(p.TeamId) && teamObjectivesCount[p.TeamId] > 0)
                {
                    p.ObjectiveParticipation =
                        (
                            p.Challenges.DragonTakedowns
                            + p.Challenges.BaronTakedowns
                            + p.Challenges.RiftHeraldTakedowns
                        ) / teamObjectivesCount[p.TeamId];
                }
                else
                {
                    p.ObjectiveParticipation = 0;
                }
            }

            var root = new JsonObject { ["matchId"] = matchId };

            var participantsStatsArr =
                JsonSerializer.SerializeToNode(participantsStatsRaw, camelCase) as JsonArray;

            root["participantsStats"] = participantsStatsArr;

            var statsJson = root.ToJsonString();

            Console.WriteLine("count check");

            foreach (KeyValuePair<int, int> pair in teamObjectivesCount)
            {
                Console.WriteLine($"team id {pair.Key}");
                Console.WriteLine($"obj count {pair.Value}");
            }

            foreach (KeyValuePair<int, int> pair in teamDragonsCount)
            {
                Console.WriteLine($"team id {pair.Key}");
                Console.WriteLine($"dragons count {pair.Value}");
            }

            Console.WriteLine(statsJson);

            // add to db
            MatchAggregatedStats mas = new MatchAggregatedStats()
            {
                MatchId = matchId,
                StatsJson = statsJson,
            };
            await db.MatchAggregatedStats.AddAsync(mas, ct);
            await db.SaveChangesAsync(ct);

            return ServiceResult<MatchAggregatedStatsResponse>.Ok(
                new MatchAggregatedStatsResponse(mas.MatchId, mas.StatsJson, mas.CreatedAt)
            );
        }
    }
}
