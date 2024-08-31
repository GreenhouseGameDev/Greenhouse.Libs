using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Result
{
    public static class DataResult
    {
        public static DataResult<R> Success<R>(R value) where R : notnull => new DataResult<R>(Optional.Of(value));
        public static DataResult<R> Partial<R>(R value, string error) where R : notnull => new DataResult<R>(Optional.Of(value), error);
        public static DataResult<R> Error<R>(string error) where R : notnull => new DataResult<R>(Optional.Empty<R>(), error);
    }

    public struct DataResult<R> where R : notnull
    {
        private readonly Optional<R> Result;
        public readonly string? ErrorMessage;
        public readonly bool IsPartial => ErrorMessage is not null && Result.HasValue;

        internal DataResult(Optional<R> result, string? errorMessage = null)
        {
            Result = result;
            ErrorMessage = errorMessage;
        }

        public R GetOrThrow(R value)
        {
            return Result.Get();
        }

        public DataResult<T> SelectWithError<T>(Func<R, T> func, Func<Exception, DataResult<T>>? exceptionFunc = null) where T : notnull
        {
            try
            {
                return new DataResult<T>(Result.Select(func), ErrorMessage);
            } 
            catch (Exception ex)
            {
                return exceptionFunc is null ? DataResult.Error<T>(ex.Message) : exceptionFunc(ex);
            }
        }
   
        public override string ToString()
        {
            string retValue = "DataResult<";
            if (Result.HasValue)
            retValue += $"Result:{Result.Get()}";
            if (IsPartial)
                retValue += ";";
            if (ErrorMessage != null)
                retValue += "Error:" + ErrorMessage;
            retValue += ">";
            return retValue;
        }
    }
}