using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12SafeContentsBag : Pkcs12SafeBag
{
	[CompilerGenerated]
	private Pkcs12SafeContents _003CSafeContents_003Ek__BackingField;

	private Pkcs12SafeContents SafeContents
	{
		[CompilerGenerated]
		set
		{
			_003CSafeContents_003Ek__BackingField = value;
		}
	}

	private Pkcs12SafeContentsBag(ReadOnlyMemory<byte> encoded)
		: base("1.2.840.113549.1.12.10.1.6", encoded)
	{
	}

	internal static Pkcs12SafeContentsBag Decode(ReadOnlyMemory<byte> encodedValue)
	{
		Pkcs12SafeContents safeContents = new Pkcs12SafeContents(encodedValue);
		return new Pkcs12SafeContentsBag(encodedValue)
		{
			SafeContents = safeContents
		};
	}
}
