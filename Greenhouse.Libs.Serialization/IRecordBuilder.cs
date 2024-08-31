using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization
{
    public interface IRecordBuilder<T> where T : class
    {
        ICodecOperation<T> Ops { get; }

        IRecordBuilder<T> Add(T key, T value);
        IRecordBuilder<T> Add(T key, DataResult<T> value);
        IRecordBuilder<T> Add(DataResult<T> key, DataResult<T> value)
        {
            return key.Select()
        }


    }
}
