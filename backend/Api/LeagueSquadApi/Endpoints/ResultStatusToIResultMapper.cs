using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;

namespace LeagueSquadApi.Endpoints
{
    public static class ResultStatusToIResultMapper<T>
    {
        public static IResult ToHttp(ServiceResult<T> serviceResult, string uri = "", string msg = "")
        {
            switch (serviceResult.Status)
            {
                case (ResultStatus.Success):
                    return Results.Ok(serviceResult.Value);
                case (ResultStatus.Created):
                    return Results.Created(uri, serviceResult.Value);
                case (ResultStatus.NotFound):
                    return Results.NotFound();
                case (ResultStatus.NoContent):
                    return Results.NoContent();
                case (ResultStatus.ValidationFailed):
                    return Results.UnprocessableEntity();
                case (ResultStatus.Conflict):
                    return Results.Conflict();
                case (ResultStatus.Forbidden):
                    return Results.Forbid();
                case (ResultStatus.Unauthorized):
                    return Results.Unauthorized();
                case (ResultStatus.Concurrency):
                    return Results.StatusCode(409);
                default:
                    return Results.StatusCode(500);
            }

        }
    }

    public static class ResultStatusToIResultMapper
    {
        public static IResult ToHttp(ServiceResult serviceResult, string uri = "", string msg = "")
        {
            switch (serviceResult.Status)
            {
                case (ResultStatus.Success):
                    return Results.Ok();
                case (ResultStatus.NotFound):
                    return Results.NotFound();
                case (ResultStatus.NoContent):
                    return Results.NoContent();
                case (ResultStatus.ValidationFailed):
                    return Results.UnprocessableEntity();
                case (ResultStatus.Conflict):
                    return Results.Conflict();
                case (ResultStatus.Forbidden):
                    return Results.Forbid();
                case (ResultStatus.Unauthorized):
                    return Results.Unauthorized();
                case (ResultStatus.Concurrency):
                    return Results.StatusCode(409);
                default:
                    return Results.StatusCode(500);
            }
        }
    }

}