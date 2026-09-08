using System.Text;

namespace System.Net.Mime;

internal static class EncodedStreamFactory
{
	private static readonly byte[] s_footer = "?="u8.ToArray();

	internal static IEncodableStream GetEncoderForHeader(Encoding encoding, bool useBase64Encoding, int headerTextLength)
	{
		byte[] header = CreateHeader(encoding, useBase64Encoding);
		byte[] footer = s_footer;
		if (useBase64Encoding)
		{
			return new Base64Stream(new Base64WriteStateInfo(1024, header, footer, 70, headerTextLength));
		}
		return new QEncodedStream(new WriteStateInfoBase(1024, header, footer, 70, headerTextLength));
	}

	private static byte[] CreateHeader(Encoding encoding, bool useBase64Encoding)
	{
		return Encoding.ASCII.GetBytes("=?" + encoding.HeaderName + "?" + (useBase64Encoding ? "B?" : "Q?"));
	}
}
