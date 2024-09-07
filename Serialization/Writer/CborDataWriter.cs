using System.Formats.Cbor;

namespace Greenhouse.Libs.Serialization.Writer;

public record CborDataWriter(CborWriter Cbor) : DataWriter {
    public ObjectDataWriter Object(int keys) {
        Cbor.WriteStartMap(keys);
        return new ObjectWriter(this);
    }

    public PrimitiveDataWriter Primitive() {
        return new PrimitiveWriter(this);
    }

    public ArrayDataWriter Array(int length) {
        Cbor.WriteStartArray(length);
        return new ArrayWriter(this);
    }

    public ArrayDataWriter FixedArray(int length) {
        Cbor.WriteStartArray(length);
        return new ArrayWriter(this);
    }

    public MapDataWriter Map(int keys) {
        Cbor.WriteStartMap(keys);
        return new MapWriter(this);
    }

    private class ArrayWriter(CborDataWriter writer) : ArrayDataWriter {
        private readonly CborDataWriter Writer = writer;

        public override void End() {
            Writer.Cbor.WriteEndArray();
        }

        public override DataWriter Value() {
            return Writer;
        }
    }

    private class MapWriter(CborDataWriter writer) : MapDataWriter {
        private readonly CborDataWriter Writer = writer;

        public override void End() {
            Writer.Cbor.WriteEndMap();
        }

        public override DataWriter Field(string name) {
            Writer.Cbor.WriteTextString(name);
            return Writer;
        }
    }

    private class ObjectWriter(CborDataWriter writer) : ObjectDataWriter {
        private readonly CborDataWriter Writer = writer;

        public override void End() {
            Writer.Cbor.WriteEndMap();
        }

        public override DataWriter Field(string name) {
            Writer.Cbor.WriteTextString(name);
            return Writer;
        }

        public override NullableFieldDataWriter NullableField(string name) {
            Writer.Cbor.WriteTextString(name);
            return new NullableWriter(Writer);
        }
    }

    private class NullableWriter(CborDataWriter writer) : NullableFieldDataWriter {
        private readonly CborDataWriter Writer = writer;

        public override void End() {}

        public override DataWriter NotNull()
            => Writer;

        public override void Null()
            => Writer.Cbor.WriteNull();
    }

    private record PrimitiveWriter(CborDataWriter Writer) : PrimitiveDataWriter {
        public void Bool(bool value) {
            Writer.Cbor.WriteBoolean(value);
        }

        public void Byte(byte value) {
            Writer.Cbor.WriteUInt32(value);
        }

        public void Char(char value) {
            Writer.Cbor.WriteUInt32(value);
        }

        public void Double(double value) {
            Writer.Cbor.WriteDouble(value);
        }

        public void Float(float value) {
            Writer.Cbor.WriteSingle(value);
        }

        public void Int(int value) {
            Writer.Cbor.WriteInt32(value);
        }

        public void Long(long value) {
            Writer.Cbor.WriteInt64(value);
        }

        public void SByte(sbyte value) {
            Writer.Cbor.WriteInt32(value);
        }

        public void Short(short value) {
            Writer.Cbor.WriteInt32(value);
        }

        public void String(string value) {
            Writer.Cbor.WriteTextString(value);
        }

        public void UInt(uint value) {
            Writer.Cbor.WriteUInt32(value);
        }

        public void ULong(ulong value) {
            Writer.Cbor.WriteUInt64(value);
        }

        public void UShort(ushort value) {
            Writer.Cbor.WriteUInt32(value);
        }
    }
}
