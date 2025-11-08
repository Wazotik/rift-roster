using LeagueSquadApi.Dtos.Enums;

namespace LeagueSquadApi.Dtos
{
    public class ServiceResult
    {
        public ResultStatus? Status { get; init; }
        public bool IsSuccessful { 
            get { return Status == ResultStatus.Success || Status == ResultStatus.Created || Status == ResultStatus.NoContent; } 
        }


        private ServiceResult() { }


        public static ServiceResult Ok(ResultStatus? status = ResultStatus.Success)
        {
            return new ServiceResult { Status = status };
        }
        public static ServiceResult Fail(ResultStatus? status)
        {
            return new ServiceResult { Status = status };
        }
    }

    public class ServiceResult<T>
    {
        public ResultStatus? Status { get; init; }
        public T? Value { get; init; }
        public bool IsSuccessful { 
            get { return Status == ResultStatus.Success || Status == ResultStatus.Created || Status == ResultStatus.NoContent; } 
        }


        private ServiceResult() { }


        public static ServiceResult<T> Ok(T value, ResultStatus? status = ResultStatus.Success)
        {
            return new ServiceResult<T> { Status = status, Value = value };
        }
        public static ServiceResult<T> Fail(ResultStatus? status, T value)
        {
            return new ServiceResult<T> { Status = status, Value = value };
        }
        public static ServiceResult<T> Fail(ResultStatus? status)
        {
            return new ServiceResult<T> { Status = status };
        }
    }
}
