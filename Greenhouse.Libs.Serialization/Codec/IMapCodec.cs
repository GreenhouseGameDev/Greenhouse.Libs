using Greenhouse.Libs.Serialization.Codec.Collection;
using Greenhouse.Libs.Serialization.Result;

// TODO: Figure out better name for this class.
namespace Greenhouse.Libs.Serialization.Codec
{
    public static class IMapCodec
    {
        public static UnitMapCodec<A> Unit<A>(Func<A> func) where A: notnull => new(func); 
    }

    public interface IMapEncoder<in A> where A : notnull
    {
        public IRecordBuilder<T> Encode<T>(A input, IRecordBuilder<T> builder) where T : notnull;
    }

    public interface IMapDecoder<A> where A : notnull
    {
        public DataResult<A> Decode<T>(RecordDictionary<T> dictionary) where T : notnull;
        public DataResult<A> Decode<T>(ICodecOps<T> ops, T element) where T : notnull
        {
            return Decode(RecordDictionary.Create(ops, element));
        }
    }

    public interface IMapCodec<A> : IMapEncoder<A>, IMapDecoder<A> where A : notnull { }

    public class UnitMapCodec<A>(Func<A> func) : IMapCodec<A> where A: notnull
    {
        public DataResult<A> Decode<T>(RecordDictionary<T> dictionary) where T : notnull => DataResult.Success(func());
        public IRecordBuilder<T> Encode<T>(A input, IRecordBuilder<T> builder) where T : notnull => builder;
    }
}
