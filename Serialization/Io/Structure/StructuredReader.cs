using System.Text;

namespace Greenhouse.Libs.Serialization.Io.Structure;

public interface StructuredReader {
    public StructuredValue ReadValue();
}

public abstract record StructuredValue {
    private StructuredValue() {}

    public abstract record Primitive : StructuredValue {
        private Primitive() {}

        public record Bool(bool Value) : Primitive;
        public record Long(long Value) : Primitive;
        public record ULong(ulong Value) : Primitive;
        public record Float(float Value) : Primitive;
        public record Double(double Value) : Primitive;
        public record String(string Value) : Primitive;
    }

    public record Array(StructuredValue[] Values) : StructuredValue {
        protected override bool PrintMembers(StringBuilder builder) {
            for (int i = 0; i < Values.Length; i++) {
                var value = Values[i];
                builder.Append(value);
                if (i < Values.Length - 1)
                    builder.Append(", ");
            }
            return true;
        }
    }
    public record Object(Dictionary<string, StructuredValue> Values) : StructuredValue {
        protected override bool PrintMembers(StringBuilder builder) {
            var keys = Values.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++) {
                var key = keys[i];
                var value = Values[key];
                builder.Append($"{key} = {value}");
                if (i < keys.Length - 1)
                    builder.Append(", ");
            }
            return true;
        }
    }
}
