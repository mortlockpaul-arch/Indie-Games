using System.Text;

namespace System.Net.Mime;

internal interface IByteEncoder
{
	int EncodeBytes(ReadOnlySpan<byte> buffer, bool dontDeferFinalBytes, bool shouldAppendSpaceToCRLF);

	void AppendPadding();

	int EncodeString(string value, Encoding encoding);

	string GetEncodedString();
}
