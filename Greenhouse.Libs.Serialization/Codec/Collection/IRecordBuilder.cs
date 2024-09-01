using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization.Codec.Collection
{
    public interface IRecordBuilder<T> where T : notnull
    {
        ICodecOps<T> Ops { get; }
        IRecordBuilder<T> Add(T key, T value);
        IRecordBuilder<T> WithErrorsFrom<E>(DataResult<E> error) where E : notnull;
        DataResult<T> Build();

        IRecordBuilder<T> Add(T key, DataResult<T> value)
        {
            return value.Apply(v => Add(key, v), e => WithErrorsFrom(DataResult.Error<string>(e)));
        }
        IRecordBuilder<T> Add(DataResult<T> key, DataResult<T> value)
        {
            return key.Apply(k => Add(k, value), e => WithErrorsFrom(DataResult.Error<string>(e)));
        }
        IRecordBuilder<T> Add(string key, T value)
        {
            return Add(Ops.Create(key), value);
        }
        IRecordBuilder<T> Add(string key, DataResult<T> value)
        {
            return Add(Ops.Create(key), value);
        }
    }
}
