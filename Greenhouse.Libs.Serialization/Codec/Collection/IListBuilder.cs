using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization.Codec.Collection
{
    public interface IListBuilder<T> where T : notnull
    {
        ICodecOps<T> Ops { get; }
        IRecordBuilder<T> Add(T value);
        IRecordBuilder<T> WithErrorsFrom<E>(DataResult<E> error) where E : notnull;
        DataResult<T> Build();

        IRecordBuilder<T> Add(DataResult<T> value)
        {
            return value.Apply(Add, e => WithErrorsFrom(DataResult.Error<string>(e)));
        }
    }
}
