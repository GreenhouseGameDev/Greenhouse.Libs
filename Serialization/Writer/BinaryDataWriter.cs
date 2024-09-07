namespace Greenhouse.Libs.Serialization.Writer;

public record BinaryDataWriter(BinaryWriter Binary) : DataWriter {
    public ArrayDataWriter Array(int length) {
        Primitive().Int(length);
        return new ArrayWriter(this);
    }
        
    public ArrayDataWriter FixedArray(int length)
        => new ArrayWriter(this);

    public ObjectDataWriter Object(int keys) {
        return new ObjectWriter(this);
    }

    public PrimitiveDataWriter Primitive()
        => new PrimitiveWriter(Binary);

    public MapDataWriter Map(int keys) {
        Primitive().Int(keys);
        return new MapWriter(this);
    }

    private record PrimitiveWriter(BinaryWriter Binary) : PrimitiveDataWriter {
        public void Bool(bool value)
            => Binary.Write(value);
        public void Byte(byte value)
            => Binary.Write(value);
        public void Char(char value)
            => Binary.Write(value);
        public void Double(double value)
            => Binary.Write(value);
        public void Float(float value)
            => Binary.Write(value);
        public void Int(int value)
            => Binary.Write(value);
        public void Long(long value)
            => Binary.Write(value);
        public void SByte(sbyte value)
            => Binary.Write(value);
        public void Short(short value)
            => Binary.Write(value);
        public void String(string value)
            => Binary.Write(value);
        public void UInt(uint value)
            => Binary.Write(value);
        public void ULong(ulong value)
            => Binary.Write(value);
        public void UShort(ushort value)
            => Binary.Write(value);
    }

    private class MapWriter(BinaryDataWriter writer) : MapDataWriter {
        private readonly BinaryDataWriter Writer = writer;

        public override void End() {}

        public override DataWriter Field(string name) {
            Writer.Primitive().String(name);
            return Writer;
        }
    }

    private class ArrayWriter(BinaryDataWriter writer) : ArrayDataWriter {
        private readonly BinaryDataWriter Writer = writer;
        public override void End() {}

        public override DataWriter Value()
            => Writer;
    }

    private class ObjectWriter(BinaryDataWriter writer) : ObjectDataWriter {
        private readonly BinaryDataWriter Writer = writer;

        public override void End() {}

        public override DataWriter Field(string name)
            => Writer;

        public override NullableFieldDataWriter NullableField(string name)
            => new NullableFieldWriter(Writer);
    }

    private class NullableFieldWriter(BinaryDataWriter writer) : NullableFieldDataWriter {
        private readonly BinaryDataWriter Writer = writer;

        public override void End() {}

        public override DataWriter NotNull() {
            Writer.Primitive().Bool(true);
            return Writer;
        }
        public override void Null() {
            Writer.Primitive().Bool(false);
        }
    }
}
