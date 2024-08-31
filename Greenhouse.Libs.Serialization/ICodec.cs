namespace Greenhouse.Libs.Serialization
{
    public interface IEncoder<in A> where A : notnull
    {
        public T Encode<T>(A input, ICodecOperation<T> operation) where T : notnull;
    }

    public interface IDecoder<A> where A : notnull
    {
        public A Decode<T>(T input, ICodecOperation<T> operation) where T : notnull;
    }

    public interface Codec<A> : IEncoder<A>, IDecoder<A> where A : notnull
    {
        
    }
}
