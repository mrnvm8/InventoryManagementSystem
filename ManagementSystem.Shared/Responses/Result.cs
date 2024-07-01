namespace ManagementSystem.Shared.Responses
{
    public class Result<T>
    {
        public T Data { get;}
        public string Message {  get;}
        public bool IsSuccess {  get;}

        public Result(T value, string message, bool isSuccess)
        {
            Data = value;
            Message = message;
            IsSuccess = isSuccess;
        }

        public static Result<T> Success(T value, string message)
        {
            return new Result<T>(value, message, true);
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>(default, message, false);
        }
    }
}
