namespace Greenhouse.Libs.Serialization.Reader;

public record BinaryDataReader(BinaryReader Binary) : DataReader {
    public ArrayDataReader Array()
        => new ArrayReader(this, Primitive().Int());
    public ArrayDataReader FixedArray(int length)
        => new ArrayReader(this, length);
    public ObjectDataReader Object()
        => new ObjectReader(this);
    public PrimitiveDataReader Primitive()
        => new PrimitiveReader(Binary);

    private record PrimitiveReader(BinaryReader Binary) : PrimitiveDataReader {
        public bool Bool()
            => Binary.ReadBoolean();
        public byte Byte()
            => Binary.ReadByte();
        public char Char()
            => Binary.ReadChar();
        public double Double()
            => Binary.ReadDouble();
        public float Float()
            => Binary.ReadSingle();
        public int Int()
            => Binary.ReadInt32();
        public long Long()
            => Binary.ReadInt64();
        public sbyte SByte()
            => Binary.ReadSByte();
        public short Short()
            => Binary.ReadInt16();
        public string String()
            => Binary.ReadString();
        public uint UInt()
            => Binary.ReadUInt32();
        public ulong ULong()
            => Binary.ReadUInt64();
        public ushort UShort()
            => Binary.ReadUInt16();
    }

    private class ArrayReader(BinaryDataReader reader, int length) : ArrayDataReader {
        private readonly BinaryDataReader Reader = reader;

        public override void End() {}
        public override int Length()
            => length;
        public override DataReader Value() 
            => Reader;
    }

    private class ObjectReader(BinaryDataReader reader) : ObjectDataReader {
        private readonly BinaryDataReader Reader = reader;

        public override void End() {}

        public override DataReader Field(string name)
            => Reader;

        public override NullableFieldDataReader NullableField(string name)
            => new NullableFieldReader(Reader, Reader.Primitive().Bool());
    }

    private class NullableFieldReader(BinaryDataReader reader, bool hasValue) : NullableFieldDataReader {
        private readonly BinaryDataReader Reader = reader;
        private readonly bool HasValue = hasValue;

        public override void End() {}

        public override bool IsNull()
            => HasValue;

        public override DataReader NotNull()
            => Reader;
    }
}
