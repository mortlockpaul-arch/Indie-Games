using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Security;

[StructLayout(LayoutKind.Auto)]
internal ref struct InputSecurityBuffer
{
	public SecurityBufferType Type;

	public ReadOnlySpan<byte> Token;

	public SafeHandle UnmanagedToken;

	public InputSecurityBuffer(ReadOnlySpan<byte> data, SecurityBufferType tokentype)
	{
		Token = data;
		Type = tokentype;
		UnmanagedToken = null;
	}

	public InputSecurityBuffer(ChannelBinding binding)
	{
		Type = SecurityBufferType.SECBUFFER_CHANNEL_BINDINGS;
		Token = default(ReadOnlySpan<byte>);
		UnmanagedToken = binding;
	}
}
