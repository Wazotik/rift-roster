using System.Net;
using LeagueSquadApi.Dtos.Enums;

public static class HttpStatusToResultStatusMapper
{
    public static ResultStatus Map(int statusCode)
    {
        if (statusCode >= 200 && statusCode <= 299)
        {
            return statusCode switch
            {
                201 => ResultStatus.Created,
                204 => ResultStatus.NoContent,
                _ => ResultStatus.Success
            };
        }

        // Non-success codes
        return statusCode switch
        {
            400 => ResultStatus.ValidationFailed,
            401 => ResultStatus.Unauthorized,
            403 => ResultStatus.Forbidden,
            404 => ResultStatus.NotFound,
            409 => ResultStatus.Conflict,
            412 => ResultStatus.Concurrency,
            422 => ResultStatus.ValidationFailed,   
            429 => ResultStatus.Conflict,           
            _ => ResultStatus.Unknown
        };
    }

    public static ResultStatus Map(HttpStatusCode code) => Map((int)code);
}

