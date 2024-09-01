namespace Greenhouse.Libs.Serialization.Data;

public static class DataResult {
    public static DataResult<R> Success<R>(R value) where R : notnull => new(value);
    public static DataResult<R> Partial<R>(R value, string error) where R : notnull => new(value, error);
    public static DataResult<R> Error<R>(string error) where R : notnull => new(error);
}

public readonly struct DataResult<R> where R : notnull {
    public readonly R? Result;
    public readonly string? Error;

    public readonly bool HasResult => Result != null;
    public readonly bool HasError => Error != null;
    public readonly bool IsPartial => HasResult && HasError;

    internal DataResult(R? result, string? error) {
        Result = result;
        Error = error;
    }
    internal DataResult(R result) {
        Result = result;
    }
    internal DataResult(string error) {
        Error = error;
    }

    public DataResult<T> Select<T>(Func<R, T> func) where T : notnull {
        return new(Result != null ? func(Result) : default, Error);
    }

    public T Apply<T>(Func<R, T> func, Func<string, T> errorFunc) {
        if (Result != null)
            return func(Result);
        return errorFunc(Error ?? "");
    }

    public R ResultOrThrow<E>(Func<string, E> exceptionFunc) where E: Exception {
        if (Result == null)
            throw exceptionFunc(Error ?? "");
        return Result;
    }

    public R ResultOrThrow() {
        return ResultOrThrow(s => new InvalidOperationException(s));
    }

    public DataResult<T> Propagate<T>(Func<R, DataResult<T>> func) where T : notnull
        => Result == null ? DataResult.Error<T>(Error!) : func(Result);

    public DataResult<T> SelectWithError<T>(Func<R, T> func, Func<Exception, DataResult<T>>? exceptionFunc = null) where T : notnull {
        try {
            if (Result == null)
                return new DataResult<T>(Error!);
            return new DataResult<T>(func(Result), Error);
        } catch (Exception ex) {
            return exceptionFunc is null ? DataResult.Error<T>(ex.Message) : exceptionFunc(ex);
        }
    }

    public override string ToString() {
        string retValue = "DataResult<";
        if (Result != null)
            retValue += $"Result:{Result}";
        if (IsPartial)
            retValue += ";";
        if (Error != null)
            retValue += "Error:" + Error;
        retValue += ">";
        return retValue;
    }
}
