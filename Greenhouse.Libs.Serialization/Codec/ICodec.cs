using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization.Codec
{
    public interface IEncoder<in A> where A : notnull
    {
        public DataResult<T> Encode<T>(A input, ICodecOps<T> ops) where T : notnull;
    }

    public interface IDecoder<A> where A : notnull
    {
        public DataResult<A> Decode<T>(T input, ICodecOps<T> ops) where T : notnull;
    }

    public interface ICodec<A> : IEncoder<A>, IDecoder<A> where A : notnull { }
}
