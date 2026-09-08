using System.Text;

namespace System.Net.Mime;

internal interface IEncodableStream
{
	int DecodeBytes(Span<byte> buffer);

	int EncodeBytes(ReadOnlySpan<byte> buffer);

	int EncodeString(string value, Encoding encoding);

	string GetEncodedString();
}
