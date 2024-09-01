using Newtonsoft.Json;

namespace Greenhouse.Libs.Serialization.Structure;

public record JsonStructuredReader(JsonReader Reader) : StructuredReader {
    public StructuredValue ReadValue() {
        if (Reader.TokenType == JsonToken.None)
            if (!Reader.Read())
                throw new Exception("Unexpected EOF.");

        switch (Reader.TokenType) {
            case JsonToken.StartObject: {
                var map = new Dictionary<string, StructuredValue>();
                
                while (true) {
                    if (!Reader.Read())
                        throw new Exception("Unexpected EOF.");
                    if (Reader.TokenType == JsonToken.EndObject)
                        return new StructuredValue.Object(map);
                    if (Reader.TokenType != JsonToken.PropertyName)
                        throw new Exception($"Unexpected JSON token: {Reader.TokenType}");
                    var key = (string)Reader.Value!;

                    if (!Reader.Read())
                        throw new Exception("Unexpected EOF.");
                    var value = ReadValue();

                    map[key] = value;
                }
            }
            case JsonToken.StartArray: {
                var arr = new List<StructuredValue>();

                while (true) {
                    if (!Reader.Read())
                        throw new Exception("Unexpected EOF.");
                    if (Reader.TokenType == JsonToken.EndArray)
                        return new StructuredValue.Array([..arr]);
                    arr.Add(ReadValue());
                }
            }
            case JsonToken.String: {
                return new StructuredValue.Primitive.String((string)Reader.Value!);
            }
            case JsonToken.Integer: {
                return new StructuredValue.Primitive.Long((long)Reader.Value!);
            }
            case JsonToken.Float: {
                return new StructuredValue.Primitive.Double((double)Reader.Value!);
            }
            case JsonToken.Boolean: {
                return new StructuredValue.Primitive.Bool((bool)Reader.Value!);
            }
        }

        throw new Exception($"Unexpected JSON token: {Reader.TokenType}");
    }
}
