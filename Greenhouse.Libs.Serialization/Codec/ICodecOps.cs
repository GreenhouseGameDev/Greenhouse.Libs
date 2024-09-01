using System.Globalization;
using Greenhouse.Libs.Serialization.Codec.Collection;
using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization.Codec
{
    public interface ICodecOps<T> where T : notnull
    {
        T Create(IConvertible convertible);
        T CreateList(IEnumerable<T> enumerable);
        T CreateDictionary(IReadOnlyDictionary<T, T> dictionary);

        DataResult<IConvertible> Get(T input);
        DataResult<IReadOnlyDictionary<T, T>> GetDictionary(T input);
        DataResult<IReadOnlyList<T>> GetList(T input);
        DataResult<U> Partial<U>(U obj, string message, T input) where U : notnull;
        DataResult<U> Error<U>(string message, T input) where U : notnull;

        T Empty() => Create(DBNull.Value);
        IListBuilder<T> ListBuilder();
        IRecordBuilder<T> RecordBuilder();

        DataResult<bool> AsBool(T input) => Get(input).SelectWithError(v => v.ToBoolean(CultureInfo.InvariantCulture), ex => Error<bool>(ex.Message, input));
        DataResult<double> AsDouble(T input) => Get(input).SelectWithError(v => v.ToDouble(CultureInfo.InvariantCulture), ex => Error<double>(ex.Message, input));
        DataResult<float> AsFloat(T input) => Get(input).SelectWithError(v => v.ToSingle(CultureInfo.InvariantCulture), ex => Error<float>(ex.Message, input));
        DataResult<int> AsInt(T input) => Get(input).SelectWithError(v => v.ToInt32(CultureInfo.InvariantCulture), ex => Error<int>(ex.Message, input));
        DataResult<string> AsString(T input) => Get(input).SelectWithError(v => v.ToString(CultureInfo.InvariantCulture), ex => Error<string>(ex.Message, input));

        U ConvertTo<U>(ICodecOps<U> operation, T input) where U : notnull;
        bool CompressMaps { get; }
    }
}