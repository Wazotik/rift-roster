using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services.Interfaces;

namespace LeagueSquadApi.Endpoints
{
    public static class Squads
    {
        public static void RegisterSquadEndpoints(this IEndpointRouteBuilder routes)
        {
            var squads = routes.MapGroup("/squads");

            // Create a squad
            squads.MapPost(
                "",
                async (SquadRequest req, ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.AddAsync(req, ct);
                    return ResultStatusToIResultMapper<SquadResponse>.ToHttp(
                        res,
                        $"/squads/{res?.Value?.Id}"
                    );
                }
            );

            // Get squad using id
            squads.MapGet(
                "/{id}",
                async (long id, ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.GetAsync(id, ct);
                    return ResultStatusToIResultMapper<SquadResponse>.ToHttp(res);
                }
            );

            // Get all squads
            squads.MapGet(
                "",
                async (ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.GetAllAsync(ct);
                    return ResultStatusToIResultMapper<List<SquadResponse>>.ToHttp(res);
                }
            );

            // Update a squads details
            squads.MapPut(
                "/{id}",
                async (long id, ISquadService ss, SquadRequest req, CancellationToken ct) =>
                {
                    var res = await ss.UpdateAsync(id, req, ct);
                    return ResultStatusToIResultMapper<SquadResponse>.ToHttp(res);
                }
            );

            // Delete a squad
            squads.MapDelete(
                "/{id}",
                async (long id, ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.DeleteAsync(id, ct);
                    return ResultStatusToIResultMapper.ToHttp(res);
                }
            );

            // Get all members of a squad
            squads.MapGet(
                "/{id}/members",
                async (long id, ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.GetAllMembersAsync(id, ct);
                    return ResultStatusToIResultMapper<List<SquadMemberResponse>>.ToHttp(res);
                }
            );

            // Add a member to a squad
            squads.MapPost(
                "/{id}/members",
                async (
                    long id,
                    SquadMemberRequest req,
                    IPlayerService ps,
                    ISquadService ss,
                    CancellationToken ct
                ) =>
                {
                    var res = await ss.AddMemberAsync(id, req, ps, ct);
                    return ResultStatusToIResultMapper<SquadMemberResponse>.ToHttp(
                        res,
                        $"/squads/{id}/members/{res?.Value?.Puuid}"
                    );
                }
            );

            // Get a member from a squad
            squads.MapGet(
                "/{id}/members/{puuid}",
                async (long id, string puuid, ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.GetMemberAsync(id, puuid, ct);
                    return ResultStatusToIResultMapper<SquadMemberResponse>.ToHttp(res);
                }
            );

            // Delete a member from a squad
            squads.MapDelete(
                "/{id}/members/{puuid}",
                async (long id, string puuid, ISquadService ss, CancellationToken ct) =>
                {
                    var res = await ss.DeleteMemberAsync(id, puuid, ct);
                    return ResultStatusToIResultMapper.ToHttp(res);
                }
            );

            // Get match history for a squad (5 games per squad)
            squads.MapGet(
                "/{id}/matches",
                async (
                    long id,
                    ISquadService ss,
                    IRiotService rs,
                    IMatchService ms,
                    ISquadMatchService sms,
                    CancellationToken ct,
                    bool forceRefresh = false
                ) =>
                {
                    var res = await ss.GetSquadMatchesAsync(id, rs, ms, sms, ct, forceRefresh);
                    return ResultStatusToIResultMapper<List<SquadMatchResponse>>.ToHttp(res);
                }
            );

            squads.MapGet(
                "/{id}/matches/{matchId}",
                async (
                    long id,
                    string matchId,
                    IMatchAggregatedStatsService mass,
                    IParticipantService ps,
                    CancellationToken ct
                ) =>
                {
                    var res = await mass.GetAsync(matchId, ps, ct);
                    return ResultStatusToIResultMapper<MatchAggregatedStatsResponse>.ToHttp(res);
                }
            );
        }
    }
}
