using Newtonsoft.Json;

namespace Greenhouse.Libs.Serialization.Writer;

public record JsonDataWriter(JsonWriter Json) : DataWriter {
    public ObjectDataWriter Object(int keys) {
        Json.WriteStartObject();
        return new ObjectWriter(this);
    }

    public MapDataWriter Map(int keys) {
        Json.WriteStartObject();
        return new MapWriter(this);
    }

    public PrimitiveDataWriter Primitive() {
        return new PrimitiveWriter(this);
    }

    public ArrayDataWriter Array(int length) {
        Json.WriteStartArray();
        return new ArrayWriter(this);
    }

    public ArrayDataWriter FixedArray(int length) {
        Json.WriteStartArray();
        return new ArrayWriter(this);
    }

    private class ArrayWriter(JsonDataWriter writer) : ArrayDataWriter {
        private readonly JsonDataWriter Writer = writer;

        public override void End() {
            Writer.Json.WriteEndArray();
        }

        public override DataWriter Value() {
            return Writer;
        }
    }

    private class MapWriter(JsonDataWriter writer) : MapDataWriter {
        private readonly JsonDataWriter Writer = writer;

        public override void End() {
            Writer.Json.WriteEndObject();
        }

        public override DataWriter Field(string name) {
            Writer.Json.WritePropertyName(name);
            return Writer;
        }
    }

    private class ObjectWriter(JsonDataWriter writer) : ObjectDataWriter {
        private readonly JsonDataWriter Writer = writer;

        public override void End() {
            Writer.Json.WriteEndObject();
        }

        public override DataWriter Field(string name) {
            Writer.Json.WritePropertyName(name);
            return Writer;
        }

        public override NullableFieldDataWriter NullableField(string name) {
            Writer.Json.WritePropertyName(name);
            return new NullableWriter(Writer);
        }
    }

    private class NullableWriter(JsonDataWriter writer) : NullableFieldDataWriter {
        private readonly JsonDataWriter Writer = writer;

        public override void End() {}

        public override DataWriter NotNull()
            => Writer;

        public override void Null()
            => Writer.Json.WriteNull();
    }

    private record PrimitiveWriter(JsonDataWriter Writer) : PrimitiveDataWriter {
        public void Bool(bool value) {
            Writer.Json.WriteValue(value);
        }

        public void Byte(byte value) {
            Writer.Json.WriteValue(value);
        }

        public void Char(char value) {
            Writer.Json.WriteValue(value);
        }

        public void Double(double value) {
            Writer.Json.WriteValue(value);
        }

        public void Float(float value) {
            Writer.Json.WriteValue(value);
        }

        public void Int(int value) {
            Writer.Json.WriteValue(value);
        }

        public void Long(long value) {
            Writer.Json.WriteValue(value);
        }

        public void SByte(sbyte value) {
            Writer.Json.WriteValue(value);
        }

        public void Short(short value) {
            Writer.Json.WriteValue(value);
        }

        public void String(string value) {
            Writer.Json.WriteValue(value);
        }

        public void UInt(uint value) {
            Writer.Json.WriteValue(value);
        }

        public void ULong(ulong value) {
            Writer.Json.WriteValue(value);
        }

        public void UShort(ushort value) {
            Writer.Json.WriteValue(value);
        }
    }
}
