
using System.Formats.Cbor;

namespace Greenhouse.Libs.Serialization.Structure;

public record CborStructuredReader(CborReader Reader) : StructuredReader {
    public StructuredValue ReadValue() {
        switch (Reader.PeekState()) {
            case CborReaderState.StartMap: {
                var map = new Dictionary<string, StructuredValue>();

                int len = Reader.ReadStartMap()!.Value;

                for (int i = 0; i < len; i++) {
                    var key = Reader.ReadTextString();
                    var value = ReadValue();
                    map[key] = value;
                }

                Reader.ReadEndMap();

                return new StructuredValue.Object(map);
            }
            case CborReaderState.StartArray: {
                var arr = new StructuredValue[Reader.ReadStartArray()!.Value];

                for (int i = 0; i < arr.Length; i++) {
                    var value = ReadValue();
                    arr[i] = value;
                }

                Reader.ReadEndArray();
                
                return new StructuredValue.Array(arr);
            }
            case CborReaderState.UnsignedInteger: {
                return new StructuredValue.Primitive.ULong(Reader.ReadUInt64());
            }
            case CborReaderState.NegativeInteger: {
                return new StructuredValue.Primitive.Long(Reader.ReadInt64());
            }
            case CborReaderState.TextString: {
                return new StructuredValue.Primitive.String(Reader.ReadTextString());
            }
            case CborReaderState.SinglePrecisionFloat: {
                return new StructuredValue.Primitive.Float(Reader.ReadSingle());
            }
            case CborReaderState.DoublePrecisionFloat: {
                return new StructuredValue.Primitive.Double(Reader.ReadDouble());
            }
            case CborReaderState.Boolean: {
                return new StructuredValue.Primitive.Bool(Reader.ReadBoolean());
            }
        }

        throw new Exception($"Unexpected CBOR token: {Reader.PeekState()}");
    }
}
