using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Result
{
    public static class DataResult
    {
        public static DataResult<R> Success<R>(R value) where R : notnull => new(Optional.Of(value), Optional.Empty<string>());
        public static DataResult<R> Partial<R>(R value, string error) where R : notnull => new(Optional.Of(value), Optional.Of(error));
        public static DataResult<R> Error<R>(string error) where R : notnull => new(Optional.Empty<R>(), Optional.Of(error));
    }

    public struct DataResult<R> where R : notnull
    {
        public readonly Optional<R> Result;
        public readonly Optional<string> Error;

        public readonly bool HasResult => Result.HasValue;
        public readonly bool HasError => Error.HasValue;
        public readonly bool IsPartial => HasResult && Error.HasValue;

        internal DataResult(Optional<R> result, Optional<string> error)
        {
            Result = result;
            Error = error;
        }

        public DataResult<T> Select<T>(Func<R, T> func) where T : notnull
        {
            return new(Result.Select(func), Error);
        }

        public T Apply<T>(Func<R, T> func, Func<string, T> errorFunc)
        {
            if (Result.HasValue)
                return func(Result.Get());
            return errorFunc(Error.Get());
        }

        public R ResultOrThrow<E>(Func<string, E> exceptionFunc) where E: Exception
        {
            if (!Result.HasValue)
                throw exceptionFunc(Error.Get());
            return Result.Get();
        }

        public R ResultOrThrow()
        {
            return ResultOrThrow(s => new InvalidOperationException(s));
        }

        public DataResult<T> Propagate<T>(Func<R, DataResult<T>> func) where T : notnull => !HasResult ? DataResult.Error<T>(Error.Get()) : func(Result.Get());

        public DataResult<T> SelectWithError<T>(Func<R, T> func, Func<Exception, DataResult<T>>? exceptionFunc = null) where T : notnull
        {
            try
            {
                return new DataResult<T>(Result.Select(func), Error);
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
            if (Error.HasValue)
                retValue += "Error:" + Error;
            retValue += ">";
            return retValue;
        }
    }
}