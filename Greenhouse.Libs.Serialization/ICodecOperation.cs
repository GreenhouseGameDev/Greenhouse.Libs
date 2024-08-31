using System.Globalization;
using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization
{
    public interface ICodecOperation<T> where T : notnull
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

        DataResult<int> AsInt(T input) => Get(input).SelectWithError(v => v.ToInt32(CultureInfo.InvariantCulture), ex => Error<int>(ex.Message, input));

        U ConvertTo<U>(ICodecOperation<U> operation, T input) where U : notnull;
        bool CompressMaps { get; }
    }
}
