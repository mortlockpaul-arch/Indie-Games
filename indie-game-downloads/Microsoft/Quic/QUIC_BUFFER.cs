using System;

namespace Microsoft.Quic;

internal struct QUIC_BUFFER
{
	internal uint Length;

	internal unsafe byte* Buffer;

	public unsafe readonly Span<byte> Span => new Span<byte>(Buffer, (int)Length);
}
