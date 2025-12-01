using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.Json;
using System.Text.Json.Serialization;
using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services
{
    public record MatchAggregatedStatsRaw(
        string MatchId,
        [property: JsonPropertyName("participantsStats")]
        List<RiotRawParticipantForStats> ParticipantsStats
    );

    public class SquadAggregatedStatsService : ISquadAggregatedStatsService
    {
        private readonly AppDbContext db;

        public SquadAggregatedStatsService(AppDbContext db)
        {
            this.db = db;
        }

        /*
            For this to work:
                - squad matches should not be 0,
                - 
        */
        public async Task<ServiceResult<List<SquadAggregatedStats>>> GetAsync(long squadId, IMatchAggregatedStatsService mas, IParticipantService ps, CancellationToken ct)
        {

            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try
            {
                List<SquadAggregatedStats> squadStats = new();

                // Get all Squad Members
                var squadMembers = await db.SquadMember.Where(s => s.SquadId == squadId).ToListAsync(ct);
                var squadMembersId = squadMembers.Select(sm => sm.Puuid).ToList();

                if (!squadMembers.Any()) return ServiceResult<List<SquadAggregatedStats>>.Fail(ResultStatus.NotFound, "No Squad Members");

                // Get all Squad Matches
                var squadMatches = await db.SquadMatch.Where(s => s.SquadId == squadId).ToListAsync(ct);

                if (!squadMatches.Any()) return ServiceResult<List<SquadAggregatedStats>>.Fail(ResultStatus.NotFound, "No Squad Matches");

                // Get squad member stats for each squad match
                var participantStatsJson = new List<string>();
                var matchAndSquadMembersStatsDict = new Dictionary<string, List<RiotRawParticipantForStats>>();


                foreach (var squadMatch in squadMatches)
                {
                    var matchAggregatedStats = (await mas.GetAsync(squadMatch.MatchId, ps, ct)).Value;
                    var matchStatsJson = matchAggregatedStats.StatsJson;
                    var fullMatchStats = JsonSerializer.Deserialize<MatchAggregatedStatsRaw>(matchStatsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var participantStats = fullMatchStats?.ParticipantsStats;
                    var squadMembersStatsList = participantStats.Where(p => squadMembersId.Contains(p.Puuid)).ToList();
                    matchAndSquadMembersStatsDict.Add(squadMatch.MatchId, squadMembersStatsList);
                }

                // squad Stats (kills, deaths, gold, etc.)
                SquadAggregatedStats mostKillsStat = new();
                SquadAggregatedStats mostDeathsStat = new();
                SquadAggregatedStats mostAssistsStat = new();
                SquadAggregatedStats leastDeathsStat = new();
                SquadAggregatedStats mostGoldStat = new();
                SquadAggregatedStats mostChampionDamageStat = new();
                SquadAggregatedStats highestKillParticipationStat = new();
                SquadAggregatedStats mostCsStat = new();
                SquadAggregatedStats mostVisionScoreStat = new();
                SquadAggregatedStats mostWardsPlacedStat = new();
                SquadAggregatedStats mostWardsDestroyedStat = new();
                SquadAggregatedStats mostDamageTakenStat = new();
                SquadAggregatedStats bestKdaStat = new();
                SquadAggregatedStats mostDamageShareStat = new();
                SquadAggregatedStats mostDmgToBuildingsStat = new(); // turrets and inhibs

                var squadMemberKillsTotalDict = new Dictionary<string, int>();
                var squadMemberDeathTotalDict = new Dictionary<string, int>();
                var squadMemberAsisstsTotalDict = new Dictionary<string, int>();
                var squadMemberGoldTotalDict = new Dictionary<string, int>();
                var squadMemberChampionDmgTotalDict = new Dictionary<string, int>();
                var squadMemberKillParticipationTotalDict = new Dictionary<string, double>();
                var squadMemberKdaDict = new Dictionary<string, double>();
                var squadMemberCsPerMinTotalDict = new Dictionary<string, double>();
                var squadMemberVisionScoreTotalDict = new Dictionary<string, int>();
                var squadMemberWardsPlacedTotalDict = new Dictionary<string, int>();
                var squadMemberWardsDestroyedTotalDict = new Dictionary<string, int>();
                var squadMemberDamageTakenTotalDict = new Dictionary<string, double>();
                var squadMemberDamageShareTotalDict = new Dictionary<string, double>();
                var squadMemberDmgToBuildingsTotalDict = new Dictionary<string, int>();


                foreach (var squadMember in squadMembers)
                {
                    var deathTotal = 0;
                    var killsTotal = 0;
                    var assistsTotal = 0;
                    var goldTotal = 0;
                    var championDmgTotal = 0;
                    var killParticipationTotal = 0.00;
                    var csPerMinTotal = 0.00;
                    var visionScoreTotal = 0;
                    var wardsPlacedTotal = 0;
                    var wardsDestroyedTotal = 0;
                    var dmgTakenTotal = 0.00;
                    var dmgShareTotal = 0.00;
                    var dmgBuildingTotal = 0;

                    foreach (var pair in matchAndSquadMembersStatsDict)
                    {
                        var currSquadMemberStats = pair.Value.Find(p => p.Puuid == squadMember.Puuid);
                        if (currSquadMemberStats == null) return ServiceResult<List<SquadAggregatedStats>>.Fail(ResultStatus.NotFound, $"no stats for squad member: {squadMember.Puuid}");
                        deathTotal += currSquadMemberStats.Deaths;
                        killsTotal += currSquadMemberStats.Kills;
                        goldTotal += currSquadMemberStats.GoldEarned;
                        championDmgTotal += currSquadMemberStats.TotalDamageDealtToChampions;
                        killParticipationTotal += currSquadMemberStats.Challenges.KillParticipation;
                        csPerMinTotal += currSquadMemberStats.CsPerMinute;
                        visionScoreTotal += currSquadMemberStats.VisionScore;
                        wardsPlacedTotal += currSquadMemberStats.WardsPlaced;
                        wardsDestroyedTotal += currSquadMemberStats.WardsKilled;
                        dmgTakenTotal += currSquadMemberStats.Challenges.DamageTakenOnTeamPercentage;
                        dmgShareTotal += currSquadMemberStats.Challenges.TeamDamagePercentage;
                        dmgBuildingTotal += currSquadMemberStats.DamageDealtToBuildings;
                    }

                    squadMemberKillsTotalDict.Add(squadMember.Puuid, killsTotal);
                    squadMemberDeathTotalDict.Add(squadMember.Puuid, deathTotal);
                    squadMemberAsisstsTotalDict.Add(squadMember.Puuid, assistsTotal);
                    squadMemberGoldTotalDict.Add(squadMember.Puuid, goldTotal);
                    squadMemberChampionDmgTotalDict.Add(squadMember.Puuid, championDmgTotal);
                    //squadMember
                    squadMemberKdaDict.Add(
                        squadMember.Puuid,
                        (squadMemberKillsTotalDict[squadMember.Puuid] + squadMemberAsisstsTotalDict[squadMember.Puuid]) / (Math.Max(1, squadMemberDeathTotalDict[squadMember.Puuid]))
                    );
                    squadMemberCsPerMinTotalDict.Add(squadMember.Puuid, csPerMinTotal);
                    squadMemberVisionScoreTotalDict.Add(squadMember.Puuid, visionScoreTotal);
                    squadMemberWardsPlacedTotalDict.Add(squadMember.Puuid, wardsPlacedTotal);
                    squadMemberWardsDestroyedTotalDict.Add(squadMember.Puuid, wardsDestroyedTotal);
                    squadMemberDamageTakenTotalDict.Add(squadMember.Puuid, dmgTakenTotal);
                    squadMemberDamageShareTotalDict.Add(squadMember.Puuid, dmgShareTotal);
                    squadMemberDmgToBuildingsTotalDict.Add(squadMember.Puuid, dmgBuildingTotal);
                    squadMemberKillParticipationTotalDict.Add(squadMember.Puuid, killParticipationTotal);
                }

                var topKda = -1.00;
                var topKdaHaver = "";
                foreach (var pair in squadMemberKdaDict)
                {
                    var currKda = pair.Value;
                    if (currKda > topKda)
                    {
                        topKda = currKda;
                        topKdaHaver = pair.Key;
                    }
                }

                var topDmgToBuildingsTotal = -1;
                var topDmgToBuildingsTotalHaver = "";
                foreach (var pair in squadMemberDmgToBuildingsTotalDict)
                {
                    var currDmgToBuildingsTotal = pair.Value;
                    if (currDmgToBuildingsTotal > topDmgToBuildingsTotal)
                    {
                        topDmgToBuildingsTotal = currDmgToBuildingsTotal;
                        topDmgToBuildingsTotalHaver = pair.Key;
                    }
                }

                var topDmgShareTotal = -1.00;
                var topDmgShareHaver = "";
                foreach (var pair in squadMemberDamageShareTotalDict)
                {
                    var currDmgShareTotal = pair.Value;
                    if (currDmgShareTotal > topDmgShareTotal)
                    {
                        topDmgShareTotal = currDmgShareTotal;
                        topDmgShareHaver = pair.Key;
                    }
                }

                var topDmgTakenTotal = -1.00;
                var topDmgTakenTotalHaver = "";
                foreach (var pair in squadMemberDamageTakenTotalDict)
                {
                    var currDmgTakenTotal = pair.Value;
                    if (currDmgTakenTotal > topDmgTakenTotal)
                    {
                        topDmgTakenTotal = currDmgTakenTotal;
                        topDmgTakenTotalHaver = pair.Key;
                    }
                }

                var topVisionScoreTotal = -1;
                var topVisionScoreTotalHaver = "";
                foreach (var pair in squadMemberVisionScoreTotalDict)
                {
                    var currVisionScoreTotal = pair.Value;
                    if (currVisionScoreTotal > topVisionScoreTotal)
                    {
                        topVisionScoreTotal = currVisionScoreTotal;
                        topVisionScoreTotalHaver = pair.Key;
                    }
                }

                var topWardsPlacedTotal = -1;
                var topWardsPlacedTotalHaver = "";
                foreach (var pair in squadMemberWardsPlacedTotalDict)
                {
                    var currWardsPlacedTotal = pair.Value;
                    if (currWardsPlacedTotal > topWardsPlacedTotal)
                    {
                        topWardsPlacedTotal = currWardsPlacedTotal;
                        topWardsPlacedTotalHaver = pair.Key;
                    }
                }

                var topWardsDestroyedTotal = -1;
                var topWardsDestroyedTotalHaver = "";
                foreach (var pair in squadMemberWardsDestroyedTotalDict)
                {
                    var currWardsDestroyedTotal = pair.Value;
                    if (currWardsDestroyedTotal > topWardsDestroyedTotal)
                    {
                        topWardsDestroyedTotal = currWardsDestroyedTotal;
                        topWardsDestroyedTotalHaver = pair.Key;
                    }
                }

                var topCsPerMinTotal = -1.00;
                var topCsPerMinTotalHaver = "";
                foreach (var pair in squadMemberCsPerMinTotalDict)
                {
                    var currCsPerMinTotal = pair.Value;
                    if (currCsPerMinTotal > topCsPerMinTotal)
                    {
                        topCsPerMinTotal = currCsPerMinTotal;
                        topCsPerMinTotalHaver = pair.Key;
                    }
                }

                var topKills = -1;
                var topKillsHaver = "";
                foreach (var pair in squadMemberKillsTotalDict)
                {
                    var currKills = pair.Value;
                    if (currKills > topKills)
                    {
                        topKills = currKills;
                        topKillsHaver = pair.Key;
                    }
                }

                var topAssistsTotal = -1;
                var topAssistsTotalHaver = "";
                foreach (var pair in squadMemberAsisstsTotalDict)
                {
                    var currAssistsTotal = pair.Value;
                    if (currAssistsTotal > topAssistsTotal)
                    {
                        topAssistsTotal = currAssistsTotal;
                        topAssistsTotalHaver = pair.Key;
                    }
                }

                var topDeaths = -1;
                var topDeathsHaver = "";
                var leastDeaths = int.MaxValue;
                var leastDeathsHaver = "";
                foreach (var pair in squadMemberDeathTotalDict)
                {
                    var currDeaths = pair.Value;
                    if (currDeaths > topDeaths)
                    {
                        topDeaths = currDeaths;
                        topDeathsHaver = pair.Key;
                    }
                    if (currDeaths < leastDeaths)
                    {
                        leastDeaths = currDeaths;
                        leastDeathsHaver = pair.Key;
                    }
                }

                var topGold = -1;
                var topGoldHaver = "";
                foreach (var pair in squadMemberChampionDmgTotalDict)
                {
                    var currGold = pair.Value;
                    if (currGold > topGold)
                    {
                        topGold = currGold;
                        topGoldHaver = pair.Key;
                    }
                }

                var topChampionDmg = -1;
                var topChampionDmgHaver = "";
                foreach (var pair in squadMemberChampionDmgTotalDict)
                {
                    var currChampionDmg = pair.Value;
                    if (currChampionDmg > topChampionDmg)
                    {
                        topChampionDmg = currChampionDmg;
                        topChampionDmgHaver = pair.Key;
                    }
                }

                var topKp = -1.00;
                var topKpHaver = "";
                foreach (var pair in squadMemberKillParticipationTotalDict)
                {
                    var currKp = pair.Value;
                    if (currKp > topKp)
                    {
                        topKp = currKp;
                        topKpHaver = pair.Key;
                    }
                }

                var highestAvgKda = -1.00;
                var highestAvgKdaHaver = "";
                foreach (var pair in squadMemberKdaDict)
                {
                    var currAvgKda = pair.Value;
                    if (currAvgKda > highestAvgKda)
                    {
                        highestAvgKda = currAvgKda;
                        highestAvgKdaHaver = pair.Key;
                    }
                }

                bestKdaStat.SquadId = squadMembers[0].SquadId;
                bestKdaStat.StatName = "highestAggegatedKda";
                bestKdaStat.StatValue = topKda;
                bestKdaStat.StatSquadMember = topKdaHaver;

                mostAssistsStat.SquadId = squadMembers[0].SquadId;
                mostAssistsStat.StatName = "highestAvgAssists";
                mostAssistsStat.StatValue = (topAssistsTotal / squadMatches.Count);
                mostAssistsStat.StatSquadMember = topAssistsTotalHaver;

                mostDmgToBuildingsStat.SquadId = squadMembers[0].SquadId;
                mostDmgToBuildingsStat.StatName = "highestAvgDmgToBuildings";
                mostDmgToBuildingsStat.StatValue = (topDmgToBuildingsTotal / squadMatches.Count);
                mostDmgToBuildingsStat.StatSquadMember = topDmgToBuildingsTotalHaver;

                mostDamageShareStat.SquadId = squadMembers[0].SquadId;
                mostDamageShareStat.StatName = "highestAvgDmgShare";
                mostDamageShareStat.StatValue = (topDmgShareTotal / squadMatches.Count);
                mostDamageShareStat.StatSquadMember = topDmgShareHaver;

                mostDamageTakenStat.SquadId = squadMembers[0].SquadId;
                mostDamageTakenStat.StatName = "highestAvgDmgTakenShare";
                mostDamageTakenStat.StatValue = (topDmgTakenTotal / squadMatches.Count);
                mostDamageTakenStat.StatSquadMember = topDmgTakenTotalHaver;

                mostVisionScoreStat.SquadId = squadMembers[0].SquadId;
                mostVisionScoreStat.StatName = "highestAvgVisionScore";
                mostVisionScoreStat.StatValue = (topVisionScoreTotal / squadMatches.Count);
                mostVisionScoreStat.StatSquadMember = topVisionScoreTotalHaver;

                mostWardsPlacedStat.SquadId = squadMembers[0].SquadId;
                mostWardsPlacedStat.StatName = "highestAvgWardsPlaced";
                mostWardsPlacedStat.StatValue = (topWardsPlacedTotal / squadMatches.Count);
                mostWardsPlacedStat.StatSquadMember = topWardsPlacedTotalHaver;

                mostWardsDestroyedStat.SquadId = squadMembers[0].SquadId;
                mostWardsDestroyedStat.StatName = "highestAvgWardsDestroyed";
                mostWardsDestroyedStat.StatValue = (topWardsDestroyedTotal / squadMatches.Count);
                mostWardsDestroyedStat.StatSquadMember = topWardsDestroyedTotalHaver;

                mostCsStat.SquadId = squadMembers[0].SquadId;
                mostCsStat.StatName = "highestAvgCsPerMin";
                mostCsStat.StatValue = (topCsPerMinTotal / squadMatches.Count);
                mostCsStat.StatSquadMember = topCsPerMinTotalHaver;

                mostGoldStat.SquadId = squadMembers[0].SquadId;
                mostGoldStat.StatName = "topEarner";
                mostGoldStat.StatValue = (topGold / squadMatches.Count); // average gold in n squad games
                mostGoldStat.StatSquadMember = topGoldHaver; // this is puuid

                mostChampionDamageStat.SquadId = squadMembers[0].SquadId;
                mostChampionDamageStat.StatName = "highestAvgChampionDamage";
                mostChampionDamageStat.StatValue = (topChampionDmg / squadMatches.Count); // avg
                mostChampionDamageStat.StatSquadMember = topChampionDmgHaver;

                mostDeathsStat.SquadId = squadMembers[0].SquadId;
                mostDeathsStat.StatName = "highestAvgDeaths";
                mostDeathsStat.StatValue = (topDeaths / squadMatches.Count);
                mostDeathsStat.StatSquadMember = topDeathsHaver;

                leastDeathsStat.SquadId = squadMembers[0].SquadId;
                leastDeathsStat.StatName = "leastAvgDeaths";
                leastDeathsStat.StatValue = (leastDeaths / squadMatches.Count);
                leastDeathsStat.StatSquadMember = leastDeathsHaver;

                mostKillsStat.SquadId = squadMembers[0].SquadId;
                mostKillsStat.StatName = "highestAvgKills";
                mostKillsStat.StatValue = (topKills / squadMatches.Count);
                mostKillsStat.StatSquadMember = topKillsHaver;

                highestKillParticipationStat.SquadId = squadMembers[0].SquadId;
                highestKillParticipationStat.StatName = "bestAvgKillParticipation";
                highestKillParticipationStat.StatValue = (topKp / squadMatches.Count); // average 
                highestKillParticipationStat.StatSquadMember = topKpHaver;

                squadStats.Add(mostKillsStat);
                squadStats.Add(mostDeathsStat);
                squadStats.Add(mostAssistsStat);
                squadStats.Add(leastDeathsStat);
                squadStats.Add(mostGoldStat);
                squadStats.Add(mostChampionDamageStat);
                squadStats.Add(highestKillParticipationStat);
                squadStats.Add(mostCsStat);
                squadStats.Add(mostVisionScoreStat);
                squadStats.Add(mostWardsPlacedStat);
                squadStats.Add(mostWardsDestroyedStat);
                squadStats.Add(mostDamageTakenStat);
                squadStats.Add(bestKdaStat);
                squadStats.Add(mostDamageShareStat);
                squadStats.Add(mostDmgToBuildingsStat);


                await db.SquadAggregatedStats.Where(sas => sas.SquadId == squadMembers[0].SquadId).ExecuteDeleteAsync(ct);
                await db.SquadAggregatedStats.AddRangeAsync(squadStats, ct);

                //Console.WriteLine($"best gold total {topGold}");
                //Console.WriteLine($"best gold person {squadMembers.Find(sm => sm.Puuid == topGoldHaver).Puuid}");

                //Console.WriteLine($"best kills total {topKills}");
                //Console.WriteLine($"best kills person {squadMembers.Find(sm => sm.Puuid == topKillsHaver)}");

                //Console.WriteLine($"most deaths total {topDeaths}");
                //Console.WriteLine($"most deaths person {squadMembers.Find(sm => sm.Puuid == topDeathsHaver)}");

                //Console.WriteLine($"least deaths total {leastDeaths}");
                //Console.WriteLine($"least deaths person {squadMembers.Find(sm => sm.Puuid == leastDeathsHaver)}");

                //Console.WriteLine($"highest total dmg to champions {topChampionDmg}");
                //Console.WriteLine($"highest dmg to champions person {squadMembers.Find(sm => sm.Puuid == topChampionDmgHaver)}");

                //Console.WriteLine($"highest total CS {topCsTotal}");
                //Console.WriteLine($"highest CS person {squadMembers.Find(sm => sm.Puuid == topCsTotalHaver)}");

                //Console.WriteLine($"highest total vision score {topVisionScoreTotal}");
                //Console.WriteLine($"highest vision score person {squadMembers.Find(sm => sm.Puuid == topVisionScoreTotalHaver)}");

                //Console.WriteLine($"most wards placed total {topWardsPlacedTotal}");
                //Console.WriteLine($"most wards placed person {squadMembers.Find(sm => sm.Puuid == topWardsPlacedTotalHaver)}");

                //Console.WriteLine($"most wards destroyed total {topWardsDestroyedTotal}");
                //Console.WriteLine($"most wards destroyed person {squadMembers.Find(sm => sm.Puuid == topWardsDestroyedTotalHaver)}");

                //Console.WriteLine($"highest total damage taken share {topDmgTakenTotal}");
                //Console.WriteLine($"highest damage taken share person {squadMembers.Find(sm => sm.Puuid == topDmgTakenTotalHaver)}");

                //Console.WriteLine($"best total kill participation {topKp}");
                //Console.WriteLine($"best kill participation person {squadMembers.Find(sm => sm.Puuid == topKpHaver)}");

                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ServiceResult<List<SquadAggregatedStats>>.Ok(squadStats);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await tx.RollbackAsync(ct);
                return ServiceResult<List<SquadAggregatedStats>>.Fail(ResultStatus.Unknown, ex.Message);
            }
        }
    }
}
