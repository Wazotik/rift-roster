using LeagueSquadApi.Dtos.Enums;

namespace LeagueSquadApi.Dtos
{
    public class ServiceResult
    {
        public ResultStatus? Status { get; init; }
        public bool IsSuccessful
        {
            get { return Status == ResultStatus.Success || Status == ResultStatus.Created || Status == ResultStatus.NoContent; }
        }
        public string? Message { get; init; }

        private ServiceResult() { }


        public static ServiceResult Ok(ResultStatus? status = ResultStatus.Success)
        {
            return new ServiceResult { Status = status };
        }
        public static ServiceResult Fail(ResultStatus? status, string? msg = "")
        {
            return new ServiceResult { Status = status, Message = msg };
        }
    }

    public class ServiceResult<T>
    {
        public ResultStatus? Status { get; init; }
        public T? Value { get; init; }
        public string? Message { get; init; }

        public bool IsSuccessful
        {
            get { return Status == ResultStatus.Success || Status == ResultStatus.Created || Status == ResultStatus.NoContent; }
        }


        private ServiceResult() { }


        public static ServiceResult<T> Ok(T value, ResultStatus? status = ResultStatus.Success)
        {
            return new ServiceResult<T> { Status = status, Value = value };
        }
        public static ServiceResult<T> Fail(ResultStatus? status, T value, string? msg = "")
        {
            return new ServiceResult<T> { Status = status, Value = value, Message = msg };
        }
        public static ServiceResult<T> Fail(ResultStatus? status, string? msg = "")
        {
            return new ServiceResult<T> { Status = status, Message = msg };
        }
    }
}
